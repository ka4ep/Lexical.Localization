// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary></summary>
    public static partial class LineExtensions
    {
        /// <summary>
        /// Append single enum case as "Key" parameter. 
        /// 
        /// Don't use this if the enum is flags.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="line"></param>
        /// <param name="enumCase"></param>
        /// <returns></returns>
        public static ILine Enum<T>(this ILine line, T enumCase)
            => line.Key(enumCase.ToString());

        /// <summary>
        /// Resolve each flag of enum type and compose into a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <param name="enumFlags"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ResolveEnumFlags<T>(this ILine line, T enumFlags, string separator)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (LineString str in ResolveEnumFlags(line, enumFlags))
            {
                if (count++ > 0) sb.Append(separator);
                sb.Append(str.Value);
            }
            return sb.ToString();
        }


        /// <summary>
        /// Resolve each flag of enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <param name="enumFlags"></param>
        /// <returns></returns>
        public static IEnumerable<LineString> ResolveEnumFlags<T>(this ILine line, T enumFlags)
        {
            Type enumType = typeof(T);
            Type underlyingType = System.Enum.GetUnderlyingType(enumType);
            if (typeof(Int32).Equals(underlyingType) || typeof(Int64).Equals(underlyingType) || typeof(Int16).Equals(underlyingType) || typeof(SByte).Equals(underlyingType))
                return resolveEnumFlagsSigned(line, enumType, ((IConvertible)enumFlags).ToInt64(null));
            if (typeof(UInt32).Equals(underlyingType) || typeof(UInt64).Equals(underlyingType) || typeof(UInt16).Equals(underlyingType) || typeof(Byte).Equals(underlyingType))
                return resolveEnumFlagsUnsigned(line, enumType, ((IConvertible)enumFlags).ToUInt64(null));
            throw new ArgumentException(nameof(T));
        }

        static IEnumerable<LineString> resolveEnumFlagsSigned(this ILine line, Type enumType, long enumFlags)
        {
            string[] keys = System.Enum.GetNames(enumType);
            Array valueArray = System.Enum.GetValues(enumType);
            int count = valueArray.Length;
            long[] values = new long[count];

            // TODO: Optimize this. Store the array in ILineEnum?
            for (int i = 0; i < count; i++) values[i] = ((IConvertible)valueArray.GetValue(i)).ToInt64(null);

            while (enumFlags != default)
            {
                bool foundCase = false;
                for (int i = 0; i < valueArray.Length; i++)
                {
                    long value = values[i];
                    if ((enumFlags & value) != 0)
                    {
                        foundCase = true;
                        LineString str = line.Key(keys[i]).ResolveString();
                        if (str.Value == null) str = new LineString(line, keys[i], LineStatus.ResolveErrorNoMatch);
                        yield return str;
                        enumFlags &= ~value;
                        break;
                    }
                }

                if (!foundCase)
                {

                    yield return new LineString(line, enumFlags.ToString(), LineStatus.ResolveErrorNoMatch);
                    yield break;
                }
            }
        }

        static IEnumerable<LineString> resolveEnumFlagsUnsigned(this ILine line, Type enumType, ulong enumFlags)
        {
            string[] keys = System.Enum.GetNames(enumType);
            Array valueArray = System.Enum.GetValues(enumType);
            int count = valueArray.Length;
            ulong[] values = new ulong[count];
            // TODO: Optimize this. Store the array in ILineEnum?
            for (int i = 0; i < count; i++) values[i] = ((IConvertible)valueArray.GetValue(i)).ToUInt64(null);

            while (enumFlags != default)
            {
                bool foundCase = false;
                for (int i = 0; i < valueArray.Length; i++)
                {
                    ulong value = values[i];
                    if ((enumFlags & value) != 0)
                    {
                        foundCase = true;
                        LineString str = line.Key(keys[i]).ResolveString();
                        if (str.Value == null) str = new LineString(line, keys[i], LineStatus.ResolveErrorNoMatch);
                        enumFlags &= ~value;
                        break;
                    }
                }

                if (!foundCase)
                {

                    yield return new LineString(line, enumFlags.ToString(), LineStatus.ResolveErrorNoMatch);
                    yield break;
                }
            }
        }

        /// <summary>
        /// Inline string value to an enum case. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <param name="enumCase"></param>
        /// <param name="string">enum description in the current active StringFormat. If no string format is present, c# format is used</param>
        /// <returns></returns>
        public static ILine InlineEnum<T>(this ILine line, T enumCase, IString @string)
        {
            ILineInlines inlines;
            ILine result = line.GetOrCreateInlines(out inlines);
            ILine key = line.Key(enumCase.ToString());
            inlines[key] = key.String(@string);
            return result;
        }

        /// <summary>
        /// Inline string value to an enum case. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <param name="enumCase"></param>
        /// <param name="string">enum description in the current active StringFormat. If no string format is present, c# format is used</param>
        /// <returns></returns>
        public static ILine InlineEnum<T>(this ILine line, T enumCase, String @string)
        {
            ILineInlines inlines;
            ILine result = line.GetOrCreateInlines(out inlines);
            ILine key = line.Key(enumCase.ToString());
            inlines[key] = key.Text(@string);
            return result;
        }

        /// <summary>
        /// Inline each case of <typeparamref name="T"/> to default string using correspoding enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <returns></returns>
        public static ILine InlineEnum<T>(this ILine line)
        {
            ILineInlines inlines;
            ILine result = line.GetOrCreateInlines(out inlines);
            foreach (string name in System.Enum.GetNames(typeof(T)))
            {
                ILine key = line.Key(name);
                inlines[key] = key.Text(name);
            }
            return result;
        }

        /// <summary>
        /// Inline case of <paramref name="enumType"/> to default string using correspoding enum value.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static ILine InlineEnum(this ILine line, Type enumType)
        {
            ILineInlines inlines;
            ILine result = line.GetOrCreateInlines(out inlines);
            foreach (string name in System.Enum.GetNames(enumType))
            {
                ILine key = line.Key(name);
                inlines[key] = key.Text(name);
            }
            return result;
        }

        /// <summary>
        /// Inline <paramref name="culture"/> specific <paramref name="text"/> to <paramref name="enumCase"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <param name="enumCase"></param>
        /// <param name="culture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ILine InlineEnum<T>(this ILine line, T enumCase, string culture, string text)
        {
            ILineInlines inlines;
            ILine result = line.GetOrCreateInlines(out inlines);
            ILine key = line.Key(enumCase.ToString());
            if (culture != null) key = key.Culture(culture);
            inlines[key] = key.Text(text);
            return result;
        }

    }

}
