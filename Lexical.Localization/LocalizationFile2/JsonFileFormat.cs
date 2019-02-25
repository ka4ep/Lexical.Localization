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
    public class JsonFileFormat : ILocalizationFileFormat//, ILocalizationTreeStreamReader
    {
        private readonly static JsonFileFormat instance = new JsonFileFormat();
        public static JsonFileFormat Instance => instance;

        public string Extension => "json";
    }

    public class JsonFileAsset : LocalizationAsset
    {
        public JsonFileAsset(string filename) : base()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IKeyTree keyTree = JsonFileFormat.Instance.ReadTree(stream, null);
                var lines = keyTree.ToLines(true).ToArray();
                AddKeySource(lines, filename);
                Load();
            }
        }

        public JsonFileAsset(Stream stream) : base()
        {
            IKeyTree keyTree = JsonFileFormat.Instance.ReadTree(stream, null);
            var lines = keyTree.ToLines(true).ToArray();
            AddKeySource(lines);
            Load();
        }
    }

}
