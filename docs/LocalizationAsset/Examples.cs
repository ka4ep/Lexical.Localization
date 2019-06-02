using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lexical.Localization;
using Lexical.Localization.Utils;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class StringAsset_Examples
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
                IAsset asset = new StringAsset().Add(source, "{Culture:}[Type:][Key]").Load();
                #endregion Snippet_1a
                ILine key = new LineRoot(asset).Type("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.Culture("en"));
                Console.WriteLine(key.Culture("de"));
            }

            {
                #region Snippet_1b
                // Create localization source
                var source = new List<ILine> {
                    { LineFormat.Parameters.Parse("Type:MyController:Key:hello").Format("Hello World!") },
                    { LineFormat.Parameters.Parse("Culture:en:Type:MyController:Key:hello").Format("Hello World!") },
                    { LineFormat.Parameters.Parse("Culture:de:Type:MyController:Key:hello").Format("Hallo Welt!")  }
                };
                // Create asset with string source
                IAsset asset = new StringAsset().Add(source).Load();
                #endregion Snippet_1b

                #region Snippet_2b
                ILine key = new LineRoot(asset).Type("MyController").Key("hello");
                Console.WriteLine(key);
                Console.WriteLine(key.Culture("en"));
                Console.WriteLine(key.Culture("de"));
                #endregion Snippet_2b
            }

            {
                #region Snippet_1c
                // Create localization source
                var source = new Dictionary<ILine, string> {
                    { new LineRoot().Type("MyController").Key("hello"),               "Hello World!" },
                    { new LineRoot().Type("MyController").Key("hello").Culture("en"), "Hello World!" },
                    { new LineRoot().Type("MyController").Key("hello").Culture("de"), "Hallo Welt!"  }
                };
                // Create asset with string source
                IAsset asset = new StringAsset().Add(source).Load();
                #endregion Snippet_1c

                #region Snippet_2c
                ILine key = new LineRoot(asset).Type("MyController").Key("hello");
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
                IAsset asset = new StringAsset().Add(source, "{Culture:}[Type:][Key]").Load();
                #region Snippet_3a
                // Extract all keys
                foreach (var _key in asset.GetStringLines(null))
                    Console.WriteLine(_key);
                #endregion Snippet_3a
            }
            { 
                #region Snippet_3b
                var source = new List<ILine> {
                    LineAppender.Default.Type("MyController").Key("hello").Format("Hello World!"),
                    LineAppender.Default.Type("MyController").Key("hello").Culture("en").Format("Hello World!"),
                    LineAppender.Default.Type("MyController").Key("hello").Culture("de").Format("Hallo Welt!")
                };
                // Keys can be filtered
                ILine filterKey = LineAppender.Default.Culture("de");
                IAsset asset = new StringAsset().Add(source, "{Culture:}[Type:][Key]").Load();
                foreach (var _key in asset.GetLines(filterKey))
                    Console.WriteLine(_key.Print(LineFormat.ParametersInclString));
                #endregion Snippet_3b

            }
        }
    }

}
