// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Class that carries a status code result as <see cref="IFormatString"/>.
    /// </summary>
    public class StatusFormatString : IFormatString
    {
        static IPlaceholder[] no_arguments = new IPlaceholder[0];
        static IFormatStringPart[] no_parts = new IFormatStringPart[0];
        static StatusFormatString _null = new StatusFormatString(null, LineStatus.FormatFailedNull);
        static StatusFormatString _no_parser = new StatusFormatString(null, LineStatus.FormatFailedNoParser);
        static StatusFormatString _parse_failed = new StatusFormatString(null, LineStatus.FormatFailedParse);

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static StatusFormatString Null => _null;

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static StatusFormatString ParseFailed => _parse_failed;

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static StatusFormatString NoParser => _no_parser;

        /// <summary>
        /// Get the status
        /// </summary>
        public LineStatus Status { get; internal set; }

        /// <summary>
        /// Get text
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// Get the parts
        /// </summary>
        public IFormatStringPart[] Parts => no_parts;

        /// <summary>
        /// Get arguments
        /// </summary>
        public IPlaceholder[] Placeholders => no_arguments;

        /// <summary>
        /// Get format provider.
        /// </summary>
        public IFormatProvider FormatProvider => null;

        /// <summary>
        /// Crate string for status
        /// </summary>
        /// <param name="text"></param>
        /// <param name="status"></param>
        public StatusFormatString(string text, LineStatus status)
        {
            this.Status = status;
            this.Text = text;
        }

    }
}
