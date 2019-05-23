// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Composition of format providers
    /// </summary>
    public class FormatProviderComposition : IFormatProvider
    {
        /// <summary>
        /// Providers
        /// </summary>
        public readonly IFormatProvider[] Providers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providers"></param>
        public FormatProviderComposition(IFormatProvider[] providers)
        {
            this.Providers = providers;
        }

        /// <summary>
        /// Search format provider from all component format providers.
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public object GetFormat(Type formatType)
        {
            foreach (IFormatProvider fp in Providers)
            {
                object o = fp.GetFormat(formatType);
                if (o != null) return o;
            }
            return null;
        }
    }

    /// <summary></summary>
    public static partial class FormatProviderExtensions
    {
        /// <summary>
        /// Add format provider to format provider slot. Replace with composition if contains already previous provider.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static void Add(ref IFormatProvider slot, IFormatProvider formatProvider)
        {
            if (formatProvider == null || slot == formatProvider) return;
            // Set provider
            if (slot == null) slot = formatProvider;
            // Append to previous composition
            else if (slot is FormatProviderComposition composition)
            {
                if (!composition.Contains(formatProvider)) composition.Add(formatProvider);
            }
            // Replace value with composition
            else
            {
                // Replace 
                IFormatProvider old = slot;
                FormatProviderComposition c = new FormatProviderComposition();
                c.Add(old);
                c.Add(formatProvider);
                slot = c;
            }
        }
    }
}
