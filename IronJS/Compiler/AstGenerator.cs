using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Ast;
using IronJS.Extensions;

using EcmaLexer = IronJS.Compiler.Parser.ES3Lexer;
using EcmaParser = IronJS.Compiler.Parser.ES3Parser;

namespace IronJS.Compiler
{
    public class AstGenerator
    {
        public List<Node> Build(string fileName, Encoding encoding)
        {
            return Build(
                System.IO.File.ReadAllText(
                    fileName, 
                    encoding
                )
            );
        }

        public List<Node> Build(string source)
        {
            var lexer = new EcmaLexer(new ANTLRStringStream(source));
            var parser = new EcmaParser(new CommonTokenStream(lexer));

            var program = parser.program();
            var root = (ITree)program.Tree;

            var nodes = new List<Node>();

            root.Print();

            if (root.IsNil)
            {
                root.EachChild(node => {
                    nodes.Add(Build(node));
                });
            }
            else
            {
                nodes.Add(Build(root));
            }

            return nodes;
        }

        private Node Build(ITree node)
        {
            if (node == null)
                return null;

            switch (node.Type)
            {
                case EcmaParser.THIS: // 'this' is just an identifier
                case EcmaParser.Identifier:
                    return BuildIdentifier(node);

                case EcmaParser.PAREXPR:
                    return Build(node.GetChildSafe(0));

                case EcmaParser.CALL:
                    return BuildCall(node);


                case EcmaParser.IF:
                case EcmaParser.QUE: // <expr> ? <expr> : <expr>
                    return BuildIf(node);

                case EcmaParser.BLOCK:
                    return BuildBlock(node);

                case EcmaParser.NEW:
                    return BuildNew(node);

                case EcmaParser.FUNCTION:
                    return BuildFunction(node);

                case EcmaParser.OBJECT:
                    return BuildObject(node);

                case EcmaParser.RETURN:
                    return BuildReturn(node);

                case EcmaParser.EXPR:
                    return Build(node.GetChildSafe(0));

                case EcmaParser.WITH:
                    return BuildWith(node);

                case EcmaParser.TRY:
                    return BuildTry(node);

                case EcmaParser.CATCH:
                    return BuildCatch(node);

                case EcmaParser.FINALLY:
                    return BuildFinally(node);

                case EcmaParser.THROW:
                    return BuildThrow(node);

                /*
                 * Property access
                 */

                case EcmaParser.BYFIELD:
                    return BuildMemberAccess(node);

                case EcmaParser.BYINDEX:
                    return BuildIndexAccess(node);

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
                    return BuildVarAssign(node.GetChildSafe(0));

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
                    throw new Compiler.CompilerError(
                        "Unrecognized token '{0}'", 
                        node, 
                        Name(node)
                    );
            }
        }

        private Node BuildDelete(ITree node)
        {
            return new DeleteNode(Build(node.GetChildSafe(0)));
        }

        private Node BuildLabelled(ITree node)
        {
            var label = node.GetChildSafe(0);
            var target = Build(node.GetChildSafe(1));


            if (!(target is ILabelableNode))
                throw new CompilerError("Can only label nodes that implement ILabelableNode");

           (target as ILabelableNode).SetLabel(label.Text);
           return target;
        }

        private Node BuildIndexAccess(ITree node)
        {
            return new IndexAccessNode(
                Build(node.GetChildSafe(0)),
                Build(node.GetChildSafe(1))
            );
        }

        private Node BuildThrow(ITree node)
        {
            return new ThrowNode(
                Build(node.GetChildSafe(0))
            );
        }

        private Node BuildFinally(ITree node)
        {
            return BuildBlock(node.GetChildSafe(0));
        }

        private Node BuildCatch(ITree node)
        {
            return new CatchNode(
                Build(node.GetChildSafe(0)),
                Build(node.GetChildSafe(1))
            );
        }

        private Node BuildTry(ITree node)
        {
            if (node.ChildCount > 2)
            {
                return new TryNode(
                    Build(node.GetChildSafe(0)),
                    (CatchNode)Build(node.GetChildSafe(1)),
                    Build(node.GetChildSafe(2))
                );
            }
            else
            {
                var secondChild = node.GetChildSafe(1);

                if (secondChild.Type == EcmaParser.FINALLY)
                {
                    return new TryNode(
                        Build(node.GetChildSafe(0)),
                        null,
                        Build(node.GetChildSafe(1))
                    );
                }
                else
                {
                    return new TryNode(
                        Build(node.GetChildSafe(0)),
                        (CatchNode)Build(node.GetChildSafe(1)),
                        null
                    );
                }
            }
        }

        private Node BuildWith(ITree node)
        {
            return new WithNode(
                Build(node.GetChildSafe(0)),
                Build(node.GetChildSafe(1))
            );
        }

