// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Compares formulation strings
    /// </summary>
    public class FormulationStringComparer : IEqualityComparer<IFormulationString>, IComparer<IFormulationString>
    {
        private static FormulationStringComparer instance = new FormulationStringComparer(FormulationStringPartComparer.Instance, FormulationStringPartComparer.Instance);

        /// <summary>
        /// Default instance
        /// </summary>
        public static FormulationStringComparer Instance => instance;

        IEqualityComparer<IFormulationStringPart> partComparer;
        IComparer<IFormulationStringPart> partComparer2;

        /// <summary>
        /// Create part comparer
        /// </summary>
        /// <param name="partComparer"></param>
        /// <param name="partComparer2"></param>
        public FormulationStringComparer(IEqualityComparer<IFormulationStringPart> partComparer, IComparer<IFormulationStringPart> partComparer2)
        {
            this.partComparer = partComparer;
            this.partComparer2 = partComparer2;
        }

        /// <summary>
        /// Compare formulation strings for sorting order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1, 0, 1</returns>
        public int Compare(IFormulationString x, IFormulationString y)
        {
            string _x = x?.Text, _y = y?.Text;
            if (_x == null && _y == null) return 0;
            if (_x == null) return -1;
            if (_y == null) return 1;
            return _x.CompareTo(_y);
        }

        /// <summary>
        /// Compare formulation strings for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IFormulationString x, IFormulationString y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x == y) return true;
            if (x.Status != y.Status) return false;
            var x_parts = x.Parts;
            var y_parts = y.Parts;
            if (x_parts.Length != y_parts.Length) return false;
            int c = x_parts.Length;
            for (int i = 0; i < c; i++)
                if (!partComparer.Equals(x_parts[i], y_parts[i])) return false;
            return true;
        }

        /// <summary>
        /// Calculate hashcode.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int GetHashCode(IFormulationString o)
        {
            if (o == null) return 0;
            int result = FNVHashBasis;

            // Hash Status
            UInt64 code = (UInt64) o.Status.Formulation();
            result ^= (int)(code & 0xffffffff);
            result ^= (int)(code>>32);
            result *= FNVHashPrime;

            // Hash FormatProvider
            if (o.FormatProvider != null) { result ^= o.FormatProvider.GetHashCode(); result *= FNVHashPrime; }

            // Hash Parts
            foreach (var part in o.Parts)
            {
                result ^= partComparer.GetHashCode(part);
                result *= FNVHashPrime;
            }
            return result;
        }

        const int FNVHashBasis = unchecked((int)2166136261);
        const int FNVHashPrime = 16777619;
    }

    /// <summary>
    /// Compares formulation strings
    /// </summary>
    public class FormulationStringPartComparer : IEqualityComparer<IFormulationStringPart>, IComparer<IFormulationStringPart>
    {
        private static FormulationStringPartComparer instance = new FormulationStringPartComparer();

        /// <summary>
        /// Default instance
        /// </summary>
        public static FormulationStringPartComparer Instance => instance;

        /// <summary>
        /// Compare formulation string parts for sorting order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-1, 0, 1</returns>
        public int Compare(IFormulationStringPart x, IFormulationStringPart y)
        {
            string _x = x?.Text, _y = y?.Text;
            if (_x == null && _y == null) return 0;
            if (_x == null) return -1;
            if (_y == null) return 1;
            return _x.CompareTo(_y);
        }

        /// <summary>
        /// Compare formulation strings for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IFormulationStringPart x, IFormulationStringPart y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x == y) return true;
            if (x.Kind != y.Kind) return false;

            if (x.Kind == FormulationStringPartKind.Text)
            {
                if (x.Text != y.Text) return false;
            }

            if (x.Kind == FormulationStringPartKind.Placeholder)
            {
                var x_arg = x as IPlaceholder;
                var y_arg = y as IPlaceholder;
                if (x_arg.PlaceholderIndex != y_arg.PlaceholderIndex) return false;
                if ((x_arg.Expression == null) != (y_arg.Expression == null)) return false;
                if ((x_arg.Expression != null) && (y_arg.Expression != null)) return StringFormatExpressionEquals.Equals(x_arg, y_arg);
            }

            return true;
        }

        /// <summary>
        /// Calculate hashcode.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int GetHashCode(IFormulationStringPart o)
        {
            if (o == null) return 0;
            int result = FNVHashBasis;

            if (o.Kind == FormulationStringPartKind.Text)
            {
                result ^= o.Text.GetHashCode();
                result *= FNVHashPrime;
            }

            if (o.Kind == FormulationStringPartKind.Placeholder && o is IPlaceholder arg)
            {
                result = new StringFormatExpressionHashCode().Hash(arg.Expression).Hash(arg.PlaceholderIndex);
            }

            return result;
        }

        const int FNVHashBasis = unchecked((int)2166136261);
        const int FNVHashPrime = 16777619;
    }
}
