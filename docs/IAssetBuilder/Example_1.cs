﻿using Lexical.Localization;
using System;
using System.Collections.Generic;

namespace docs
{
    public class IAssetBuilder_Example_1
    {
        public static void Main(string[] args)
        {
            // Create dictionary of strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };

            #region Snippet
            // Create AssetBuilder
            IAssetBuilder builder = new AssetBuilder();
            // Add IAssetSource that adds cache 
            builder.AddCache();
            // Add IAssetSource that adds strings
            builder.AddStrings(strings, KeyPrinter.Default);
            #endregion Snippet

            // Instantiate IAsset
            IAsset asset = builder.Build();

            // Create string key
            ILine key = new LineRoot().Key("hello").Culture("en");
            // Request string
            IFormulationString str = asset.GetString(key);
            // Print result
            Console.WriteLine(str);
        }
    }

}
