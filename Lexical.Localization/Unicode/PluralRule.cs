// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public class PluralRule : IPluralityCase
    {
        public readonly string Count;

        public PluralRule(string count, string text)
        {

        }

        public IPluralityCategory Category => throw new NotImplementedException();

        public int CaseIndex => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public bool Optional => throw new NotImplementedException();

        public bool Evaluate(object value, string formatted)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Plurality rule expression
        /// </summary>
        public class Exp
        {

        }

    }

}
