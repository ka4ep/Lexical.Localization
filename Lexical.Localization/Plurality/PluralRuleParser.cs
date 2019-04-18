// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Parses rule string into expression
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Plural_rules_syntax"/>
    /// </summary>
    public class PluralRuleParser
    {
        /// <summary>
        /// Parse string into expression.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Exp Parse<Exp>(string str) where Exp : class, IExpression
        {
            Tokens reader = new Tokens(str);
            int ix = reader.Index;
            Tokens.Taker0<Exp> taker;
            if (typeof(Exp) == typeof(IPluralRulesExpression)) taker = (Tokens.Taker0<Exp>) (object)PluralRules;
            else throw new ArgumentException($"Cannot parse {nameof(Exp)}.");
            Exp exp = reader.Take<Exp>(taker);
            if (exp == null) throw new ArgumentException($"Could not parse {nameof(Exp)} \"{exp}\"");
            reader.TakeAll(TokenKind.NonEssential);
            if (reader.Index!=reader.EndIndex) throw new ArgumentException($"Could not fully parse {nameof(Exp)} \"{exp}\", index at {reader.Index}");
            return exp;
        }

        /// <summary>
        /// Read <see cref="IPluralRuleExpression"/>.
        /// 
        /// "§one i=1 and v=0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
        /// Or
        /// "#en #fi §one §one i=1 and v=0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
        /// </summary>
        public static Tokens.Taker0<IPluralRulesExpression> PluralRules = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // Names
            StructList16<string> names = new StructList16<string>();
            while (true)
            {
                // #
                reader.TakeAll(TokenKind.NonEssential);
                Token t = reader.Take(TokenKind.Hash);
                if (t == null) break;

                // Name
                string name = reader.Take(TokenKind.NameLiteral)?.Value?.ToString();
                if (name == null) { reader.Index = ix; return null; }

                // Add name
                names.Add(name);
            }

            // Rules
            StructList8<IPluralRuleExpression> rules = new StructList8<IPluralRuleExpression>();
            while (true)
            {
                reader.TakeAll(TokenKind.NonEssential);
                IPluralRuleExpression rule = reader.Take(PluralRule);
                if (rule == null) break;
                rules.Add(rule);
            }
            reader.TakeAll(TokenKind.NonEssential);

            // Create expression
            return new PluralRulesExpression(names.ToArray(), rules.ToArray());
        };

        /// <summary>
        /// Read rule set.
        /// 
        /// e.g. "$CLDRv35 #ast ca de en et fi fy gl ia io it ji nl pt_PT sc scn sv sw ur yi §one i = 1 and v = 0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
        /// </summary>
        public static Tokens.Taker0<IPluralRuleSetExpression> PluralRuleSet = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // (Optional) $Name
            string name = null;
            Token t = reader.Take(TokenKind.Dollar);
            if (t != null)
            {
                reader.Index = ix;
                name = reader.Take(TokenKind.NameLiteral)?.Value?.ToString();
                if (name == null) { reader.Index = ix; }
            }

            // Rules
            StructList8<IPluralRulesExpression> rulesList = new StructList8<IPluralRulesExpression>();
            while (true)
            {
                reader.TakeAll(TokenKind.NonEssential);
                IPluralRulesExpression rules = reader.Take(PluralRules);
                if (rules == null) break;
                rulesList.Add(rules);
            }
            reader.TakeAll(TokenKind.NonEssential);

            // Create expression
            return new PluralRuleSetExpression(name, rulesList.ToArray());
        };

        /// <summary>
        /// Read rule sets.
        /// 
        /// e.g. "$CLDRv35 #fi §other $CLDRv34 #fi §other".
        /// </summary>
        public static Tokens.Taker0<IPluralRuleSetsExpression> PluralRuleSets = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // Rulesetes
            StructList8<IPluralRuleSetExpression> rulesList = new StructList8<IPluralRuleSetExpression>();
            while (true)
            {
                reader.TakeAll(TokenKind.NonEssential);
                IPluralRuleSetExpression rules = reader.Take(PluralRuleSet);
                if (rules == null) break;
                rulesList.Add(rules);
            }
            reader.TakeAll(TokenKind.NonEssential);

            // Create expression
            return new PluralRuleSetsExpression(rulesList.ToArray());
        };

        /// <summary>
        /// Read <see cref="IPluralRuleExpression"/>.
        /// 
        /// "§one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
        /// </summary>
        public static Tokens.Taker0<IPluralRuleExpression> PluralRule = (ref Tokens reader) =>
        {
            // Whitespace
            reader.TakeAll(TokenKind.NonEssential);

            // (Optional) §Name
            string name = null;
            int ix = reader.Index;
            Token t = reader.Take(TokenKind.Section);
            if (t != null) {
                reader.Index = ix;
                name = reader.Take(TokenKind.NameLiteral)?.Value?.ToString();
                if (name == null) { reader.Index = ix; }
            }

            // Try read boolean expression
            reader.TakeAll(TokenKind.NonEssential);
            IExpression rule = reader.Take(BooleanExpression);
            reader.TakeAll(TokenKind.NonEssential);

            // Try read samples
            StructList4<ISamplesExpression> samplesList = new StructList4<ISamplesExpression>();
            ISamplesExpression samples = null;
            while ((samples = reader.Take(SamplesExpression)) != null)
            {
                samplesList.Add(samples);
                reader.TakeAll(TokenKind.NonEssential);
            }

            // Create expression
            return new PluralRuleExpression(name, rule, samplesList.ToArray());
        };

        /// <summary>
        /// Read <see cref="ISamplesExpression"/>
        /// 
        /// "@integer 0, 1, 2, 3, …"
        /// </summary>
        public static Tokens.Taker0<ISamplesExpression> SamplesExpression = (ref Tokens reader) =>
        {
            reader.TakeAll(TokenKind.NonEssential);

            // @
            int ix = reader.Index;
            Token t = reader.Take(TokenKind.At);
            if (t == null) { reader.Index = ix; return null; }

            // Name
            string name = reader.Take(TokenKind.NameLiteral)?.Value?.ToString();
            if (name == null) { reader.Index = ix; return null; }

            // Read integer and float samples
            reader.TakeAll(TokenKind.NonEssential);
            StructList16<IExpression> exps = new StructList16<IExpression>();
            Token token;
            bool tilde = false;
            while ((token = reader.Take(TokenKind.IntegerLiteral | TokenKind.FloatLiteral | TokenKind.Ellipsis | TokenKind.Tilde)) != null)
            { 
                if (token.Kind == TokenKind.IntegerLiteral || token.Kind == TokenKind.FloatLiteral)
                {
                    ConstantExpression exp = new ConstantExpression(token.Value);
                    if (tilde && exps.Count > 0 && exps[exps.Count - 1] is ConstantExpression prevExp)
                        exps[exps.Count - 1] = new RangeExpression(prevExp, exp);
                    else
                        exps.Add(exp);
                    tilde = false;
                } else if (token.Kind == TokenKind.Ellipsis)
                {
                    exps.Add(new InfiniteExpression());
                    break;
                } else if (token.Kind == TokenKind.Tilde)
                {
                    tilde = true;
                }
                reader.TakeAll(TokenKind.NonEssential);
            }

            // Create samples expression
            return new SamplesExpression(name, exps.ToArray());
        };

        /// <summary>
        /// Read boolean expression
        /// </summary>
        public static Tokens.Taker0<IExpression> BooleanExpression = (ref Tokens reader) =>
        {
            int ix = reader.Index;
            reader.TakeAll(TokenKind.NonEssential);

            // Try read binary
            // Try read unary
            // Try read literal

            IExpression exp1 = reader.Take(ValueExpression);
            if (exp1 == null) { reader.Index = ix; return null; }

            // Binary Op
            Token token = reader.Take(TokenKind.NameLiteral|TokenKind.Equals|TokenKind.InEquals2);
            if (token == null) { reader.Index = ix; return null; }

            if (token.Kind == TokenKind.NameLiteral && token.Value is String name)
            {
                if (name == "and")
                {

                } else if (name == "not")
                {

                } else
                {
                    reader.Index = ix; return null;
                }
            }


            return null;
        };


        /// <summary>
        /// Read any expression that returns a value.
        /// 
        /// One of:
        /// <see cref="IConstantExpression"/>
        /// <see cref="IRangeExpression"/>
        /// <see cref="IArgumentNameExpression"/>
        /// <see cref="IBinaryOpExpression"/>
        /// <see cref="IParenthesisExpression"/>
        /// </summary>
        public static Tokens.Taker0<IExpression> ValueExpression = (ref Tokens reader) =>
        {
            int ix = reader.Index;
            reader.TakeAll(TokenKind.NonEssential);
            // Unary operator
            // Value or variable
            Token token = reader.Take(TokenKind.IntegerLiteral|TokenKind.FloatLiteral|TokenKind.NameLiteral|TokenKind.LParenthesis);
            if (token == null) { reader.Index = ix; return null; }

            // Parenthesis
            if (token.Kind == TokenKind.LParenthesis)
            {
                reader.TakeAll(TokenKind.NonEssential);
                IExpression _exp = reader.Take(ValueExpression);
                if (_exp == null) { reader.Index = ix; return null; }
                reader.TakeAll(TokenKind.NonEssential);
                Token _rparenthesis = reader.Take(TokenKind.RParenthesis);
                if (_rparenthesis == null) { reader.Index = ix; return null; }
                return new ParenthesisExpression(_exp);
            }

            // Convert to expression
            IExpression exp = null;
            // Argument name
            if (token.Kind == TokenKind.NameLiteral && token.Value is string name)
            {
                exp = new ArgumentNameExpression(name);

                // TODO assert name is : 'n' | 'i' | 'f' | 't' | 'v' | 'w'
            }
            // Integer or Integer Range
            else if (token.Kind == TokenKind.IntegerLiteral)
            {
                // Read range
                reader.TakeAll(TokenKind.NonEssential);
                Token rangeToken = reader.Take(TokenKind.Range);
                if (rangeToken != null)
                {
                    reader.TakeAll(TokenKind.NonEssential);
                    Token rangeEndToken = reader.Take(TokenKind.IntegerLiteral | TokenKind.FloatLiteral);
                    if (rangeEndToken == null) { reader.Index = ix; return null; }

                    if (rangeEndToken.Kind == TokenKind.IntegerLiteral)
                    {
                        ConstantExpression rangeStart = new ConstantExpression(new DecimalNumber.Long((long)token.Value));
                        ConstantExpression rangeEnd = new ConstantExpression(new DecimalNumber.Long((long)token.Value));
                        exp = new RangeExpression(rangeStart, rangeEnd);
                    }
                    else
                    {
                        ConstantExpression rangeStart = new ConstantExpression(new DecimalNumber.Double((double)(long)token.Value));
                        ConstantExpression rangeEnd = new ConstantExpression(new DecimalNumber.Double((double)token.Value));
                        exp = new RangeExpression(rangeStart, rangeEnd);
                    }
                }
                else
                {
                    exp = new ConstantExpression(new DecimalNumber.Long((long)token.Value));
                }
            }
            // Float or Float Range
            else if (token.Kind == TokenKind.FloatLiteral)
            {
                exp = new ConstantExpression(new DecimalNumber.Double((double)token.Value));

                // Read range
                reader.TakeAll(TokenKind.NonEssential);
                Token rangeToken = reader.Take(TokenKind.Range);
                if (rangeToken != null)
                {
                    reader.TakeAll(TokenKind.NonEssential);
                    Token rangeEndToken = reader.Take(TokenKind.FloatLiteral);
                    if (rangeEndToken == null) { reader.Index = ix; return null; }
                    ConstantExpression rangeEndExp = new ConstantExpression(new DecimalNumber.Double((double)token.Value));
                    exp = new RangeExpression(exp as IConstantExpression, rangeEndExp);
                }
            }
            // Failed
            else { reader.Index = ix; return null; }

            // TODO Add list: ',' <value> along with range

            // Binary Op: %
            reader.TakeAll(TokenKind.NonEssential);
            Token binaryOpToken = reader.Take(TokenKind.Percent);
            if (binaryOpToken!=null)
            {
                reader.TakeAll(TokenKind.NonEssential);
                IExpression moduloExp = reader.Take(ValueExpression);
                if (moduloExp == null) { reader.Index = ix; return null; }
                exp = new BinaryOpExpression(BinaryOp.Modulo, exp, moduloExp);
            }

            // Return value
            return exp;
        };


    }
}
