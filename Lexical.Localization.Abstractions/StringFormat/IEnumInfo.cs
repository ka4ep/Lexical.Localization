// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// <see cref="IEnumInfo"/> resolver.
    /// </summary>
    public interface IEnumInfoResolver
    {
        /// <summary>
        /// Get enum info from <paramref name="enumType"/>.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        IEnumInfo GetEnumInfo(Type enumType);
    }

    /// <summary>
    /// Information about enumeration type.
    /// </summary>
    public interface IEnumInfo
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Enumeration has <see cref="FlagsAttribute"/>.
        /// </summary>
        bool Flags { get; }

        /// <summary>
        /// Case informations.
        /// </summary>
        IEnumCase[] Cases { get; }

        /// <summary>
        /// Cases in evaluation order.
        /// 
        /// Ordered by 1. number of bits, decending, 2. original position in <see cref="Cases"/>.
        /// </summary>
        IEnumCase[] EvalCases { get; }

        /// <summary>
        /// Search key to the enumration. Used for 
        /// </summary>
        ILine Key { get; }
    }

    /// <summary>
    /// Information about enumeration case.
    /// </summary>
    public interface IEnumCase
    {
        /// <summary>
        /// Name of the case.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// (optional) Description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Value in bits
        /// </summary>
        ulong Value { get; }

        /// <summary>
        /// Key to use to for searching from asset.
        /// </summary>
        ILine Key { get; }

        /// <summary>
        /// Case index in <see cref="IEnumInfo" />.
        /// </summary>
        int CaseIndex { get; }
    }
}
