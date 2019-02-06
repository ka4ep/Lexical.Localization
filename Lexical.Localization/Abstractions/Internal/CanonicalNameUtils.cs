// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Internal
{
    public static class CanonicalNameUtils
    {
        /// <summary>
        /// Print full canonical name for type including generic parameters.
        /// 
        /// e.g. <see cref="List{T}"/> -> "System.Collections.Generics.List&lt;System.Int&gt;".
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns>Full canonical name</returns>
        public static string CanonicalName(this Type type, CanonicalNameOptions options = CanonicalNameOptions.Default)
        {
            if (type == null) return "";
            StringBuilder sb = new StringBuilder(canonicalNameLength(type, options));
            AppendCanonicalName(type, sb, options);
            return sb.ToString();
        }

        public static void AppendCanonicalName(Type type, StringBuilder sb, CanonicalNameOptions options = CanonicalNameOptions.Default)
        {
            // Declaring Type "MyClass+Mutable"
            CanonicalNameOptions localOptions = options;
            if (type.DeclaringType != null) {
                AppendCanonicalName(type.DeclaringType, sb, options &~ (CanonicalNameOptions.IncludeAssembly|CanonicalNameOptions.IncludeGenerics) );
                sb.Append('+');
                localOptions = options & (CanonicalNameOptions.IncludeAssembly|CanonicalNameOptions.IncludeGenerics);
            }

            // Namespace: "System.Collections.Generic"
            if (((localOptions & CanonicalNameOptions.IncludeNamespace) != 0) && !type.IsGenericParameter && type.Namespace != null) { sb.Append(type.Namespace); sb.Append("."); }

            // Name
            //  without generics argument count `1 "SomeType`1"
            if (type.IsGenericType)
            {
                string name = type.Name ?? type.FullName;
                int ix = name.IndexOf('`');
                if (ix > 0) sb.Append(name, 0, ix);
                else sb.Append(name);
            }
            else
                sb.Append(type.Name);

            // Assembly
            if ((localOptions & CanonicalNameOptions.IncludeAssembly) != 0)
            {
                sb.Append('@');
                sb.Append(type.Assembly.GetName().Name);
            }

            // Generics<>
            if (((options & CanonicalNameOptions.IncludeGenerics) != 0) && type.IsGenericType)
            {
                sb.Append('<');

                // T, T2
                Type[] args = type.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    AppendCanonicalName(args[i], sb, options);
                }
                sb.Append('>');
            }
        }

        private static int canonicalNameLength(Type type, CanonicalNameOptions options = CanonicalNameOptions.Default)
        {
            int result = 0;

            // Declaring Type "MyClass+Mutable"
            CanonicalNameOptions localOptions = options;
            if (type.DeclaringType != null)
            {
                result += canonicalNameLength(type.DeclaringType, options & ~(CanonicalNameOptions.IncludeAssembly | CanonicalNameOptions.IncludeGenerics));
                result++;
                localOptions = options & (CanonicalNameOptions.IncludeAssembly | CanonicalNameOptions.IncludeGenerics);
            }

            // Namespace: "System.Collections.Generic"
            if (((localOptions & CanonicalNameOptions.IncludeNamespace) != 0) && !type.IsGenericParameter && type.Namespace != null) result += type.Namespace.Length + 1;

            // Name
            // Remove generics argument count `1 "SomeType`1"
            if (type.IsGenericType)
                result += type.Name.IndexOf('`');
            else
                result += type.Name.Length;

            // Assembly Name "@assembly"
            if ((localOptions & CanonicalNameOptions.IncludeAssembly) != 0)
            {
                result += 1 + type.Assembly.GetName().Name.Length;
            }

            // Generics<>
            if (((options & CanonicalNameOptions.IncludeGenerics) != 0) && type.IsGenericType)
            {
                // <>
                result += 2;

                // T, T2
                Type[] args = type.GetGenericArguments();

                // ,
                if (args.Length > 1) result += args.Length - 1;
                // T
                foreach (Type arg in args)
                    result += canonicalNameLength(arg);
            }

            return result;
        }

    }

    public class CanonicalName
    {
        public static Regex CanonicalPattern = new Regex(@"(?<name>[^@<]*)(@(?<assy>[^@<]*))?(<(?<arguments>.*)>)?", RegexOptions.Singleline|RegexOptions.Compiled|RegexOptions.ExplicitCapture);

        public string Name;
        public string AssemblyName;
        public List<CanonicalName> Arguments;

        public static CanonicalName FromType(Type type)
        {
            CanonicalName result = new CanonicalName();
            result.Name = type.CanonicalName(CanonicalNameOptions.IncludeNamespace);
            result.AssemblyName = type.Assembly.GetName().Name;
            if (type.IsGenericType)
            {
                Type[] args = type.GetGenericArguments();
                result.Arguments = new List<CanonicalName>(args.Length);
                for (int i = 0; i < args.Length; i++)
                    result.Arguments.Add( FromType(args[i]) );
            }
            return result;
        }

        public static CanonicalName Parse(String name)
        {
            int ix = 0;
            return Parse(name, ref ix, name.Length);
        }

        public static CanonicalName Parse(string name, ref int index, int length)
        {
            Match m = CanonicalPattern.Match(name, index, length);
            if (!m.Success) throw new ArgumentException();
            CanonicalName result = new CanonicalName();
            result.Name = m.Groups["name"].Value;
            result.AssemblyName = m.Groups["assy"].Value;
            Group argumentGroup = m.Groups["arguments"];
            if (argumentGroup.Success)
            {
                result.Arguments = new List<CanonicalName>(1);
                for (int ix=argumentGroup.Index; ix<argumentGroup.Index+argumentGroup.Length-1; )
                {
                    CanonicalName arg = Parse(name, ref ix, argumentGroup.Length);
                    result.Arguments.Add(arg);
                    if (ix == argumentGroup.Index + argumentGroup.Length) break;
                    if (ix >= name.Length) throw new ArgumentException($"Unexpected end of char stream at {ix}, for string \"{name}\".");
                    char ch = name[ix];
                    if (ch == ',') { /*Good*/}
                    else throw new ArgumentException($"Unexpected character '{ch}' at index {ix}, when expecting ','.");
                }
            }
            index = m.Index + m.Length;
            return result;
        }

        public Type BuildType()
        {
            Assembly asm = Assembly.Load(AssemblyName);
            string typename = Name;

            // Get non-generics type
            if (Arguments == null || Arguments.Count == 0)
            {
                return asm.GetType(typename);
            }

            // Get generics type
            int count = Arguments.Count;
            Type genericsType = asm.GetType(typename + "`" + count);
            Type[] argTypes = Arguments.Select(arg => arg.BuildType()).ToArray();
            return genericsType.MakeGenericType(argTypes);
        }

        public void Visit(Action<CanonicalName> visitor)
        {
            visitor(this);
            if (Arguments != null) foreach (var arg in Arguments) arg.Visit(visitor);
        }

        public void AppendToString(StringBuilder sb)
        {
            if (Name != null) sb.Append(Name);
            if (AssemblyName != null) { sb.Append('@'); sb.Append(AssemblyName); }
            if (Arguments!=null && Arguments.Count>=0)
            {
                sb.Append('<');
                for (int i=0; i<Arguments.Count; i++)
                {
                    Arguments[i].AppendToString(sb);
                    if (i > 0) sb.Append(',');
                }
                sb.Append('>');
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendToString(sb);
            return sb.ToString();
        }
    }

    [Flags]
    public enum CanonicalNameOptions : UInt32
    {
        None = 0,
        All = 0xffffffff,
        IncludeNamespace = 1,
        IncludeGenerics = 2,
        IncludeAssembly = 4,
        Default = IncludeNamespace | IncludeGenerics,
    }

}
