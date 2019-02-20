// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.LocalizationAsset
{
    /// <summary>
    /// Asset that contains key value pairs in memory. 
    /// 
    /// Keys are context-free 
    /// </summary>
    public class LocalizationKeyDictionary
    {
        protected IReadOnlyDictionary<Key, string> source;

    }
}
