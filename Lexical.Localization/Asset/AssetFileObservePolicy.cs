// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           30.8.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Policy whether asset files should be observed and reloaded when modified.
    /// </summary>
    public class AssetFileObservePolicy : ObservableValue<bool>, IAssetObservePolicy
    {
        /// <summary>
        /// Read-only static instance for policy that observes files.
        /// </summary>
        protected static IAssetObservePolicy observing = new AssetFileObservePolicy(true, true);

        /// <summary>
        /// Read-only static instance for policy that doesn't observe files.
        /// </summary>
        protected static IAssetObservePolicy noObserving = new AssetFileObservePolicy(false, true);

        /// <summary>
        /// Read-only static instance for policy that observes files.
        /// </summary>
        public static IAssetObservePolicy Observing => observing;

        /// <summary>
        /// Read-only static instance for policy that doesn't observe files.
        /// </summary>
        public static IAssetObservePolicy NoObserving => noObserving;

        /// <summary>
        /// Policy whether assest files should be observed.
        /// </summary>
        public bool Observe { get => Value; set => Value = Value; }

        /// <summary>
        /// Create observe policy with false default value.
        /// </summary>
        public AssetFileObservePolicy()
        {
        }

        /// <summary>
        /// Create observe policy with <paramref name="observe"/> initial value.
        /// </summary>
        /// <param name="observe"></param>
        /// <param name="isReadonly"></param>
        public AssetFileObservePolicy(bool observe, bool isReadonly) : base(observe, isReadonly)
        {
        }
    }
}
