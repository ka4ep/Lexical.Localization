// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           21.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.Resolver;
using System;
using System.Globalization;

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Features that were extracted from the key, key's inlines and asset's line.
    /// </summary>
    public struct LineFeatures
    {
        /// <summary>
        /// A group of resolvers
        /// </summary>
        public IResolver Resolvers;

        /// <summary>
        /// Effective culture policy
        /// </summary>
        public ICulturePolicy CulturePolicy;

        /// <summary>
        /// Overriding culture
        /// </summary>
        public CultureInfo Culture;

        /// <summary>
        /// Configured loggers
        /// </summary>
        public StructList2<ILocalizationLogger> Loggers;

        /// <summary>
        /// Assets
        /// </summary>
        public StructList1<IAsset> Assets;

        /// <summary>
        /// Status code from reading lines
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// Scan features from <paramref name="line"/>
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="LineException">If resolve fails.</exception>
        public void ScanFeatures(ILine line)
        {
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineCulturePolicy cp && cp != null) CulturePolicy = cp.CulturePolicy;
                if (l is ILineCulture c && c.Culture != null) Culture = c.Culture;
                if (l is ILineLogger ll && ll.Logger != null) Loggers.AddIfNew(ll.Logger);
                if (l is ILineAsset la && la.Asset != null) Assets.AddIfNew(la.Asset);

                if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                    {
                        string name = lineParameter.ParameterName, value = lineParameter.ParameterValue;

                        if (name == "Culture") try { Culture = CultureInfo.GetCultureInfo(value); } catch (Exception) { }
                    }
                }

                if (l is ILineParameter parameter)
                {
                    string name = parameter.ParameterName, value = parameter.ParameterValue;

                    if (name == "Culture") try { Culture = CultureInfo.GetCultureInfo(value); } catch (Exception) { }
                }
            }
        }


        /// <summary>
        /// Log error <paramref name="e"/>, if loggers are configured.
        /// </summary>
        /// <param name="e"></param>
        public void LogStreamResolve(Exception e)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                if (Loggers[i] is IObserver<LineResourceStream> lineLogger) lineLogger.OnError(e);
            }
        }

        /// <summary>
        /// Log <paramref name="str"/>, if loggers are configured.
        /// </summary>
        /// <param name="str"></param>
        public void LogStreamResolve(LineResourceStream str)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                if (Loggers[i] is IObserver<LineResourceStream> lineLogger) lineLogger.OnNext(str);
            }
        }

        /// <summary>
        /// Log error and string, if loggers are configured.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="stream"></param>
        public void LogStreamResolve(Exception e, LineResourceStream stream)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                if (Loggers[i] is IObserver<LineResourceStream> logger)
                {
                    logger.OnNext(stream);
                    logger.OnError(e);
                }
            }
        }


        /// <summary>
        /// Log error <paramref name="e"/>, if loggers are configured.
        /// </summary>
        /// <param name="e"></param>
        public void LogBytesResolve(Exception e)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                if (Loggers[i] is IObserver<LineResourceBytes> lineLogger) lineLogger.OnError(e);
            }
        }

        /// <summary>
        /// Log <paramref name="str"/>, if loggers are configured.
        /// </summary>
        /// <param name="str"></param>
        public void LogBytesResolve(LineResourceBytes str)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                if (Loggers[i] is IObserver<LineResourceBytes> lineLogger) lineLogger.OnNext(str);
            }
        }

        /// <summary>
        /// Log error and string, if loggers are configured.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="stream"></param>
        public void LogBytesResolve(Exception e, LineResourceBytes stream)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                if (Loggers[i] is IObserver<LineResourceBytes> logger)
                {
                    logger.OnNext(stream);
                    logger.OnError(e);
                }
            }
        }

    }

}
