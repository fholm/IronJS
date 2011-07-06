using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public class FinallyBreakJump : Exception
    {
        public int LabelId
        {
            get;
            private set;
        }

        public FinallyBreakJump(int labelId)
            : base()
        {
            LabelId = labelId;
        }
    }

    public class FinallyContinueJump : Exception
    {
        public int LabelId
        {
            get;
            private set;
        }

        public FinallyContinueJump(int labelId)
            : base()
        {
            LabelId = labelId;
        }
    }

    public class FinallyReturnJump : Exception
    {
        public BoxedValue Value
        {
            get;
            private set;
        }

        public FinallyReturnJump(BoxedValue value)
            : base()
        {
            Value = value;
        }
    }
}
