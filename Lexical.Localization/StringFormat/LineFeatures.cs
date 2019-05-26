// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           21.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
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
        public ResolverSet Resolvers;

        /// <summary>
        /// Format arguments
        /// </summary>
        public object[] FormatArgs;

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
        public StructList2<IObserver<LineString>> Loggers;

        /// <summary>
        /// Effective plural rules
        /// </summary>
        public IPluralRules PluralRules;

        /// <summary>
        /// Active string format
        /// </summary>
        public IStringFormat StringFormat;

        /// <summary>
        /// Placeholder for <see cref="IFormatString"/>, if set before <see cref="StringFormats"/>.
        /// </summary>
        public string ValueText;

        /// <summary>
        /// Value.
        /// </summary>
        public IFormatString Value;

        /// <summary>
        /// Get Value or parsed ValueText. Call this after all features have been read.
        /// </summary>
        public IFormatString EffectiveValue => Value ?? (Value = (StringFormat ?? CSharpFormat.Instance).Parse(ValueText));

        /// <summary>
        /// Test if has value.
        /// </summary>
        public bool HasValue => ValueText != null || Value != null;

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
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineFormatArgs fa && fa != null) FormatArgs = fa.Args;
                if (l is ILineCulturePolicy cp && cp != null) CulturePolicy = cp.CulturePolicy;
                if (l is ILineCulture c && c.Culture != null) Culture = c.Culture;
                if (l is ILineFormatProvider fp && fp.FormatProvider != null) FormatProviders.AddIfNew(fp.FormatProvider);
                if (l is ILineFunctions funcs && funcs.Functions != null) Functions.AddIfNew(funcs.Functions);
                if (l is ILineInlines inlines) Inlines.AddIfNew(inlines);
                if (l is ILineLogger ll && ll.Logger != null) Loggers.AddIfNew(ll.Logger);
                if (l is ILinePluralRules pl && pl.PluralRules != null) PluralRules = pl.PluralRules;
                if (l is ILineStringFormat sf && sf.StringFormat != null) StringFormat = sf.StringFormat;
                if (l is ILineValue lv && lv.Value != null) { Value = lv.Value; ValueText = null; }
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
                            if (Resolvers.FormatProviderResolver.TryResolve(value, out _fp)) FormatProviders.AddIfNew(_fp); else Status.UpFormat(LineStatus.FormatErrorFormatProviderResolveFailed);
                        }

                        else if (name == "Functions" && !(l is ILineFunctions funcs_ && funcs_.Functions != null /*to not add second time*/))
                        {
                            IFunctions _funcs;
                            if (Resolvers.FunctionsResolver.TryResolve(value, out _funcs)) Functions.AddIfNew(_funcs); else Status.UpFormat(LineStatus.FormatErrorFunctionsResolveFailed);
                        }

                        else if (name == "PluralRules" && !(l is ILinePluralRules pluralRules_ && pluralRules_.PluralRules != null /*to not add second time*/))
                        {
                            IPluralRules _rules;
                            if (Resolvers.PluralRulesResolver.TryResolve(name, out _rules)) PluralRules = _rules; else Status.UpPlurality(LineStatus.PluralityErrorRulesResolveError);
                        }

                        else if (name == "StringFormat" && !(l is ILineStringFormat stringFormat_ && stringFormat_.StringFormat != null /*to not add second time*/))
                        {
                            IStringFormat _stringFormat;
                            if (Resolvers.StringFormatResolver.TryResolve(value, out _stringFormat)) StringFormat = _stringFormat; else Status.UpFormat(LineStatus.FormatErrorStringFormatResolveFailed);
                        }

                        else if (name == "Value" && !(l is ILineValue vl_ && vl_.Value != null /*to not add second time*/))
                        {
                            Value = null;
                            ValueText = value; // Parse later
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
                        if (Resolvers.FormatProviderResolver.TryResolve(value, out _fp)) FormatProviders.AddIfNew(_fp); else Status.UpFormat(LineStatus.FormatErrorFormatProviderResolveFailed);
                    }

                    else if (name == "Functions" && !(l is ILineFunctions funcs_ && funcs_.Functions != null /*to not add second time*/))
                    {
                        IFunctions _funcs;
                        if (Resolvers.FunctionsResolver.TryResolve(value, out _funcs)) Functions.AddIfNew(_funcs); else Status.UpFormat(LineStatus.FormatErrorFunctionsResolveFailed);
                    }

                    else if (name == "PluralRules" && !(l is ILinePluralRules pluralRules_ && pluralRules_.PluralRules != null /*to not add second time*/))
                    {
                        IPluralRules _rules;
                        if (Resolvers.PluralRulesResolver.TryResolve(name, out _rules)) PluralRules = _rules; else Status.UpPlurality(LineStatus.PluralityErrorRulesResolveError);
                    }

                    else if (name == "StringFormat" && !(l is ILineStringFormat stringFormat_ && stringFormat_.StringFormat != null /*to not add second time*/))
                    {
                        IStringFormat _stringFormat;
                        if (Resolvers.StringFormatResolver.TryResolve(value, out _stringFormat)) StringFormat = _stringFormat; else Status.UpFormat(LineStatus.FormatErrorStringFormatResolveFailed);
                    }

                    else if (name == "Value" && !(l is ILineValue vl_ && vl_.Value != null /*to not add second time*/))
                    {
                        Value = null;
                        ValueText = value; // Parse later
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
                Loggers[i].OnError(e);
        }

        /// <summary>
        /// Log string <paramref name="str"/>, if loggers are configured.
        /// </summary>
        /// <param name="str"></param>
        public void Log(LineString str)
        {
            for (int i = 0; i < Loggers.Count; i++)
                Loggers[i].OnNext(str);
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
