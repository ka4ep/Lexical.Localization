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
    public class LineLocalizationValuePart : ILineValue, ILinePart
    {
        public IFormulationString Value { get; internal set; }
        public ILinePart PreviousPart { get; internal set; }
        public ILinePartAppender Appender { get; internal set; }

        public LineLocalizationValuePart(ILinePart previousPart, ILinePartAppender appender, IFormulationString localizationString)
        {
            Appender = appender;
            PreviousPart = previousPart;
            Value = localizationString;
        }
    }

    public class StringLocalizerLocalizationValuePart : StringLocalizerKey, ILineValue, ILinePart
    {
        public IFormulationString Value { get; internal set; }
        public ILinePart PreviousPart { get; internal set; }
        public ILinePartAppender Appender { get; internal set; }

        public StringLocalizerLocalizationValuePart(ILinePartAppender appender, ILinePart previousPart, IFormulationString localizationString) : base(appender, previousPart, null)
        {
            Value = localizationString;
        }
    }
}
