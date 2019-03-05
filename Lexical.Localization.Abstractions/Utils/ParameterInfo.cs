// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Information about parameters.
    /// </summary>
    public class ParameterInfos : Dictionary<string, ParameterInfo>
    {
        private static ParameterInfos instance;
        public static ParameterInfos Instance => instance;

        static ParameterInfos()
        {
            instance = new ParameterInfos();

        }
    }

    public class ParameterInfo
    {
        public readonly string ParameterName;
        public readonly bool IsCanonical;
        public readonly bool IsNonCanonical;
        public readonly bool IsSection;
        public readonly int PreferredSortIndex;

        public ParameterInfo(string parameterName, bool isCanonical, bool isNonCanonical, bool isSection, int preferredSortIndex)
        {
            ParameterName = parameterName;
            IsCanonical = isCanonical;
            IsNonCanonical = isNonCanonical;
            IsSection = isSection;
            PreferredSortIndex = preferredSortIndex;
        }
    }
}
