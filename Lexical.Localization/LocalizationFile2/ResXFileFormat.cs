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
    public class ResXFileFormat : ILocalizationFileFormat//, ILocalizationTreeStreamReader
    {
        private readonly static ResXFileFormat instance = new ResXFileFormat();
        public static ResXFileFormat Instance => instance;

        public string Extension => "resx";
    }

    public class ResXFileAsset : LocalizationAsset
    {
        public ResXFileAsset(string filename) : base()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var lines = ResXFileFormat.Instance.ReadLines(stream, null).ToArray();
                AddKeySource(lines, filename);
                Load();
            }
        }

        public ResXFileAsset(Stream stream) : base()
        {
            var lines = ResXFileFormat.Instance.ReadLines(stream, null).ToArray();
            AddKeySource(lines);
            Load();
        }
    }

}
