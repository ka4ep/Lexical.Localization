// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public class PluralRule : IPluralCase
    {
        string name;

        /// <summary>
        // 
        /// </summary>
        /// <param name="count">Name of the case</param>
        /// <param name="ruleText">Rule text</param>
        public PluralRule(string name, string ruleText)
        {
            this.name = name;
        }

        public IPluralCategory Category => throw new NotImplementedException();

        public int CaseIndex => throw new NotImplementedException();

        public string Name => name;

        public bool Optional => throw new NotImplementedException();

        public bool Evaluate(object value, string formatted)
        {
            throw new NotImplementedException();
        }

    }


}