        private Node BuildDoWhile(ITree node)
        {
            var body = Build(node.GetChildSafe(0));
            var test = Build(node.GetChildSafe(1));

            return new WhileNode(
                test,
                body,
                WhileType.Do
            );
        }

        private Node BuildContinue(ITree node)
        {
            if (node.ChildCount == 0)
            {
                return new ContinueNode();
            }

            return new ContinueNode(node.GetChildSafe(0).Text);
        }

        // 12.8
        private Node BuildBreak(ITree node)
        {
            if (node.ChildCount == 0)
            {
                return new BreakNode(null);
            }

            return new BreakNode(node.GetChildSafe(0).Text);
        }

        private Node BuildFor(ITree node)
        {
            var body = Build(node.GetChildSafe(1));
            var type = node.GetChildSafe(0);

            // 12.6.3
            if (type.Type == EcmaParser.FORSTEP)
            {
                var init = type.GetChildSafe(0);
                var test = type.GetChildSafe(1);
                var incr = type.GetChildSafe(2);

                var initNode = init.ChildCount > 0
                             ? Build(init)
                             : new NullNode();

                var testNode = test.ChildCount > 0
                             ? Build(test)
                             : new BooleanNode(true);

                var incrNode = incr.ChildCount > 0
                             ? Build(incr)
                             : new NullNode();

                return new ForStepNode(
                    initNode,
                    testNode,
                    incrNode,
                    body
                );
            }
            // 12.6.4
            else if(type.Type == EcmaParser.FORITER)
            {
                return new ForInNode(
                    Build(type.GetChildSafe(0)),
                    Build(type.GetChildSafe(1)),
                    body
                );
            }

            throw new NotImplementedException();
        }

        private Node BuildUnsignedRightShiftAssign(ITree node)
        {
            return new AssignNode(
                Build(node.GetChildSafe(0)),
                new UnsignedRightShiftNode(
                    Build(node.GetChildSafe(0)),
                    Build(node.GetChildSafe(1))
                )
            );
        }

        private Node BuildUnsignedRightShift(ITree node)
        {
            return new UnsignedRightShiftNode(
                Build(node.GetChildSafe(0)),
                Build(node.GetChildSafe(1))
            );
        }

        private Node BuildStrictCompare(ITree node, ExpressionType type)
        {
            return new StrictCompareNode(
                Build(node.GetChildSafe(0)), 
                Build(node.GetChildSafe(1)),
                type
            );
        }

        private Node BuildVoidOp(ITree node)
        {
            return new VoidNode(Build(node.GetChildSafe(0)));
        }

        private Node BuildBoolean(ITree node)
        {
            return new BooleanNode(node.Type == 5);
        }

        private Node BuildTypeOfOp(ITree node)
        {
            return new TypeOfNode(Build(node.GetChildSafe(0)));
        }

        private Node BuildIncDecNode(ITree node, ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                    return new AssignNode(
                        Build(node.GetChildSafe(0)),
                        new BinaryOpNode(
                            Build(node.GetChildSafe(0)),
                            new NumberNode(1.0),
                            type == ExpressionType.PreIncrementAssign
                                  ? ExpressionType.Add
                                  : ExpressionType.Subtract
                        )
                    );

                case ExpressionType.PostIncrementAssign:
                    return new PostfixOperatorNode(
                        Build(node.GetChildSafe(0)),
                        ExpressionType.PostIncrementAssign
                    );

                case ExpressionType.PostDecrementAssign:
                    return new PostfixOperatorNode(
                        Build(node.GetChildSafe(0)),
                        ExpressionType.PostDecrementAssign
                    );
            }

            throw new NotImplementedException();
        }

        private Node BuildLogicalNode(ITree node, ExpressionType op)
        {
            return new LogicalNode(
                Build(node.GetChildSafe(0)),
                Build(node.GetChildSafe(1)),
                op
            );
        }

        private Node BuildUnuaryOp(ITree node, ExpressionType op)
        {
            return new UnaryOpNode(
                Build(node.GetChildSafe(0)),
                op
            );
        }

        private Node BuildBinaryOpAssign(ITree node, ExpressionType op)
        {
            return new AssignNode(
                Build(node.GetChildSafe(0)),
                new BinaryOpNode(
                    Build(node.GetChildSafe(0)),
                    Build(node.GetChildSafe(1)),
                    op
                )
            );
        }

        private Node BuildBinaryOp(ITree node, ExpressionType op)
        {
            return new BinaryOpNode(
                Build(node.GetChildSafe(0)),
                Build(node.GetChildSafe(1)),
                op
            );
        }

        private Node BuildReturn(ITree node)
        {
            if (node.ChildCount == 0)
                return new ReturnNode(new NullNode());

            return new ReturnNode(Build(node.GetChildSafe(0)));
        }

