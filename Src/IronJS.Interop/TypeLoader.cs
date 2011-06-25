namespace IronJS.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class TypeLoader
    {
        public static CommonObject Require(FunctionObject func, CommonObject @this, string uriString)
        {
            uriString = uriString ?? string.Empty;
            var uri = new Uri(uriString);

            switch (uri.Scheme)
            {
                case "clr":
                    return RequireClr(func, @this, uri);
                default:
                    throw new UriFormatException("Unexpected Uri scheme '" + uri.Scheme + "'.");
            }
        }

        private static CommonObject RequireClr(FunctionObject func, CommonObject @this, Uri uri)
        {
            var requireInfo = ParseClrUri(uri);
            var typeName = requireInfo.Item1;
            var assembly = requireInfo.Item2;

            var type = Type.GetType(typeName);

            return TypeWrapper.Create(func.Env, type);
        }

        private static Tuple<string, string> ParseClrUri(Uri uri)
        {
            var parts = uri.PathAndQuery.Split(';');
            var typeName = parts[0].Trim();

            if (string.IsNullOrEmpty(typeName))
            {
                throw new UriFormatException("CLR namespace URIs must specify a type name.");
            }

            var assembly = (string)null;

            for (int i = 1; i < parts.Length; i++)
            {
                var paramParts = parts[i].Split(new[] { '=' }, 2);
                var parameter = paramParts[0].Trim();

                switch (parameter)
                {
                    case "Assembly":
                        if (paramParts.Length != 2)
                        {
                            throw new UriFormatException("The assembly parameter requires the name of an assembly from which to load types.");
                        }

                        var value = paramParts[1].Trim();
                        if (string.IsNullOrEmpty(value))
                        {
                            throw new UriFormatException("The assembly parameter requires the name of an assembly from which to load types.");
                        }

                        if (assembly != null)
                        {
                            throw new UriFormatException("It is invalid to specify an assembly more than once.");
                        }

                        assembly = value;

                        break;
                    default:
                        throw new UriFormatException("The parameter '" + parameter + "' was unexpected at this time.");
                }
            }

            return Tuple.Create(typeName, assembly);
        }
    }
}
