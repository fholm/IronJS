using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using Antlr.Runtime;
using System.Globalization;
using IronJS.Compiler.Ast;

namespace IronJS.Compiler
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using EcmaParser = IronJS.Compiler.Parser.ES3Parser;
    using EcmaLexer = IronJS.Compiler.Parser.ES3Lexer;

    public class IjsAstGenerator
    {
        public List<INode> Build(string fileName, Encoding encoding)
        {
            return Build(
                System.IO.File.ReadAllText(
                    fileName, 
                    encoding
                )
            );
        }

        public List<INode> Build(string source)
        {
            var lexer = new EcmaLexer(new ANTLRStringStream(source));
            var parser = new EcmaParser(new CommonTokenStream(lexer));

            var program = parser.program();
            var root = (ITree)program.Tree;
            var nodes = new List<INode>();

            if (root == null)
            {
                nodes.Add(new NullNode(null));
            }
            else if (root.IsNil)
            {
                ITreeTools.EachChild(root, delegate(ITree node) {
                    nodes.Add(Build(node));
                });
            }
            else
            {
                nodes.Add(Build(root));
            }

            return nodes;
        }

        private INode Build(ITree node)
        {
            if (node == null)
                return null;

            switch (node.Type)
            {

                case EcmaParser.WITH:
                    return BuildWith(node);

                case EcmaParser.BLOCK:
                    return BuildBlock(node);


                case EcmaParser.RegularExpressionLiteral:
                    return BuildRegex(node);

                /*
                 * Expresion statements
                 */

                case EcmaParser.PAREXPR:
                    return Build(ITreeTools.GetChildSafe(node, 0));

                case EcmaParser.EXPR:
                    return Build(ITreeTools.GetChildSafe(node, 0));

                case EcmaParser.CEXPR:
                    return BuildCommaExpression(node);

                /*
                 * Objects
                 */
                case EcmaParser.OBJECT:
                    return BuildObject(node);

                case EcmaParser.NEW:
                    return BuildNew(node);

                case EcmaParser.INSTANCEOF:
                    return BuildInstanceOf(node);

                case EcmaParser.ARRAY:
                    return BuildArray(node);

                /*
                 * Functions
                 */

                case EcmaParser.FUNCTION:
                    return BuildFunction(node);

                case EcmaParser.RETURN:
                    return BuildReturn(node);

                case EcmaParser.CALL:
                    return BuildCall(node);

                /*
                 * If statement
                 */

                case EcmaParser.IF:
                case EcmaParser.QUE: // <expr> ? <expr> : <expr>
                    return BuildIf(node);


                case EcmaParser.SWITCH:
                    return BuildSwitch(node);

                /*
                 * Variables
                 */
                case EcmaParser.THIS: // 'this' is just an identifier
                case EcmaParser.Identifier:
                    return BuildIdentifier(node);

                /*
                 * Exceptions
                 */

                case EcmaParser.TRY:
                    return BuildTry(node);

                case EcmaParser.FINALLY:
                    return BuildFinally(node);

                case EcmaParser.THROW:
                    return BuildThrow(node);

                /*
                 * Property
                 */

                case EcmaParser.BYFIELD:
                    return BuildMemberAccess(node);

                case EcmaParser.BYINDEX:
                    return BuildIndexAccess(node);

                case EcmaLexer.IN:
                    return BuildIn(node);

                /*
                 * Loops
                 */
                case EcmaParser.WHILE:
                    return BuildWhile(node);

                case EcmaParser.FOR:
                    return BuildFor(node);

                case EcmaParser.DO:
                    return BuildDoWhile(node);

                case EcmaParser.BREAK:
                    return BuildBreak(node);

                case EcmaParser.CONTINUE:
                    return BuildContinue(node);

                case EcmaParser.LABELLED:
                    return BuildLabelled(node);

                /*
                 * Literals
                 */
                case EcmaParser.DecimalLiteral:
                    return BuildNumber(node);

                case EcmaParser.StringLiteral:
                    return BuildString(node);

                case EcmaParser.NULL:
                    return BuildNull(node);

                case EcmaParser.TRUE:
                case EcmaParser.FALSE:
                    return BuildBoolean(node);

                /*
                 * Assignments
                 */
                case EcmaParser.VAR:
                    return BuildVarAssign(node);

                case EcmaParser.ASSIGN:
                    return BuildAssign(node, false);

                /*
                 * Binary Operators
                 */
                // 1 + 2
                case EcmaParser.ADD:
                    return BuildBinaryOp(node, ExpressionType.Add);

                // 1 - 2
                case EcmaParser.SUB:
                    return BuildBinaryOp(node, ExpressionType.Subtract);

                // 1 * 2
                case EcmaParser.MUL:
                    return BuildBinaryOp(node, ExpressionType.Multiply);

                // 1 / 2
                case EcmaParser.DIV:
                    return BuildBinaryOp(node, ExpressionType.Divide);

                // 1 % 2
                case EcmaParser.MOD:
                    return BuildBinaryOp(node, ExpressionType.Modulo);

                // foo += 1
                case EcmaParser.ADDASS:
                    return BuildBinaryOpAssign(node, ExpressionType.Add);

                // foo -= 1
                case EcmaParser.SUBASS:
                    return BuildBinaryOpAssign(node, ExpressionType.Subtract);

                // foo *= 1
                case EcmaParser.MULASS:
                    return BuildBinaryOpAssign(node, ExpressionType.Multiply);

                // foo /= 1
                case EcmaParser.DIVASS:
                    return BuildBinaryOpAssign(node, ExpressionType.Divide);

                // foo %= 1
                case EcmaParser.MODASS:
                    return BuildBinaryOpAssign(node, ExpressionType.Modulo);

                // 1 == 2
                case EcmaParser.EQ:
                    return BuildBinaryOp(node, ExpressionType.Equal);

                // 1 != 2
                case EcmaParser.NEQ:
                    return BuildBinaryOp(node, ExpressionType.NotEqual);

                // 1 === 2
                case EcmaParser.SAME:
                    return BuildStrictCompare(node, ExpressionType.Equal);

                // 1 !== 2
                case EcmaParser.NSAME:
                    return BuildStrictCompare(node, ExpressionType.NotEqual);

                // 1 < 2
                case EcmaParser.LT:
                    return BuildBinaryOp(node, ExpressionType.LessThan);

                // 1 > 2
                case EcmaParser.GT:
                    return BuildBinaryOp(node, ExpressionType.GreaterThan);

                // 1 >= 2
                case EcmaParser.GTE:
                    return BuildBinaryOp(node, ExpressionType.GreaterThanOrEqual);

                // 1 <= 2
                case EcmaParser.LTE:
                    return BuildBinaryOp(node, ExpressionType.LessThanOrEqual);

                // 1 >> 2
                case EcmaParser.SHR:
                    return BuildBinaryOp(node, ExpressionType.RightShift);

                // 1 << 2
                case EcmaParser.SHL:
                    return BuildBinaryOp(node, ExpressionType.LeftShift);

                // 1 >>> 2
                case EcmaParser.SHU:
                    return BuildUnsignedRightShift(node);

                // foo >>= 1
                case EcmaParser.SHRASS:
                    return BuildBinaryOpAssign(node, ExpressionType.RightShift);

                // foo <<= 1
                case EcmaParser.SHLASS:
                    return BuildBinaryOpAssign(node, ExpressionType.LeftShift);

                // foo >>>= 1
                case EcmaParser.SHUASS:
                    return BuildUnsignedRightShiftAssign(node);

                // 1 & 2
                case EcmaParser.AND:
                    return BuildBinaryOp(node, ExpressionType.And);

                // 1 | 2
                case EcmaParser.OR:
                    return BuildBinaryOp(node, ExpressionType.Or);

                // 1 ^ 2
                case EcmaParser.XOR:
                    return BuildBinaryOp(node, ExpressionType.ExclusiveOr);

                // foo &= 1
                case EcmaParser.ANDASS:
                    return BuildBinaryOpAssign(node, ExpressionType.And);

                // foo |= 1
                case EcmaParser.ORASS:
                    return BuildBinaryOpAssign(node, ExpressionType.Or);

                // foo ^= 1
                case EcmaParser.XORASS:
                    return BuildBinaryOpAssign(node, ExpressionType.ExclusiveOr);

                // true && false
                case EcmaParser.LAND:
                    return BuildLogicalNode(node, ExpressionType.AndAlso);

                // true || false
                case EcmaParser.LOR:
                    return BuildLogicalNode(node, ExpressionType.OrElse);

                /*
                 * Increment/Decrement operators
                 */
                // foo++
                case EcmaParser.PINC:
                    return BuildIncDecNode(node, ExpressionType.PostIncrementAssign);

                // foo--
                case EcmaParser.PDEC:
                    return BuildIncDecNode(node, ExpressionType.PostDecrementAssign);

                // ++foo
                case EcmaParser.INC:
                    return BuildIncDecNode(node, ExpressionType.PreIncrementAssign);

                // --foo
                case EcmaParser.DEC:
                    return BuildIncDecNode(node, ExpressionType.PreDecrementAssign);

                /*
                 * Unary operators
                 */
                //TODO: make sure ~, ! and - have the right ExpressionType
                // ~foo
                case EcmaParser.INV:
                    return BuildUnuaryOp(node, ExpressionType.OnesComplement);

                // !foo
                case EcmaParser.NOT:
                    return BuildUnuaryOp(node, ExpressionType.Not);

                // -foo
                case EcmaParser.NEG:
                    return BuildUnuaryOp(node, ExpressionType.Negate);

                // +foo
                case EcmaParser.POS:
                    return BuildUnuaryOp(node, ExpressionType.UnaryPlus);

                // typeof foo
                case EcmaParser.TYPEOF:
                    return BuildTypeOfOp(node);

                // void foo
                case EcmaParser.VOID:
                    return BuildVoidOp(node);

                // delete foo
                case EcmaParser.DELETE:
                    return BuildDelete(node);

                /*
                 * Error handling
                 */
                default:
                    throw new Compiler.IjsCompilerError(
                        String.Format("Unrecognized token '{0}'", Name(node))
                    );
            }
        }

        private INode BuildRegex(ITree node)
        {
            return new RegexNode(node.Text, node);
        }

        private INode BuildArray(ITree node)
        {
            return new ArrayNode(
                ITreeTools.Map<INode>(node, delegate(ITree child) {
                    return Build(ITreeTools.GetChildSafe(child, 0));
                }),
                node
            );
        }

        private INode BuildInstanceOf(ITree node)
        {
            return new InstanceOfNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)),
                node
            );
        }

        private INode BuildSwitch(ITree node)
        {
            var def = (INode)(new NullNode(node));
            var cases = new List<Tuple<INode, INode>>();

            for (int i = 1; i < node.ChildCount; ++i)
            {
                var child = ITreeTools.GetChildSafe(node, i);

                if (child.Type == EcmaParser.DEFAULT)
                {
                    def = Build(ITreeTools.GetChildSafe(child, 0));
                }
                else
                {
                    var caseBlock = new List<INode>();

                    for(int j = 1; j < child.ChildCount; ++j)
                        caseBlock.Add(
                            Build(ITreeTools.GetChildSafe(child, j))
                        );

                    cases.Add(
                        Tuple.Create(
                            Build(ITreeTools.GetChildSafe(child, 0)),
                            (INode)new BlockNode(caseBlock, node)
                        )
                    );
                }
            }

            return new SwitchNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                def,
                cases,
                node
            );

            throw new NotImplementedException();
        }

        private INode BuildCommaExpression(ITree node)
        {
            return new AssignmentBlockNode(
                ITreeTools.Map(node, delegate(ITree child) { return Build(child); }),
                false,
                node
            );
        }

        private INode BuildIn(ITree node)
        {
            return new InNode(
                Build(ITreeTools.GetChildSafe(node, 1)),
                Build(ITreeTools.GetChildSafe(node, 0)),
                node
            );
        }

        private INode BuildDelete(ITree node)
        {
            return new DeleteNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                node
            );
        }

        private INode BuildLabelled(ITree node)
        {
            ITree label = ITreeTools.GetChildSafe(node, 0);
            INode target = Build(ITreeTools.GetChildSafe(node, 1));

            if (!(target is ILabelableNode))
                throw new IjsCompilerError("Can only label nodes that implement ILabelableNode");

           (target as ILabelableNode).SetLabel(label.Text);
           return target;
        }

        private INode BuildIndexAccess(ITree node)
        {
            if (RewriteIfContainsNew(ITreeTools.GetChildSafe(node, 0)))
            {
                return new NewNode(Build(node), node);
            }
            else
            {
                return new IndexAccessNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    Build(ITreeTools.GetChildSafe(node, 1)),
                    node
                );
            }
        }

        private INode BuildThrow(ITree node)
        {
            return new ThrowNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                node
            );
        }

        private INode BuildFinally(ITree node)
        {
            return BuildBlock(ITreeTools.GetChildSafe(node, 0));
        }

        private INode BuildTry(ITree node)
        {
            if (node.ChildCount > 2)
            {
                ITree _catch = ITreeTools.GetChildSafe(node, 1);

                return new TryNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    Build(ITreeTools.GetChildSafe(_catch, 0)),
                    Build(ITreeTools.GetChildSafe(_catch, 1)),
                    Build(ITreeTools.GetChildSafe(node, 2)),
                    node
                );
            }
            else
            {
                ITree secondChild = ITreeTools.GetChildSafe(node, 1);

                if (secondChild.Type == EcmaParser.FINALLY)
                {
                    return new TryNode(
                        Build(ITreeTools.GetChildSafe(node, 0)),
                        null,
                        null,
                        Build(ITreeTools.GetChildSafe(node, 1)),
                        node
                    );
                }
                else
                {
                    ITree _catch = ITreeTools.GetChildSafe(node, 1);

                    return new TryNode(
                        Build(ITreeTools.GetChildSafe(node, 0)),
                        Build(ITreeTools.GetChildSafe(_catch, 0)),
                        Build(ITreeTools.GetChildSafe(_catch, 1)),
                        null,
                        node
                    );
                }
            }
        }

        private INode BuildWith(ITree node)
        {
            return new WithNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)),
                node
            );
        }

        private INode BuildDoWhile(ITree node)
        {
            INode body = Build(ITreeTools.GetChildSafe(node, 0));
            INode test = Build(ITreeTools.GetChildSafe(node, 1));

            return new WhileNode(
                test,
                body,
                WhileType.DoWhile,
                node
            );
        }

        private INode BuildContinue(ITree node)
        {
            if (node.ChildCount == 0)
                return new ContinueNode(null, node);

            return new ContinueNode(
                ITreeTools.GetChildSafe(node, 0).Text, 
                node
            );
        }

        // 12.8
        private INode BuildBreak(ITree node)
        {
            if (node.ChildCount == 0)
                return new BreakNode(null, node);

            return new BreakNode(
                ITreeTools.GetChildSafe(node, 0).Text, 
                node
            );
        }

        private INode BuildFor(ITree node)
        {
            INode body = Build(ITreeTools.GetChildSafe(node, 1));
            ITree type = ITreeTools.GetChildSafe(node, 0);

            // 12.6.3
            if (type.Type == EcmaParser.FORSTEP)
            {
                ITree init = ITreeTools.GetChildSafe(type, 0);
                ITree test = ITreeTools.GetChildSafe(type, 1);
                ITree incr = ITreeTools.GetChildSafe(type, 2);

                INode initNode = init.ChildCount > 0
                             ? Build(init)
                             : new NullNode(node);

                INode testNode = test.ChildCount > 0
                             ? Build(test)
                             : new BooleanNode(true, node);

                INode incrNode = incr.ChildCount > 0
                             ? Build(incr)
                             : new NullNode(node);

                return new ForStepNode(
                    initNode,
                    testNode,
                    incrNode,
                    body,
                    node
                );
            }
            // 12.6.4
            else if(type.Type == EcmaParser.FORITER)
            {
                return new ForInNode(
                    Build(ITreeTools.GetChildSafe(type, 0)),
                    Build(ITreeTools.GetChildSafe(type, 1)),
                    body,
                    node
                );
            }

            throw new NotImplementedException();
        }

        private INode BuildUnsignedRightShiftAssign(ITree node)
        {
            return new AssignNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                new UnsignedRightShiftNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    Build(ITreeTools.GetChildSafe(node, 1)),
                    ITreeTools.GetChildSafe(node, 0)
                ),
                node
            );
        }

        private INode BuildUnsignedRightShift(ITree node)
        {
            return new UnsignedRightShiftNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)),
                node
            );
        }

        private INode BuildStrictCompare(ITree node, ExpressionType type)
        {
            return new StrictCompareNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)),
                type,
                node
            );
        }

        private INode BuildVoidOp(ITree node)
        {
            return new VoidNode(
                Build(ITreeTools.GetChildSafe(node, 0)), 
                node
            );
        }

        private INode BuildBoolean(ITree node)
        {
            return new BooleanNode(
                node.Type == 5, 
                node
            );
        }

        private INode BuildTypeOfOp(ITree node)
        {
            return new TypeOfNode(
                Build(ITreeTools.GetChildSafe(node, 0)), 
                node
            );
        }

        private INode BuildIncDecNode(ITree node, ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                    return new AssignNode(
                        Build(ITreeTools.GetChildSafe(node, 0)),
                        new BinaryOpNode(
                            Build(ITreeTools.GetChildSafe(node, 0)),
                            new NumberNode<long>(1L, NodeType.Double, node),
                            type == ExpressionType.PreIncrementAssign
                                  ? ExpressionType.Add
                                  : ExpressionType.Subtract,
                            node
                        ),
                        node
                    );

                case ExpressionType.PostIncrementAssign:
                    return new PostfixOperatorNode(
                        Build(ITreeTools.GetChildSafe(node, 0)),
                        ExpressionType.PostIncrementAssign,
                        node
                    );

                case ExpressionType.PostDecrementAssign:
                    return new PostfixOperatorNode(
                        Build(ITreeTools.GetChildSafe(node, 0)),
                        ExpressionType.PostDecrementAssign,
                        node
                    );
            }

            throw new NotImplementedException();
        }

        private INode BuildLogicalNode(ITree node, ExpressionType op)
        {
            return new LogicalNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)),
                op,
                node
            );
        }

        private INode BuildUnuaryOp(ITree node, ExpressionType op)
        {
            return new UnaryOpNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                op,
                node
            );
        }

        private INode BuildBinaryOpAssign(ITree node, ExpressionType op)
        {
            return new AssignNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                new BinaryOpNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    Build(ITreeTools.GetChildSafe(node, 1)),
                    op,
                    node
                ),
                node
            );
        }

        private INode BuildBinaryOp(ITree node, ExpressionType op)
        {
            return new BinaryOpNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)),
                op,
                node
            );
        }

        private INode BuildReturn(ITree node)
        {
            if (node.ChildCount == 0)
                return new ReturnNode(new NullNode(node), node);

            return new ReturnNode(Build(ITreeTools.GetChildSafe(node, 0)), node);
        }

        private INode BuildVarAssign(ITree node)
        {
            List<INode> nodes = new List<INode>();

            for (int i = 0; i < node.ChildCount; ++i)
            {
                INode assignNode = Build(ITreeTools.GetChildSafe(node, i));

                if (assignNode is AssignNode)
                {
                    INode target = ((AssignNode)assignNode).Target;

                    if (target is IdentifierNode)
                        ((IdentifierNode)target).IsDefinition = true;
                }

                IdentifierNode identifierNode = assignNode as IdentifierNode;

                if (identifierNode != null)
                    identifierNode.IsDefinition = true;

                nodes.Add(assignNode);
            }

            if (nodes.Count == 1)
                return nodes[0];

            return new AssignmentBlockNode(nodes, true, node);
        }

        private INode BuildObject(ITree node)
        {
            var propertyDict = new Dictionary<string, INode>();

            ITreeTools.EachChild(node, delegate(ITree child) {
                propertyDict.Add(
                    ITreeTools.GetChildSafe(child, 0).Text.Trim('\'', '"'),
                    Build(ITreeTools.GetChildSafe(child, 1))
                );
            });

            return new ObjectNode(propertyDict, node);
        }

        private INode BuildWhile(ITree node)
        {
            var testNode = Build(ITreeTools.GetChildSafe(node, 0));
            var bodyNode = Build(ITreeTools.GetChildSafe(node, 1));

            return new WhileNode(
                testNode, 
                bodyNode,
                WhileType.While,
                node
            );
        }

        private INode BuildFunction(ITree node)
        {
            if (node.ChildCount > 2)
            {
                return BuildLambda(
                    ITreeTools.GetChildSafe(node, 1),
                    ITreeTools.GetChildSafe(node, 2),
                    ITreeTools.GetChildSafe(node, 0).Text,
                    node
                );
            }
            else
            {
                return BuildLambda(
                    ITreeTools.GetChildSafe(node, 0),
                    ITreeTools.GetChildSafe(node, 1), 
                    null,
                    node
                );
            }
        }

        private INode BuildLambda(ITree args, ITree body, string name, ITree node)
        {
            return new FuncNode(
                (name == null)
                    ? null 
                    : new IdentifierNode(name, args),
                    ITreeTools.Map(args, delegate(ITree child){
                        return child.Text;
                    }),
                BuildBlock(body), 
                node
            );
        }

        private INode BuildNew(ITree node)
        {
            return new NewNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                node
            );
        }

        private INode BuildString(ITree node)
        {
            return new StringNode(
                node.Text.Substring(1, node.Text.Length - 2),
                node.Text[0],
                node
            );
        }

        private INode BuildBlock(ITree node)
        {
            List<INode> nodes = new List<INode>();

            ITreeTools.EachChild(node, delegate(ITree child){
                nodes.Add(Build(child));
            });

            return new BlockNode(nodes, node);
        }

        private INode BuildIf(ITree node)
        {
            return new IfNode(
                Build(ITreeTools.GetChildSafe(node, 0)),
                Build(ITreeTools.GetChildSafe(node, 1)), 
                Build(node.GetChild(2)), // can be null
                node.Type == EcmaParser.QUE,
                node
            );
        }

        private INode BuildMemberAccess(ITree node)
        {
            if (RewriteIfContainsNew(ITreeTools.GetChildSafe(node, 0)))
            {
                return new NewNode(Build(node), node);
            }
            else
            {
                return new MemberAccessNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    ITreeTools.GetChildSafe(node, 1).Text,
                    node
                );
            }
        }

        private INode BuildCall(ITree node)
        {
            if (RewriteIfContainsNew(ITreeTools.GetChildSafe(node, 0)))
            {
                return new NewNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    ITreeTools.Map(
                        ITreeTools.GetChildSafe(node, 1),
                        delegate(ITree child) {
                            return Build(child);
                        }
                    ),
                    node
                );
            }
            else
            {
                return new CallNode(
                    Build(ITreeTools.GetChildSafe(node, 0)),
                    ITreeTools.Map(
                        ITreeTools.GetChildSafe(node, 1),
                        delegate(ITree child)
                        {
                            return Build(child);
                        }
                    ),
                    node
                );
            }
        }

        private INode BuildNull(ITree node)
        {
            return new NullNode(node);
        }

        private INode BuildNumber(ITree node)
        {
            if (node.Text.Contains("."))
                return new NumberNode<double>(double.Parse(node.Text, CultureInfo.InvariantCulture), NodeType.Double, node);
            else
                return new NumberNode<long>(long.Parse(node.Text, CultureInfo.InvariantCulture), NodeType.Integer, node);
        }

        private INode BuildIdentifier(ITree node)
        {
            return new IdentifierNode(node.Text, node);
        }

        private INode BuildAssign(ITree node, bool isLocal)
        {
            var lhs = ITreeTools.GetChildSafe(node, 0);
            var rhs = ITreeTools.GetChildSafe(node, 1);

            return new AssignNode(
                Build(lhs), 
                Build(rhs),
                node
            );
        }

        private bool RewriteIfContainsNew(ITree node)
        {
            while (node != null)
            {
                if (node.Type == EcmaParser.NEW)
                {
                    var child = ITreeTools.GetChildSafe(node, 0);

                    var idNode = new CommonTree(
                        new CommonToken(
                            child.Type,
                            child.Text
                        )
                    );

                    node.Parent.ReplaceChildren(0, 0, idNode);
                    return true;
                }

                if (node.Type == EcmaParser.CALL)
                    return false;

                if (node.Type == EcmaParser.PAREXPR)
                    return false;

                node = node.GetChild(0);
            }

            return false;
        }

        static internal string Name(int type)
        {
            return EcmaParser.tokenNames[type];
        }

        static internal string Name(ITree node)
        {
            return Name(node.Type);
        }
    }
}