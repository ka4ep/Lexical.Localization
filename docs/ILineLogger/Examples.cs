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
    public class ILogger_Examples
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
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
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
                #region Snippet_4a
                LoggerFactory loggerFactory = new LoggerFactory();
                loggerFactory.AddConsole(LogLevel.Trace);
                ILine root = LineRoot.Global.ILogger(loggerFactory);
                #endregion Snippet_4a
                #region Snippet_4b
                Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_4b
                //info: MyClass[0]
                //ResolveOkFromKey | CultureOkMatchedNoCulture | PluralityOkNotUsed | StringFormatOkString Type: MyClass: Key: OK = "OK"
            }
            {
                #region Snippet_5a
                NLog.ILogger nlog = NLog.LogManager.GetLogger("MyClass");
                ILine root = LineRoot.Global.NLog(nlog);
                #endregion Snippet_5a
                #region Snippet_5b
                Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_5b
                // 2019-06-08 14:10:46.4939|INFO|MyClass|ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
            }
            {
                #region Snippet_6a
                ILine root = LineRoot.Global.NLog(NLog.LogManager.LogFactory);
                #endregion Snippet_6a
                #region Snippet_6b
                Console.WriteLine(root.Type("MyClass").Key("OK").Text("OK"));
                #endregion Snippet_6b
                // 2019-06-08 14:10:58.9517|INFO|MyClass|ResolveOkFromKey|CultureOkMatchedNoCulture|PluralityOkNotUsed|StringFormatOkString Type:MyClass:Key:OK = "OK"
                Console.WriteLine(root.Key("OK").Text("OK"));
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
