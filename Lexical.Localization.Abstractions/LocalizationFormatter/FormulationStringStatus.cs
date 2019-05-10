// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           10.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Formulation string for error value.
    /// </summary>
    public class FormulationStringStatus : IFormulationString
    {
        static IFormulationStringArgument[] no_arguments = new IFormulationStringArgument[0];
        static IFormulationStringPart[] no_parts = new IFormulationStringPart[0];
        static FormulationStringStatus _null = new FormulationStringStatus(null, LocalizationStatus.FormulationFailedNull);
        static FormulationStringStatus _no_parser = new FormulationStringStatus(null, LocalizationStatus.FormulationFailedNoParser);
        static FormulationStringStatus _parse_failed = new FormulationStringStatus(null, LocalizationStatus.FormulationFailedParse);

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static FormulationStringStatus Null => _null;

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static FormulationStringStatus ParseFailed => _parse_failed;

        /// <summary>
        /// Status for null value.
        /// </summary>
        public static FormulationStringStatus NoParser => _no_parser;

        /// <summary>
        /// Get the status
        /// </summary>
        public LocalizationStatus Status { get; internal set; }

        /// <summary>
        /// Get text
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// Get the parts
        /// </summary>
        public IFormulationStringPart[] Parts => no_parts;

        /// <summary>
        /// Get arguments
        /// </summary>
        public IFormulationStringArgument[] Arguments => no_arguments;

        /// <summary>
        /// Get format provider.
        /// </summary>
        public IFormatProvider FormatProvider => null;

        /// <summary>
        /// Crate string for status
        /// </summary>
        /// <param name="text"></param>
        /// <param name="status"></param>
        public FormulationStringStatus(string text, LocalizationStatus status)
        {
            this.Status = status;
            this.Text = text;
        }

    }
}
