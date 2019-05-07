// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Line parts form a linked list or trie of a <see cref="ILine"/>.
    /// </summary>
    public interface ILinePart : ILine
    {
        /// <summary>
        /// (Optional) Previous part.
        /// </summary>
        ILine PreviousPart { get; set; }
    }

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Get previous part
        /// </summary>
        /// <param name="line"></param>
        /// <returns>part or null</returns>
        public static ILine GetPreviousPart(this ILine line)
            => line is ILinePart part ? part.PreviousPart : null;
    }
}
