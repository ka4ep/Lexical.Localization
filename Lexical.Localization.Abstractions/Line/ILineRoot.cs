// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------

namespace Lexical.Localization
{
    /// <summary>
    /// This interface signals that this key is the root key.
    /// Used with dependency injection to inject root class.
    /// </summary>
    public interface ILineRoot : ILine
    {
    }

}
