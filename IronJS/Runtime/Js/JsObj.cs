using System.Collections.Generic;

namespace IronJS.Runtime.Js
{
    public class JsProperty
    {
        // Attributes
        public const int NO_ATTRS = 0;
        public const int READ_ONLY = 1;
        public const int DONT_ENUM = 2;
        public const int DONT_DELETE = 4;

        public int Attrs;
        public object Value;

        public JsProperty(object value, int attrs = 0)
        {
            Value = value;
            Attrs = attrs;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class JsObj
    {
        public Dictionary<object, JsProperty> Properties =
            new Dictionary<object, JsProperty>();

        public void Set(object name, object value)
        {
            JsProperty property;

            if (Properties.TryGetValue(name, out property))
            {
                if ((property.Attrs & JsProperty.READ_ONLY) == 1)
                    throw new PropertyIsReadOnly();

                property.Value = value;
                return;
            }

            Properties[name] = new JsProperty(value);
        }

        public object Get(object name)
        {
            JsProperty property;

            if (Properties.TryGetValue(name, out property))
                return property.Value;

            return Undefined.Instance;
        }
    }
}
