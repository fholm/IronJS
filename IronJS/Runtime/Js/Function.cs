using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime
{
    public class Function : Obj, IFunction
    {
        public Function(IFrame frame, Lambda lambda)
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

        public IObj Construct()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
