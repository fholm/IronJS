using System;

namespace IronJS.Compiler
{
    public class IjsClosureCell<T>
    {
        public T Value;
        public bool Deleted;
    }

    /*
    public class IjsDeletableCell<T>
    {
        public bool Deleted;

        T _value;
        public T Value
        {
            get
            {
                if (Deleted)
                    throw new ArgumentException("Variable is deleted");

                return _value;
            }
            set
            {
                Deleted = false;
                _value = value;
            }
        }
    }
    */
}