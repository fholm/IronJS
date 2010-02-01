using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class IjsAnalyzeScope
    {
        public IjsAnalyzeScope Parent { get; protected set; }
        public IjsFuncInfo FuncInfo { get; protected set; }

        public IjsAnalyzeScope(IjsAnalyzeScope parent, IjsFuncInfo funcInfo)
        {
            Parent = parent;
            FuncInfo = funcInfo;
            _variable = new Dictionary<string, IjsVarInfo>();
        }

        public IjsAnalyzeScope Enter(IjsFuncInfo funcInfo)
        {
            return new IjsAnalyzeScope(this, funcInfo);
        }

        public IjsAnalyzeScope Exit()
        {
            return Parent;
        }

        Dictionary<string, IjsVarInfo> _variable;
        public bool HasVariable(string name)
        {
            return _variable.ContainsKey(name);
        }

        public IjsVarInfo CreateVariable(string name)
        {
            if (HasVariable(name))
                throw new AstCompilerError("A variable named {0} already exists", name);

            _variable.Add(name, new IjsVarInfo(name));
            return _variable[name];
        }

        public bool GetVariable(string name, out IjsVarInfo variable)
        {
            if (_variable.TryGetValue(name, out variable))
                return true;

            var parent = Parent;
            while (parent != null)
            {
                if (parent._variable.TryGetValue(name, out variable))
                {
                    if (parent.Parent != null)
                        variable.IsClosedOver = true;

                    return true;
                }

                parent = parent.Parent;
            }

            variable = null;
            return false;
        }
    }
}
