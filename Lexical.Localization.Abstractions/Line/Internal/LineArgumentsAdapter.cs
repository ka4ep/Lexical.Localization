// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Line.Internal
{
    /// <summary>
    /// Adapts <see cref="ILineArguments"/> into type-casted query.
    /// </summary>
    public class LineArgumentsAdapterComposition : ILineFactoryByArgument
    {
        /// <summary>
        /// Default instance
        /// </summary>
        static LineArgumentsAdapterComposition instance = new LineArgumentsAdapterComposition();

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static LineArgumentsAdapterComposition Instance => instance;

        ConcurrentDictionary<Type, LineArgumentsAdapter> adapters0 = new ConcurrentDictionary<Type, LineArgumentsAdapter>();

        public bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
        {
            // Should there be two factories? factory to cast and factory to use in the call?
            line = default;
            return false;
        }
    }
    abstract class LineArgumentsAdapter : ILineFactoryByArgument
    {
        public abstract bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line);
    }
    class LineArgumentsAdapter<Intf> : LineArgumentsAdapter
    {
        public override bool TryCreate(ILineFactory factory, ILine previous, ILineArguments arguments, out ILine line)
        {
            //if (factory is ILineArguments
            line = default;
            return false;
        }
    }


}
