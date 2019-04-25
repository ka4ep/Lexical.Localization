// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Resolver that instantiates "RuleSet" parameter by
    /// instantiating assembly qualitifed type name, or by parsing expression string.
    /// </summary>
    public class PluralRulesResolver : IPluralRulesEvaluatable
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly PluralRulesResolver instance = new PluralRulesResolver();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static PluralRulesResolver Instance => instance;

        /// <summary>
        /// Function that resolves type name into <see cref="Type"/>.
        /// </summary>
        protected Func<Assembly, string, bool, Type> typeResolver;

        /// <summary>
        /// Function that reads assembly from file.
        /// </summary>
        protected Func<AssemblyName, Assembly> assemblyResolver;

        /// <summary>
        /// Function that parses expressions.
        /// </summary>
        protected Func<string, IEnumerable<IPluralRule>> ruleExpressionParser;

        /// <summary>
        /// Cache of type-resolved rule sets.
        /// </summary>
        protected ConcurrentDictionary<string, ResultLine> resolvedRulesetCache = new ConcurrentDictionary<string, ResultLine>();

        /// <summary>
        /// Cache of parsed rule sets.
        /// </summary>
        protected ConcurrentDictionary<string, ResultLine> parsedRulesCache = new ConcurrentDictionary<string, ResultLine>();

        /// <summary>
        /// Function that resolves type name to ruless.
        /// </summary>
        protected Func<string, ResultLine> resolveRuleSetFunc;

        /// <summary>
        /// Function that resolves expression to rules.
        /// </summary>
        protected Func<string, ResultLine> parseRulesFunc;

        /// <summary>
        /// Create rule resolver with default settings.
        /// 
        /// Parses expressions and instantiates types that are found in the app domain.
        /// Does not load external dll files.
        /// </summary>
        public PluralRulesResolver() : this(LocalAssemblyResolver, TypeResolver, RuleExpressionParser)
        {
        }

        /// <summary>
        /// Create plural rules resolver
        /// </summary>
        /// <param name="assemblyLoader">(optional) function that reads assembly from file.</param>
        /// <param name="typeResolver">(optional) Function that resolves type name into <see cref="Type"/>.</param>
        /// <param name="ruleExpressionParser">(optional) rule parser. If null, will not parser expressions.</param>
        public PluralRulesResolver(Func<AssemblyName, Assembly> assemblyLoader, Func<Assembly, string, bool, Type> typeResolver, Func<string, IEnumerable<IPluralRule>> ruleExpressionParser)
        {
            this.assemblyResolver = assemblyLoader;
            this.typeResolver = typeResolver;
            this.ruleExpressionParser = ruleExpressionParser;
            this.resolveRuleSetFunc = (string rulesetName) =>
            {
                try
                {
                    IPluralRulesEnumerable enumr = ResolveRuleSet(rulesetName);
                    return new ResultLine { Rules = enumr };
                }
                catch (Exception e)
                {
                    return new ResultLine { Error = e };
                }
            };
            this.parseRulesFunc = (string exp) =>
            {
                try
                {
                    // Assert ruleExpressionParser is not null
                    if (ruleExpressionParser == null) throw new InvalidOperationException($"{nameof(ruleExpressionParser)} is null");
                    // Parse
                    IEnumerable<IPluralRule> enumr = this.ruleExpressionParser(exp);
                    // Cast
                    IPluralRulesEnumerable rulesEnumr = enumr is IPluralRulesEnumerable casted ? casted : new PluralRules(enumr);
                    // Result
                    return new ResultLine { Rules = rulesEnumr };
                }
                catch (Exception e)
                {
                    // Error result
                    return new ResultLine { Error = e };
                }
            };

        }

        /// <summary>
        /// Function that parses ruleset expression into <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns>parsed rules</returns>
        /// <exception cref="ArgumentException">error with string</exception>
        public static Func<string, IEnumerable<IPluralRule>> RuleExpressionParser = (string rulesExpression) =>
        {
            // Create expression parser
            IEnumerable<IPluralRuleExpression> expressionParser = PluralRuleExpressionParser.CreateParser(rulesExpression);
            // Convert to rules
            IEnumerable<IPluralRule> rules = expressionParser.Select(exp => new PluralRule.ExpressionCase(exp.Infos, exp.Rule, exp.Samples));
            // Read into array
            return new PluralRules(rules);
        };

        /// <summary>
        /// Assembly resolver that looks into AppDomain, but will not load external file.
        /// </summary>
        public static Func<AssemblyName, Assembly> LocalAssemblyResolver = asmName => Assembly.Load(asmName);

        /// <summary>
        /// Assembly resolver that searches dlls from application directory.
        /// </summary>
        public static Func<AssemblyName, Assembly> FileAssemblyResolver = asmName =>
        {
            try
            {
                Assembly a = Assembly.Load(asmName);
                if (a != null) return a;
            } catch (Exception)
            {
            }

            string dir = typeof(PluralRulesResolver).Assembly.Location;
            if (dir != null)
            {
                string dllName = asmName.Name + ".dll";
                string dllPath = Path.Combine(dir, asmName.Name + ".dll");
                if (!File.Exists(dllPath)) throw new FileNotFoundException(dllName);
                return Assembly.LoadFile(dllPath);
            }
            return null;
        };

        /// <summary>
        /// Default type resolver that does following name mapping.
        /// 
        /// Converts "Unicode.CLDR" -> "Lexical.Localization.UnicodeCLDR"
        /// </summary>
        public static Func<Assembly, string, bool, Type> TypeResolver = (Assembly a, string typename, bool throwOnError) =>
        {
            if (typename.StartsWith("Unicode.CLDR")) typename = "Lexical.Localization.UnicodeCLDR" + typename.Substring("Unicode.CLDR".Length);
            return a.GetType(typename);
        };

        /// <summary>
        /// Get-cached-or-resolve ruleset name into <see cref="IPluralRulesEnumerable"/>
        /// </summary>
        /// <param name="rulesetTypeName"></param>
        /// <returns>resolve result</returns>
        public ResultLine GetOrResolveRuleSet(string rulesetTypeName)
            => resolvedRulesetCache.GetOrAdd(rulesetTypeName, resolveRuleSetFunc);

        /// <summary>
        /// Get-cached-or-parse rule expresion into <see cref="IPluralRulesEnumerable"/>
        /// </summary>
        /// <param name="rulesExpression"></param>
        /// <returns>parse result</returns>
        public ResultLine GetOrParseRules(string rulesExpression)
            => parsedRulesCache.GetOrAdd(rulesExpression, parseRulesFunc);

        /// <summary>
        /// Resolve ruleset name into <see cref="IPluralRulesEnumerable"/>. Does not use cache. Please use <see cref="GetOrResolveRuleSet(string)"/> instead as it caches result.
        /// </summary>
        /// <param name="rulesetTypeName"></param>
        /// <returns>rules</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rulesetTypeName"/> is null</exception>
        /// <exception cref="TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
        /// <exception cref="TypeLoadException">type is not found</exception>
        /// <exception cref="ArgumentException">An error occurs when typeName is parsed into a type name and an name contains invalid syntax </exception>
        /// <exception cref="FileNotFoundException">assembly is not not found</exception>
        /// <exception cref="FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.</exception>
        /// <exception cref="BadImageFormatException">The assembly or one of its dependencies is not valid</exception>
        /// <exception cref="InvalidCastException">If ruleset doesn't implement IEnumerable&lt;IPluralRule&gt;</exception>
        public IPluralRulesEnumerable ResolveRuleSet(string rulesetTypeName)
        {
            // Assert assemblyResolver is not null
            if (assemblyResolver == null) throw new InvalidOperationException($"{nameof(assemblyResolver)} is null");
            // Assert typeResolver is not null
            if (typeResolver == null) throw new InvalidOperationException($"{nameof(typeResolver)} is null");
            // Try get type
            Type type = Type.GetType(rulesetTypeName, assemblyResolver, typeResolver, true);
            // Assert type was loaded
            if (type == null) throw new TypeLoadException($"Could not resolve Type {rulesetTypeName}.");
            // Assert type is assignable
            if (!typeof(IEnumerable<IPluralRule>).IsAssignableFrom(type)) throw new InvalidCastException($"{rulesetTypeName} doesn't implement {nameof(IEnumerable<IPluralRule>)}");
            // Instantiate type
            object obj = Activator.CreateInstance(type);
            // Cast
            IEnumerable<IPluralRule> enumr = (IEnumerable<IPluralRule>)obj;
            // Read into array
            return new PluralRules(enumr);
        }

        /// <summary>
        /// Try to ruleset and number.
        /// </summary>
        /// <param name="subset"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="Exception">On wrapped error</exception>
        public IPluralRule[] Evaluate(PluralRuleInfo subset, IPluralNumber number)
        {
            if (subset.RuleSet != null)
            {
                ResultLine line = GetOrResolveRuleSet(subset.RuleSet);
                if (line.Error != null) throw new Exception(line.Error.Message, line.Error);
                if (line.Rules is IPluralRulesEvaluatable eval) return eval.Evaluate(subset, number);
            }
            return null;
        }

        /// <summary>
        /// Record that contains either result or an error.
        /// </summary>
        public struct ResultLine
        {
            /// <summary>
            /// Error
            /// </summary>
            public Exception Error;

            /// <summary>
            /// Result
            /// </summary>
            public IPluralRulesEnumerable Rules;
        }

    }

}
