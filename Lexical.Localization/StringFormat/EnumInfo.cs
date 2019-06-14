// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Information about enumeration and its cases.
    /// 
    /// Reads <see cref="FlagsAttribute"/>.
    /// Reads <see cref="DescriptionAttribute"/> of each case.
    /// </summary>
    public class EnumInfo : IEnumInfo
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Enumeration has <see cref="FlagsAttribute"/>.
        /// </summary>
        public bool Flags { get; protected set; }

        /// <summary>
        /// Case informations.
        /// </summary>
        public IEnumCase[] Cases { get; protected set; }

        /// <summary>
        /// Cases put into evaluation order.
        /// 
        /// Ordered by 1. number of bits, decending, 2. original position.
        /// </summary>
        public IEnumCase[] EvalCases { get; protected set; }

        /// <summary>
        /// Key to enumration
        /// </summary>
        public ILine Key { get; protected set; }

        /// <summary>
        /// Create enumeration information
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="key"></param>
        /// <param name="cases"></param>
        public EnumInfo(bool flags, ILine key, params IEnumCase[] cases)
        {
            this.Flags = flags;
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
            this.Cases = cases ?? throw new ArgumentNullException(nameof(cases));
            this.EvalCases = SortEvalOrder(this.Cases);
        }

        /// <summary>
        /// Create enum info from <paramref name="enumType"/>.
        /// 
        /// If <paramref name="key"/> is not provided then "Assembly:asm:Type:enumType" is used.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="key">(optional) Key to use for searching enumeration in localization assets</param>
        public EnumInfo(Type enumType, ILine key = default)
        {
            if (!enumType.IsEnum) throw new ArgumentException("not enum " + enumType.FullName, nameof(enumType));
            this.Flags = enumType.GetCustomAttribute(typeof(FlagsAttribute)) != null;
            this.Key = key ?? LineAppender.NonResolving.Assembly(enumType.Assembly).Type(enumType);
            Array array = Enum.GetValues(enumType);
            this.Cases = new IEnumCase[array.Length];
            for (int i = 0; i < Cases.Length; i++)
            {
                Enum _case = (Enum)array.GetValue(i);
                ILine caseKey = this.Key.Key(_case.ToString());
                this.Cases[i] = new EnumCase(_case, caseKey);
            }
            this.EvalCases = SortEvalOrder(this.Cases);
        }

        /// <summary>
        /// Sort <paramref name="cases"/> into order that is suitable for evaluation.
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        public static IEnumCase[] SortEvalOrder(IEnumCase[] cases)
        {
            int c = cases.Length;

            // Make records
            (IEnumCase, int, int)[] list = new (IEnumCase, int, int)[c];
            for (int i = 0; i < c; i++) list[i] = (cases[i], EnumCase.CountBits(cases[i].Value), i);

            // Sort by 1. bits, 2. index
            Array.Sort(list, _comparer.instance);

            // Create result
            IEnumCase[] result = new IEnumCase[c];
            for (int i = 0; i < c; i++) result[i] = list[i].Item1;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Name;

        /// <summary>
        /// (case, bits, index)
        /// </summary>
        class _comparer : IComparer<(IEnumCase, int, int)>
        {
            public static _comparer instance = new _comparer();
            public int Compare((IEnumCase, int, int) x, (IEnumCase, int, int) y)
            {
                // Compare by bits
                int d = y.Item2 - x.Item2;
                if (d != 0) return d;

                // Compare by index
                d = x.Item3 - y.Item3;
                return d;
            }
        }
    }

    /// <summary>
    /// Information about enumeration case.
    /// </summary>
    public class EnumCase : IEnumCase
    {
        /// <summary>
        /// Name of the case.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// (optional) Description.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Value in bits
        /// </summary>
        public ulong Value { get; protected set; }

        /// <summary>
        /// Key to use to for searching from asset.
        /// </summary>
        public ILine Key { get; protected set; }

        /// <summary>
        /// Case index in <see cref="IEnumInfo" />.
        /// </summary>
        public int CaseIndex { get; protected set; }

        /// <summary>
        /// Create enumeration case record.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="value"></param>
        /// <param name="caseIndex">case index in <see cref="IEnumInfo"/></param>
        /// <param name="key"></param>
        public EnumCase(string name, string description, ulong value, int caseIndex, ILine key)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Value = value;
            this.CaseIndex = caseIndex;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <summary>
        /// Create enumeration case record from <paramref name="enumCase"/>.
        /// 
        /// <see cref="Description"/> is read from <see cref="DescriptionAttribute"/>.
        /// 
        /// <paramref name="enumCase"/> is the value as bits.
        /// </summary>
        /// <param name="enumCase"></param>
        /// <param name="key">(optional) key to the case. If null then "Assembly:asm:Type:enumtype:Key:enumcase" is used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public EnumCase(Enum enumCase, ILine key = default)
        {
            // Asserts
            if (enumCase == null) throw new ArgumentNullException(nameof(enumCase));
            Type enumType = enumCase.GetType();
            if (!enumType.IsEnum) throw new ArgumentException("not enum " + enumType.FullName, nameof(enumCase));

            // Name
            Name = enumCase.ToString();

            // Description
            Description = enumType.GetMember(Name).FirstOrDefault()?.GetCustomAttribute<DescriptionAttribute>()?.Description;

            // Value
            Value = ToUInt64(enumCase);

            // Key
            Key = key ?? LineAppender.NonResolving.Assembly(enumType.Assembly).Type(enumType).Key(Name);

            // Case index
            string[] names = Enum.GetNames(enumType);
            for (int i = 0; i < names.Length; i++)
                if (Name == names[i]) { CaseIndex = i; break; }
        }

        /// <summary>
        /// Convert <paramref name="enumValue"/> to ulong value.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static ulong ToUInt64(Enum enumValue)
        {
            Type underlyingType = System.Enum.GetUnderlyingType(enumValue.GetType());
            bool isSigned = typeof(Int32).Equals(underlyingType) || typeof(Int64).Equals(underlyingType) || typeof(Int16).Equals(underlyingType) || typeof(SByte).Equals(underlyingType);
            return (isSigned ? unchecked((ulong)((IConvertible)enumValue).ToInt64(null)) : ((IConvertible)enumValue)).ToUInt64(null);
        }

        /// <summary>
        /// Calculate the number of bits
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBits(ulong value)
        {
            int bits = 0;
            ulong x = 1;
            for (int i = 0; i < 64; i++)
            {
                if ((x & value) != 0UL) bits++;
                x <<= 1;
            }
            return bits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Name;
    }


    /// <summary>
    /// Dictionary containing enum infos.
    /// </summary>
    public class EnumInfoResolver : ConcurrentDictionary<Type, IEnumInfo>, IEnumInfoResolver
    {
        static Func<Type, ILine> defaultEnumTypeKeyFactory = t => LineAppender.NonResolving.Assembly(t.Assembly).Type(t);
        static Lazy<EnumInfoResolver> instance = new Lazy<EnumInfoResolver>();

        /// <summary>
        /// Singleton map.
        /// 
        /// Note, keeps strong references to types and assemblies acquired with this map.
        /// At the time of writing, .net can't unload assemblies anyway.
        /// </summary>
        public static EnumInfoResolver Default => instance.Value;

        /// <summary>
        /// Function that creates enuminfo from type.
        /// </summary>
        protected Func<Type, IEnumInfo> infoFactory;

        /// <summary>
        /// Function that creates a search key for an enum type.
        /// 
        /// Search key is used for searching enumeration localization strings from asset.
        /// </summary>
        protected Func<Type, ILine> enumTypeKeyFactory;

        /// <summary>
        /// Default function that creates a search key for enum type.
        /// 
        /// Search key is used for searching enumeration localization strings from asset.
        /// </summary>
        public static Func<Type, ILine> DefaultEnumTypeKeyFactory => defaultEnumTypeKeyFactory;

        /// <summary>
        /// Create map of enum types.
        /// </summary>
        public EnumInfoResolver()
        {
            this.enumTypeKeyFactory = DefaultEnumTypeKeyFactory;
            this.infoFactory = t => new EnumInfo(t, this.enumTypeKeyFactory(t));
        }

        /// <summary>
        /// Create map of enum types.
        /// </summary>
        /// <param name="enumTypeKeyFactory">function that creates search key for enum type.</param>
        public EnumInfoResolver(Func<Type, ILine> enumTypeKeyFactory)
        {
            this.enumTypeKeyFactory = enumTypeKeyFactory ?? throw new ArgumentNullException(nameof(enumTypeKeyFactory));
            this.infoFactory = t => new EnumInfo(t, this.enumTypeKeyFactory(t));
        }

        /// <summary>
        /// Get-or-create enum info from <paramref name="enumType"/>.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IEnumInfo GetEnumInfo(Type enumType)
            => GetOrAdd(enumType, infoFactory);

    }

}
