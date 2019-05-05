// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Basic line part.
    /// </summary>
    [DebuggerDisplay("{DebugPrint()}")]
    [Serializable]
    public class LinePart : ILinePart, ILineDefaultHashCode, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Comparer that compares <see cref="ILineKey"/> and <see cref="ILineFormatArgsPart"/> parts.
        /// </summary>
        static IEqualityComparer<ILinePart> keyAndArgsComparer =
            new LineComparer()
                .AddCanonicalComparer(ParameterComparer.Instance)
                .AddComparer(NonCanonicalComparer.Instance)
                .AddComparer(new LocalizationKeyFormatArgsComparer())
                .SetReadonly();

        /// <summary>
        /// Comparer that compares <see cref="ILineKey"/> and <see cref="ILineFormatArgsPart"/> parts.
        /// </summary>
        public static IEqualityComparer<ILinePart> FormatArgsComparer => keyAndArgsComparer;

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
        protected ILinePartAppender appender;

        /// <summary>
        /// Cached dynamic object.
        /// </summary>
        protected DynamicMetaObject dynamicMetaObject;

        /// <summary>
        /// Previous part.
        /// </summary>
        public ILinePart PreviousPart { get; protected set; }

        /// <summary>
        /// (optional) Get part appender.
        /// </summary>
        public virtual ILinePartAppender Appender { get => appender; set => throw new InvalidOperationException(nameof(Appender) + " is read-only"); }

        /// <summary>
        /// Create line part.
        /// </summary>
        /// <param name="appender">(optional) Explicit appender, if null uses the Appender in <paramref name="previousPart"/></param>
        /// <param name="previousPart">(optional) link to previous part.</param>
        public LinePart(ILinePartAppender appender, ILinePart previousPart)
        {
            Appender = appender;
            PreviousPart = previousPart;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LinePart(SerializationInfo info, StreamingContext context)
        {
            this.PreviousPart = info.GetValue(nameof(PreviousPart), typeof(ILinePart)) as ILinePart;
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
            if (this is ILineKeyCanonicallyCompared == false && this is ILineKeyNonCanonicallyCompared == false && this.PreviousPart is ILineDefaultHashCode prevDefaultHashcode)
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
            return dynamicMetaObject = new LocalizationKeyDynamicMetaObject(Library.Default, expression, BindingRestrictions.GetTypeRestriction(expression, typeof(ILinePart)), this);
        }

        /// <summary>
        /// Print parameters.
        /// </summary>
        /// <returns></returns>
        public string DebugPrint()
            => ParameterPolicy.Instance.Print(this); // KeyPrinter.Default.Print(this);

        /// <summary>
        /// Equals comparison. The default comparer compares <see cref="ILineKey"/> and <see cref="ILineFormatArgsPart"/> parts.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => keyAndArgsComparer.Equals(this, obj as ILinePart);

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
                    .AddExtensionMethods(typeof(ILinePartExtensions))
                    .AddExtensionMethods(typeof(AssetKeyExtensions))
                    .AddInterface(typeof(ILinePart))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeyAssigned))
                    .AddInterface(typeof(ILinePart))
                    .AddInterface(typeof(IAssetKeyAssetAssigned))
                    .AddInterface(typeof(IAssetKeyAssignable))
                    .AddInterface(typeof(IAssetKeySectionAssigned))
                    .AddInterface(typeof(IAssetKeyLocationAssigned))
                    .AddInterface(typeof(ILineKeyType))
                    .AddInterface(typeof(IAssetKeyTypeAssignable))
                    .AddInterface(typeof(ILineKeyAssembly))
                    .AddInterface(typeof(IAssetKeyResourceAssigned))
                    .AddExtensionMethods(typeof(ILinePartExtensions))
                    .AddInterface(typeof(ILineKeyCulture))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssigned))
                    .AddInterface(typeof(ILocalizationKeyCulturePolicyAssignable))
                    .AddInterface(typeof(ILineFormatArgsPart))
                    .AddInterface(typeof(ILineInlinesAssigned))
                    .AddInterface(typeof(ILineInlines));
        }
    }

    public partial class LinePartAppender : ILinePartAppender0<ILinePart>
    {
        /// <summary>
        /// Append <see cref="LinePart"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <returns></returns>
        public ILinePart Append(ILinePart previous)
            => new LinePart(this, previous);
    }

    // StringLocalizerPart

    /*
    public partial class StringLocalizerPartAppender : ILinePartAppender2<ILineParameterPart, string, string>
    {
        /// <summary>
        /// Append <see cref="LineParameterPart"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public ILineParameterPart Append(ILinePart previous, string parameterName, string parameterValue)
            => new StringLocalizerParameterPart(this, previous, parameterName, parameterValue);
    }
*/



}
