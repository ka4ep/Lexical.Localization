// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// A composition of <see cref="IAsset"/> instances.
    /// </summary>
    public class AssetComposition : CopyOnWriteList<IAsset>, IAssetComposition, IAssetReloadable, IDisposable
    {
        Dictionary<Type, Array> snapshotByType = new Dictionary<Type, Array>();
        Dictionary<Type, Array> recursiveSnapshotByType = new Dictionary<Type, Array>();

        /// <summary>
        /// Create asset composition
        /// </summary>
        public AssetComposition() : base() { }

        /// <summary>
        /// Create asset composition
        /// </summary>
        /// <param name="assets"></param>
        public AssetComposition(params IAsset[] assets) : base(assets) { }

        /// <summary>
        /// Create asset composition
        /// </summary>
        /// <param name="assets"></param>
        public AssetComposition(IEnumerable<IAsset> assets) : base(assets) { }

        /// <summary>
        /// Immutable version of composition where components can only be added in the constructor.
        /// </summary>
        public new class Immutable : AssetComposition
        {
            /// <summary>
            /// Is read-only
            /// </summary>
            public override bool IsReadOnly => true;

            /// <summary>
            /// Create immutable asset composition.
            /// </summary>
            public Immutable() : base() { }

            /// <summary>
            /// Create immutable asset composition.
            /// </summary>
            /// <param name="strsEnumr"></param>
            public Immutable(params IAsset[] strsEnumr) : base(strsEnumr) { }

            /// <summary>
            /// Create immutable asset composition.
            /// </summary>
            /// <param name="strsEnumr"></param>
            public Immutable(IEnumerable<IAsset> strsEnumr) : base(strsEnumr) { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public override void Insert(int index, IAsset item) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public override bool Remove(IAsset item) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            public override void RemoveAt(int index) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            public override void Add(IAsset item) => throw new InvalidOperationException("Immutable");

            /// <summary>
            /// 
            /// </summary>
            public override void Clear() => throw new InvalidOperationException("Immutable");
        }

        /// <summary>
        /// Get components of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] _getComponents<T>()
        {
            lock (m_lock)
            {
                // Find existing
                Array array;
                if (!snapshotByType.TryGetValue(typeof(T), out array))
                    snapshotByType[typeof(T)] = array = Array.Where(_ => _ is T).Cast<T>().ToArray();
                return (T[])array;
            }
        }

        /// <summary>
        /// Get all compositions recursive, includes self.
        /// </summary>
        /// <returns></returns>
        IAssetComposition[] _getCompositionsRecursive()
        {
            lock(m_lock)
            {
                // Find existing
                Array array;
                if (recursiveSnapshotByType.TryGetValue(typeof(IAssetComposition), out array)) return (IAssetComposition[])array;

                List<IAssetComposition> list = new List<IAssetComposition>();
                list.Add(this);
                foreach(IAssetComposition child in _getComponents<IAssetComposition>())
                {
                    list.Add(child);
                    IEnumerable<IAssetComposition> decendents = child.GetComponents<IAssetComposition>(true);
                    if (decendents != null) list.AddRange(decendents);
                }
                recursiveSnapshotByType[typeof(IAssetComposition)] = array = list == null ? new IAssetComposition[0] : list.ToArray();
                return (IAssetComposition[])array;
            }
        }

        /// <summary>
        /// Get components recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] _getComponentsRecursive<T>() where T : IAsset
        {
            lock(m_lock)
            {
                // Find existing
                Array array;
                if (recursiveSnapshotByType.TryGetValue(typeof(T), out array)) return (T[])array;

                // Now build result
                IEnumerable<T> list = null;
                foreach (IAssetComposition composition in _getCompositionsRecursive())
                {
                    IEnumerable<T> _result = composition.GetComponents<T>(false);
                    if (_result != null && (_result is Array _array ? _array.Length>0 : true)) list = list == null ? _result : list.Concat(_result);
                }
                // Add recursive
                recursiveSnapshotByType[typeof(T)] = array = list == null ? new T[0] : list.ToArray();
                return (T[]) array;
            }
        }

        /// <summary>
        /// Get components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public IEnumerable<T> GetComponents<T>(bool recursive) where T : IAsset
           => recursive ? _getComponentsRecursive<T>() : _getComponents<T>();

        /// <summary>
        /// Clear cache
        /// </summary>
        protected override void ClearCache()
        {
            lock (m_lock)
            {
                base.ClearCache();
                snapshotByType.Clear();
                recursiveSnapshotByType.Clear();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="errors"></param>
        protected virtual void Dispose(ref StructList4<Exception> errors)
        {
            foreach (IDisposable disposable in _getComponents<IDisposable>())
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            lock (m_lock)
            {
                Clear();
                ClearCache();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            StructList4<Exception> errors = new StructList4<Exception>();
            Dispose(ref errors);
            if (errors.Count == 1) throw new Exception(errors[0].Message, errors[0]);
            if (errors.Count > 1) throw new AggregateException(errors);
        }

        /// <summary>
        /// Reload components
        /// </summary>
        /// <returns></returns>
        public IAsset Reload()
        {
            foreach (IAssetReloadable reloadable in _getComponentsRecursive<IAssetReloadable>())
                reloadable.Reload();
            ClearCache();
            return this;
        }

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({string.Join(", ", Array.Select(a=>a.ToString()))})";
    }
}
