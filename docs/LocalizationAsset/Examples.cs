using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lexical.Localization;
using Lexical.Localization.Utils;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class LocalizationAsset_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1a
                // Create localization source
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!"    },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().Add(source, "{Culture:}[Type:][Key]").Load();
                #endregion Snippet_1a
                ILine key = new LocalizationRoot(asset).Type("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.Culture("en"));
                Console.WriteLine(key.Culture("de"));
            }

            {
                #region Snippet_1b
                // Create localization source
                var source = new Dictionary<ILine, string> {
                    { ParameterPolicy.Instance.Parse("Type:MyController:Key:hello", Key.Root),            "Hello World!" },
                    { ParameterPolicy.Instance.Parse("Culture:en:Type:MyController:Key:hello", Key.Root), "Hello World!" },
                    { ParameterPolicy.Instance.Parse("Culture:de:Type:MyController:Key:hello", Key.Root), "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().Add(source).Load();
                #endregion Snippet_1b

                #region Snippet_2b
                ILine key = new LocalizationRoot(asset).Type("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.Culture("en"));
                Console.WriteLine(key.Culture("de"));
                #endregion Snippet_2b
            }

            {
                #region Snippet_1c
                // Create localization source
                var source = new Dictionary<ILine, string> {
                    { new LocalizationRoot().Type("MyController").Key("hello"),               "Hello World!" },
                    { new LocalizationRoot().Type("MyController").Key("hello").Culture("en"), "Hello World!" },
                    { new LocalizationRoot().Type("MyController").Key("hello").Culture("de"), "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().Add(source).Load();
                #endregion Snippet_1c

                #region Snippet_2c
                ILine key = new LocalizationRoot(asset).Type("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.Culture("en"));
                Console.WriteLine(key.Culture("de"));
                #endregion Snippet_2c
            }

            {
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                // Create asset with string source
                IAsset asset = new LocalizationAsset().Add(source, "{Culture:}[Type:][Key]").Load();
                #region Snippet_3a
                // Extract all keys
                foreach (ILine _key in asset.GetKeyLines(null).Select(line=>line.Key))
                    Console.WriteLine(_key);
                #endregion Snippet_3a

                #region Snippet_3b
                // Keys can be filtered
                ILine filterKey = LocalizationRoot.Global.Culture("de");
                foreach (ILine _key in asset.GetKeyLines(filterKey).Select(line => line.Key))
                    Console.WriteLine(_key);
                #endregion Snippet_3b

            }
        }
    }

}
