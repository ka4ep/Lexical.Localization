using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace docs
{
    public class ILocalizationLogger_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                ILine root = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
                #endregion Snippet_0a
                #region Snippet_0b
                Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_0b
                // ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
            }
            {
                #region Snippet_1a
                (LineRoot.Global as LineRoot.Mutable).Logger = new LineTextLogger(Console.Out, LineStatusSeverity.Ok);
                #endregion Snippet_1a
            }
            {
                #region Snippet_1b
                Console.WriteLine(LineRoot.Global.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_1b
                // ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
                (LineRoot.Global as LineRoot.Mutable).Logger = null;
            }

            {
                #region Snippet_2a
                Trace.Listeners.Add(new ConsoleTraceListener());
                ILine root = LineRoot.Global.DiagnosticsTrace(LineStatusSeverity.Ok);
                #endregion Snippet_2a
                #region Snippet_2b
                Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_2b
                // docs Information: 0 : ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
            }
            {
                #region Snippet_3a
                LoggerFactory loggerFactory = new LoggerFactory();
                loggerFactory.AddConsole(LogLevel.Trace);
                ILogger logger = loggerFactory.CreateLogger("MyClass");
                ILine root = LineRoot.Global.ILogger(logger);
                #endregion Snippet_3a
                #region Snippet_3b
                Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_3b
                //info: MyClass[0]
                //ResolveOkFromKey | CultureOkMatchedNoCulture | PluralityOkNotUsed | StringFormatOkString Type: MyClass: Key: OK = "OK"
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
            {
                #region Snippet_7
                #endregion Snippet_7
            }
            {
                #region Snippet_8
                #endregion Snippet_8
            }
        }
    }

}
