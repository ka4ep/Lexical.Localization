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

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Plural rule expression parser.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Plural_rules_syntax"/>
    /// </summary>
    public class PluralRuleExpressionParser
    {
        /// <summary>
        /// Parse string into expressions.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">error with string</exception>
        public static IEnumerable<IPluralRuleExpression> CreateParser(string str)
        {
            Tokens reader = new Tokens(str);
            int ix = reader.Index;
            IPluralRuleExpression exp;
            while ((exp = reader.Take(PluralRule))!=null)
                yield return exp;
            reader.TakeAll(TokenKind.NonEssential);
            if (reader.Index!=reader.EndIndex) throw new ArgumentException($"Could not fully parse {nameof(Exp)} \"{str}\", index at {reader.Index}");
        }

        /// <summary>
        /// Read <see cref="IPluralRuleInfosExpression"/>.
        /// 
        /// e.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one]"
        /// </summary>
        public readonly static Tokens.Taker0<IPluralRuleInfosExpression> PluralRuleInfos = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // [
            reader.Take(TokenKind.NonEssential);
            Token t = reader.Take(TokenKind.LBracket);
            if (t == null) { reader.Index = ix; return null; }

            StructList8<IPluralRuleInfoExpression> infos = new StructList8<IPluralRuleInfoExpression>();
            while (true)
            {
                IPluralRuleInfoExpression info = reader.Take(PluralRuleInfo);
                if (info == null) break;
                infos.Add(info);

                reader.Take(TokenKind.NonEssential);
                t = reader.Take(TokenKind.Comma);
                if (t == null) break;
            }

            // ]
            reader.Take(TokenKind.NonEssential);
            t = reader.Take(TokenKind.RBracket);
            if (t == null) { reader.Index = ix; return null; }

            // Create expression
            return new PluralRuleInfosExpression(infos.ToArray());
        };


        /// <summary>
        /// Read <see cref="IPluralRuleInfoExpression"/>.
        /// 
        /// e.g. "RuleSet=Unicode.CLDRv35"
        /// </summary>
        public readonly static Tokens.Taker0<IPluralRuleInfoExpression> PluralRuleInfo = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // Name
            reader.Take(TokenKind.NonEssential);
            Token t = reader.Take(TokenKind.NameLiteral | TokenKind.AtNameLiteral | TokenKind.AtQuotedNameLiteral | TokenKind.AtLongQuotedNameLiteral);
            string name = t?.Value?.ToString();
            if (name == null) { reader.Index = ix; return null; }

            // =
            reader.Take(TokenKind.NonEssential);
            t = reader.Take(TokenKind.Equals);
            if (t == null) { reader.Index = ix; return null; }

            // Value
            reader.Take(TokenKind.NonEssential);
            t = reader.Take(TokenKind.NameLiteral | TokenKind.AtNameLiteral | TokenKind.AtQuotedNameLiteral | TokenKind.AtLongQuotedNameLiteral | TokenKind.IntegerLiteral);
            string value = t?.Value?.ToString();
            if (value == null) { reader.Index = ix; return null; }

            return new PluralRuleInfoExpression(name, value);
        };

        /// <summary>
        /// Read <see cref="IPluralRuleExpression"/>.
        /// 
        /// "§one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
        /// </summary>
        public readonly static Tokens.Taker0<IPluralRuleExpression> PluralRule = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // infos, e.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one]"
            reader.TakeAll(TokenKind.NonEssential);
            IPluralRuleInfosExpression infos = reader.Take(PluralRuleInfos);            

            // Try read boolean expression
            reader.TakeAll(TokenKind.NonEssential);
            IExpression rule = reader.Take(BooleanExpression);
            //if (rule == null) { reader.Index = ix; return null; }
            reader.TakeAll(TokenKind.NonEssential);

            // Try read samples
            StructList4<ISamplesExpression> samplesList = new StructList4<ISamplesExpression>();
            ISamplesExpression samples = null;
            while ((samples = reader.Take(SamplesExpression)) != null)
            {
                samplesList.Add(samples);
                reader.TakeAll(TokenKind.NonEssential);
            }

            // No result
            if (infos == null && rule == null && samplesList.Count == 0) { reader.Index = ix; return null; }

            // Create expression
            return new PluralRuleExpression(infos, rule, samplesList.ToArray());
        };

        /// <summary>
        /// Read <see cref="ISamplesExpression"/>
        /// 
        /// "@integer 0, 1, 2, 3, …"
        /// </summary>
        public readonly static Tokens.Taker0<ISamplesExpression> SamplesExpression = (ref Tokens reader) =>
        {
            int ix = reader.Index;
            reader.TakeAll(TokenKind.NonEssential);

            // @Name
            Token t = reader.Take(TokenKind.AtNameLiteral);
            string name = t?.Value?.ToString();
            if (name == null) { reader.Index = ix; return null; }

            // Read integer and float samples
            reader.TakeAll(TokenKind.NonEssential);
            StructList16<IExpression> exps = new StructList16<IExpression>();
            Token token;
            bool tilde = false;
            while ((token = reader.Take(TokenKind.IntegerLiteral | TokenKind.FloatLiteral | TokenKind.Ellipsis | TokenKind.Tilde | TokenKind.Comma)) != null)
            { 
                if (token.Kind == TokenKind.IntegerLiteral || token.Kind == TokenKind.FloatLiteral)
                {
                    ConstantExpression exp = new ConstantExpression(token.Text);
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
                } else if (token.Kind == TokenKind.Comma)
                {
                    // move to next
                    if (tilde) throw new ArgumentException("Sample range '~' left open.");
                }
                reader.TakeAll(TokenKind.NonEssential);
            }
            if (tilde) throw new ArgumentException("Sample range '~' left open.");

            // Create samples expression
            return new SamplesExpression(name, exps.ToArray());
        };

        /// <summary>
        /// Read boolean expression.
        /// 
        /// Rules:
        ///  "and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).
        /// </summary>
        public readonly static Tokens.Taker0<IExpression> BooleanExpression = (ref Tokens reader) =>
        {
            int ix = reader.Index;

            // Boolean exp
            IExpression exp = null;

            // Read UnaryOp or constant "true" or "false"
            {
                int ix2 = reader.Index;
                reader.TakeAll(TokenKind.NonEssential);
                Token token = reader.Take(TokenKind.Exclamation | TokenKind.NameLiteral);
                // "true"
                if (token != null && token.Kind == TokenKind.NameLiteral && token.Value is String name)
                {
                    if (name == "true")
                    {
                        exp = new ConstantExpression(true);
                    } else if (name == "false")
                    {
                        exp = new ConstantExpression(false);
                    } else if (name == "not")
                    {
                        IExpression nextExp = reader.Take(BooleanExpression);
                        if (nextExp != null) exp = new UnaryOpExpression(UnaryOp.Not, nextExp); else reader.Index = ix2;
                    } else
                    {
                        reader.Index = ix2;
                    }
                }
                else if (token != null && token.Kind == TokenKind.Exclamation)
                {
                    IExpression nextExp = reader.Take(BooleanExpression);
                    if (nextExp != null) exp = new UnaryOpExpression(UnaryOp.Not, nextExp); else reader.Index = ix2;
                }
                else reader.Index = ix2;
            }

            // Read equality or inequality: exp1 ¤ exp2
            if (exp == null)
            {
                IExpression exp1 = reader.Take(ValueExpression);
                if (exp1 != null)
                {
                    int ix_ = reader.Index;
                    // Binary Op
                    Token token = reader.Take(TokenKind.Equals | TokenKind.InEquals2 | TokenKind.Lt | TokenKind.Gt | TokenKind.LtOrEq | TokenKind.GtOrEq);
                    if (token == null) { reader.Index = ix; return null; }

                    // Value
                    reader.TakeAll(TokenKind.NonEssential);
                    IExpression exp2 = reader.Take(ValueExpression);
                    if (exp2 != null)
                    {
                        // exp1 = exp2
                        if (token.Kind == TokenKind.Equals) exp = new BinaryOpExpression(BinaryOp.Equal, exp1, exp2);
                        // exp1 != exp2
                        else if (token.Kind == TokenKind.InEquals2) exp = new BinaryOpExpression(BinaryOp.NotEqual, exp1, exp2);
                        // exp1 < exp2
                        else if (token.Kind == TokenKind.Lt) exp = new BinaryOpExpression(BinaryOp.LessThan, exp1, exp2);
                        // exp1 <= exp2
                        else if (token.Kind == TokenKind.LtOrEq) exp = new BinaryOpExpression(BinaryOp.LessThanOrEqual, exp1, exp2);
                        // exp1 > exp2
                        else if (token.Kind == TokenKind.Gt) exp = new BinaryOpExpression(BinaryOp.GreaterThan, exp1, exp2);
                        // exp1 >= exp2
                        else if (token.Kind == TokenKind.GtOrEq) exp = new BinaryOpExpression(BinaryOp.GreaterThanOrEqual, exp1, exp2);
                        // Revert
                        else reader.Index = ix_;
                    }
                }
            }

            // Continue boolean expression
            if (exp != null)
            {
                int ix2 = reader.Index;
                IExpression exp2 = reader.Take(BooleanBinaryOp, exp);
                if (exp2 != null) exp = exp2; else reader.Index = ix2;
            }

            // No boolean expression
            if (exp == null) { reader.Index = ix; return null; }

            // Return boolean exp
            return exp;
        };

        /// <summary>
        /// Read boolean expression with already read left expression.
        /// 
        /// Rules:
        ///  "and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).
        /// </summary>
        public readonly static Tokens.Taker1<IExpression, IExpression> BooleanBinaryOp = (ref Tokens reader, IExpression leftExp) =>
        {
            if (leftExp == null) return null;

            int ix = reader.Index;

            // Boolean exp
            IExpression exp = null;

            // ¤
            Token token = reader.Take(TokenKind.NameLiteral | TokenKind.Equals2 | TokenKind.InEquals2);
            if (token == null) { reader.Index = ix; return leftExp; }

            // Expression
            reader.TakeAll(TokenKind.NonEssential);
            IExpression rightExp = reader.Take(BooleanExpression);
            if (rightExp != null)
            {
                // exp1 and exp2
                if (token.Kind == TokenKind.NameLiteral && token.Value is String name_ && name_ == "and") exp = new BinaryOpExpression(BinaryOp.LogicalAnd, leftExp, rightExp);
                // exp1 or exp2
                else if (token.Kind == TokenKind.NameLiteral && token.Value is String name__ && name__ == "or") exp = new BinaryOpExpression(BinaryOp.LogicalOr, leftExp, rightExp);
                // exp1 == exp2
                else if (token.Kind == TokenKind.Equals2) exp = new BinaryOpExpression(BinaryOp.Equal, leftExp, rightExp);
                // exp1 != exp2
                else if (token.Kind == TokenKind.InEquals2) exp = new BinaryOpExpression(BinaryOp.NotEqual, leftExp, rightExp);
                // No exp
                else { reader.Index = ix; return null; }
            } else { reader.Index = ix; return null; }

            // Keep reading boolean expressions
            exp = reader.Take(BooleanBinaryOp, exp);

            return exp;
        };

        /// <summary>
        /// Read any expression that returns a value.
        /// 
        /// One of:
        /// <see cref="IConstantExpression"/>
        /// <see cref="IRangeExpression"/>
        /// <see cref="IGroupExpression"/>
        /// <see cref="IArgumentNameExpression"/>
        /// <see cref="IBinaryOpExpression"/>
        /// <see cref="IParenthesisExpression"/>
        /// </summary>
        public readonly static Tokens.Taker0<IExpression> ValueExpression = (ref Tokens reader) =>
        {
            int ix = reader.Index;
            IExpression exp = reader.Take(ValueExpressionWithoutGroup);
            if (exp == null) return null;

            // GroupExpression: ',' <value>.
            StructList16<IExpression> groupExpressions = new StructList16<IExpression>();
            groupExpressions.Add(exp);
            reader.TakeAll(TokenKind.NonEssential);
            Token groupSeparatorToken = null;
            while ((groupSeparatorToken = reader.Take(TokenKind.Comma)) != null)
            {
                // Read next 
                IExpression nextValue = reader.Take(ValueExpressionWithoutGroup);
                // Failed to read next expression after comma
                if (nextValue == null) { reader.Index = ix; return null; }
                // 
                groupExpressions.Add(nextValue);
                // 
                reader.TakeAll(TokenKind.NonEssential);
            }
            if (groupExpressions.Count == 1) return exp;

            // Create group expression
            return new GroupExpression(groupExpressions.ToArray());
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
        public readonly static Tokens.Taker0<IExpression> ValueExpressionWithoutGroup = (ref Tokens reader) =>
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

            // Argument name
            IExpression exp = null;
            if (token.Kind == TokenKind.NameLiteral && token.Value is string name)
            {
                // Argument expression
                if (name == "n" || name == "i" || name == "f" || name == "t" || name == "v" || name == "w" || name == "e")
                {
                    exp = new ArgumentNameExpression(name);
                } else
                {
                    // Failed
                    reader.Index = ix; return null;
                }
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
                        ConstantExpression rangeEnd = new ConstantExpression(new DecimalNumber.Long((long)rangeEndToken.Value));
                        exp = new RangeExpression(rangeStart, rangeEnd);
                    }
                    else
                    {
                        ConstantExpression rangeStart = new ConstantExpression(new DecimalNumber.Double((double)(long)token.Value));
                        ConstantExpression rangeEnd = new ConstantExpression(new DecimalNumber.Double((double)rangeEndToken.Value));
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
                    ConstantExpression rangeEndExp = new ConstantExpression(new DecimalNumber.Double((double)rangeEndToken.Value));
                    exp = new RangeExpression(exp as IConstantExpression, rangeEndExp);
                }
            }
            // Failed
            else { reader.Index = ix; return null; }

            // Binary Op: %
            if (exp != null)
            {
                int ix2 = reader.Index;
                IExpression exp2 = reader.Take(ValueBinaryOp, exp);
                if (exp2 != null) exp = exp2; else reader.Index = ix2;
            }

            // Return value
            return exp;
        };

        /// <summary>
        /// Read binary op for values
        /// </summary>
        public readonly static Tokens.Taker1<IExpression, IExpression> ValueBinaryOp = (ref Tokens reader, IExpression leftExp) =>
        {
            int ix = reader.Index;
            reader.TakeAll(TokenKind.NonEssential);
            Token token = reader.Take(TokenKind.Percent);
            if (token != null)
            {
                reader.TakeAll(TokenKind.NonEssential);
                IExpression rightExp = reader.Take(ValueExpression);
                if (rightExp == null) { reader.Index = ix; return null; }

                if (token.Kind == TokenKind.Percent) return new BinaryOpExpression(BinaryOp.Modulo, leftExp, rightExp);
            }
            reader.Index = ix;
            return null;
        };

    }
}
