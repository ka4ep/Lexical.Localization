// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the keypart is used as a key, and is hash-equals comparable
    /// </summary>
    public interface ILineKey : ILineParameter
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
    /// Interface for a line that can enumerate non-canonically compared keys
    /// </summary>
    public interface ILineKeysNonCanonicallyCompared : ILine
    {
        /// <summary>
        /// Non-canonically compared keys.
        /// 
        /// The preferred implementation is of <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> NonCanonicallyComparedKeys { get; set; }
    }

    /// <summary>
    /// Interface for a line that can enumerate canonically compared keys (from root towards tail).
    /// </summary>
    public interface ILineKeysCanonicallyCompared : ILine
    {
        /// <summary>
        /// Canonically compared keys.
        /// 
        /// The preferred implementation is of <see cref="IList{T}"/>
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> CanonicallyComparedKeys { get; set; }
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

        /// <summary>
        /// Get all non-canonical keys as parameterName,parameterValue dictionary.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>dictionary of keys</returns>
        public static IReadOnlyDictionary<string, string> GetNonCanonicalKeys(this ILine line)
        {
            if (line is ILineKeysNonCanonicallyCompared lineKeys)
            {
                var enumr = lineKeys.NonCanonicallyComparedKeys;
                if (enumr is IReadOnlyDictionary<string, string> map) return map;
                Dictionary<string, string> result = new Dictionary<string, string>();
                if (enumr != null) foreach (var key in enumr) result[key.Key] = key.Value;
                return result;
            }

            if (line is ILinePart part)
            {
                int count = 0;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineKeyNonCanonicallyCompared key && key.ParameterName != null && key.ParameterValue != null) count++;

                Dictionary<string, string> result = new Dictionary<string, string>(count);
                int ix = count;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                {
                    if (p is ILineKeyNonCanonicallyCompared key && key.ParameterName != null && key.ParameterValue != null)
                        result[key.ParameterName] = key.ParameterValue;
                }

                return result;
            }

            return no_noncanonicalkeys;
        }
        static IReadOnlyDictionary<string, string> no_noncanonicalkeys = new Dictionary<string, string>(0);

        /// <summary>
        /// Get value of non-canonical key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="parameterName"></param>
        /// <returns>value or null</returns>
        public static string GetNonCanonicalKey(this ILine line, string parameterName)
        {
            if (line is ILineKeysNonCanonicallyCompared lineKeys)
            {
                var enumr = lineKeys.NonCanonicallyComparedKeys;
                if (enumr is IReadOnlyDictionary<string, string> map)
                {
                    string result;
                    if (map.TryGetValue(parameterName, out result)) return result;
                }
                if (enumr != null)
                {
                    string result = null;
                    foreach (var kv in enumr)
                        if (kv.Key == parameterName) return kv.Value;
                    return result;
                }
            }

            if (line is ILinePart tail)
            {
                for (ILinePart p = tail; p != null; p = p.PreviousPart)
                    if (p is ILineKeyNonCanonicallyCompared key && key.ParameterName == parameterName)
                        return key.ParameterValue;
            }

            return null;
        }

        /// <summary>
        /// Get all canonical keys as parameterName,parameterValue list.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>dictionary of keys</returns>
        public static IList<KeyValuePair<string, string>> GetCanonicalKeys(this ILine line)
        {
            if (line is ILineKeysCanonicallyCompared lineKeys)
            {
                var enumr = lineKeys.CanonicallyComparedKeys;
                return enumr is IList<KeyValuePair<string, string>> list ? list : enumr.ToList();
            }

            if (line is ILinePart part)
            {
                int count = 0;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineKeyCanonicallyCompared key && key.ParameterName != null && key.ParameterValue != null) count++;

                if (count == 0) return no_keys;

                KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
                int ix = count;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineParameter parameter && parameter.ParameterName != null && parameter.ParameterValue != null)
                        result[--ix] = new KeyValuePair<string, string>(parameter.ParameterName, parameter.ParameterValue);

                return result;
            }

            return no_keys;
        }
        static KeyValuePair<string, string>[] no_keys = new KeyValuePair<string, string>[0];

    }
}
