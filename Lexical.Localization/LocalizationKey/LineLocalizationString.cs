// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    public class LineLocalizationString : ILineLocalizationString, ILinePart
    {
        public IFormulationString LocalizationString { get; internal set; }
        public string Name => "Value";
        public ILinePart PreviousPart { get; internal set; }
        public ILinePartAppender Appender { get; internal set; }

        public LineLocalizationString(IFormulationString localizationString, ILinePart previousPart, ILinePartAppender appender)
        {
            PreviousPart = previousPart;
            Appender = appender;
            LocalizationString = localizationString;
        }
    }

    public class StringLocalizerLocalizationString : StringLocalizerKey, ILineLocalizationString, ILinePart
    {
        public IFormulationString LocalizationString { get; internal set; }
        public string Name => "Value";
        public ILinePart PreviousPart { get; internal set; }
        public ILinePartAppender Appender { get; internal set; }

        public StringLocalizerLocalizationString(IFormulationString localizationString, ILinePart previousPart, ILinePartAppender appender) : base(null, "Value")
        {
            PreviousPart = previousPart;
            Appender = appender;
            LocalizationString = localizationString;
        }
    }
}
