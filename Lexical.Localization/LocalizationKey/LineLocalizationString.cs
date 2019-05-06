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
    public class LineLocalizationValuePart : ILineValue, ILine
    {
        public IFormulationString Value { get; internal set; }
        public ILine PreviousPart { get; internal set; }
        public ILineFactory Appender { get; internal set; }

        public LineLocalizationValuePart(ILine previousPart, ILineFactory appender, IFormulationString localizationString)
        {
            Appender = appender;
            PreviousPart = previousPart;
            Value = localizationString;
        }
    }

    public class StringLocalizerLocalizationValuePart : StringLocalizerKey, ILineValue, ILine
    {
        public IFormulationString Value { get; internal set; }
        public ILine PreviousPart { get; internal set; }
        public ILineFactory Appender { get; internal set; }

        public StringLocalizerLocalizationValuePart(ILineFactory appender, ILine previousPart, IFormulationString localizationString) : base(appender, previousPart, null)
        {
            Value = localizationString;
        }
    }
}
