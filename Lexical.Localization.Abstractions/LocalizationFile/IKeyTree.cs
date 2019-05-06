// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Tree structure that holds key-values. 
    /// 
    /// Used for reading and writing structure based localization files such as .xml and .json.
    /// </summary>
    public interface IKeyTree
    {
        /// <summary>
        /// Parent node, unless is root then null.
        /// </summary>
        IKeyTree Parent { get; }

        /// <summary>
        /// Parameters that are associated with this particular node.
        /// </summary>
        ILine Key { get; set; }

        /// <summary>
        /// Associated values.
        /// </summary>
        IList<IFormulationString> Values { get; }

        /// <summary>
        /// Test if has child nodes.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Child nodes.
        /// </summary>
        IReadOnlyCollection<IKeyTree> Children { get; }

        /// <summary>
        /// Test if has values.
        /// </summary>
        bool HasValues { get; }

        /// <summary>
        /// Add new child to the key tree.
        /// </summary>
        /// <returns>new child</returns>
        IKeyTree CreateChild();

        /// <summary>
        /// Search children by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>child nodes or null if none was found</returns>
        IEnumerable<IKeyTree> GetChildren(ILine key);

        /// <summary>
        /// Remove self from parent.
        /// </summary>
        void Remove();
    }
}
