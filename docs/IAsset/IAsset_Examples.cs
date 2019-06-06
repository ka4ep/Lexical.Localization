using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.StringFormat;
using System.Collections.Generic;

namespace docs
{
    public class IAsset_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1
                // Language string source
                Dictionary<string, string> src = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
                // Create Asset
                IAsset asset = new StringAsset(src, LineParameterPrinter.Default);
                #endregion Snippet_1

                #region Snippet_2
                // Create key
                ILine key = new LineRoot().Key("hello").Culture("en");
                // Resolve string - Call to StringAssetExtensions.GetString()
                IString str = asset.GetLine(key).GetString();
                #endregion Snippet_2

            }
        }
    }

}
