// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary></summary>
    public static partial class LineExtensions
    {
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
            ILine subline = line == inlines ? line.GetPreviousPart() : line;
            EnumInfo enumInfo = new EnumInfo(typeof(T));
            foreach (IEnumCase @case in enumInfo.Cases)
            {
                ILine key = subline.Key(@case.Name);
                inlines[key] = key.Text(@case.Description ?? @case.Name);
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
            ILine subline = line == inlines ? line.GetPreviousPart() : line;
            EnumInfo enumInfo = new EnumInfo(enumType);
            foreach (IEnumCase @case in enumInfo.Cases)
            {
                ILine key = subline.Key(@case.Name);
                inlines[key] = key.Text(@case.Description ?? @case.Name);
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
            ILine subline = line == inlines ? line.GetPreviousPart() : line;
            subline = subline.Key(enumCase.ToString());
            if (culture != null) subline = subline.Culture(culture);
            inlines[subline] = subline.Text(text);
            return result;
        }

    }

}
