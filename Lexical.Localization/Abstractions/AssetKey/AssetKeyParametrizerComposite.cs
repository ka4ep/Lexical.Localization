using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Asset
{
    /// <summary>
    /// Composition of <see cref="IAssetKeyParametrizer" />s.
    /// </summary>
    public class AssetKeyParametrizerComposite : IAssetKeyParametrizer
    {
        public readonly IAssetKeyParametrizer[] components;

        public AssetKeyParametrizerComposite(params IAssetKeyParametrizer[] components)
        {
            this.components = components ?? throw new ArgumentNullException(nameof(components));
        }

        public AssetKeyParametrizerComposite(IEnumerable<IAssetKeyParametrizer> components)
        {
            this.components = components?.ToArray() ?? throw new ArgumentNullException(nameof(components));
        }

        IEnumerable<object> IAssetKeyParametrizer.Break(object key)
        {
            foreach(IAssetKeyParametrizer component in components)
            {
                IEnumerable<object> result = component.Break(key);
                if (result != null) return result;
            }
            return null;
        }

        object IAssetKeyParametrizer.GetPreviousPart(object part)
        {
            foreach (IAssetKeyParametrizer component in components)
            {
                object result = component.GetPreviousPart(part);
                if (result != null) return result;
            }
            return null;
        }

        string[] IAssetKeyParametrizer.GetPartParameters(object part)
        {
            foreach (IAssetKeyParametrizer component in components)
            {
                string[] result = component.GetPartParameters(part);
                if (result != null) return result;
            }
            return null;
        }

        string IAssetKeyParametrizer.GetPartValue(object part, string parameter)
        {
            foreach (IAssetKeyParametrizer component in components)
            {
                string result = component.GetPartValue(part, parameter);
                if (result != null) return result;
            }
            return null;
        }

        object IAssetKeyParametrizer.TryCreatePart(object key, string parameterName, string parameterValue)
        {
            foreach (IAssetKeyParametrizer component in components)
            {
                object result = component.TryCreatePart(key, parameterName, parameterValue);
                if (result != null) return result;
            }
            return null;
        }

        void IAssetKeyParametrizer.VisitParts<T>(object key, ParameterPartVisitor<T> visitor, ref T data)
        {
            foreach (IAssetKeyParametrizer component in components)
                component.VisitParts(key, visitor, ref data);
        }

        public bool IsCanonical(object part, string parameterName)
        {
            foreach (IAssetKeyParametrizer component in components)
            {
                bool result = component.IsCanonical(part, parameterName);
                if (result) return true;
            }
            return false;
        }

        public bool IsNonCanonical(object part, string parameterName)
        {
            foreach (IAssetKeyParametrizer component in components)
            {
                bool result = component.IsNonCanonical(part, parameterName);
                if (result) return true;
            }
            return false;
        }

    }

    public static partial class AssetKeyParametrizerExtensions
    {
        /// <summary>
        /// Concatenate two or more <see cref="IAssetKeyParametrizer"/>s.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>concatenation</returns>
        public static IAssetKeyParametrizer Concat(this IAssetKeyParametrizer left, IAssetKeyParametrizer right)
        {
            IEnumerable<IAssetKeyParametrizer> left_enumr = left is AssetKeyParametrizerComposite left_composite ? left_composite.components : Enumerable.Repeat(left, 1);
            IEnumerable<IAssetKeyParametrizer> right_enumr = right is AssetKeyParametrizerComposite right_composite ? right_composite.components : Enumerable.Repeat(right, 1);
            return new AssetKeyParametrizerComposite(left_enumr.Concat(right_enumr));
        }

        /// <summary>
        /// Concatenate two or more <see cref="IAssetKeyParametrizer"/>s.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right_enumr"></param>
        /// <returns>concatenation</returns>
        public static IAssetKeyParametrizer Concat(this IAssetKeyParametrizer left, IEnumerable<IAssetKeyParametrizer> right_enumr)
        {
            IEnumerable<IAssetKeyParametrizer> left_enumr = left is AssetKeyParametrizerComposite left_composite ? left_composite.components : Enumerable.Repeat(left, 1);
            return new AssetKeyParametrizerComposite(left_enumr.Concat(right_enumr));
        }


        /// <summary>
        /// Concatenate two or more <see cref="IAssetKeyParametrizer"/>s.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right_array"></param>
        /// <returns>concatenation</returns>
        public static IAssetKeyParametrizer Concat(this IAssetKeyParametrizer left, IAssetKeyParametrizer[] right_array)
        {
            IEnumerable<IAssetKeyParametrizer> left_enumr = left is AssetKeyParametrizerComposite left_composite ? left_composite.components : Enumerable.Repeat(left, 1);
            return new AssetKeyParametrizerComposite(left_enumr.Concat(right_array));
        }
    }

}
