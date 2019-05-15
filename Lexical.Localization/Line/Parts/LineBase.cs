// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Basic line part with following features:
    /// <list type="bullet">
    ///     <item>Hash-Equals comparison</item>
    ///     <item>ToString resolves string</item>
    ///     <item>Debug print returns parameters</item>
    ///     <item>Hashcode caching for default comparer</item>
    ///     <item>dynamic</item>
    ///     <item>ILinePart.PreviousPart</item>
    ///     <item>ILineAppendable.Appender</item>
    /// </list>
    /// </summary>
    [DebuggerDisplay("{DebugPrint()}")]
    [Serializable]
    public class LineBase : ILinePart, ILineDefaultHashCode, IDynamicMetaObjectProvider, ILineAppendable
    {
        /// <summary>
        /// Comparer that compares <see cref="ILineKey"/> and <see cref="ILineFormatArgs"/> parts.
        /// </summary>
        static IEqualityComparer<ILine> keyAndArgsComparer =
            new LineComparer(ParameterInfos.Default)
                .AddCanonicalComparer(ParameterComparer.Instance)
                .AddComparer(NonCanonicalComparer.Instance)
                .AddComparer(new LineFormatArgsComparer())
                .SetReadonly();

        /// <summary>
        /// Comparer that compares <see cref="ILineKey"/> and <see cref="ILineFormatArgs"/> parts.
        /// </summary>
        public static IEqualityComparer<ILine> FormatArgsComparer => keyAndArgsComparer;

        /// <summary>
        /// Cached hashcode
        /// </summary>
        int hashcode = -1, defaultHashcode = -1;

        /// <summary>
        /// Determines if hashcode is calculated and cached
        /// </summary>
        bool hashcodeCalculated, defaultHashcodeCalculated;

        /// <summary>
        /// Appender
        /// </summary>
        protected ILineFactory appender;

        /// <summary>
        /// Cached dynamic object.
        /// </summary>
        protected DynamicMetaObject dynamicMetaObject;

        /// <summary>
        /// Previous part.
        /// </summary>
        protected ILine previousPart;
            
        /// <summary>
        /// Previous part.
        /// </summary>
        public ILine PreviousPart { get => previousPart; set => new InvalidOperationException(); }

        /// <summary>
        /// (optional) Get part appender.
        /// </summary>
        public virtual ILineFactory Appender { get => appender; set => throw new InvalidOperationException(nameof(Appender) + " is read-only"); }

        /// <summary>
        /// Create line part.
        /// </summary>
        /// <param name="appender">(optional) Explicit appender, if null uses the Appender in <paramref name="previousPart"/></param>
        /// <param name="previousPart">(optional) link to previous part.</param>
        public LineBase(ILineFactory appender, ILine previousPart)
        {
            this.appender = appender;
            this.previousPart = previousPart;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineBase(SerializationInfo info, StreamingContext context)
        {
            this.PreviousPart = info.GetValue(nameof(PreviousPart), typeof(ILine)) as ILine;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(PreviousPart), PreviousPart);
        }

        /// <summary>
        /// Calculate hashcode with hashesin format arguments. Result is cached.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (hashcodeCalculated) return hashcode;
            hashcode = keyAndArgsComparer.GetHashCode(this);
            hashcodeCalculated = true;
            return hashcode;
        }

        /// <summary>
        /// Calculate default reference hashcode. Result is cached.
        /// </summary>
        /// <returns></returns>
        int ILineDefaultHashCode.GetDefaultHashCode()
        {
            // Return cached default hashcode
            if (defaultHashcodeCalculated) return defaultHashcode;

            // Get previous key's default hashcode
            if (this is ILineCanonicalKey == false && this is ILineNonCanonicalKey == false && this.GetPreviousPart() is ILineDefaultHashCode prevDefaultHashcode)
            {
                defaultHashcode = prevDefaultHashcode.GetDefaultHashCode();
            }
            else
            {
                defaultHashcode = LineComparer.Default.CalculateHashCode(this);
            }

            // Mark calculated
            Thread.MemoryBarrier();
            defaultHashcodeCalculated = true;
            return defaultHashcode;
        }

        /// <summary>
        /// Get-or-create dynamic object.
        /// </summary>
        /// <param name="expression">the expression in the calling environgment</param>
        /// <returns>object</returns>
        public virtual DynamicMetaObject GetMetaObject(Expression expression)
        {
            var prev = dynamicMetaObject;
            if (prev?.Expression == expression) return prev;
            return dynamicMetaObject = new LineDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(ILine)), this);
        }

        /// <summary>
        /// Print parameters.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint()
        {
            StringBuilder sb = new StringBuilder();
            StructList12<ILineParameter> list = new StructList12<ILineParameter>();

            int c = 0;
            foreach(ILine line in GetInlines())
            {
                if (c++> 0) sb.Append("\n");
                list.Clear();
                line.GetParameterParts<StructList12<ILineParameter>>(ref list);
                for (int i = list.Count-1; i >=0; i--)
                {
                    if (i < list.Count-1) sb.Append(':');
                    var parameter = list[i];
                    if (parameter.ParameterName != "Value") continue;
                    sb.Append(parameter.ParameterName);
                    sb.Append(':');
                    sb.Append(parameter.ParameterValue);
                }
                string value = this.GetParameter("Value");
                if (value != null)
                {
                    sb.Append(" = ");
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        IEnumerable<ILine> GetInlines()
        {
            yield return this;
            for (ILine line = this; line != null; line = line.GetPreviousPart())
                if (line is ILineInlines inlines)
                    foreach (var l in inlines)
                        yield return l.Value;
        }

        /// <summary>
        /// Equals comparison. The default comparer compares <see cref="ILineKey"/> and <see cref="ILineFormatArgs"/> parts.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => keyAndArgsComparer.Equals(this, obj as ILine);

        /// <summary>
        /// Produce string using the following algorithm:
        ///   1. Search for language strings
        ///      a. Search for formultion arguments. Apply arguments. Return
        ///   2. Build and return key identity.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => this.ResolveFormulatedString().Value ?? this.DebugPrint();

        /// <summary>
        /// A library of interfaces and extension methods that DynamicMetaObject implementation seaches from when 
        /// invoked with dynamic calls.
        /// </summary>
        public static class Library
        {
            private static DynamicObjectLibrary instance = CreateDefault();

            /// <summary>
            /// Library of methods, fields and properties for dynamic object.
            /// </summary>
            public static DynamicObjectLibrary Default => instance;

            /// <summary>
            /// Create library of methods, fields and properties for dynamic object implementation.
            /// </summary>
            /// <returns></returns>
            public static DynamicObjectLibrary CreateDefault()
                => new DynamicObjectLibrary()
                    .AddExtensionMethods(typeof(ILineExtensions))
                    .AddInterface(typeof(ILine))
                    .AddInterface(typeof(ILineAppendable))
                    .AddInterface(typeof(ILineKey))
                    .AddInterface(typeof(ILineParameter))
                    .AddInterface(typeof(ILineAsset))
                    .AddInterface(typeof(ILineArguments))
                    .AddInterface(typeof(ILineAsset))
                    .AddInterface(typeof(ILineCulture))
                    .AddInterface(typeof(ILineCulturePolicy))
                    .AddInterface(typeof(ILineAssembly))
                    .AddInterface(typeof(ILineFormatArgs))
                    .AddInterface(typeof(ILineFormatProvider))
                    .AddInterface(typeof(ILineStringResolverPart))
                    .AddInterface(typeof(ILineLogger))
                    .AddInterface(typeof(ILineValue))
                    .AddInterface(typeof(ILinePluralRules))
                    .AddInterface(typeof(ILineRoot))
                    .AddInterface(typeof(ILineType))
                    .AddInterface(typeof(ILineInlines));
        }
    }

    /// <summary>
    /// Basic line part with following features:
    /// <list type="bullet">
    ///     <item>IStringLocalizer</item>
    ///     <item>Hash-Equals comparison</item>
    ///     <item>ToString resolves string</item>
    ///     <item>Debug print returns parameters</item>
    ///     <item>Hashcode caching for default comparer</item>
    ///     <item>dynamic</item>
    ///     <item>ILinePart.PreviousPart</item>
    ///     <item>ILineAppendable.Appender</item>
    /// </list>
    /// </summary>
    public class StringLocalizerBase : LineBase, ILine, IStringLocalizerLine, IStringLocalizer, IStringLocalizerFactory
    {
        /// <summary>
        /// Create string localizer part.
        /// </summary>
        /// <param name="previousPart"></param>
        /// <param name="appender"></param>
        public StringLocalizerBase(ILineFactory appender, ILine previousPart) : base(appender, previousPart) { }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerBase(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Create Resource key part.
        /// </summary>
        /// <param name="basename"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public IStringLocalizer Create(string basename, string location)
        {
            ILine result = this;
            if (location != null) result = result.Append<ILineNonCanonicalKey, string, string>("Location", location);
            if (basename != null) result = result.Append<ILineNonCanonicalKey, string, string>("BaseName", basename);
            return (IStringLocalizer)result;
        }

        /// <summary>
        /// Create Type key part.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IStringLocalizer Create(Type type)
            => (IStringLocalizer) this.Append<ILineType, Type>(type);

        /// <summary>
        /// Create Culture key part.
        /// </summary>
        /// <param name="newCulture"></param>
        /// <returns></returns>
        public IStringLocalizer WithCulture(CultureInfo newCulture)
        {
            ILine part = this;
            // Find culture key
            ILine oldCultureKey;
            // Old culture exists
            if (this.TryGetCultureKey(out oldCultureKey))
            {
                // Old culture matches the new, return as is
                if (oldCultureKey.GetCultureName() == newCulture?.Name) return this;
                // Remove culture part
                ILineFactory appender = this.GetAppender();
                StructList16<ILine> args = new StructList16<ILine>();
                for (ILine l = this; l != null; l = l.GetPreviousPart())
                {
                    if (l == oldCultureKey) break; // Stop iteration
                    if (l is ILineArguments || l is ILineArgumentsEnumerable) args.Add(l);
                }
                // Re-append everything but culture
                part = oldCultureKey?.GetPreviousParameterPart();
                for (int i = args.Count - 1; i >= 0; i--)
                {
                    ILine l = args[i];
                    if (l is ILineArgumentsEnumerable enumr)
                        foreach (ILineArguments args_ in enumr)                            
                            if (args_ is ILineArguments<ILineCulture, CultureInfo> == false && args_.GetParameterName() != "Culture")
                                part = appender.Create(part, args_);
                    if (l is ILineArguments arg)
                        if (arg is ILineArguments<ILineCulture, CultureInfo> == false && arg.GetParameterName() != "Culture")
                            part = part.Append(arg);
                }
            }

            // Append culture
            return (IStringLocalizer)this.Append<ILineCulture, CultureInfo>(newCulture);
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                ILine line = this;
                if (name != null) line = line.Append<ILineNonCanonicalKey, string, string>("Key", name);                
                LineString printedString = line.ResolveFormulatedString();
                if (printedString.Value == null)
                    return new LocalizedString(name, line.Print(), true);
                else
                    return new LocalizedString(name, printedString.Value);
            }
        }

        /// <summary>
        /// Create localized string with format arguments.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                ILine line = this;
                if (name != null) line = line.Append<ILineNonCanonicalKey, string, string>("Key", name);
                if (arguments != null) line = line.Append<ILineFormatArgs, object[]>(arguments);
                LineString printedString = line.ResolveFormulatedString();
                if (printedString.Value == null)
                    return new LocalizedString(name, line.Print(), true);
                else
                    return new LocalizedString(name, printedString.Value);
            }
        }

        /// <summary>
        /// Get all strings as localized strings.
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            ILocalizationStringLinesEnumerable collections = this.FindAsset() as ILocalizationStringLinesEnumerable;
            if (collections == null) return null;

            CultureInfo ci = null;
            if (includeParentCultures && this.TryGetCultureInfo(out ci))
            {
                IEnumerable<LocalizedString> result = null;
                while (true)
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> strs = collections?.GetAllStringLines(this);
                    if (strs != null)
                    {
                        IEnumerable<LocalizedString> converted = ConvertStrings(strs);
                        result = result == null ? converted : result.Concat(converted);
                    }

                    if (ci.Parent == ci || ci.Parent == null || ci.Name == ci.Parent?.Name) break;
                }
                return result;
            }
            else
            {
                IEnumerable<KeyValuePair<string, IFormulationString>> strs = collections?.GetAllStringLines(this);
                return strs == null ? null : ConvertStrings(strs);
            }
        }

        /// <summary>
        /// Convert strings to localized strings.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        IEnumerable<LocalizedString> ConvertStrings(IEnumerable<KeyValuePair<string, IFormulationString>> lines)
        {
            foreach (var kp in lines)
            {
                string value = kp.Value.Text; 
                yield return new LocalizedString(kp.Key, value);
            }
        }

    }

}
