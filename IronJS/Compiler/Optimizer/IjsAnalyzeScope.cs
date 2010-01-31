using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class IjsAnalyzeScope
    {
        public IjsAnalyzeScope Parent { get; protected set; }
        public IjsFuncInfo FuncInfo { get; protected set; }
        public Dictionary<string, IjsVarInfo> Variables { get; protected set; }

        public IjsAnalyzeScope(IjsAnalyzeScope parent, IjsFuncInfo funcInfo)
        {
            Parent = parent;
            FuncInfo = funcInfo;
            Variables = new Dictionary<string, IjsVarInfo>();
        }

        public IjsAnalyzeScope Enter(IjsFuncInfo funcInfo)
        {
            return new IjsAnalyzeScope(this, funcInfo);
        }

        public IjsAnalyzeScope Exit()
        {
            return Parent;
        }

        public IjsVarInfo CreateVariable(string name)
        {
            if (Variables.ContainsKey(name))
                throw new AstCompilerError("A variable named {0} already exists", name);

            Variables.Add(name, new IjsVarInfo(name));
            return Variables[name];
        }

        public bool GetVariable(string name, out IjsVarInfo variable)
        {
            if (Variables.TryGetValue(name, out variable))
                return true;

            if (Parent != null)
            {
                if (Parent.GetVariable(name, out variable))
                {
                    variable.IsClosedOver = true;
                    return true;
                }
            }

            variable = null;
            return false;
        }
    }
}
