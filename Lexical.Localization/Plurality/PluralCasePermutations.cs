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
    /// PluralCasePermutations yields keys that have been appended with plural cases ("N:case").
    /// It yields all permutations of different cases.
    /// 
    /// Yields keys in following order:
    ///   1. All optional case permutations
    ///   2. Combination of optional and required case permutations
    ///   3. Required case permutations
    ///   4. Permutations where one of the cases are omited
    ///   
    /// The last yielded key is the original key.
    /// 
    /// For instance, for format string "{cardinal:0} {cardinal:1}" and evaluated cases of ["zero","other"] and ["zero", "other"]
    /// would yield the following keys.
    /// 
    /// N:zero:N1:zero
    /// N:other:N1:zero
    /// N:zero:N1:other
    /// N:other:N1:other
    /// N1:zero
    /// N1:other
    /// N:zero
    /// N:other
    /// --
    /// 
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
        public readonly ILine key;

        /// <summary>
        /// Number of permutations in the first step of the yield operation.
        /// Returns all cases where every case is represented.
        /// </summary>
        int count1;

        /// <summary>
        /// Number of all permutations. Note, that the last item of the count is the original key.
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
            this.count1 = 1;
        }

        /// <summary>
        /// Add placeholder and cases that apply to that placeholder.
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="placeholderValue"></param>
        /// <param name="pluralRules"></param>
        /// <param name="culture"></param>
        public void AddPlaceholder(IPlaceholder placeholder, IPluralNumber placeholderValue, IPluralRules pluralRules, string culture)
        {
            IExpression e = placeholder?.Expression;
            if (e == null) return;

            // Query cases
            PluralRuleInfo query = new PluralRuleInfo(null, placeholder.PluralCategory, culture, null, -1);
            IPluralRule[] cases = pluralRules.Evaluate(query, placeholderValue);
            if (cases == null || cases.Length == 0) return;
            int optionalCount = cases.Length == 1 ? 0 : cases.Length - 1;
            int requiredCount = cases.Length > 0 ? 1 : 0;

            // Scan arguments
            StructList4<int> argIndices = new StructList4<int>();
            GetArguments(e, ref argIndices);

            // Add and unify cases
            for (int ix=0; ix<argIndices.Count; ix++)
            {
                int argIx = argIndices[ix];

                // Find if argument already exists
                int prevIx = -1;
                for (int j = 0; j < arguments.Count; j++) if (arguments[j].ArgumentIndex == argIx) { prevIx = j; break; }

                if (prevIx<0)
                {
                    // Add argument
                    Entry entry = new Entry { Cases = cases, ArgumentIndex = argIx, OptionalCases = optionalCount, RequiredCases = requiredCount };
                    arguments.Add(entry);
                    this.Count *= (1 + cases.Length);
                    this.count1 *= cases.Length;
                }
                else
                {
                    // Previous entry
                    Entry entry = arguments[prevIx];
                    // Unify entries
                    StructList8<IPluralRule> optionalCases = new StructList8<IPluralRule>(IPluralRuleComparer.Default);
                    StructList8<IPluralRule> requiredCases = new StructList8<IPluralRule>(IPluralRuleComparer.Default);
                    foreach(var c in entry.Cases)
                    {
                        if (c.Info.Optional == 1) optionalCases.AddIfNew(c);
                        else if (c.Info.Optional == 0) requiredCases.AddIfNew(c);
                    }
                    foreach (var c in cases)
                    {
                        if (c.Info.Optional == 1) optionalCases.AddIfNew(c);
                        else if (c.Info.Optional == 0) requiredCases.AddIfNew(c);
                    }
                    StructList8<IPluralRule> allCases = new StructList8<IPluralRule>(IPluralRuleComparer.Default);
                    for (int i = 0; i < optionalCases.Count; i++) allCases.Add(optionalCases[i]);
                    for (int i = 0; i < requiredCases.Count; i++) allCases.Add(requiredCases[i]);

                    // Create new entry
                    Entry newEntry = new Entry { Cases = allCases.ToArray(), ArgumentIndex = argIx, OptionalCases = optionalCases.Count, RequiredCases = requiredCases.Count };

                    // Update
                    arguments[prevIx] = newEntry;

                    this.Count /= (1 + entry.Cases.Length);
                    this.Count *= (1 + newEntry.Cases.Length);
                    this.count1 /= entry.Cases.Length;
                    this.count1 *= newEntry.Cases.Length;
                }

            }
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

                if (index < count1)
                {
                    //  1. optional cases
                    //  2. optional and mandatory cases
                    //  3. mandatory cases
                    for (int ph = 0; ph < arguments.Count; ph++)
                    {
                        var e = arguments[ph];
                        int len = e.Cases.Length;
                        int arg_ix = index % len;
                        index /= len;
                        if (arg_ix < len)
                        {
                            string caseName = e.Cases[arg_ix].Info.Case;
                            int n_ix = e.ArgumentIndex;
                            result = result.N(n_ix, caseName);
                        }
                    }
                    //  XXX. This implementation doesn't work exactly correctly in a rare case where there are more than one mandatory case per entry.
                    //  It occurs only when there is string with two categories for same argument in different placeholders, and there are multiple placeholder that do this.
                    //  e.g. "{cardinal:0} {ordinal:0} {cardinal:1} {ordinal:1}". 
                    // 
                    //  It's not worth addressing, since this usecase makes no sense, and the current implementation works well enough.
                }

                else
                {
                    //  4. omit cases
                    index -= count1;
                    // bitmask of arguments that are null
                    ulong arg_is_null_mask = 1;
                    while (index>=0)
                    {
                        // Count number of cases for this arg_is_null_mask
                        int c = 1;
                        for (int ph = 0; ph < arguments.Count; ph++)
                        {
                            ulong mask = 1UL << ph;
                            if ((arg_is_null_mask & mask) == mask) c *= arguments[ph].Cases.Length;
                        }
                        if (c == 0) break;
                        // Go to next arg_is_null_mask
                        if (index >= c) { arg_is_null_mask++; index -= c; continue; }
                        // Yield key from this arg_is_null_mask
                        for (int arg = 0; arg < arguments.Count; arg++)
                        {
                            // Argument is null
                            ulong mask = 1UL << arg;
                            if ((arg_is_null_mask & mask) == mask) continue;
                            //
                            var e = arguments[arg];
                            int len = e.Cases.Length;
                            int arg_ix = index % len;
                            index /= len;

                            if (arg_ix < len)
                            {
                                string caseName = e.Cases[arg_ix].Info.Case;
                                int n_ix = e.ArgumentIndex;
                                result = result.N(n_ix, caseName);
                            }
                        }
                        index -= c;
                    }
                }

                System.Console.WriteLine(LineFormat.Line.Print(result));
                return result;
            }
        }

        /// <summary>
        /// Get all the argument indices that are expressed in <paramref name="exp"/>.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="argumentIndices"></param>
        static void GetArguments(IExpression exp, ref StructList4<int> argumentIndices)
        {
            StructList8<IExpression> stack = new StructList8<IExpression>();
            stack.Add(exp);
            while (stack.Count>0)
            {
                IExpression e = stack.Dequeue();
                if (e is IArgumentIndexExpression arg) argumentIndices.Add(arg.Index);
                if (e is ICompositeExpression compositeExpression)
                    for(int i=0; i<compositeExpression.ComponentCount; i++)
                    {
                        IExpression ce = compositeExpression.GetComponent(i);
                        if (ce != null) stack.Add(ce);
                    }
            }
        }

        struct Entry
        {
            public IPluralRule[] Cases;
            public int ArgumentIndex;
            public int OptionalCases;
            public int RequiredCases;
        }
    }
}
