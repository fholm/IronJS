using IronJS.Ast.Nodes;

namespace IronJS.Ast {
    public static class Node {
        public static Local Parameter(string name) {
            return new Param(name);
        }

        public static Local Variable(string name) {
            return new Local(name, NodeType.Local);
        }

        public static Enclosed Enclosed(Lambda lambda, string name) {
            return new Enclosed(name);
        }
    }
}
