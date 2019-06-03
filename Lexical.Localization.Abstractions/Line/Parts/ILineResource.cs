// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Resource;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization string value.
    /// </summary>
    public interface ILineResource : ILine
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        byte[] Resource { get; set; }
    }
    
    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="ILineResource"/> part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineResource Resource(this ILine part, byte[] value)
            => part.Append<ILineResource, byte[]>(value);

        /// <summary>
        /// Create <see cref="ILineResource"/> part.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineResource Resource(this ILineFactory lineFactory, byte[] value)
            => lineFactory.Create<ILineResource, byte[]>(null, value);

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
            if (value != null) subkey = subkey.Resource(value);
            if (subkey != null) inlines[subkey] = subkey;
            return inlines;
        }

        /// <summary>
        /// Try get the <paramref name="line"/>'s bytes.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns>true if part was found</returns>
        public static bool TryGetResource(this ILine line, out byte[] result)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineResource valuePart && valuePart.Resource != null) { result = valuePart.Resource; return true; }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Finds a part that implements <see cref="ILineResource"/> or is a parameter "Value.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns>true if part was found</returns>
        public static bool TryGetResourcePart(this ILine line, out ILine result)
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineResource valuePart && valuePart.Resource != null) { result = part; return true; }
            }
            result = default;
            return false;
        }

    }

}

