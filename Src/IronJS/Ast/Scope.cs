using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Ast.Nodes;
using System.Collections;

namespace IronJS.Ast {
    public class Scope : IEnumerable {
        public Dictionary<string, Variable>.ValueCollection All {
            get { return _variables.Values; }
        }

        public int ParameterCount {
            get { return Parameters.Count; }
        }

        public List<Local> Locals { get; protected set; }
        public List<Local> Parameters { get; protected set; }
        public List<Enclosed> Enclosed { get; protected set; }

        Dictionary<string, Variable> _variables;

        public Scope() {
            Locals = new List<Local>();
            Parameters = new List<Local>();
            Enclosed = new List<Enclosed>();
            _variables = new Dictionary<string, Variable>();
        }

        public Variable Get(string name) {
            return _variables[name];
        }

        public bool Get(string name, out Variable var) {
            return _variables.TryGetValue(name, out var);
        }

        public INode Add(Variable variable) {
            if (_variables.ContainsKey(variable.Name))
                throw new AstError("A variable named '" + variable.Name + "' already exist");

            if (variable.NodeType == NodeType.Local) {
                Locals.Add((Local)variable);
            } else if (variable.NodeType == NodeType.Param) {
                Parameters.Add((Local)variable);
            } else if (variable.NodeType == NodeType.Closed) {
                Enclosed.Add((Enclosed)variable);
            } else {
                throw new AstError("Unkown variable type");
            }

            return (INode) (_variables[variable.Name] = variable);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return _variables.Values.GetEnumerator();
        }

        #endregion
    }
}
