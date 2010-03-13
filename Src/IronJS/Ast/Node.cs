using IronJS.Ast.Nodes;

namespace IronJS.Ast {
    public static class Node {
        public static Local Parameter(string name) {
            return new Local(name, NodeType.Param);
        }

        public static Local Variable(string name) {
            return new Local(name, NodeType.Local);
        }

        public static Enclosed Enclosed(Lambda lambda, string name) {
            return new Enclosed(lambda, name);
        }
    }
}
