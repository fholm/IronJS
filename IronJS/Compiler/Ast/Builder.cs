using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using IronJS.Extensions;

namespace IronJS.Compiler.Ast
{
    using EcmaLexer = IronJS.Compiler.Parser.ES3Lexer;
    using EcmaParser = IronJS.Compiler.Parser.ES3Parser;

    public class Builder
    {
        public Builder()
        {

        }

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

                case EcmaParser.NULL:
                    return BuildNull(node);

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
                case EcmaParser.EQ:
                    return BuildBinaryOp(node, BinaryOp.Eq);

                case EcmaParser.LT:
                    return BuildBinaryOp(node, BinaryOp.Lt);

                case EcmaParser.ADD:
                    return BuildBinaryOp(node, BinaryOp.Add);

                //
                default:
                    throw new Compiler.CompilerError("Unrecognized token '{0}'", node, Name(node));
            }
        }

        private Node BuildReturn(ITree node)
        {
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

            return new WhileNode(testNode, bodyNode);
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

        private Node BuildBinaryOp(ITree node, BinaryOp op)
        {
            return new BinaryOpNode(
                Build(node.GetChildSafe(0)), 
                Build(node.GetChildSafe(1)), 
                op
            );
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
