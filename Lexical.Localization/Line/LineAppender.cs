// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Default part appender.
    /// </summary>
    public partial class LineAppender : LineFactoryComposition
    {
        private readonly static ILineFactory instance = new LineAppender().ReadOnly();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ILineFactory Default => instance;

        /// <summary>
        /// Create new part factory with default factories.
        /// </summary>
        public LineAppender()
        {
        }
    }

    /// <summary>
    /// Default part appender.
    /// </summary>
    public partial class StringLocalizerAppender : LineFactoryComposition
    {
        private readonly static ILineFactory instance = new StringLocalizerAppender().ReadOnly();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ILineFactory Default => instance;

        /// <summary>
        /// Create new part factory with default factories.
        /// </summary>
        public StringLocalizerAppender()
        {
        }
    }

}
