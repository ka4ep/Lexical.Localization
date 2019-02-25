// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization.LocalizationFile2
{
    public class IniFileFormat : ILocalizationFileFormat//, ILocalizationTreeStreamReader
    {
        private readonly static IniFileFormat instance = new IniFileFormat();
        public static IniFileFormat Instance => instance;

        public string Extension => "ini";
    }

    public class IniFileAsset : LocalizationAsset
    {
        public IniFileAsset(string filename) : base()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IKeyTree keyTree = IniFileFormat.Instance.ReadTree(stream, null);
                var lines = keyTree.ToLines(true).ToArray();
                AddKeySource(lines, filename);
                Load();
            }
        }

        public IniFileAsset(Stream stream) : base()
        {
            IKeyTree keyTree = IniFileFormat.Instance.ReadTree(stream, null);
            var lines = keyTree.ToLines(true).ToArray();
            AddKeySource(lines);
            Load();
        }
    }

}
