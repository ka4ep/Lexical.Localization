// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           21.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Common;
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using Lexical.Localization.Resolver;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.StringFormat
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
        /// Format value arguments.
        /// </summary>
        public object[] ValueArgs;

        /// <summary>
        /// Effective culture policy
        /// </summary>
        public ICulturePolicy CulturePolicy;

        /// <summary>
        /// Overriding culture
        /// </summary>
        public CultureInfo Culture;

        /// <summary>
        /// Custom format providers for "Format" string function.
        /// </summary>
        public StructList2<IFormatProvider> FormatProviders;

        /// <summary>
        /// String functions for <see cref="IStringFormat"/>.
        /// </summary>
        public StructList2<IFunctions> Functions;

        /// <summary>
        /// Inlines
        /// </summary>
        public StructList1<IDictionary<ILine, ILine>> Inlines;

        /// <summary>
        /// Configured loggers
        /// </summary>
        public StructList2<IStringResolverLogger> Loggers;

        /// <summary>
        /// Effective plural rules
        /// </summary>
        public IPluralRules PluralRules;

        /// <summary>
        /// Active string format
        /// </summary>
        public IStringFormat StringFormat;

        /// <summary>
        /// Placeholder for <see cref="IString"/>, if set before <see cref="StringFormats"/>.
        /// </summary>
        public string StringText;

        /// <summary>
        /// String.
        /// </summary>
        public IString String;

        /// <summary>
        /// Get String or parsed StringText. Call this after all features have been read.
        /// </summary>
        public IString EffectiveString => String ?? (String = (StringFormat ?? CSharpFormat.Default).Parse(StringText));

        /// <summary>
        /// Test if has value.
        /// </summary>
        public bool HasValue => StringText != null || String != null;

        /// <summary>
        /// Assets
        /// </summary>
        public StructList1<IAsset> Assets;

        /// <summary>
        /// Status code from reading lines
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// Scan features from a line
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="LineException">If resolve fails.</exception>
        public void ScanFeatures(ILine line)
        {
            bool valueSet = false;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineValue fa && fa != null) ValueArgs = fa.Value;
                if (l is ILineCulturePolicy cp && cp != null) CulturePolicy = cp.CulturePolicy;
                if (l is ILineCulture c && c.Culture != null) Culture = c.Culture;
                if (l is ILineFormatProvider fp && fp.FormatProvider != null) FormatProviders.AddIfNew(fp.FormatProvider);
                if (l is ILineFunctions funcs && funcs.Functions != null) Functions.AddIfNew(funcs.Functions);
                if (l is ILineInlines inlines) Inlines.AddIfNew(inlines);
                if (l is ILineLogger ll && ll.Logger is IStringResolverLogger logger) Loggers.AddIfNew(logger);
                if (l is ILinePluralRules pl && pl.PluralRules != null) PluralRules = pl.PluralRules;
                if (l is ILineStringFormat sf && sf.StringFormat != null) StringFormat = sf.StringFormat;
                if (!valueSet && l is ILineString lv && lv.String != null) { String = lv.String; StringText = null; valueSet = true; }
                if (l is ILineAsset la && la.Asset != null) Assets.AddIfNew(la.Asset);

                if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                    {
                        string name = lineParameter.ParameterName, value = lineParameter.ParameterValue;

                        if (name == "Culture") try { Culture = CultureInfo.GetCultureInfo(value); } catch (Exception) { }

                        else if (name == "FormatProvider" && !(l is ILineFormatProvider fp_ && fp_.FormatProvider != null /*to not add second time*/))
                        {
                            IFormatProvider _fp;
                            if (Resolvers.TryResolve<IFormatProvider>(value, out _fp)) FormatProviders.AddIfNew(_fp); else Status.UpStringFormat(LineStatus.ResolveErrorFormatProviderResolveFailed);
                        }

                        else if (name == "Functions" && !(l is ILineFunctions funcs_ && funcs_.Functions != null /*to not add second time*/))
                        {
                            IFunctions _funcs;
                            if (Resolvers.TryResolve<IFunctions>(value, out _funcs)) Functions.AddIfNew(_funcs); else Status.UpStringFormat(LineStatus.ResolveErrorFunctionsResolveFailed);
                        }

                        else if (name == "PluralRules" && !(l is ILinePluralRules pluralRules_ && pluralRules_.PluralRules != null /*to not add second time*/))
                        {
                            IPluralRules _rules;
                            if (Resolvers.TryResolve<IPluralRules>(value, out _rules)) PluralRules = _rules; else Status.UpPlurality(LineStatus.ResolveErrorPluralRulesResolveFailed);
                        }

                        else if (name == "StringFormat" && !(l is ILineStringFormat stringFormat_ && stringFormat_.StringFormat != null /*to not add second time*/))
                        {
                            IStringFormat _stringFormat;
                            if (Resolvers.TryResolve<IStringFormat>(value, out _stringFormat)) StringFormat = _stringFormat; else Status.UpStringFormat(LineStatus.ResolveErrorStringFormatResolveFailed);
                        }

                        else if (!valueSet && name == "String")
                        {
                            valueSet = true;
                            String = null;
                            StringText = value; // Parse later
                        }
                    }
                }

                if (l is ILineParameter parameter)
                {
                    string name = parameter.ParameterName, value = parameter.ParameterValue;

                    if (name == "Culture") try { Culture = CultureInfo.GetCultureInfo(value); } catch (Exception) { }

                    else if (name == "FormatProvider" && !(l is ILineFormatProvider fp_ && fp_.FormatProvider != null /*to not add second time*/))
                    {
                        IFormatProvider _fp;
                        if (Resolvers.TryResolve<IFormatProvider>(value, out _fp)) FormatProviders.AddIfNew(_fp); else Status.UpStringFormat(LineStatus.ResolveErrorFormatProviderResolveFailed);
                    }

                    else if (name == "Functions" && !(l is ILineFunctions funcs_ && funcs_.Functions != null /*to not add second time*/))
                    {
                        IFunctions _funcs;
                        if (Resolvers.TryResolve<IFunctions>(value, out _funcs)) Functions.AddIfNew(_funcs); else Status.UpStringFormat(LineStatus.ResolveErrorFunctionsResolveFailed);
                    }

                    else if (name == "PluralRules" && !(l is ILinePluralRules pluralRules_ && pluralRules_.PluralRules != null /*to not add second time*/))
                    {
                        IPluralRules _rules;
                        if (Resolvers.TryResolve<IPluralRules>(value, out _rules)) PluralRules = _rules; else Status.UpPlurality(LineStatus.ResolveErrorPluralRulesResolveFailed);
                    }

                    else if (name == "StringFormat" && !(l is ILineStringFormat stringFormat_ && stringFormat_.StringFormat != null /*to not add second time*/))
                    {
                        IStringFormat _stringFormat;
                        if (Resolvers.TryResolve<IStringFormat>(value, out _stringFormat)) StringFormat = _stringFormat; else Status.UpStringFormat(LineStatus.ResolveErrorStringFormatResolveFailed);
                    }

                    else if (!valueSet && name == "String")
                    {
                        valueSet = true;
                        String = null;
                        StringText = value; // Parse later
                    }

                }
            }
        }

        /// <summary>
        /// Scan value feature from a line
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="LineException">If resolve fails.</exception>
        public void ScanValueFeature(ILine line)
        {
            bool valueSet = false;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineStringFormat sf && sf.StringFormat != null) StringFormat = sf.StringFormat;
                if (!valueSet && l is ILineString lv && lv.String != null) { String = lv.String; StringText = null; valueSet = true; }

                if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                    {
                        string name = lineParameter.ParameterName, value = lineParameter.ParameterValue;

                        if (name == "StringFormat" && !(l is ILineStringFormat stringFormat_ && stringFormat_.StringFormat != null /*to not add second time*/))
                        {
                            IStringFormat _stringFormat;
                            if (Resolvers.TryResolve<IStringFormat>(value, out _stringFormat)) StringFormat = _stringFormat; else Status.UpStringFormat(LineStatus.ResolveErrorStringFormatResolveFailed);
                        }

                        else if (!valueSet && name == "String")
                        {
                            valueSet = true;
                            String = null;
                            StringText = value; // Parse later
                        }
                    }
                }

                if (l is ILineParameter parameter)
                {
                    string name = parameter.ParameterName, value = parameter.ParameterValue;

                    if (name == "StringFormat" && !(l is ILineStringFormat stringFormat_ && stringFormat_.StringFormat != null /*to not add second time*/))
                    {
                        IStringFormat _stringFormat;
                        if (Resolvers.TryResolve<IStringFormat>(value, out _stringFormat)) StringFormat = _stringFormat; else Status.UpStringFormat(LineStatus.ResolveErrorStringFormatResolveFailed);
                    }

                    else if (!valueSet && name == "String")
                    {
                        valueSet = true;
                        String = null;
                        StringText = value; // Parse later
                    }

                }
            }
        }

        /// <summary>
        /// Log error <paramref name="e"/>, if loggers are configured.
        /// </summary>
        /// <param name="e"></param>
        public void Log(Exception e)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                var logger = Loggers[i];
                logger.OnError(e);
            }
        }

        /// <summary>
        /// Log string <paramref name="str"/>, if loggers are configured.
        /// </summary>
        /// <param name="str"></param>
        public void Log(LineString str)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                var logger = Loggers[i];
                logger.OnNext(str);
            }
        }

        /// <summary>
        /// Log error and string, if loggers are configured.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="str"></param>
        public void Log(Exception e,LineString str)
        {
            for (int i = 0; i < Loggers.Count; i++)
            {
                var logger = Loggers[i];
                logger.OnNext(str);
                logger.OnError(e);
            }
        }

    }

}
