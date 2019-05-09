// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Localization string value.
    /// </summary>
    public interface ILineValue : ILine
    {
        /// <summary>
        /// Localization string value.
        /// </summary>
        IFormulationString Value { get; set; }
    }

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="ILineValue"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineValue Value(this ILine part, IFormulationString value)
            => part.Append<ILineValue, IFormulationString>(value);

        /// <summary>
        /// Get the <see cref="IFormulationString"/> of a <see cref="ILineValue"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>value or null</returns>
        public static IFormulationString GetValue(this ILine line)
        {
            for (ILine p = line; p != null; p = p.GetPreviousPart())
                if (p is ILineValue valuePart && valuePart.Value != null) return valuePart.Value;
            return null;
        }
    }
}

