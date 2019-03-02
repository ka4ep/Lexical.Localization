using Lexical.Localization;
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class Key_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                Key key = new Key("culture", "en").Append("type", "MyController").Append("key", "Success");
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                #endregion Snippet_2
            }
            {
                #region Snippet_3a
                #endregion Snippet_3a
            }
            {
                #region Snippet_3b
                #endregion Snippet_3b
            }

            {
                #region Snippet_4
                #endregion Snippet_4
            }

            {
                #region Snippet_5
                #endregion Snippet_5
            }

            {
                #region Snippet_6
                #endregion Snippet_6
            }
        }
    }
}

