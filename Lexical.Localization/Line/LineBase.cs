// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
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
        /// Comparer that compares <see cref="ILineKey"/> and <see cref="ILineFormatArgsPart"/> parts.
        /// </summary>
        static IEqualityComparer<ILine> keyAndArgsComparer =
            new LineComparer()
                .AddCanonicalComparer(ParameterComparer.Instance)
                .AddComparer(NonCanonicalComparer.Instance)
                .AddComparer(new LocalizationKeyFormatArgsComparer())
                .SetReadonly();

        /// <summary>
        /// Comparer that compares <see cref="ILineKey"/> and <see cref="ILineFormatArgsPart"/> parts.
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
            return dynamicMetaObject = new LocalizationKeyDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(ILine)), this);
        }

        /// <summary>
        /// Print parameters.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint()
            => ParameterPolicy.Instance.Print(this);

        /// <summary>
        /// Equals comparison. The default comparer compares <see cref="ILineKey"/> and <see cref="ILineFormatArgsPart"/> parts.
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
                    .AddExtensionMethods(typeof(AssetKeyExtensions))
                    .AddInterface(typeof(ILine))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeyAssigned))
                    .AddInterface(typeof(ILine))
                    .AddInterface(typeof(IAssetKeyAssetAssigned))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeySectionAssigned))
                    .AddInterface(typeof(IAssetKeyLocationAssigned))
                    .AddInterface(typeof(ILineKeyType))
                    .AddInterface(typeof(ILineKeyAssembly))
                    .AddInterface(typeof(IAssetKeyResourceAssigned))
                    .AddExtensionMethods(typeof(ILineExtensions))
                    .AddInterface(typeof(ILineKeyCulture))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssigned))
                    .AddInterface(typeof(ILineFormatArgsPart))
                    .AddInterface(typeof(ILineInlinesAssigned))
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
            if (basename != null) result = result.Append<ILineNonCanonicalKey, string, string>("BaseName", basename);
            if (location != null) result = result.Append<ILineNonCanonicalKey, string, string>("Location", location);
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
            // Find culture key
            ILine oldCultureKey;
            if (!this.TryGetCultureKey(out oldCultureKey))
            {
                // No culture key, create new
                if (oldCultureKey == null) return newCulture == null ? this : (IStringLocalizer)this.Append<ILineCulture, CultureInfo>(newCulture);
                // Old culture matches the new, return as is
                if (oldCultureKey.GetCultureName() == newCulture?.Name) return this;
            }

            // Replace culture part
            ILine beforeCultureKey = oldCultureKey?.GetPreviousPart();
            StructList16<ILine> parts = new StructList16<ILine>();
            for (ILine l = this; l != oldCultureKey; l = l.GetPreviousPart()) parts.Add(l);
            ILine result = beforeCultureKey;
            foreach (ILine l in parts) result = result.Append(l);
            return (IStringLocalizer)result;
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
                LocalizationString printedString = line.ResolveFormulatedString();
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
                LocalizationString printedString = line.ResolveFormulatedString();
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
