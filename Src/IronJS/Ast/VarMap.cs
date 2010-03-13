using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Ast.Nodes;
using System.Collections;

namespace IronJS.Ast {
    public class VarMap : IEnumerable {
        public Dictionary<string, Variable>.ValueCollection All {
            get { return _variables.Values; }
        }

        public Local[] Locals {
            get { return _locals.ToArray(); }
        }

        public Local[] Parameters {
            get { return _parameters.ToArray(); }
        }

        public Enclosed[] Enclosed {
            get { return _enclosed.ToArray(); }
        }

        List<Local> _locals;
        List<Local> _parameters;
        List<Enclosed> _enclosed;
        Dictionary<string, Variable> _variables;

        public VarMap() {
            _locals = new List<Local>();
            _parameters = new List<Local>();
            _enclosed = new List<Enclosed>();
            _variables = new Dictionary<string, Variable>();
        }

        public Variable Get(string name) {
            return _variables[name];
        }

        public bool Get(string name, out Variable var) {
            return _variables.TryGetValue(name, out var);
        }

        public INode Add(INode node) {
            Variable variable = (Variable)node;

            if (_variables.ContainsKey(variable.Name))
                throw new AstError("A variable named '" + variable.Name + "' already exist");

            if (variable.NodeType == NodeType.Local) {
                _locals.Add((Local)variable);
            } else if (variable.NodeType == NodeType.Param) {
                _parameters.Add((Local)variable);
            } else if (variable.NodeType == NodeType.Closed) {
                _enclosed.Add((Enclosed)variable);
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
