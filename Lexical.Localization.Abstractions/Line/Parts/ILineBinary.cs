// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Binary;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization binary resource.
    /// </summary>
    public interface ILineBinary : ILine
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        byte[] Binary { get; set; }
    }
    
    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="ILineBinary"/> part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineBinary Binary(this ILine part, byte[] value)
            => part.Append<ILineBinary, byte[]>(value);

        /// <summary>
        /// Create <see cref="ILineBinary"/> part.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineBinary Binary(this ILineFactory lineFactory, byte[] value)
            => lineFactory.Create<ILineBinary, byte[]>(null, value);

        /// <summary>
        /// Create inlined <paramref name="subkey"/> resource.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="subkey">subkey in parametrized format, e.g. "Culture:en"</param>
        /// <param name="value">(optional) value to append, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="lineFactory"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILineFactory lineFactory, ILine subkey, byte[] value)
        {
            ILineInlines inlines = lineFactory.Create<ILineInlines>(null);
            if (value != null) subkey = subkey.Binary(value);
            if (subkey != null) inlines[subkey] = subkey;
            return inlines;
        }

        /// <summary>
        /// Try get the <paramref name="line"/>'s bytes.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns>true if part was found</returns>
        public static bool TryGetBinary(this ILine line, out byte[] result)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineBinary valuePart && valuePart.Binary != null) { result = valuePart.Binary; return true; }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Finds a part that implements <see cref="ILineBinary"/> or is a parameter "Value.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns>true if part was found</returns>
        public static bool TryGetBinaryPart(this ILine line, out ILine result)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineBinary valuePart && valuePart.Binary != null) { result = part; return true; }
            }
            result = default;
            return false;
        }

    }

}

