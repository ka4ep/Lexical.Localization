// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Parses rule string into expression
    /// </summary>
    public class PluralRuleParser
    {
        /// <summary>
        /// Parse string into expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static IPluralRuleExpression Parse(string exp)
        {
            TokenEnumerator enumr = new TokenEnumerator(new Tokens(exp));



            return null;
        }


        static Tokens.Taker0<IExpression> booleanExp = ReadBooleanExpression;
        public static IExpression ReadBooleanExpression(ref Tokens tokens)
        {
            // Try read binary
            // Try read unary
            // Try read literal
            // Try 
            return null;
        }

        public static IExpression ReadUnaryExpression(ref Tokens reader)
        {
            Tokens ne0 = reader.TakeAll(TokenKind.NonEssential);
            Token op = reader.Take(TokenKind.Plus | TokenKind.Minus | TokenKind.Exclamation | TokenKind.Tilde);
            // Convert tokenKind to UnaryOp
            if (op == null) return null;
            IExpression element = reader.Take(booleanExp);
            if (element == null) return null;
            return new Expression.UnaryOp(default, element);
        }

        

    }
}
