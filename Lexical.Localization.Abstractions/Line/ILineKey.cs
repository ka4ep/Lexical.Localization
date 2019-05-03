// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the keypart is used as a key, and is hash-equals comparable
    /// </summary>
    public interface ILineKey : ILineParameterPart
    {
    }

    /// <summary>
    /// Signals that the key part is to be compared non-canonically. 
    /// 
    /// Non-canonical key parts are compared so that their order of appearance doesn't matter.
    /// </summary>
    public interface ILineKeyNonCanonicallyCompared : ILineKey
    {
    }

    /// <summary>
    /// Signals that this key part is to be compared non-canonically.
    /// 
    /// The order of canonical key parts must match compared in the order of occurance.
    /// </summary>
    public interface ILineKeyCanonicallyCompared : ILineKey
    {
    }

    /// <summary>
    /// Interface for a line that can enumerate non-canonically compared keys (from root towards tail).
    /// </summary>
    public interface ILineKeyNonCanonicallyComparedEnumerable : ILine, IEnumerable<KeyValuePair<string, string>> // <-- XXX Cannot be implemented
    {
         // <- Add GetEnumerable here
    }

    /// <summary>
    /// Interface for a line that can enumerate canonically compared keys (from root towards tail).
    /// </summary>
    public interface ILineKeyCanonicallyComparedEnumerable : ILine, IEnumerable<KeyValuePair<string, string>>
    {
        // <- Add GetEnumerable here
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Get part that implements <see cref="ILineKeyCanonicallyCompared"/>, either this or preceding, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns><paramref name="part"/>, or preceding canonical part or null</returns>
        public static ILineKeyCanonicallyCompared GetCanonicalKey(this ILinePart part)
        {
            for (ILinePart p = part; p != null; p = p is ILinePart linkedKey ? linkedKey.PreviousPart : null)
            {
                if (p is ILineKeyCanonicallyCompared kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get preceding part that implements <see cref="ILineKeyCanonicallyCompared"/>, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>preceding canonical part or null</returns>
        public static ILineKeyCanonicallyCompared GetPreviousCanonicalKey(this ILinePart part)
        {
            for (ILinePart k = part is ILinePart lkk ? lkk.PreviousPart : null; k != null; k = k is ILinePart nlkk ? nlkk.PreviousPart : null)
            {
                if (k is ILineKeyCanonicallyCompared kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get part that implements <see cref="ILineKeyNonCanonicallyCompared"/>, either this or preceding one, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns><paramref name="part"/>, or preceding non-canonical part or null</returns>
        public static ILineKeyNonCanonicallyCompared GetNonCanonicalKey(this ILinePart part)
        {
            for (ILinePart k = part; k != null; k = k is ILinePart linkedKey ? linkedKey.PreviousPart : null)
            {
                if (k is ILineKeyNonCanonicallyCompared kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get preceding part that implements <see cref="ILineKeyNonCanonicallyCompared"/>, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>preceding non-canonical part or null</returns>
        public static ILineKeyNonCanonicallyCompared GetPreviousNonCanonicalKey(this ILinePart part)
        {
            for (ILinePart k = part is ILinePart lkk ? lkk.PreviousPart : null; k != null; k = k is ILinePart nlkk ? nlkk.PreviousPart : null)
            {
                if (k is ILineKeyNonCanonicallyCompared kk) return kk;
            }
            return null;
        }

    }
}
