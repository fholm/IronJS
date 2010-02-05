using System;

namespace IronJS
{
    public abstract class Error : Exception
    {
        internal Error(string msg)
            : base(msg)
        {

        }
    }
}
