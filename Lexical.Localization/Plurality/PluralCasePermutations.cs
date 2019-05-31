// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           22.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
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
        /// Information about arguments
        /// </summary>
        StructList8<Entry> arguments;

        /// <summary>
        /// Base key to append "N" parts to.
        /// </summary>
        ILine key;

        /// <summary>
        /// Number of permutations.
        /// </summary>
        public int Count;

        /// <summary>
        /// Number of arguments.
        /// </summary>
        public int ArgumentCount => arguments.Count;

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
            IExpression e = placeholder?.Expression;
            if (e == null || cases == null || cases.Length == 0) return;

            // TODO: Scan ArgumentIndexes, capture cases, unify cases.


            arguments.Add(new Entry { Placeholder = placeholder, Cases = cases });
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

                // TODO Change this algorithm so that: 
                //  1. offers all optional cases
                //  2. all permutations of optional and mandatory
                //  3. the one mandatory case
                //  4. no cases
                // Update contract above.

                for (int ph = 0; ph < arguments.Count; ph++)
                {
                    var e = arguments[ph];
                    int len = e.Cases.Length + 1;
                    int ph_ix = index % len;
                    index /= len;

                    if (ph_ix < e.Cases.Length)
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