        private Node BuildVarAssign(ITree node)
        {
            var assignNode = Build(node);

            if (assignNode is AssignNode)
            {
                var target = ((AssignNode)assignNode).Target;

                if (target is IdentifierNode)
                    ((IdentifierNode)target).IsLocal = true;
            }

            return assignNode;
        }

        private Node BuildObject(ITree node)
        {
            var namedProps = node.Map(
                    x => new AutoPropertyNode(
                        x.GetChildSafe(0).Text.Trim('\'', '"'),
                        Build(x.GetChildSafe(1))
                    )   
                );

            return new NewNode(
                new IdentifierNode("Object"), 
                new List<Node>(), 
                namedProps
            );
        }

        private Node BuildWhile(ITree node)
        {
            var testNode = Build(node.GetChildSafe(0));
            var bodyNode = Build(node.GetChildSafe(1));

            return new WhileNode(
                testNode, 
                bodyNode,
                WhileType.While
            );
        }

        private Node BuildFunction(ITree node)
        {
            if (node.ChildCount > 2)
            {
                return new AssignNode(
                    Build(node.GetChildSafe(0)), 
                    BuildLambda(
                        node.GetChildSafe(1), 
                        node.GetChildSafe(2), 
                        node.GetChildSafe(0).Text
                    ));
            }
            else
            {
                return BuildLambda(
                    node.GetChildSafe(0), 
                    node.GetChildSafe(1), 
                    "<lambda>"
                );
            }
        }

        private Node BuildLambda(ITree argsNode, ITree block, string name)
        {
            var args = argsNode.Map(x => new IdentifierNode(x.Text));
            var body = BuildBlock(block);

            return new LambdaNode(args, body, name);
        }

        private Node BuildNew(ITree node)
        {
            var newNode = node.GetChildSafe(0);
            var argsNode = node.GetChildSafe(1);

            return new NewNode(
                Build(newNode.GetChildSafe(0)), 
                argsNode.Map( x => { return Build(x); })
            );
        }

        private Node BuildString(ITree node)
        {
            return new StringNode(
                node.Text.Substring(1, node.Text.Length - 2)
            );
        }

        private Node BuildBlock(ITree node)
        {
            var nodes = new List<Node>();

            node.EachChild( x => nodes.Add(Build(x)) );

            return new BlockNode(nodes);
        }

        private Node BuildIf(ITree node)
        {
            return new IfNode(
                Build(node.GetChildSafe(0)), 
                Build(node.GetChildSafe(1)), 
                Build(node.GetChild(2)) // can be null
            );
        }

        private Node BuildMemberAccess(ITree node)
        {
            return new MemberAccessNode(
                Build(node.GetChildSafe(0)), 
                node.GetChildSafe(1).Text
            );
        }

        private Node BuildCall(ITree node)
        {
            var callTree = node.GetChildSafe(0);

            if (callTree.Type == EcmaParser.NEW)
            {
                return BuildNew(node);
            }
            else
            {
                var argsTree = node.GetChildSafe(1);

                // we need to rewrite the tree 
                // if we have a new node nested
                // inside byfield/byindex nodes

                var firstChild = callTree;
                var foundNewNode = false;

                while (firstChild != null)
                {
                    if (firstChild.Type == EcmaParser.NEW)
                    {
                        var child = firstChild.GetChildSafe(0);

                        var idNode = new CommonTree(
                            new CommonToken(
                                child.Type, 
                                child.Text
                            )
                        );

                        firstChild.Parent.ReplaceChildren(0, 0, idNode);
                        foundNewNode = true;
                        break;
                    }

                    firstChild = firstChild.GetChild(0);
                }

                if (foundNewNode)
                {
                    // if we found a new-node and 
                    // rewrote the tree
                    return new NewNode(
                        Build(callTree),
                        argsTree.Map(x => Build(x))
                    );
                }
                else
                {
                    // if we fail, it's just a normal function call
                    return new CallNode(
                        Build(callTree),
                        argsTree.Map(x =>Build(x))
                    );
                }
            }
        }

        private Node BuildNull(ITree node)
        {
            return new NullNode();
        }

        private Node BuildNumber(ITree node)
        {
            return new NumberNode(Double.Parse(node.Text, CultureInfo.InvariantCulture));
        }

        private Node BuildIdentifier(ITree node)
        {
            return new IdentifierNode(node.Text);
        }

        private Node BuildAssign(ITree node, bool isLocal)
        {
            var lhs = node.GetChildSafe(0);
            var rhs = node.GetChildSafe(1);

            return new AssignNode(
                Build(lhs), 
                Build(rhs)
            );
        }

        static public string Name(int type)
        {
            return EcmaParser.tokenNames[type];
        }

        static public string Name(ITree node)
        {
            return Name(node.Type);
        }
    }
}
