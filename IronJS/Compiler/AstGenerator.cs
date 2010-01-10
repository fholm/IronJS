using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using IronJS.Extensions;
using IronJS.Compiler;

namespace IronJS.Compiler.Ast
{
    using EcmaLexer = IronJS.Compiler.Parser.ES3Lexer;
    using EcmaParser = IronJS.Compiler.Parser.ES3Parser;
    using System.Linq.Expressions;

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

            if (root.IsNil)
            {
                root.EachChild(node => 
                {
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
                case EcmaParser.THIS:
                case EcmaParser.Identifier:
                    return BuildIdentifier(node);

                case EcmaParser.PAREXPR:
                    return Build(node.GetChildSafe(0));

                case EcmaParser.CALL:
                    return BuildCall(node);

                case EcmaParser.BYFIELD:
                    return BuildMemberAccess(node);

                case EcmaParser.IF:
                    return BuildIf(node);

                case EcmaParser.BLOCK:
                    return BuildBlock(node);

                case EcmaParser.NEW:
                    return BuildNew(node);

                case EcmaParser.FUNCTION:
                    return BuildFunction(node);

                case EcmaParser.WHILE:
                    return BuildWhile(node);

                case EcmaParser.OBJECT:
                    return BuildObject(node);

                case EcmaParser.RETURN:
                    return BuildReturn(node);

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

                /*
                //TODO: add support for !== and ===
                // 1 === 2
                case EcmaParser.SAME:
                    return BuildBinaryOp(node, ExpressionType.EqEq);

                // 1 !=== 2
                case EcmaParser.NSAME:
                    return BuildBinaryOp(node, ExpressionType.NotEqEq);
                */

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

                /* TODO: add support for >>>
                // 1 >>> 2
                case EcmaParser.SHU:
                    return BuildBinaryOp(node, ExpressionType.ShiftRightZero);
                */

                // foo >>= 1
                case EcmaParser.SHRASS:
                    return BuildBinaryOpAssign(node, ExpressionType.RightShift);

                // foo <<= 1
                case EcmaParser.SHLASS:
                    return BuildBinaryOpAssign(node, ExpressionType.LeftShift);

                /* TODO: add support for >>>=
                // foo >>>= 1
                case EcmaParser.SHUASS:
                    return BuildBinaryOp(node, ExpressionType.ShiftRightZero);
                */

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

                //
                default:
                    throw new Compiler.CompilerError(
                        "Unrecognized token '{0}'", 
                        node, 
                        Name(node)
                    );
            }
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
                    break;

                case ExpressionType.PostIncrementAssign:
                    return new PostfixOperatorNode(
                        Build(node.GetChildSafe(0)),
                        ExpressionType.PostIncrementAssign
                    );
                    break;

                case ExpressionType.PostDecrementAssign:
                    return new PostfixOperatorNode(
                        Build(node.GetChildSafe(0)),
                        ExpressionType.PostDecrementAssign
                    );
                    break;
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
                        x.GetChildSafe(0).Text, 
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
                bodyNode
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
                (BlockNode) Build(node.GetChildSafe(1)), 
                Build(node.GetChild(2)) as BlockNode // can be null
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

                return new CallNode(
                    Build(callTree), 
                    argsTree.Map(x => { return Build(x); })
                );
            }
        }

        private Node BuildNull(ITree node)
        {
            return new NullNode();
        }

        private Node BuildNumber(ITree node)
        {
            return new NumberNode(Double.Parse(node.Text));
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
