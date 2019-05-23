// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           22.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Represents a list of possible plural case permutations.
    /// 
    /// For instance, for key "Key:Successful" and placeholders [0]=zero,other and [1]=one
    /// would permutate keys:
    /// 
    /// <list type="bullet">
    /// <item>"Key:Successful:N:other:N1:one"</item>
    /// <item>"Key:Successful:N:other"</item>
    /// <item>"Key:Successful:N:zero:N1:one"</item>
    /// <item>"Key:Successful:N:zero"</item>
    /// <item>"Key:Successful:N1:one"</item>
    /// <item>"Key:Successful"</item>
    /// </list>
    /// </summary>
    public struct PluralCasePermutations
    {
        /// <summary>
        /// Information about placeholders
        /// </summary>
        StructList8<Entry> placeholders;

        /// <summary>
        /// Base key to append "N" parts to.
        /// </summary>
        ILine key;

        /// <summary>
        /// Number of entries.
        /// </summary>
        public int Count;

        /// <summary>
        /// Create plural key permutation enumerable
        /// </summary>
        /// <param name="key"></param>
        public PluralCasePermutations(ILine key) : this()
        {
            this.key = key;
            this.Count = 1;
        }

        /// <summary>
        /// Add placeholder and cases that apply to that placeholder.
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="cases"></param>
        public void AddPlaceholder(IPlaceholder placeholder, IPluralRule[] cases)
        {
            placeholders.Add(new Entry { Placeholder = placeholder, Cases = cases });
            this.Count *= (1 + cases.Length);
        }

        /// <summary>
        /// Get line that has "N:" plural case parts appended
        /// </summary>
        /// <param name="index"></param>
        /// <returns>cases</returns>
        public ILine this[int index]
        {
            get
            {
                ILine result = key;

                for (int ph = 0; ph < placeholders.Count; ph++)
                {
                    var e = placeholders[ph];
                    int len = e.Cases.Length + 1;
                    int ph_ix = index % (len + 1);
                    index /= (len + 1);

                    if (ph_ix < len)
                    {
                        string caseName = e.Cases[ph_ix].Info.Case;
                        int n_ix = e.Placeholder.PlaceholderIndex;
                        result = result.N(n_ix, caseName);
                    }
                }

                return result;
            }
        }

        struct Entry
        {
            public IPlaceholder Placeholder;
            public IPluralRule[] Cases;
        }
    }
}
