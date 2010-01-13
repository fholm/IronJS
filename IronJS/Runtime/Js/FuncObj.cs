using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime
{
    public class FuncObj : IFunction
    {
        public FuncObj(IFrame frame, Lambda lambda)
        {
            Frame = frame;
            Lambda = lambda;
        }

        #region IFunction Members

        public IFrame Frame
        {
            get;
            set;
        }

        public Lambda Lambda
        {
            get;
            set;
        }

        #endregion

        #region IObj Members

        public ObjClass Class
        {
            get;
            set;
        }

        public IObj Prototype
        {
            get;
            set;
        }

        public Context Context
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public IObj Construct()
        {
            throw new NotImplementedException();
        }

        public object Get(object name)
        {
            throw new NotImplementedException();
        }

        public object Put(object name, object value)
        {
            throw new NotImplementedException();
        }

        public bool CanPut(object name)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(object name)
        {
            throw new NotImplementedException();
        }

        public bool Delete(object name)
        {
            throw new NotImplementedException();
        }

        public object DefaultValue(ValueHint hint)
        {
            throw new NotImplementedException();
        }

        public bool HasOwnProperty(object name)
        {
            throw new NotImplementedException();
        }

        public object SetOwnProperty(object name)
        {
            throw new NotImplementedException();
        }

        public object GetOwnProperty(object name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
