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
        /// <param name="formatProviders"></param>
        /// <returns></returns>
        public static void Add(ref IFormatProvider slot, params IFormatProvider[] formatProviders)
        {
            if (formatProviders == null || formatProviders.Length == 0) return;

            // Set provider
            if (slot == null)
            {
                if (formatProviders.Length == 1) slot = formatProviders[0];
                else slot = new FormatProviderComposition(formatProviders);
            }

            // Append to previous composition
            else if (slot is FormatProviderComposition composition)
            {
                IFormatProvider[] arr = new IFormatProvider[composition.Providers.Length + formatProviders.Length];
                composition.Providers.CopyTo(arr, 0);
                formatProviders.CopyTo(arr, composition.Providers.Length);
                slot = new FormatProviderComposition(arr);
            }
            // Replace value with composition
            else
            {
                IFormatProvider[] arr = new IFormatProvider[1 + formatProviders.Length];
                arr[0] = slot;
                formatProviders.CopyTo(arr, 1);
                slot = new FormatProviderComposition(arr);
            }
        }
    }
}
