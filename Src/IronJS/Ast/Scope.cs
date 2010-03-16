using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Ast.Nodes;
using System.Collections;

namespace IronJS.Ast {
    public class Scope : IEnumerable {
        public Dictionary<string, IVariable>.ValueCollection All {
            get { return Variables.Values; }
        }

        public int ParameterCount {
            get { return Parameters.Count; }
        }

        public List<Local> Locals { get; protected set; }
        public List<Param> Parameters { get; protected set; }
        public List<Enclosed> Enclosed { get; protected set; }

        protected Dictionary<string, IVariable> Variables;

        public Scope() {
            Locals = new List<Local>();
            Parameters = new List<Param>();
            Enclosed = new List<Enclosed>();
            Variables = new Dictionary<string, IVariable>();
        }

        public IVariable Get(string name) {
            return Variables[name];
        }

        public bool Get(string name, out IVariable var) {
            return Variables.TryGetValue(name, out var);
        }

        public INode Add(IVariable variable) {
            if (Variables.ContainsKey(variable.Name))
                throw new AstError("A variable named '" + variable.Name + "' already exist");

            if (variable is Param) {
                Parameters.Add((Param)variable);

            } else if (variable is Local) {
                Locals.Add((Local)variable);

            } else if (variable is Enclosed) {
				Enclosed.Add((Enclosed)variable);

            } else {
                throw new AstError("Unkown variable type");
            }

            return (INode) (Variables[variable.Name] = variable);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return Variables.Values.GetEnumerator();
        }

        #endregion
    }
}
