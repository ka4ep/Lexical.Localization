using System;
using System.Collections;
using System.Collections.Generic;
using Lexical.Localization.Exp;
using Lexical.Localization.Plurality;

namespace Unicode
{
    /// <summary>
    /// This class is derivate of Unicode CLDR plurals.xml and ordinals.xml, and is licensed as "Data Files".
	/// Unicode CLDR "Data Files" are copyright of Unicode, Inc.
    /// ----
    /// UNICODE, INC. LICENSE AGREEMENT - DATA FILES AND SOFTWARE
    /// 
    /// See Terms of Use for definitions of Unicode Inc.'s
    /// Data Files and Software.
    /// 
    /// 
    /// NOTICE TO USER: Carefully read the following legal agreement.
    /// BY DOWNLOADING, INSTALLING, COPYING OR OTHERWISE USING UNICODE INC.'S
    /// DATA FILES ("DATA FILES"), AND/OR SOFTWARE("SOFTWARE"),
    /// YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY, ALL OF THE
    /// TERMS AND CONDITIONS OF THIS AGREEMENT.
    /// IF YOU DO NOT AGREE, DO NOT DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE
    /// THE DATA FILES OR SOFTWARE.
    /// 
    /// COPYRIGHT AND PERMISSION NOTICE
    /// 
    /// Copyright © 1991-2019 Unicode, Inc.All rights reserved.
    /// Distributed under the Terms of Use in https://www.unicode.org/copyright.html.
    /// Permission is hereby granted, free of charge, to any person obtaining
    /// a copy of the Unicode data files and any associated documentation
    /// (the "Data Files") or Unicode software and any associated documentation
    /// (the "Software") to deal in the Data Files or Software
    /// without restriction, including without limitation the rights to use,
    /// copy, modify, merge, publish, distribute, and/or sell copies of
    /// the Data Files or Software, and to permit persons to whom the Data Files
    /// or Software are furnished to do so, provided that either
    /// (a) this copyright and permission notice appear with all copies
    /// of the Data Files or Software, or
    /// (b) this copyright and permission notice appear in associated
    /// Documentation.
    /// 
    /// THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF
    /// ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    /// NONINFRINGEMENT OF THIRD PARTY RIGHTS.
    /// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS
    /// NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
    /// DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
    /// DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
    /// TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
    /// PERFORMANCE OF THE DATA FILES OR SOFTWARE.
    /// 
    /// Except as contained in this notice, the name of a copyright holder
    /// shall not be used in advertising or otherwise to promote the sale,
    /// use or other dealings in these Data Files or Software without prior
    /// written authorization of the copyright holder.
    /// ----
	/// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    internal class CLDR35 : PluralRulesEvaluatable
    {
        /// <summary>
        /// Lazy loader.
        /// </summary>
        private static readonly Lazy<CLDR35> instance = new Lazy<CLDR35>();

        /// <summary>
        /// Unicode CLDR.
        /// 
        /// Reads embedded CLDR plural data files.
        /// Data files are licensed under <see href="https://unicode.org/repos/cldr/tags/release-35/unicode-license.txt"/>.
        /// </summary>
        public static CLDR35 Instance => instance.Value;

        /// <summary>
        /// Create new Unicode CLDR35 rules. 
        /// </summary>
        public CLDR35() : base((IPluralRules)new Queryable())
        {
        }

        /// <summary>
        /// Queryable and enumerable, but not cached.
        /// </summary>
        internal class Queryable : CLDR, IPluralRulesQueryable, IPluralRulesEnumerable
        {
            /// <summary>
            /// Rule set
            /// </summary>
            public readonly string RuleSet = "Unicode.CLDR35";

            /// <summary>
            /// 
            /// </summary>
            /// <param name="f"></param>
            /// <returns></returns>
            public IPluralRulesEnumerable Query(PluralRuleInfo f)
            {
                if (f.RuleSet != null && f.RuleSet != RuleSet) return null;
                PluralRulesList result = new PluralRulesList();

                if (f.Category == null || f.Category == "cardinal")
                {
                    // , bm, bo, dz, id, ig, ii, in, ja, jbo, jv, jw, kde, kea, km, ko, lkt, lo, ms, my, nqo, sah, ses, sg, th, to, vi, wo, yo, yue, zh
                    foreach (string culture in c_ad538525)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E982833E3, E4320FAC8));
                    }
                    // af, asa, az, bem, bez, bg, brx, ce, cgg, chr, ckb, dv, ee, el, eo, es, eu, fo, fur, gsw, ha, haw, hu, jgo, jmc, ka, kaj, kcg, kk, kkj, kl, ks, ksb, ku, ky, lb, lg, mas, mgo, ml, mn, mr, nah, nb, nd, ne, nn, nnh, no, nr, ny, nyn, om, or, os, pap, ps, rm, rof, rwk, saq, sd, sdh, seh, sn, so, sq, ss, ssy, st, syr, ta, te, teo, tig, tk, tn, tr, ts, ug, uz, ve, vo, vun, wae, xh, xog
                    foreach (string culture in c_505e5f53)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4B5D780D, E9A0E44B4));
                    }
                    // ak, bh, guw, ln, mg, nso, pa, ti, wa
                    foreach (string culture in c_766a620a)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E3987DFFC, EF758082A, E214176C0));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE6CB67EF, ED3125EE4));
                    }
                    // am, as, bn, fa, gu, hi, kn, zu
                    foreach (string culture in c_c5bed141)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E0A18C452, EF758082A, E7143B999));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE6CB67EF, EC0BDA170));
                    }
                    // ar, ars
                    foreach (string culture in c_a5a38d0e)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 0), null, EC3516619, EB81439C2, E6B373360));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E3F0B167B, EB614369C, E26C984B0));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E5738BF6D, E2C0A6AD4, E8B31405B));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E3EC25B77, E463CD75D, E128CE41A));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4D1CEB1A, EDD390F19));
                    }
                    // ast, ca, de, en, et, fi, fy, gl, ia, io, it, ji, nl, pt-PT, sc, scn, sv, sw, ur, yi
                    foreach (string culture in c_a34e8318)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EFFFC2537, EB9143B55));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4B5D780D, E4320FAC8));
                    }
                    // be
                    foreach (string culture in c_382ba080)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E84A1942F, EB152352E, E68B1E329));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, EE89E9716, EDBD924C8, ECD444539));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, EDD8F92EE, E8148790B, E98DA6EDC));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE2A81867));
                    }
                    // br
                    foreach (string culture in c_4f2bc4b5)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EFBB24E31, E63345F3F, E1F378BF0));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, ECC2A9E1D, EC91BCA52, E0943D865));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, ED6218FB7, E7421E24A, E2ED8B14F));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E4D83051C, E260F6011, E915FF900));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E7FEF7670, EDD06EF4E));
                    }
                    // bs, hr, sh, sr
                    foreach (string culture in c_7100128a)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E71BE1840, EB152352E, EF13564AD));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E802DCD2C, EDBD924C8, E3FA7CA56));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E8148790B, ED48DC621));
                    }
                    // ceb, fil, tl
                    foreach (string culture in c_372eafd4)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E77C90E06, E4E67F0DD, E50D6947E));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EF0161866, E1B284677));
                    }
                    // cs, sk
                    foreach (string culture in c_e3f5abf1)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EFFFC2537, EB9143B55));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E264ACB7C, E15E3D309));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E7D97B2C4, E4320FAC8));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E8148790B));
                    }
                    // cy
                    foreach (string culture in c_502987b1)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 0), null, EC3516619, EB81439C2, E6B373360));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E3F0B167B, EB614369C, E26C984B0));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, EC153A18A, EB714382F, EEC063218));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E3F4C191F, EB2143050, ED3A073F0));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EF7179B11, ED3125EE4));
                    }
                    // da
                    foreach (string culture in c_581cc856)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E26C25720, EB9143B55, E4CBE7CF4));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4B5D780D, EB73D72FE));
                    }
                    // dsb, hsb
                    foreach (string culture in c_e8c1d0bb)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E81365D4D, E441A5A7E, EF13564AD));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, EF0ACC361, EB8582B29, E21525782));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E40592561, E16FDB0A4, E99F271AF));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E8148790B, ED48DC621));
                    }
                    // ff, fr, hy, kab, pt
                    foreach (string culture in c_a5c3e674)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EE5C8BDA7, EF758082A, ED7798032));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE6CB67EF, E47A544D8));
                    }
                    // ga
                    foreach (string culture in c_40207425)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E3F0B167B, EB614369C, E26C984B0));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E2951DF68, E580D87B4, E6584DA98));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E5033232B, E42E4C727, E713D3C18));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E263C5779, E49A0C5A9));
                    }
                    // gd
                    foreach (string culture in c_3b206c46)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EE6FFFCD6, E62152D88, E228C2FD7));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, ECB109DEE, EA7D89E52, ED379571C));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E80C36837, E81F8FA2D, E0742DB5F));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EF419640F, E49A0C5A9));
                    }
                    // gv
                    foreach (string culture in c_49208250)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E3E805D25, E571D9611));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E3E2A37EA, ED71F448B));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E8CF84894, E4CAB1716));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E7D97B2C4, E4320FAC8));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E620AB079));
                    }
                    // he, iw
                    foreach (string culture in c_337e680e)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EFFFC2537, EB9143B55));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E95A4B850, EB614369C));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, EE978F1CF, EEB8A4639));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E534B3B37, E4320FAC8));
                    }
                    // is
                    foreach (string culture in c_4e388f15)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, ED296C19F, EB152352E, E7AEF0F49));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4B5D780D, E7D97AA09));
                    }
                    // iu, naq, se, sma, smi, smj, smn, sms
                    foreach (string culture in c_f918c364)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E3F0B167B, EB614369C, E26C984B0));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EB3062ECF, E9A0E44B4));
                    }
                    // ksh
                    foreach (string culture in c_752de1c9)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 0), null, EC3516619, EB81439C2, E6B373360));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE6CB67EF, ED3125EE4));
                    }
                    // kw
                    foreach (string culture in c_5a3d1f27)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 0), null, EC3516619, EB81439C2, E6B373360));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, EE2596300, EA6A336DC, E8E8728DF));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, EABEE7EEC, EE8CE77D1, E12D0CC0E));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, EDC252060, EC437968E, E70EF10E9));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E53EAFB8F, E5EE15A4E));
                    }
                    // lag
                    foreach (string culture in c_5f608e3b)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 0), null, EC3516619, EB81439C2, E6B373360));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E8ADA7F20, EB9143B55, E4CBE7CF4));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE6CB67EF, E47A544D8));
                    }
                    // lt
                    foreach (string culture in c_5d31eaed)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, ED141DA67, EB152352E, E68B1E329));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E03CAF549, E24E4BF2D, E00CE1EE9));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E44CCE0D4, EE2A81867));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E7B6DDDF3, EC9D009EA));
                    }
                    // lv, prg
                    foreach (string culture in c_66d93ae6)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 0), null, EEA009B9F, E7B6DDDF3, EC9D009EA));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E5B3EA793, EB152352E, E48928607));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E24E4BF2D, E17DD8CF2));
                    }
                    // mk
                    foreach (string culture in c_5e2e1ae9)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E71BE1840, EB152352E, EF13564AD));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4B5D780D, E2C671DE9));
                    }
                    // mo, ro
                    foreach (string culture in c_f255519e)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EFFFC2537, EB9143B55));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E062AAF64, E7FB07C93, E4320FAC8));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E90131C27));
                    }
                    // mt
                    foreach (string culture in c_572e0fe4)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EBD4F1E10, EB9143B55, EE9057C68));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, EC15E4BFF, EE30B61D6, EC3CA1B79));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E9338945F, E6BB4A5DD, E128CE41A));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E90131C27, EDD390F19));
                    }
                    // pl
                    foreach (string culture in c_454e4739)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EFFFC2537, EB9143B55));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E4E88644C, EDBD924C8));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E0CAFD5D4, E8148790B));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4320FAC8));
                    }
                    // ru, uk
                    foreach (string culture in c_0a441d2c)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, ECAB49249, EB152352E));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E4E88644C, EDBD924C8));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "many", 0), null, E35C79881, E8148790B));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E4320FAC8));
                    }
                    // shi
                    foreach (string culture in c_e52df395)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E0A18C452, EF758082A, E7143B999));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, EBF96A952, E20ACF1F6, E6E411094));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E8AA9704B, E032410E9));
                    }
                    // si
                    foreach (string culture in c_405210f1)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, EDA9A4B53, EF758082A, E791DD688));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, EE6CB67EF, ECE70A2D4));
                    }
                    // sl
                    foreach (string culture in c_435215aa)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, E4FADB3DD, E441A5A7E));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "two", 0), null, E19B77C72, EB8582B29));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "few", 0), null, E2617C675, E16FDB0A4, E4320FAC8));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E8148790B));
                    }
                    // tzm
                    foreach (string culture in c_c0315742)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Zero(new PluralRuleInfo(RuleSet, "cardinal", culture, "zero", 1)));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "cardinal", culture, "one", 0), null, ED1051BB7, EFC992FDE, E37BE0A31));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "cardinal", culture, "other", 0), null, null, E6BAD1FB4, ED3125EE4));
                    }
                }
                if (f.Category == null || f.Category == "ordinal")
                {
                    // , af, am, ar, bg, bs, ce, cs, da, de, dsb, el, es, et, eu, fa, fi, fy, gl, gsw, he, hr, hsb, ia, id, in, is, iw, ja, km, kn, ko, ky, lt, lv, ml, mn, my, nb, nl, pa, pl, prg, ps, pt, ru, sd, sh, si, sk, sl, sr, sw, ta, te, th, tr, ur, uz, yue, zh, zu
                    foreach (string culture in c_4cf8b047)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E982833E3));
                    }
                    // as, bn
                    foreach (string culture in c_20ca8aa9)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E5C665074, EF35205FB));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E27804086, EA02DA6BA));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E4360D23D, EB4143376));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E3F4C191F, EB2143050));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E263C5779));
                    }
                    // az
                    foreach (string culture in c_55251262)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E7909FE8D, E2170F568));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E72EAC562, EF4866074));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E91FCEE49, E043D1268));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EA8538A54));
                    }
                    // be
                    foreach (string culture in c_382ba080)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E374D1302, E2B1D626B));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EFA63AF5A));
                    }
                    // ca
                    foreach (string culture in c_58299449)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E3D53A975, EE7CAAE43));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E3F0B167B, EB614369C));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E4360D23D, EB4143376));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E8148790B));
                    }
                    // cy
                    foreach (string culture in c_502987b1)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "zero") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "zero", 0), null, E5AE59525, EB1CDEA56));
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EBD4F1E10, EB9143B55));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E3F0B167B, EB614369C));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E1E3A2A10, E6440295C));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E0D7D2C88, EBC300370));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EAC450D5F));
                    }
                    // en
                    foreach (string culture in c_411a658a)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E84A1942F, EB152352E));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E199BF7AF, E7BC3E6A8));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, ECD38005F, EA26DC2F6));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E9B8D06B5));
                    }
                    // fil, fr, ga, hy, lo, mo, ms, ro, tl, vi
                    foreach (string culture in c_b59fa1f0)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EBD4F1E10, EB9143B55));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E4B5D780D));
                    }
                    // gd
                    foreach (string culture in c_3b206c46)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EE6FFFCD6, E62152D88));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, ECB109DEE, EA7D89E52));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E7574BF5A, EA7F98EB0));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, ECE306B2B));
                    }
                    // gu, hi
                    foreach (string culture in c_559dfe2e)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EBD4F1E10, EB9143B55));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E27804086, EA02DA6BA));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E4360D23D, EB4143376));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E3F4C191F, EB2143050));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E32E02E0D));
                    }
                    // hu
                    foreach (string culture in c_4c3aca86)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E19478C3F, EEDCAB7B5));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E0060D900));
                    }
                    // it, sc, scn
                    foreach (string culture in c_dd9192a6)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E2E2A901D, EB84947E1));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EA02B52DF));
                    }
                    // ka
                    foreach (string culture in c_483d02d1)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E49A58121, EB9143B55));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E669B110C, E7FB07C93));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E1C8A142B));
                    }
                    // kk
                    foreach (string culture in c_3e3cf313)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E4C5775FB, E818EAD2B));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EBFEA5C82));
                    }
                    // kw
                    foreach (string culture in c_5a3d1f27)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EB8D55BD0, EFD50E885));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, EEED9E998, E78162D52));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EE783DF46));
                    }
                    // mk
                    foreach (string culture in c_5e2e1ae9)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EFE72E477, EB152352E));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E6EE6DFCF, E7BC3E6A8));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, EF4449A1A, EBEA662F3));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EC4967A7E));
                    }
                    // mr
                    foreach (string culture in c_552e0cbe)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EBD4F1E10, EB9143B55));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E27804086, EA02DA6BA));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E4360D23D, EB4143376));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E8148790B));
                    }
                    // ne
                    foreach (string culture in c_5836603c)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E4FE21A9C, E95A531B4));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E8148790B));
                    }
                    // or
                    foreach (string culture in c_5d342984)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, E8730DC87, EC0345761));
                        if (f.Case == null || f.Case == "two") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "two", 0), null, E27804086, EA02DA6BA));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E4360D23D, EB4143376));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, E3F4C191F, EB2143050));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E5F659133));
                    }
                    // sq
                    foreach (string culture in c_48521d89)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EBD4F1E10, EB9143B55));
                        if (f.Case == null || f.Case == "many") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "many", 0), null, EFB5AAB57, E64D7705C));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E1CCB859A));
                    }
                    // sv
                    foreach (string culture in c_49521f1c)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 0), null, EC3E62A5A, E26F9940B));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, EB3062ECF));
                    }
                    // tk
                    foreach (string culture in c_3e4541d8)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, E43CCD76C, E74312601));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E14FF0595));
                    }
                    // uk
                    foreach (string culture in c_5e4335a1)
                    {
                        if (f.Culture != null && f.Culture != culture) continue;
                        if (f.Case == null || f.Case == "one") result.Add(new PluralRule.One(new PluralRuleInfo(RuleSet, "ordinal", culture, "one", 1)));
                        if (f.Case == null || f.Case == "few") result.Add(new PluralRule.Expression(new PluralRuleInfo(RuleSet, "ordinal", culture, "few", 0), null, ECD38005F, EA26DC2F6));
                        if (f.Case == null || f.Case == "other") result.Add(new PluralRule.TrueWithExpression(new PluralRuleInfo(RuleSet, "ordinal", culture, "other", 0), null, null, E2B0C4A40));
                    }
                }

                return result;
            }

            /// <summary>Get enumerator to all rules</summary>
            public IEnumerator<IPluralRule> GetEnumerator()
                => Query(PluralRuleInfo.Empty).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => Query(PluralRuleInfo.Empty).GetEnumerator();
        }
    }

    /// <summary>
    /// Unicode CLDR plurality Rules shared expressions.
    /// 
    /// This code is derivate of plurals.xml, and is licensed as "Data Files".
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules"/>
    /// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
    /// <see href="http://cldr.unicode.org/translation/plurals"/>
    /// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
    /// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
    /// </summary>
    internal class CLDR : IPluralRules
    {
        /// <summary>
        /// Create abstract CLDR ruleset.
        /// </summary>
        public CLDR() { }

        /// <summary>ast, ca, de, en, et, fi, fy, gl, ia, io, it, ji, nl, pt-PT, sc, scn, sv, sw, ur, yi</summary>
        protected string[] c_a34e8318 = new[] { "ast", "ca", "de", "en", "et", "fi", "fy", "gl", "ia", "io", "it", "ji", "nl", "pt-PT", "sc", "scn", "sv", "sw", "ur", "yi" };
        /// <summary>ar, ars</summary>
        protected string[] c_a5a38d0e = new[] { "ar", "ars" };
        /// <summary>ff, fr, hy, kab, pt</summary>
        protected string[] c_a5c3e674 = new[] { "ff", "fr", "hy", "kab", "pt" };
        /// <summary>, bm, bo, dz, id, ig, ii, in, ja, jbo, jv, jw, kde, kea, km, ko, lkt, lo, ms, my, nqo, sah, ses, sg, th, to, vi, wo, yo, yue, zh</summary>
        protected string[] c_ad538525 = new[] { "", "bm", "bo", "dz", "id", "ig", "ii", "in", "ja", "jbo", "jv", "jw", "kde", "kea", "km", "ko", "lkt", "lo", "ms", "my", "nqo", "sah", "ses", "sg", "th", "to", "vi", "wo", "yo", "yue", "zh" };
        /// <summary>fil, fr, ga, hy, lo, mo, ms, ro, tl, vi</summary>
        protected string[] c_b59fa1f0 = new[] { "fil", "fr", "ga", "hy", "lo", "mo", "ms", "ro", "tl", "vi" };
        /// <summary>tzm</summary>
        protected string[] c_c0315742 = new[] { "tzm" };
        /// <summary>am, as, bn, fa, gu, hi, kn, zu</summary>
        protected string[] c_c5bed141 = new[] { "am", "as", "bn", "fa", "gu", "hi", "kn", "zu" };
        /// <summary>it, sc, scn</summary>
        protected string[] c_dd9192a6 = new[] { "it", "sc", "scn" };
        /// <summary>cs, sk</summary>
        protected string[] c_e3f5abf1 = new[] { "cs", "sk" };
        /// <summary>shi</summary>
        protected string[] c_e52df395 = new[] { "shi" };
        /// <summary>dsb, hsb</summary>
        protected string[] c_e8c1d0bb = new[] { "dsb", "hsb" };
        /// <summary>mo, ro</summary>
        protected string[] c_f255519e = new[] { "mo", "ro" };
        /// <summary>iu, naq, se, sma, smi, smj, smn, sms</summary>
        protected string[] c_f918c364 = new[] { "iu", "naq", "se", "sma", "smi", "smj", "smn", "sms" };
        /// <summary>ru, uk</summary>
        protected string[] c_0a441d2c = new[] { "ru", "uk" };
        /// <summary>as, bn</summary>
        protected string[] c_20ca8aa9 = new[] { "as", "bn" };
        /// <summary>he, iw</summary>
        protected string[] c_337e680e = new[] { "he", "iw" };
        /// <summary>ceb, fil, tl</summary>
        protected string[] c_372eafd4 = new[] { "ceb", "fil", "tl" };
        /// <summary>be</summary>
        protected string[] c_382ba080 = new[] { "be" };
        /// <summary>gd</summary>
        protected string[] c_3b206c46 = new[] { "gd" };
        /// <summary>kk</summary>
        protected string[] c_3e3cf313 = new[] { "kk" };
        /// <summary>tk</summary>
        protected string[] c_3e4541d8 = new[] { "tk" };
        /// <summary>ga</summary>
        protected string[] c_40207425 = new[] { "ga" };
        /// <summary>si</summary>
        protected string[] c_405210f1 = new[] { "si" };
        /// <summary>en</summary>
        protected string[] c_411a658a = new[] { "en" };
        /// <summary>sl</summary>
        protected string[] c_435215aa = new[] { "sl" };
        /// <summary>pl</summary>
        protected string[] c_454e4739 = new[] { "pl" };
        /// <summary>ka</summary>
        protected string[] c_483d02d1 = new[] { "ka" };
        /// <summary>sq</summary>
        protected string[] c_48521d89 = new[] { "sq" };
        /// <summary>gv</summary>
        protected string[] c_49208250 = new[] { "gv" };
        /// <summary>sv</summary>
        protected string[] c_49521f1c = new[] { "sv" };
        /// <summary>hu</summary>
        protected string[] c_4c3aca86 = new[] { "hu" };
        /// <summary>, af, am, ar, bg, bs, ce, cs, da, de, dsb, el, es, et, eu, fa, fi, fy, gl, gsw, he, hr, hsb, ia, id, in, is, iw, ja, km, kn, ko, ky, lt, lv, ml, mn, my, nb, nl, pa, pl, prg, ps, pt, ru, sd, sh, si, sk, sl, sr, sw, ta, te, th, tr, ur, uz, yue, zh, zu</summary>
        protected string[] c_4cf8b047 = new[] { "", "af", "am", "ar", "bg", "bs", "ce", "cs", "da", "de", "dsb", "el", "es", "et", "eu", "fa", "fi", "fy", "gl", "gsw", "he", "hr", "hsb", "ia", "id", "in", "is", "iw", "ja", "km", "kn", "ko", "ky", "lt", "lv", "ml", "mn", "my", "nb", "nl", "pa", "pl", "prg", "ps", "pt", "ru", "sd", "sh", "si", "sk", "sl", "sr", "sw", "ta", "te", "th", "tr", "ur", "uz", "yue", "zh", "zu" };
        /// <summary>is</summary>
        protected string[] c_4e388f15 = new[] { "is" };
        /// <summary>br</summary>
        protected string[] c_4f2bc4b5 = new[] { "br" };
        /// <summary>cy</summary>
        protected string[] c_502987b1 = new[] { "cy" };
        /// <summary>af, asa, az, bem, bez, bg, brx, ce, cgg, chr, ckb, dv, ee, el, eo, es, eu, fo, fur, gsw, ha, haw, hu, jgo, jmc, ka, kaj, kcg, kk, kkj, kl, ks, ksb, ku, ky, lb, lg, mas, mgo, ml, mn, mr, nah, nb, nd, ne, nn, nnh, no, nr, ny, nyn, om, or, os, pap, ps, rm, rof, rwk, saq, sd, sdh, seh, sn, so, sq, ss, ssy, st, syr, ta, te, teo, tig, tk, tn, tr, ts, ug, uz, ve, vo, vun, wae, xh, xog</summary>
        protected string[] c_505e5f53 = new[] { "af", "asa", "az", "bem", "bez", "bg", "brx", "ce", "cgg", "chr", "ckb", "dv", "ee", "el", "eo", "es", "eu", "fo", "fur", "gsw", "ha", "haw", "hu", "jgo", "jmc", "ka", "kaj", "kcg", "kk", "kkj", "kl", "ks", "ksb", "ku", "ky", "lb", "lg", "mas", "mgo", "ml", "mn", "mr", "nah", "nb", "nd", "ne", "nn", "nnh", "no", "nr", "ny", "nyn", "om", "or", "os", "pap", "ps", "rm", "rof", "rwk", "saq", "sd", "sdh", "seh", "sn", "so", "sq", "ss", "ssy", "st", "syr", "ta", "te", "teo", "tig", "tk", "tn", "tr", "ts", "ug", "uz", "ve", "vo", "vun", "wae", "xh", "xog" };
        /// <summary>az</summary>
        protected string[] c_55251262 = new[] { "az" };
        /// <summary>mr</summary>
        protected string[] c_552e0cbe = new[] { "mr" };
        /// <summary>gu, hi</summary>
        protected string[] c_559dfe2e = new[] { "gu", "hi" };
        /// <summary>mt</summary>
        protected string[] c_572e0fe4 = new[] { "mt" };
        /// <summary>da</summary>
        protected string[] c_581cc856 = new[] { "da" };
        /// <summary>ca</summary>
        protected string[] c_58299449 = new[] { "ca" };
        /// <summary>ne</summary>
        protected string[] c_5836603c = new[] { "ne" };
        /// <summary>kw</summary>
        protected string[] c_5a3d1f27 = new[] { "kw" };
        /// <summary>lt</summary>
        protected string[] c_5d31eaed = new[] { "lt" };
        /// <summary>or</summary>
        protected string[] c_5d342984 = new[] { "or" };
        /// <summary>mk</summary>
        protected string[] c_5e2e1ae9 = new[] { "mk" };
        /// <summary>uk</summary>
        protected string[] c_5e4335a1 = new[] { "uk" };
        /// <summary>lag</summary>
        protected string[] c_5f608e3b = new[] { "lag" };
        /// <summary>lv, prg</summary>
        protected string[] c_66d93ae6 = new[] { "lv", "prg" };
        /// <summary>bs, hr, sh, sr</summary>
        protected string[] c_7100128a = new[] { "bs", "hr", "sh", "sr" };
        /// <summary>ksh</summary>
        protected string[] c_752de1c9 = new[] { "ksh" };
        /// <summary>ak, bh, guw, ln, mg, nso, pa, ti, wa</summary>
        protected string[] c_766a620a = new[] { "ak", "bh", "guw", "ln", "mg", "nso", "pa", "ti", "wa" };

        /// <summary></summary>
        protected IExpression _31aab26f, _31e49d3a, _31f67aab, _320da9ff, _322b674a, _32301bae, _323ec34a, _325421b6, _3263a15b, _32f3f2d7, _32f6a02f, _332b68dd, _345424dc, _34f3f456, _34f67f64, _3554266f, _35802b84, _35c79881, _36542802, _367d1ed9, _371340c0, _374d1302, _37542995, _37f3f90f, _3805c12c, _3987dffc, _39fb4e19, _3b922da3, _3c00b9f8, _3c30db95, _3d00bb8b, _3d2475d8, _3d53a975, _3d5e8a34, _3e24776b, _3e2a37ea, _3e805d25, _3ec25b77, _3ede2427, _3f0b167b, _3f4c191f, _3ff8dd22, _3fffe6b6, _404fb29e, _40592561, _41e20ec0, _42e21053, _42e3f86c, _4360d23d, _43ccd76c, _43d64505, _43e211e6, _44cce0d4, _44e21379, _44e9b57d, _45332857, _45d73a52, _45e2150c, _468378ea, _46ad7de1, _46e2169f, _47e21832, _47ed7465, _4816c5de, _486af022, _48d25993, _48e219c5, _4937ffba, _49a58121, _49a7bfb8, _49bf8916, _49d95c79, _4a26b276, _4abba532, _4b0cf081, _4b0ee30e, _4b83d017, _4c5775fb, _4c64a931, _4c6db99b, _4c7fae6d, _4cbd4f38, _4cdbc305, _4d64aac4, _4d83051c, _4dbd50cb, _4de221a4, _4e64ac57, _4e88644c, _4ee22337, _4f64adea, _4f98894d, _4fadb3dd, _4fbf9288, _4fd11aca, _4fe21a9c, _5033232b, _5064af7d, _508be571, _50ae9ffd, _50bc71df, _50bf941b, _50fb55e9, _519ba030, _51bf95ae, _51d5343d, _51e459a6, _52ada190, _52bf9741, _52e81d7c, _53bd5a3d, _53bf98d4, _53e1e263, _53e81f0f, _54bf9a67, _55712385, _55759678, _55bf9bfa, _55cea684, _56bf9d8d, _5738bf6d, _58e826ee, _59a97b61, _59e82881, _59f58855, _5ae59525, _5b3ea793, _5b603073, _5b8c4d33, _5bacbded, _5bcfc261, _5c665074, _5c674af6, _5cab36e5, _5cceb86f, _5d6bfbf5, _5deb8672, _5e0b1160, _5e12e1fa, _5e3cec93, _5e8aad1c, _5f80534a, _608d2f94, _60d363b0, _619ef28d, _61eee76e, _63a9d953, _63aefa47, _63db2c0a, _63e4d29a, _63e94a2b, _641140b2, _65358758, _65961d56, _669b110c, _67adbebc, _68397994, _69114891, _6cd19676, _6cfb81e6, _6d12b621, _6da348f2, _6dd19809, _6e0821ba, _6eccde4e, _6ed1999c, _6ee6dfcf, _6f3358de, _6fd19b2f, _6fe9bc5d, _6ffae315, _70d19cc2, _71be1840, _71d19e55, _72b6dcc0, _72c788f1, _72eac562, _731859ee, _734bd952, _73602f26, _737d7515, _73d9e621, _7432aaa1, _7490fbb8, _74e1e9ac, _7574bf5a, _7590fd4b, _75dc4b6c, _7690fede, _76b1a96f, _76bf91a4, _76def1b2, _779038ee, _77910071, _77b6ba85, _77c90e06, _77e9a35e, _780a8eb7, _78910204, _7909fe8d, _79910397, _7ac0d2fc, _7b17078c, _7b2eb74f, _7c134ff1, _7c9b4215, _7d140262, _7d97b2c4, _7df0645a, _7f3c2e35, _7fd9e0c1, _7ff0739a, _800b9663, _802dcd2c, _80c36837, _81365d4d, _81bc9acc, _82b7905c, _82bf64c0, _82f74277, _83188450, _837ec443, _83951640, _83b791ef, _841885e3, _84a1942f, _84a306cf, _84c18e51, _84e93e2b, _85d169aa, _85f92cd1, _86aad6ff, _8730dc87, _8803bd20, _88bd1628, _88dd4214, _88f46cc8, _894efd50, _89bd17bb, _89f46e5b, _8a4843ee, _8ada7f20, _8af46fee, _8afd045d, _8b1acf7f, _8b1ed530, _8b42049b, _8b484581, _8b837f74, _8bea5a8a, _8bf47181, _8c484714, _8cbd1c74, _8cc30050, _8cf47314, _8cf84894, _8d4848a7, _8d95d195, _8dc301e3, _8df33fdf, _8df474a7, _8e484a3a, _8ec30376, _8ef4763a, _8f44c9d2, _8f484bcd, _8fc30509, _8fc543a0, _8ff477cd, _9016a675, _90c3069c, _90c54533, _911af69c, _91468072, _91704d05, _918569fe, _91c3082f, _91c546c6, _91fc58ff, _91fcee49, _92108a80, _92120075, _9272c33f, _92c309c2, _92c54859, _92d452e3, _92fa4c69, _9338945f, _934c92a5, _93c30b55, _93c549ec, _93d45476, _93d79ee8, _94c30ce8, _94c54b7f, _94d1c0ca, _94d45609, _951a8ca1, _9536218a, _9565b229, _95a4b850, _95c30e7b, _95c54d12, _95c9ca40, _95d4579c, _96a80ae2, _96bdb99f, _96c54ea5, _96c9cbd3, _96d4592f, _970d5eb3, _97b930ad, _97d45ac2, _980fd23f, _984a4d02, _984f1eac, _98a8b99f, _98d45c55, _995b85e0, _99c5535e, _9a283e93, _9a308830, _9a924772, _9ac554f1, _9b3089c3, _9b845998, _9c308b56, _9ccc1a7a, _9d2807a8, _9d308ce9, _9dc9d6d8, _9dd46434, _9e05d853, _9e1154f7, _9e176829, _9e712c17, _9ec9d86b, _9ed465c7, _9f3fcddd, _9fc9d9fe, _a0c9db91, _a0d9aeeb, _a0db24b2, _a1309335, _a1c9dd24, _a2176e75, _a23b1bec, _a2c9deb7, _a34f39d0, _a3c9e04a, _a3db296b, _a42ebc10, _a4ad7b30, _a4c9e1dd, _a4db2afe, _a501a4ad, _a52ebda3, _a537c1ea, _a6db2e24, _a71fcd3a, _a72e2716, _a73870a7, _a77c4bb7, _a7db2fb7, _a80ff1db, _a88dc95c, _a8db314a, _a9ad830f, _a9db32dd, _aa2ec582, _aa70f92a, _aa7d95f9, _aab8d1ee, _ab377d66, _ab9594d8, _abbf99cf, _abee7eec, _ac3b2baa, _ac83cd3d, _ad21fd82, _ad8dd13b, _ae93f940, _af1b43a1, _af780c38, _af93fad3, _aff7a8cb, _b293ff8c, _b2b04063, _b394011f, _b39b6550, _b39db0ab, _b49402b2, _b49b66e3, _b5940445, _b59b6876, _b647bec9, _b69b6a09, _b69db564, _b6b8e1cd, _b79b6b9c, _b79db6f7, _b87b46b6, _b87fa440, _b887e6e3, _b89b6d2f, _b89db88a, _b8d55bd0, _b99b6ec2, _b99dba1d, _b9e2dde1, _ba9b7055, _baefeb76, _bb5c4b5d, _bbdb662b, _bbefed09, _bc4f8f1c, _bcbacb59, _bcefee9c, _bd021d9b, _bd43ae72, _bd4f1e10, _bdbaccec, _bdc4bce0, _bdd71b71, _be1b127b, _beafb136, _bec4be73, _beeff1c2, _bf96a952, _bfc4c006, _c01d5c76, _c0aa13f3, _c0c4c199, _c1002565, _c153a18a, _c15e4bff, _c1acaaac, _c1b3e81c, _c20d803a, _c2554682, _c3516619, _c3a787a5, _c3add6c4, _c3e62a5a, _c4002a1e, _c4449aa8, _c4b11118, _c535ad25, _c536acce, _c5bad984, _c6e67040, _c7acb41e, _c7bc7c25, _c7c949cc, _c7e671d3, _c87535b4, _c8c94b5f, _c8e67366, _c8e86837, _c915ff3b, _c988b523, _c99dfe4c, _c9aaa8cc, _c9c94cf2, _c9e674f9, _ca854908, _cab3e756, _cab49249, _cac94e85, _cae6768c, _cb109dee, _cbd249fb, _cbe6781f, _cc2a9e1d, _cc6e7e03, _cc93277e, _cce679b2, _cd38005f, _cd396a5a, _cd654849, _cdbbde88, _cde67b45, _cdedfc57, _ce227343, _ce48cf20, _cf98944b, _cfc1f627, _d0e67ffe, _d1051bb7, _d141da67, _d148d3d9, _d1809b3e, _d1884d73, _d1e68191, _d261c63b, _d296c19f, _d2d743de, _d2f330e2, _d42d8187, _d4488666, _d4e966f6, _d547e80e, _d5baf2b4, _d6218fb7, _d689b45a, _d6a8739a, _d6bf1d5a, _d77c08cf, _d7af0040, _d7baf5da, _d7d9a4df, _d7dcae69, _d805a520, _d8bca617, _d905a6b3, _da9a4b53, _dac25b46, _db39458e, _db46a3fa, _db4c9660, _db7ff637, _dc252060, _dc48e52a, _dc7e3086, _dd8f92ee, _df429cf6, _df6a33c5, _df7a1dd4, _e005b1b8, _e103cb87, _e105b34b, _e14996b7, _e18d12a4, _e205b4de, _e214f988, _e2596300, _e2927129, _e2de4c1a, _e305b671, _e405b804, _e41128b9, _e505b997, _e5c8bda7, _e5f5eb57, _e605bb2a, _e6fffcd6, _e705bcbd, _e749d68c, _e74f39b3, _e79986fe, _e79d56da, _e7ad2eba, _e82b0586, _e89e9716, _e912f243, _e915a0df, _e978f1cf, _e9809404, _e994c828, _e997b997, _ea009b9f, _ea123c60, _eb11dd3f, _eb3a32e1, _eb8497d4, _ecaa6db6, _ed63b9b6, _ed76e557, _edc2b146, _edd26642, _ee0ad371, _eea9287a, _eed9e998, _eeddeba1, _eefddf4b, _eff9b115, _f01004f2, _f0acc361, _f0b17619, _f0e2aa5f, _f22f4fc0, _f2e9cdc1, _f32f5153, _f33a65f8, _f40e857b, _f42b1ec6, _f42f52e6, _f4449a1a, _f468fab6, _f52f5479, _f568fc49, _f5b81ba0, _f5ca20bf, _f5dc663a, _f668fddc, _f6ca2252, _f6cc60e9, _f6ed1fc8, _f6f99720, _f72f579f, _f796f8d6, _f7ca23e5, _f7f998b3, _f81cfe45, _f8320d9c, _f8f99a46, _f98ecfca, _f9f99bd9, _fa66671d, _faf99d6c, _fb32828e, _fb3f0513, _fb5aab57, _fbb24e31, _fbbb4831, _fbca1eb0, _fbf99eff, _fc313a7c, _fc934342, _fc942d8e, _fddf0321, _fe690a74, _fe72e477, _fe8e107e, _ff1a15ef, _ff690c07, _ffc48d42, _fffc2537, _00eaa432, _0156f279, _02c92c95, _02cc73cd, _033f3325, _03c3a4b8, _03caf549, _03e9776c, _05775918, _058073c0, _062aaf64, _06589d34, _07b44be0, _07e43c29, _0971950d, _09b8f622, _0a18c452, _0a191120, _0b73aad6, _0bb4522c, _0c3e27de, _0c73ac69, _0cafd5d4, _0cb453bf, _0d003efb, _0d73adfc, _0d7d2c88, _0db45552, _0e48e275, _0e73af8f, _0eb2d035, _0eb456e5, _0ed14480, _0ede1073, _0f73b122, _0fc06c63, _0fd14613, _1073b2b5, _10d147a6, _11d14939, _120ea07f, _12afeb73, _12d14acc, _13d14c5f, _13d1c717, _14278fae, _14d14df2, _14e33184, _15ab5885, _15bcedf1, _173dad66, _17aafbf8, _18686237, _18d1543e, _19478c3f, _1966e0d0, _199bf7af, _19b77c72, _19ccbf15, _19d155d1, _1bbdf62b, _1bebfe2e, _1cccdd5c, _1cf15395, _1d2676cb, _1dccdeef, _1dccf45b, _1de6d934, _1e3a2a10, _1f372ac0, _1f472b36, _1f4d627e, _1f6682e6, _20046cf0, _20199c06, _201c05c9, _2049f8b2, _20cce3a8, _21cf23d2, _22cf2565, _22d74f20, _22dac12e, _233eec59, _23a2de29, _23cce861, _23cf26f8, _23e9af52, _24919eff, _24cce9f4, _2549312e, _254b891d, _25cceb87, _2617c675, _262a66ba, _264acb7c, _26c25720, _26cced1a, _26cf2bb1, _27020e27, _27804086, _27cceead, _27cf2d44, _287d77df, _28c98a6f, _28cf2ed7, _2951df68, _29cf306a, _2a0d05fa, _2a548a39, _2ab277e1, _2acf31fd, _2b548bcc, _2b74b6bb, _2bcfa0cb, _2bfba9ef, _2c1311b5, _2c2b5dd8, _2c548d5f, _2cb43c67, _2d16decf, _2d2b5f6b, _2d548ef2, _2d718e33, _2e2a901d, _2e549085, _2e894d42, _2fd39d10, _2ff66220, _302b6424, _305e95d5, _30f67918;
        /// <summary>10000.0</summary>
        protected IExpression E31AAB26F => _31aab26f ?? (_31aab26f = new ConstantExpression(new DecimalNumber.Text("10000.0")));
        /// <summary>v!=0 and f % 10!=4,6,9</summary>
        protected IExpression E31E49D3A => _31e49d3a ?? (_31e49d3a = new BinaryOpExpression(BinaryOp.LogicalAnd, E7D97B2C4, E59A97B61));
        /// <summary>0.01</summary>
        protected IExpression E31F67AAB => _31f67aab ?? (_31f67aab = new ConstantExpression(new DecimalNumber.Text("0.01")));
        /// <summary>0.1..0.9</summary>
        protected IExpression E320DA9FF => _320da9ff ?? (_320da9ff = new RangeExpression(EE105B34B, ED905A6B3));
        /// <summary>703</summary>
        protected IExpression E322B674A => _322b674a ?? (_322b674a = new ConstantExpression(new DecimalNumber.Text("703")));
        /// <summary>n % 1000</summary>
        protected IExpression E32301BAE => _32301bae ?? (_32301bae = new BinaryOpExpression(BinaryOp.Modulo, EBDBACCEC, EFC934342));
        /// <summary>n % 10=1,2</summary>
        protected IExpression E323EC34A => _323ec34a ?? (_323ec34a = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E85D169AA));
        /// <summary>6.4</summary>
        protected IExpression E325421B6 => _325421b6 ?? (_325421b6 = new ConstantExpression(new DecimalNumber.Text("6.4")));
        /// <summary>100000</summary>
        protected IExpression E3263A15B => _3263a15b ?? (_3263a15b = new ConstantExpression(new DecimalNumber.Text("100000")));
        /// <summary>0.5..1.0</summary>
        protected IExpression E32F3F2D7 => _32f3f2d7 ?? (_32f3f2d7 = new RangeExpression(EE505B997, E46E2169F));
        /// <summary>n % 10=5..9</summary>
        protected IExpression E32F6A02F => _32f6a02f ?? (_32f6a02f = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E2D16DECF));
        /// <summary>702</summary>
        protected IExpression E332B68DD => _332b68dd ?? (_332b68dd = new ConstantExpression(new DecimalNumber.Text("702")));
        /// <summary>6.2</summary>
        protected IExpression E345424DC => _345424dc ?? (_345424dc = new ConstantExpression(new DecimalNumber.Text("6.2")));
        /// <summary>3..17</summary>
        protected IExpression E34F3F456 => _34f3f456 ?? (_34f3f456 = new RangeExpression(EC7E671D3, EA3C9E04A));
        /// <summary>0.04</summary>
        protected IExpression E34F67F64 => _34f67f64 ?? (_34f67f64 = new ConstantExpression(new DecimalNumber.Text("0.04")));
        /// <summary>6.3</summary>
        protected IExpression E3554266F => _3554266f ?? (_3554266f = new ConstantExpression(new DecimalNumber.Text("6.3")));
        /// <summary>62.0</summary>
        protected IExpression E35802B84 => _35802b84 ?? (_35802b84 = new ConstantExpression(new DecimalNumber.Text("62.0")));
        /// <summary>v=0 and i % 10=0 or v=0 and i % 10=5..9 or v=0 and i % 100=11..14</summary>
        protected IExpression E35C79881 => _35c79881 ?? (_35c79881 = new BinaryOpExpression(BinaryOp.LogicalOr, E5E8AAD1C, EF0E2AA5F));
        /// <summary>6.0</summary>
        protected IExpression E36542802 => _36542802 ?? (_36542802 = new ConstantExpression(new DecimalNumber.Text("6.0")));
        /// <summary>5.00</summary>
        protected IExpression E367D1ED9 => _367d1ed9 ?? (_367d1ed9 = new ConstantExpression(new DecimalNumber.Text("5.00")));
        /// <summary>8.0</summary>
        protected IExpression E371340C0 => _371340c0 ?? (_371340c0 = new ConstantExpression(new DecimalNumber.Text("8.0")));
        /// <summary>n % 10=2,3 and n % 100!=12,13</summary>
        protected IExpression E374D1302 => _374d1302 ?? (_374d1302 = new BinaryOpExpression(BinaryOp.LogicalAnd, EFC313A7C, E92120075));
        /// <summary>6.1</summary>
        protected IExpression E37542995 => _37542995 ?? (_37542995 = new ConstantExpression(new DecimalNumber.Text("6.1")));
        /// <summary>3..10</summary>
        protected IExpression E37F3F90F => _37f3f90f ?? (_37f3f90f = new RangeExpression(EC7E671D3, E9EC9D86B));
        /// <summary>i!=1</summary>
        protected IExpression E3805C12C => _3805c12c ?? (_3805c12c = new BinaryOpExpression(BinaryOp.NotEqual, EBCBACB59, ED6BF1D5A));
        /// <summary>n=0..1</summary>
        protected IExpression E3987DFFC => _3987dffc ?? (_3987dffc = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ED2F330E2));
        /// <summary>17</summary>
        protected IExpression E39FB4E19 => _39fb4e19 ?? (_39fb4e19 = new ConstantExpression(new DecimalNumber.Long(17)));
        /// <summary>i % 10=3,4</summary>
        protected IExpression E3B922DA3 => _3b922da3 ?? (_3b922da3 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, E63DB2C0A));
        /// <summary>0.000</summary>
        protected IExpression E3C00B9F8 => _3c00b9f8 ?? (_3c00b9f8 = new ConstantExpression(new DecimalNumber.Text("0.000")));
        /// <summary>6..20</summary>
        protected IExpression E3C30DB95 => _3c30db95 ?? (_3c30db95 = new RangeExpression(ECAE6768C, E8CC30050));
        /// <summary>0.001</summary>
        protected IExpression E3D00BB8B => _3d00bb8b ?? (_3d00bb8b = new ConstantExpression(new DecimalNumber.Text("0.001")));
        /// <summary>0.0000</summary>
        protected IExpression E3D2475D8 => _3d2475d8 ?? (_3d2475d8 = new ConstantExpression(new DecimalNumber.Text("0.0000")));
        /// <summary>n=1,3</summary>
        protected IExpression E3D53A975 => _3d53a975 ?? (_3d53a975 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E1DCCF45B));
        /// <summary>n=5</summary>
        protected IExpression E3D5E8A34 => _3d5e8a34 ?? (_3d5e8a34 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EBEAFB136));
        /// <summary>0.0001</summary>
        protected IExpression E3E24776B => _3e24776b ?? (_3e24776b = new ConstantExpression(new DecimalNumber.Text("0.0001")));
        /// <summary>v=0 and i % 10=2</summary>
        protected IExpression E3E2A37EA => _3e2a37ea ?? (_3e2a37ea = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EE994C828));
        /// <summary>v=0 and i % 10=1</summary>
        protected IExpression E3E805D25 => _3e805d25 ?? (_3e805d25 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, E0FC06C63));
        /// <summary>n % 100=11..99</summary>
        protected IExpression E3EC25B77 => _3ec25b77 ?? (_3ec25b77 = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, E96BDB99F));
        /// <summary>9.0</summary>
        protected IExpression E3EDE2427 => _3ede2427 ?? (_3ede2427 = new ConstantExpression(new DecimalNumber.Text("9.0")));
        /// <summary>n=2</summary>
        protected IExpression E3F0B167B => _3f0b167b ?? (_3f0b167b = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E7C9B4215));
        /// <summary>n=6</summary>
        protected IExpression E3F4C191F => _3f4c191f ?? (_3f4c191f = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E84C18E51));
        /// <summary>10</summary>
        protected IExpression E3FF8DD22 => _3ff8dd22 ?? (_3ff8dd22 = new ConstantExpression(new DecimalNumber.Long(10)));
        /// <summary>n % 100</summary>
        protected IExpression E3FFFE6B6 => _3fffe6b6 ?? (_3fffe6b6 = new BinaryOpExpression(BinaryOp.Modulo, EBDBACCEC, EE7AD2EBA));
        /// <summary>100,200,300,400,500,600,700,800,900</summary>
        protected IExpression E404FB29E => _404fb29e ?? (_404fb29e = new GroupExpression(EE7AD2EBA, EA501A4AD, E1966E0D0, EBE1B127B, E1F472B36, EE2927129, E74E1E9AC, E63AEFA47, E984A4D02));
        /// <summary>v=0 and i % 100=3..4 or f % 100=3..4</summary>
        protected IExpression E40592561 => _40592561 ?? (_40592561 = new BinaryOpExpression(BinaryOp.LogicalOr, E13D1C717, E058073C0));
        /// <summary>1.5</summary>
        protected IExpression E41E20EC0 => _41e20ec0 ?? (_41e20ec0 = new ConstantExpression(new DecimalNumber.Text("1.5")));
        /// <summary>1.4</summary>
        protected IExpression E42E21053 => _42e21053 ?? (_42e21053 = new ConstantExpression(new DecimalNumber.Text("1.4")));
        /// <summary>n % 10=2..9</summary>
        protected IExpression E42E3F86C => _42e3f86c ?? (_42e3f86c = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, EE749D68C));
        /// <summary>n=4</summary>
        protected IExpression E4360D23D => _4360d23d ?? (_4360d23d = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EC0AA13F3));
        /// <summary>n % 10=6,9 or n=10</summary>
        protected IExpression E43CCD76C => _43ccd76c ?? (_43ccd76c = new BinaryOpExpression(BinaryOp.LogicalOr, ED689B45A, EE214F988));
        /// <summary>1.5..2.0</summary>
        protected IExpression E43D64505 => _43d64505 ?? (_43d64505 = new RangeExpression(E41E20EC0, E8AF46FEE));
        /// <summary>1.7</summary>
        protected IExpression E43E211E6 => _43e211e6 ?? (_43e211e6 = new ConstantExpression(new DecimalNumber.Text("1.7")));
        /// <summary>f!=0</summary>
        protected IExpression E44CCE0D4 => _44cce0d4 ?? (_44cce0d4 = new BinaryOpExpression(BinaryOp.NotEqual, EC5BAD984, ED8BCA617));
        /// <summary>1.6</summary>
        protected IExpression E44E21379 => _44e21379 ?? (_44e21379 = new ConstantExpression(new DecimalNumber.Text("1.6")));
        /// <summary>1.00</summary>
        protected IExpression E44E9B57D => _44e9b57d ?? (_44e9b57d = new ConstantExpression(new DecimalNumber.Text("1.00")));
        /// <summary>6..17</summary>
        protected IExpression E45332857 => _45332857 ?? (_45332857 = new RangeExpression(ECAE6768C, EA3C9E04A));
        /// <summary>21..24</summary>
        protected IExpression E45D73A52 => _45d73a52 ?? (_45d73a52 = new RangeExpression(EE79D56DA, ED1884D73));
        /// <summary>1.1</summary>
        protected IExpression E45E2150C => _45e2150c ?? (_45e2150c = new ConstantExpression(new DecimalNumber.Text("1.1")));
        /// <summary>161.0</summary>
        protected IExpression E468378EA => _468378ea ?? (_468378ea = new ConstantExpression(new DecimalNumber.Text("161.0")));
        /// <summary>61.0</summary>
        protected IExpression E46AD7DE1 => _46ad7de1 ?? (_46ad7de1 = new ConstantExpression(new DecimalNumber.Text("61.0")));
        /// <summary>1.0</summary>
        protected IExpression E46E2169F => _46e2169f ?? (_46e2169f = new ConstantExpression(new DecimalNumber.Text("1.0")));
        /// <summary>1.3</summary>
        protected IExpression E47E21832 => _47e21832 ?? (_47e21832 = new ConstantExpression(new DecimalNumber.Text("1.3")));
        /// <summary>2.5..2.7</summary>
        protected IExpression E47ED7465 => _47ed7465 ?? (_47ed7465 = new RangeExpression(E8FF477CD, E8DF474A7));
        /// <summary>1001.0</summary>
        protected IExpression E4816C5DE => _4816c5de ?? (_4816c5de = new ConstantExpression(new DecimalNumber.Text("1001.0")));
        /// <summary>f % 10=1</summary>
        protected IExpression E486AF022 => _486af022 ?? (_486af022 = new BinaryOpExpression(BinaryOp.Equal, ECAB3E756, ED6BF1D5A));
        /// <summary>7.0000</summary>
        protected IExpression E48D25993 => _48d25993 ?? (_48d25993 = new ConstantExpression(new DecimalNumber.Text("7.0000")));
        /// <summary>1.2</summary>
        protected IExpression E48E219C5 => _48e219c5 ?? (_48e219c5 = new ConstantExpression(new DecimalNumber.Text("1.2")));
        /// <summary>61..64</summary>
        protected IExpression E4937FFBA => _4937ffba ?? (_4937ffba = new RangeExpression(E61EEE76E, E2BFBA9EF));
        /// <summary>i=1</summary>
        protected IExpression E49A58121 => _49a58121 ?? (_49a58121 = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, ED6BF1D5A));
        /// <summary>i=0</summary>
        protected IExpression E49A7BFB8 => _49a7bfb8 ?? (_49a7bfb8 = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, ED8BCA617));
        /// <summary>109</summary>
        protected IExpression E49BF8916 => _49bf8916 ?? (_49bf8916 = new ConstantExpression(new DecimalNumber.Text("109")));
        /// <summary>i % 100!=12..14</summary>
        protected IExpression E49D95C79 => _49d95c79 ?? (_49d95c79 = new BinaryOpExpression(BinaryOp.NotEqual, E84E93E2B, EB647BEC9));
        /// <summary>11.0</summary>
        protected IExpression E4A26B276 => _4a26b276 ?? (_4a26b276 = new ConstantExpression(new DecimalNumber.Text("11.0")));
        /// <summary>v=0 and i!=1 and i % 10=0..1</summary>
        protected IExpression E4ABBA532 => _4abba532 ?? (_4abba532 = new BinaryOpExpression(BinaryOp.LogicalAnd, E0C3E27DE, EEEFDDF4B));
        /// <summary>11,71,91</summary>
        protected IExpression E4B0CF081 => _4b0cf081 ?? (_4b0cf081 = new GroupExpression(E8DF33FDF, E9016A675, E780A8EB7));
        /// <summary>800</summary>
        protected IExpression E4B0EE30E => _4b0ee30e ?? (_4b0ee30e = new ConstantExpression(new DecimalNumber.Text("800")));
        /// <summary>v=2 and f % 100=11..19</summary>
        protected IExpression E4B83D017 => _4b83d017 ?? (_4b83d017 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5B8C4D33, EE14996B7));
        /// <summary>n % 10=6 or n % 10=9 or n % 10=0 and n!=0</summary>
        protected IExpression E4C5775FB => _4c5775fb ?? (_4c5775fb = new BinaryOpExpression(BinaryOp.LogicalOr, EEE0AD371, E14E33184));
        /// <summary>10.4</summary>
        protected IExpression E4C64A931 => _4c64a931 ?? (_4c64a931 = new ConstantExpression(new DecimalNumber.Text("10.4")));
        /// <summary>7.00</summary>
        protected IExpression E4C6DB99B => _4c6db99b ?? (_4c6db99b = new ConstantExpression(new DecimalNumber.Text("7.00")));
        /// <summary>29.0</summary>
        protected IExpression E4C7FAE6D => _4c7fae6d ?? (_4c7fae6d = new ConstantExpression(new DecimalNumber.Text("29.0")));
        /// <summary>110</summary>
        protected IExpression E4CBD4F38 => _4cbd4f38 ?? (_4cbd4f38 = new ConstantExpression(new DecimalNumber.Text("110")));
        /// <summary>1.0000</summary>
        protected IExpression E4CDBC305 => _4cdbc305 ?? (_4cdbc305 = new ConstantExpression(new DecimalNumber.Text("1.0000")));
        /// <summary>10.3</summary>
        protected IExpression E4D64AAC4 => _4d64aac4 ?? (_4d64aac4 = new ConstantExpression(new DecimalNumber.Text("10.3")));
        /// <summary>n!=0 and n % 1000000=0</summary>
        protected IExpression E4D83051C => _4d83051c ?? (_4d83051c = new BinaryOpExpression(BinaryOp.LogicalAnd, EC1B3E81C, E120EA07F));
        /// <summary>111</summary>
        protected IExpression E4DBD50CB => _4dbd50cb ?? (_4dbd50cb = new ConstantExpression(new DecimalNumber.Text("111")));
        /// <summary>1.9</summary>
        protected IExpression E4DE221A4 => _4de221a4 ?? (_4de221a4 = new ConstantExpression(new DecimalNumber.Text("1.9")));
        /// <summary>10.2</summary>
        protected IExpression E4E64AC57 => _4e64ac57 ?? (_4e64ac57 = new ConstantExpression(new DecimalNumber.Text("10.2")));
        /// <summary>v=0 and i % 10=2..4 and i % 100!=12..14</summary>
        protected IExpression E4E88644C => _4e88644c ?? (_4e88644c = new BinaryOpExpression(BinaryOp.LogicalAnd, EAA70F92A, E49D95C79));
        /// <summary>1.8</summary>
        protected IExpression E4EE22337 => _4ee22337 ?? (_4ee22337 = new ConstantExpression(new DecimalNumber.Text("1.8")));
        /// <summary>10.1</summary>
        protected IExpression E4F64ADEA => _4f64adea ?? (_4f64adea = new ConstantExpression(new DecimalNumber.Text("10.1")));
        /// <summary>1011</summary>
        protected IExpression E4F98894D => _4f98894d ?? (_4f98894d = new ConstantExpression(new DecimalNumber.Text("1011")));
        /// <summary>v=0 and i % 100=1</summary>
        protected IExpression E4FADB3DD => _4fadb3dd ?? (_4fadb3dd = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, ECF98944B));
        /// <summary>103</summary>
        protected IExpression E4FBF9288 => _4fbf9288 ?? (_4fbf9288 = new ConstantExpression(new DecimalNumber.Text("103")));
        /// <summary>9..19</summary>
        protected IExpression E4FD11ACA => _4fd11aca ?? (_4fd11aca = new RangeExpression(ED1E68191, E95C9CA40));
        /// <summary>n=1..4</summary>
        protected IExpression E4FE21A9C => _4fe21a9c ?? (_4fe21a9c = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E734BD952));
        /// <summary>n=7..10</summary>
        protected IExpression E5033232B => _5033232b ?? (_5033232b = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EB9E2DDE1));
        /// <summary>10.0</summary>
        protected IExpression E5064AF7D => _5064af7d ?? (_5064af7d = new ConstantExpression(new DecimalNumber.Text("10.0")));
        /// <summary>7..9</summary>
        protected IExpression E508BE571 => _508be571 ?? (_508be571 = new RangeExpression(ECBE6781F, ED1E68191));
        /// <summary>v=0 and i % 100=11..14</summary>
        protected IExpression E50AE9FFD => _50ae9ffd ?? (_50ae9ffd = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, ED42D8187));
        /// <summary>12.000</summary>
        protected IExpression E50BC71DF => _50bc71df ?? (_50bc71df = new ConstantExpression(new DecimalNumber.Text("12.000")));
        /// <summary>102</summary>
        protected IExpression E50BF941B => _50bf941b ?? (_50bf941b = new ConstantExpression(new DecimalNumber.Text("102")));
        /// <summary>400..402</summary>
        protected IExpression E50FB55E9 => _50fb55e9 ?? (_50fb55e9 = new RangeExpression(E0F73B122, E0D73ADFC));
        /// <summary>v=0 and i % 10=5..9</summary>
        protected IExpression E519BA030 => _519ba030 ?? (_519ba030 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EDC7E3086));
        /// <summary>101</summary>
        protected IExpression E51BF95AE => _51bf95ae ?? (_51bf95ae = new ConstantExpression(new DecimalNumber.Text("101")));
        /// <summary>13</summary>
        protected IExpression E51D5343D => _51d5343d ?? (_51d5343d = new ConstantExpression(new DecimalNumber.Long(13)));
        /// <summary>43</summary>
        protected IExpression E51E459A6 => _51e459a6 ?? (_51e459a6 = new ConstantExpression(new DecimalNumber.Long(43)));
        /// <summary>n % 100!=13</summary>
        protected IExpression E52ADA190 => _52ada190 ?? (_52ada190 = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E51D5343D));
        /// <summary>100</summary>
        protected IExpression E52BF9741 => _52bf9741 ?? (_52bf9741 = new ConstantExpression(new DecimalNumber.Text("100")));
        /// <summary>1.1..1.8</summary>
        protected IExpression E52E81D7C => _52e81d7c ?? (_52e81d7c = new RangeExpression(E45E2150C, E4EE22337));
        /// <summary>117</summary>
        protected IExpression E53BD5A3D => _53bd5a3d ?? (_53bd5a3d = new ConstantExpression(new DecimalNumber.Text("117")));
        /// <summary>107</summary>
        protected IExpression E53BF98D4 => _53bf98d4 ?? (_53bf98d4 = new ConstantExpression(new DecimalNumber.Text("107")));
        /// <summary>42</summary>
        protected IExpression E53E1E263 => _53e1e263 ?? (_53e1e263 = new ConstantExpression(new DecimalNumber.Long(42)));
        /// <summary>1.1..1.9</summary>
        protected IExpression E53E81F0F => _53e81f0f ?? (_53e81f0f = new RangeExpression(E45E2150C, E4DE221A4));
        /// <summary>106</summary>
        protected IExpression E54BF9A67 => _54bf9a67 ?? (_54bf9a67 = new ConstantExpression(new DecimalNumber.Text("106")));
        /// <summary>21.0</summary>
        protected IExpression E55712385 => _55712385 ?? (_55712385 = new ConstantExpression(new DecimalNumber.Text("21.0")));
        /// <summary>12..17</summary>
        protected IExpression E55759678 => _55759678 ?? (_55759678 = new RangeExpression(EA0C9DB91, EA3C9E04A));
        /// <summary>105</summary>
        protected IExpression E55BF9BFA => _55bf9bfa ?? (_55bf9bfa = new ConstantExpression(new DecimalNumber.Text("105")));
        /// <summary>161</summary>
        protected IExpression E55CEA684 => _55cea684 ?? (_55cea684 = new ConstantExpression(new DecimalNumber.Text("161")));
        /// <summary>104</summary>
        protected IExpression E56BF9D8D => _56bf9d8d ?? (_56bf9d8d = new ConstantExpression(new DecimalNumber.Text("104")));
        /// <summary>n % 100=3..10</summary>
        protected IExpression E5738BF6D => _5738bf6d ?? (_5738bf6d = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, E305E95D5));
        /// <summary>1.1..1.6</summary>
        protected IExpression E58E826EE => _58e826ee ?? (_58e826ee = new RangeExpression(E45E2150C, E44E21379));
        /// <summary>f % 10!=4,6,9</summary>
        protected IExpression E59A97B61 => _59a97b61 ?? (_59a97b61 = new BinaryOpExpression(BinaryOp.NotEqual, ECAB3E756, EF6ED1FC8));
        /// <summary>1.1..1.7</summary>
        protected IExpression E59E82881 => _59e82881 ?? (_59e82881 = new RangeExpression(E45E2150C, E43E211E6));
        /// <summary>18.0</summary>
        protected IExpression E59F58855 => _59f58855 ?? (_59f58855 = new ConstantExpression(new DecimalNumber.Text("18.0")));
        /// <summary>n=0,7,8,9</summary>
        protected IExpression E5AE59525 => _5ae59525 ?? (_5ae59525 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EC988B523));
        /// <summary>n % 10=1 and n % 100!=11 or v=2 and f % 10=1 and f % 100!=11 or v!=2 and f % 10=1</summary>
        protected IExpression E5B3EA793 => _5b3ea793 ?? (_5b3ea793 = new BinaryOpExpression(BinaryOp.LogicalOr, E84A1942F, EBD43AE72));
        /// <summary>i % 10=1,2,5,7,8</summary>
        protected IExpression E5B603073 => _5b603073 ?? (_5b603073 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, EE79986FE));
        /// <summary>v=2</summary>
        protected IExpression E5B8C4D33 => _5b8c4d33 ?? (_5b8c4d33 = new BinaryOpExpression(BinaryOp.Equal, ED5BAF2B4, E7C9B4215));
        /// <summary>2..20</summary>
        protected IExpression E5BACBDED => _5bacbded ?? (_5bacbded = new RangeExpression(E7C9B4215, EE997B997));
        /// <summary>v=0</summary>
        protected IExpression E5BCFC261 => _5bcfc261 ?? (_5bcfc261 = new BinaryOpExpression(BinaryOp.Equal, ED5BAF2B4, ED8BCA617));
        /// <summary>n=1,5,7,8,9,10</summary>
        protected IExpression E5C665074 => _5c665074 ?? (_5c665074 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ED4488666));
        /// <summary>n % 100!=11..19</summary>
        protected IExpression E5C674AF6 => _5c674af6 ?? (_5c674af6 = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, EA77C4BB7));
        /// <summary>n % 100!=12</summary>
        protected IExpression E5CAB36E5 => _5cab36e5 ?? (_5cab36e5 = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E93D79EE8));
        /// <summary>n % 100!=14</summary>
        protected IExpression E5CCEB86F => _5cceb86f ?? (_5cceb86f = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E77E9A35E));
        /// <summary>n % 100=2..19</summary>
        protected IExpression E5D6BFBF5 => _5d6bfbf5 ?? (_5d6bfbf5 = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, E97B930AD));
        /// <summary>i % 100=2..20,40,60,80</summary>
        protected IExpression E5DEB8672 => _5deb8672 ?? (_5deb8672 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, E63A9D953));
        /// <summary>92</summary>
        protected IExpression E5E0B1160 => _5e0b1160 ?? (_5e0b1160 = new ConstantExpression(new DecimalNumber.Long(92)));
        /// <summary>n % 1000000=100000</summary>
        protected IExpression E5E12E1FA => _5e12e1fa ?? (_5e12e1fa = new BinaryOpExpression(BinaryOp.Equal, EC01D5C76, E7D140262));
        /// <summary>i % 10</summary>
        protected IExpression E5E3CEC93 => _5e3cec93 ?? (_5e3cec93 = new BinaryOpExpression(BinaryOp.Modulo, EBCBACB59, E3FF8DD22));
        /// <summary>v=0 and i % 10=0</summary>
        protected IExpression E5E8AAD1C => _5e8aad1c ?? (_5e8aad1c = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EEDC2B146));
        /// <summary>70..79</summary>
        protected IExpression E5F80534A => _5f80534a ?? (_5f80534a = new RangeExpression(E0A191120, E6FE9BC5D));
        /// <summary>4.0000</summary>
        protected IExpression E608D2F94 => _608d2f94 ?? (_608d2f94 = new ConstantExpression(new DecimalNumber.Text("4.0000")));
        /// <summary>8.0000</summary>
        protected IExpression E60D363B0 => _60d363b0 ?? (_60d363b0 = new ConstantExpression(new DecimalNumber.Text("8.0000")));
        /// <summary>102.0</summary>
        protected IExpression E619EF28D => _619ef28d ?? (_619ef28d = new ConstantExpression(new DecimalNumber.Text("102.0")));
        /// <summary>61</summary>
        protected IExpression E61EEE76E => _61eee76e ?? (_61eee76e = new ConstantExpression(new DecimalNumber.Long(61)));
        /// <summary>2..20,40,60,80</summary>
        protected IExpression E63A9D953 => _63a9d953 ?? (_63a9d953 = new GroupExpression(E5BACBDED, E7FD9E0C1, E63E94A2B, E934C92A5));
        /// <summary>800</summary>
        protected IExpression E63AEFA47 => _63aefa47 ?? (_63aefa47 = new ConstantExpression(new DecimalNumber.Long(800)));
        /// <summary>3,4</summary>
        protected IExpression E63DB2C0A => _63db2c0a ?? (_63db2c0a = new GroupExpression(E72B6DCC0, EC0AA13F3));
        /// <summary>12,72,92</summary>
        protected IExpression E63E4D29A => _63e4d29a ?? (_63e4d29a = new GroupExpression(E93D79EE8, E6E0821BA, E5E0B1160));
        /// <summary>60</summary>
        protected IExpression E63E94A2B => _63e94a2b ?? (_63e94a2b = new ConstantExpression(new DecimalNumber.Long(60)));
        /// <summary>0.2..0.9</summary>
        protected IExpression E641140B2 => _641140b2 ?? (_641140b2 = new RangeExpression(EE205B4DE, ED905A6B3));
        /// <summary>40,60,90</summary>
        protected IExpression E65358758 => _65358758 ?? (_65358758 = new GroupExpression(E7FD9E0C1, E63E94A2B, E2A0D05FA));
        /// <summary>0.2..1.0</summary>
        protected IExpression E65961D56 => _65961d56 ?? (_65961d56 = new RangeExpression(EE205B4DE, E46E2169F));
        /// <summary>i=0 or i % 100=2..20,40,60,80</summary>
        protected IExpression E669B110C => _669b110c ?? (_669b110c = new BinaryOpExpression(BinaryOp.LogicalOr, E49A7BFB8, E5DEB8672));
        /// <summary>61..64</summary>
        protected IExpression E67ADBEBC => _67adbebc ?? (_67adbebc = new RangeExpression(E25CCEB87, E20CCE3A8));
        /// <summary>n % 1000=0 and n % 100000=1000..20000,40000,60000,80000</summary>
        protected IExpression E68397994 => _68397994 ?? (_68397994 = new BinaryOpExpression(BinaryOp.LogicalAnd, EE5F5EB57, E05775918));
        /// <summary>0.2..0.4</summary>
        protected IExpression E69114891 => _69114891 ?? (_69114891 = new RangeExpression(EE205B4DE, EE405B804));
        /// <summary>3.5</summary>
        protected IExpression E6CD19676 => _6cd19676 ?? (_6cd19676 = new ConstantExpression(new DecimalNumber.Text("3.5")));
        /// <summary>10..19,70..79,90..99</summary>
        protected IExpression E6CFB81E6 => _6cfb81e6 ?? (_6cfb81e6 = new GroupExpression(EDF429CF6, E5F80534A, E1F6682E6));
        /// <summary>4.2..4.4</summary>
        protected IExpression E6D12B621 => _6d12b621 ?? (_6d12b621 = new RangeExpression(E0DB45552, E07B44BE0));
        /// <summary>v=0 and i % 10!=4,6,9 or v!=0 and f % 10!=4,6,9</summary>
        protected IExpression E6DA348F2 => _6da348f2 ?? (_6da348f2 = new BinaryOpExpression(BinaryOp.LogicalOr, E76DEF1B2, E31E49D3A));
        /// <summary>3.4</summary>
        protected IExpression E6DD19809 => _6dd19809 ?? (_6dd19809 = new ConstantExpression(new DecimalNumber.Text("3.4")));
        /// <summary>72</summary>
        protected IExpression E6E0821BA => _6e0821ba ?? (_6e0821ba = new ConstantExpression(new DecimalNumber.Long(72)));
        /// <summary>n % 10</summary>
        protected IExpression E6ECCDE4E => _6eccde4e ?? (_6eccde4e = new BinaryOpExpression(BinaryOp.Modulo, EBDBACCEC, E3FF8DD22));
        /// <summary>3.3</summary>
        protected IExpression E6ED1999C => _6ed1999c ?? (_6ed1999c = new ConstantExpression(new DecimalNumber.Text("3.3")));
        /// <summary>i % 10=2 and i % 100!=12</summary>
        protected IExpression E6EE6DFCF => _6ee6dfcf ?? (_6ee6dfcf = new BinaryOpExpression(BinaryOp.LogicalAnd, EE994C828, E17AAFBF8));
        /// <summary>20.0</summary>
        protected IExpression E6F3358DE => _6f3358de ?? (_6f3358de = new ConstantExpression(new DecimalNumber.Text("20.0")));
        /// <summary>3.2</summary>
        protected IExpression E6FD19B2F => _6fd19b2f ?? (_6fd19b2f = new ConstantExpression(new DecimalNumber.Text("3.2")));
        /// <summary>79</summary>
        protected IExpression E6FE9BC5D => _6fe9bc5d ?? (_6fe9bc5d = new ConstantExpression(new DecimalNumber.Long(79)));
        /// <summary>500..502</summary>
        protected IExpression E6FFAE315 => _6ffae315 ?? (_6ffae315 = new RangeExpression(E8F484BCD, E8D4848A7));
        /// <summary>3.1</summary>
        protected IExpression E70D19CC2 => _70d19cc2 ?? (_70d19cc2 = new ConstantExpression(new DecimalNumber.Text("3.1")));
        /// <summary>v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11</summary>
        protected IExpression E71BE1840 => _71be1840 ?? (_71be1840 = new BinaryOpExpression(BinaryOp.LogicalOr, ECAB49249, E76B1A96F));
        /// <summary>3.0</summary>
        protected IExpression E71D19E55 => _71d19e55 ?? (_71d19e55 = new ConstantExpression(new DecimalNumber.Text("3.0")));
        /// <summary>3</summary>
        protected IExpression E72B6DCC0 => _72b6dcc0 ?? (_72b6dcc0 = new ConstantExpression(new DecimalNumber.Long(3)));
        /// <summary>83.0</summary>
        protected IExpression E72C788F1 => _72c788f1 ?? (_72c788f1 = new ConstantExpression(new DecimalNumber.Text("83.0")));
        /// <summary>i % 10=3,4 or i % 1000=100,200,300,400,500,600,700,800,900</summary>
        protected IExpression E72EAC562 => _72eac562 ?? (_72eac562 = new BinaryOpExpression(BinaryOp.LogicalOr, E3B922DA3, EBBDB662B));
        /// <summary>19.0</summary>
        protected IExpression E731859EE => _731859ee ?? (_731859ee = new ConstantExpression(new DecimalNumber.Text("19.0")));
        /// <summary>1..4</summary>
        protected IExpression E734BD952 => _734bd952 ?? (_734bd952 = new RangeExpression(ED6BF1D5A, EC0AA13F3));
        /// <summary>11.000</summary>
        protected IExpression E73602F26 => _73602f26 ?? (_73602f26 = new ConstantExpression(new DecimalNumber.Text("11.000")));
        /// <summary>f % 100=2</summary>
        protected IExpression E737D7515 => _737d7515 ?? (_737d7515 = new BinaryOpExpression(BinaryOp.Equal, E779038EE, E7C9B4215));
        /// <summary>2.1..2.7</summary>
        protected IExpression E73D9E621 => _73d9e621 ?? (_73d9e621 = new RangeExpression(E8BF47181, E8DF474A7));
        /// <summary>2,22,42,62,82</summary>
        protected IExpression E7432AAA1 => _7432aaa1 ?? (_7432aaa1 = new GroupExpression(E7C9B4215, E8D95D195, E53E1E263, E07E43C29, EA73870A7));
        /// <summary>301</summary>
        protected IExpression E7490FBB8 => _7490fbb8 ?? (_7490fbb8 = new ConstantExpression(new DecimalNumber.Text("301")));
        /// <summary>700</summary>
        protected IExpression E74E1E9AC => _74e1e9ac ?? (_74e1e9ac = new ConstantExpression(new DecimalNumber.Long(700)));
        /// <summary>n=3,13</summary>
        protected IExpression E7574BF5A => _7574bf5a ?? (_7574bf5a = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ED7AF0040));
        /// <summary>300</summary>
        protected IExpression E7590FD4B => _7590fd4b ?? (_7590fd4b = new ConstantExpression(new DecimalNumber.Text("300")));
        /// <summary>41</summary>
        protected IExpression E75DC4B6C => _75dc4b6c ?? (_75dc4b6c = new ConstantExpression(new DecimalNumber.Long(41)));
        /// <summary>303</summary>
        protected IExpression E7690FEDE => _7690fede ?? (_7690fede = new ConstantExpression(new DecimalNumber.Text("303")));
        /// <summary>f % 10=1 and f % 100!=11</summary>
        protected IExpression E76B1A96F => _76b1a96f ?? (_76b1a96f = new BinaryOpExpression(BinaryOp.LogicalAnd, E486AF022, EF42B1EC6));
        /// <summary>2,12</summary>
        protected IExpression E76BF91A4 => _76bf91a4 ?? (_76bf91a4 = new GroupExpression(E7C9B4215, E93D79EE8));
        /// <summary>v=0 and i % 10!=4,6,9</summary>
        protected IExpression E76DEF1B2 => _76def1b2 ?? (_76def1b2 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EDB4C9660));
        /// <summary>f % 100</summary>
        protected IExpression E779038EE => _779038ee ?? (_779038ee = new BinaryOpExpression(BinaryOp.Modulo, EC5BAD984, EE7AD2EBA));
        /// <summary>302</summary>
        protected IExpression E77910071 => _77910071 ?? (_77910071 = new ConstantExpression(new DecimalNumber.Text("302")));
        /// <summary>44</summary>
        protected IExpression E77B6BA85 => _77b6ba85 ?? (_77b6ba85 = new ConstantExpression(new DecimalNumber.Long(44)));
        /// <summary>v=0 and i=1,2,3 or v=0 and i % 10!=4,6,9 or v!=0 and f % 10!=4,6,9</summary>
        protected IExpression E77C90E06 => _77c90e06 ?? (_77c90e06 = new BinaryOpExpression(BinaryOp.LogicalOr, ECC93277E, E6DA348F2));
        /// <summary>14</summary>
        protected IExpression E77E9A35E => _77e9a35e ?? (_77e9a35e = new ConstantExpression(new DecimalNumber.Long(14)));
        /// <summary>91</summary>
        protected IExpression E780A8EB7 => _780a8eb7 ?? (_780a8eb7 = new ConstantExpression(new DecimalNumber.Long(91)));
        /// <summary>305</summary>
        protected IExpression E78910204 => _78910204 ?? (_78910204 = new ConstantExpression(new DecimalNumber.Text("305")));
        /// <summary>i % 10=1,2,5,7,8 or i % 100=20,50,70,80</summary>
        protected IExpression E7909FE8D => _7909fe8d ?? (_7909fe8d = new BinaryOpExpression(BinaryOp.LogicalOr, E5B603073, EF33A65F8));
        /// <summary>304</summary>
        protected IExpression E79910397 => _79910397 ?? (_79910397 = new ConstantExpression(new DecimalNumber.Text("304")));
        /// <summary>7</summary>
        protected IExpression E7AC0D2FC => _7ac0d2fc ?? (_7ac0d2fc = new ConstantExpression(new DecimalNumber.Long(7)));
        /// <summary>21..24</summary>
        protected IExpression E7B17078C => _7b17078c ?? (_7b17078c = new RangeExpression(E8DC301E3, E90C3069C));
        /// <summary>80000</summary>
        protected IExpression E7B2EB74F => _7b2eb74f ?? (_7b2eb74f = new ConstantExpression(new DecimalNumber.Long(80000)));
        /// <summary>14.0</summary>
        protected IExpression E7C134FF1 => _7c134ff1 ?? (_7c134ff1 = new ConstantExpression(new DecimalNumber.Text("14.0")));
        /// <summary>2</summary>
        protected IExpression E7C9B4215 => _7c9b4215 ?? (_7c9b4215 = new ConstantExpression(new DecimalNumber.Long(2)));
        /// <summary>100000</summary>
        protected IExpression E7D140262 => _7d140262 ?? (_7d140262 = new ConstantExpression(new DecimalNumber.Long(100000)));
        /// <summary>v!=0</summary>
        protected IExpression E7D97B2C4 => _7d97b2c4 ?? (_7d97b2c4 = new BinaryOpExpression(BinaryOp.NotEqual, ED5BAF2B4, ED8BCA617));
        /// <summary>t!=0</summary>
        protected IExpression E7DF0645A => _7df0645a ?? (_7df0645a = new BinaryOpExpression(BinaryOp.NotEqual, ED7BAF5DA, ED8BCA617));
        /// <summary>12.00</summary>
        protected IExpression E7F3C2E35 => _7f3c2e35 ?? (_7f3c2e35 = new ConstantExpression(new DecimalNumber.Text("12.00")));
        /// <summary>40</summary>
        protected IExpression E7FD9E0C1 => _7fd9e0c1 ?? (_7fd9e0c1 = new ConstantExpression(new DecimalNumber.Long(40)));
        /// <summary>18</summary>
        protected IExpression E7FF0739A => _7ff0739a ?? (_7ff0739a = new ConstantExpression(new DecimalNumber.Long(18)));
        /// <summary>14..21</summary>
        protected IExpression E800B9663 => _800b9663 ?? (_800b9663 = new RangeExpression(EA2C9DEB7, E8DC301E3));
        /// <summary>v=0 and i % 10=2..4 and i % 100!=12..14 or f % 10=2..4 and f % 100!=12..14</summary>
        protected IExpression E802DCD2C => _802dcd2c ?? (_802dcd2c = new BinaryOpExpression(BinaryOp.LogicalOr, E4E88644C, EF796F8D6));
        /// <summary>n=3..10,13..19</summary>
        protected IExpression E80C36837 => _80c36837 ?? (_80c36837 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EF2E9CDC1));
        /// <summary>v=0 and i % 100=1 or f % 100=1</summary>
        protected IExpression E81365D4D => _81365d4d ?? (_81365d4d = new BinaryOpExpression(BinaryOp.LogicalOr, E4FADB3DD, ECD396A5A));
        /// <summary>i % 10=6</summary>
        protected IExpression E81BC9ACC => _81bc9acc ?? (_81bc9acc = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, E84C18E51));
        /// <summary>2.0..3.4</summary>
        protected IExpression E82B7905C => _82b7905c ?? (_82b7905c = new RangeExpression(E8AF46FEE, E6DD19809));
        /// <summary>3..4</summary>
        protected IExpression E82BF64C0 => _82bf64c0 ?? (_82bf64c0 = new RangeExpression(E72B6DCC0, EC0AA13F3));
        /// <summary>i=0 and f=1</summary>
        protected IExpression E82F74277 => _82f74277 ?? (_82f74277 = new BinaryOpExpression(BinaryOp.LogicalAnd, E49A7BFB8, ECA854908));
        /// <summary>20..34</summary>
        protected IExpression E83188450 => _83188450 ?? (_83188450 = new RangeExpression(E8CC30050, E96C54EA5));
        /// <summary>v=0 and n!=0..10</summary>
        protected IExpression E837EC443 => _837ec443 ?? (_837ec443 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EAA7D95F9));
        /// <summary>23</summary>
        protected IExpression E83951640 => _83951640 ?? (_83951640 = new ConstantExpression(new DecimalNumber.Long(23)));
        /// <summary>2.0..3.5</summary>
        protected IExpression E83B791EF => _83b791ef ?? (_83b791ef = new RangeExpression(E8AF46FEE, E6CD19676));
        /// <summary>20..35</summary>
        protected IExpression E841885E3 => _841885e3 ?? (_841885e3 = new RangeExpression(E8CC30050, E95C54D12));
        /// <summary>n % 10=1 and n % 100!=11</summary>
        protected IExpression E84A1942F => _84a1942f ?? (_84a1942f = new BinaryOpExpression(BinaryOp.LogicalAnd, EC20D803A, E22DAC12E));
        /// <summary>1002.0</summary>
        protected IExpression E84A306CF => _84a306cf ?? (_84a306cf = new ConstantExpression(new DecimalNumber.Text("1002.0")));
        /// <summary>6</summary>
        protected IExpression E84C18E51 => _84c18e51 ?? (_84c18e51 = new ConstantExpression(new DecimalNumber.Long(6)));
        /// <summary>i % 100</summary>
        protected IExpression E84E93E2B => _84e93e2b ?? (_84e93e2b = new BinaryOpExpression(BinaryOp.Modulo, EBCBACB59, EE7AD2EBA));
        /// <summary>1,2</summary>
        protected IExpression E85D169AA => _85d169aa ?? (_85d169aa = new GroupExpression(ED6BF1D5A, E7C9B4215));
        /// <summary>2.2..2.4</summary>
        protected IExpression E85F92CD1 => _85f92cd1 ?? (_85f92cd1 = new RangeExpression(E88F46CC8, E8EF4763A));
        /// <summary>0..15</summary>
        protected IExpression E86AAD6FF => _86aad6ff ?? (_86aad6ff = new RangeExpression(EC8E67366, EA1C9DD24));
        /// <summary>n=1,5,7..9</summary>
        protected IExpression E8730DC87 => _8730dc87 ?? (_8730dc87 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E1CF15395));
        /// <summary>3..6</summary>
        protected IExpression E8803BD20 => _8803bd20 ?? (_8803bd20 = new RangeExpression(EC7E671D3, ECAE6768C));
        /// <summary>10..24</summary>
        protected IExpression E88BD1628 => _88bd1628 ?? (_88bd1628 = new RangeExpression(E9EC9D86B, E90C3069C));
        /// <summary>4.00</summary>
        protected IExpression E88DD4214 => _88dd4214 ?? (_88dd4214 = new ConstantExpression(new DecimalNumber.Text("4.00")));
        /// <summary>2.2</summary>
        protected IExpression E88F46CC8 => _88f46cc8 ?? (_88f46cc8 = new ConstantExpression(new DecimalNumber.Text("2.2")));
        /// <summary>81</summary>
        protected IExpression E894EFD50 => _894efd50 ?? (_894efd50 = new ConstantExpression(new DecimalNumber.Long(81)));
        /// <summary>10..25</summary>
        protected IExpression E89BD17BB => _89bd17bb ?? (_89bd17bb = new RangeExpression(E9EC9D86B, E91C3082F));
        /// <summary>2.3</summary>
        protected IExpression E89F46E5B => _89f46e5b ?? (_89f46e5b = new ConstantExpression(new DecimalNumber.Text("2.3")));
        /// <summary>505</summary>
        protected IExpression E8A4843EE => _8a4843ee ?? (_8a4843ee = new ConstantExpression(new DecimalNumber.Text("505")));
        /// <summary>i=0,1 and n!=0</summary>
        protected IExpression E8ADA7F20 => _8ada7f20 ?? (_8ada7f20 = new BinaryOpExpression(BinaryOp.LogicalAnd, EE5C8BDA7, EC1B3E81C));
        /// <summary>2.0</summary>
        protected IExpression E8AF46FEE => _8af46fee ?? (_8af46fee = new ConstantExpression(new DecimalNumber.Text("2.0")));
        /// <summary>111.0</summary>
        protected IExpression E8AFD045D => _8afd045d ?? (_8afd045d = new ConstantExpression(new DecimalNumber.Text("111.0")));
        /// <summary>20..22</summary>
        protected IExpression E8B1ACF7F => _8b1acf7f ?? (_8b1acf7f = new RangeExpression(E8CC30050, E8EC30376));
        /// <summary>7..20</summary>
        protected IExpression E8B1ED530 => _8b1ed530 ?? (_8b1ed530 = new RangeExpression(ECBE6781F, E8CC30050));
        /// <summary>n % 100!=10..19,70..79,90..99</summary>
        protected IExpression E8B42049B => _8b42049b ?? (_8b42049b = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E6CFB81E6));
        /// <summary>504</summary>
        protected IExpression E8B484581 => _8b484581 ?? (_8b484581 = new ConstantExpression(new DecimalNumber.Text("504")));
        /// <summary>71.0</summary>
        protected IExpression E8B837F74 => _8b837f74 ?? (_8b837f74 = new ConstantExpression(new DecimalNumber.Text("71.0")));
        /// <summary>82.0</summary>
        protected IExpression E8BEA5A8A => _8bea5a8a ?? (_8bea5a8a = new ConstantExpression(new DecimalNumber.Text("82.0")));
        /// <summary>2.1</summary>
        protected IExpression E8BF47181 => _8bf47181 ?? (_8bf47181 = new ConstantExpression(new DecimalNumber.Text("2.1")));
        /// <summary>503</summary>
        protected IExpression E8C484714 => _8c484714 ?? (_8c484714 = new ConstantExpression(new DecimalNumber.Text("503")));
        /// <summary>10..20</summary>
        protected IExpression E8CBD1C74 => _8cbd1c74 ?? (_8cbd1c74 = new RangeExpression(E9EC9D86B, E8CC30050));
        /// <summary>20</summary>
        protected IExpression E8CC30050 => _8cc30050 ?? (_8cc30050 = new ConstantExpression(new DecimalNumber.Text("20")));
        /// <summary>2.6</summary>
        protected IExpression E8CF47314 => _8cf47314 ?? (_8cf47314 = new ConstantExpression(new DecimalNumber.Text("2.6")));
        /// <summary>v=0 and i % 100=0,20,40,60,80</summary>
        protected IExpression E8CF84894 => _8cf84894 ?? (_8cf84894 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EE82B0586));
        /// <summary>502</summary>
        protected IExpression E8D4848A7 => _8d4848a7 ?? (_8d4848a7 = new ConstantExpression(new DecimalNumber.Text("502")));
        /// <summary>22</summary>
        protected IExpression E8D95D195 => _8d95d195 ?? (_8d95d195 = new ConstantExpression(new DecimalNumber.Long(22)));
        /// <summary>21</summary>
        protected IExpression E8DC301E3 => _8dc301e3 ?? (_8dc301e3 = new ConstantExpression(new DecimalNumber.Text("21")));
        /// <summary>11</summary>
        protected IExpression E8DF33FDF => _8df33fdf ?? (_8df33fdf = new ConstantExpression(new DecimalNumber.Long(11)));
        /// <summary>2.7</summary>
        protected IExpression E8DF474A7 => _8df474a7 ?? (_8df474a7 = new ConstantExpression(new DecimalNumber.Text("2.7")));
        /// <summary>501</summary>
        protected IExpression E8E484A3A => _8e484a3a ?? (_8e484a3a = new ConstantExpression(new DecimalNumber.Text("501")));
        /// <summary>22</summary>
        protected IExpression E8EC30376 => _8ec30376 ?? (_8ec30376 = new ConstantExpression(new DecimalNumber.Text("22")));
        /// <summary>2.4</summary>
        protected IExpression E8EF4763A => _8ef4763a ?? (_8ef4763a = new ConstantExpression(new DecimalNumber.Text("2.4")));
        /// <summary>7,8</summary>
        protected IExpression E8F44C9D2 => _8f44c9d2 ?? (_8f44c9d2 = new GroupExpression(E7AC0D2FC, E98A8B99F));
        /// <summary>500</summary>
        protected IExpression E8F484BCD => _8f484bcd ?? (_8f484bcd = new ConstantExpression(new DecimalNumber.Text("500")));
        /// <summary>23</summary>
        protected IExpression E8FC30509 => _8fc30509 ?? (_8fc30509 = new ConstantExpression(new DecimalNumber.Text("23")));
        /// <summary>33</summary>
        protected IExpression E8FC543A0 => _8fc543a0 ?? (_8fc543a0 = new ConstantExpression(new DecimalNumber.Text("33")));
        /// <summary>2.5</summary>
        protected IExpression E8FF477CD => _8ff477cd ?? (_8ff477cd = new ConstantExpression(new DecimalNumber.Text("2.5")));
        /// <summary>71</summary>
        protected IExpression E9016A675 => _9016a675 ?? (_9016a675 = new ConstantExpression(new DecimalNumber.Long(71)));
        /// <summary>24</summary>
        protected IExpression E90C3069C => _90c3069c ?? (_90c3069c = new ConstantExpression(new DecimalNumber.Text("24")));
        /// <summary>32</summary>
        protected IExpression E90C54533 => _90c54533 ?? (_90c54533 = new ConstantExpression(new DecimalNumber.Text("32")));
        /// <summary>2,3</summary>
        protected IExpression E911AF69C => _911af69c ?? (_911af69c = new GroupExpression(E7C9B4215, E72B6DCC0));
        /// <summary>51.0</summary>
        protected IExpression E91468072 => _91468072 ?? (_91468072 = new ConstantExpression(new DecimalNumber.Text("51.0")));
        /// <summary>10.000</summary>
        protected IExpression E91704D05 => _91704d05 ?? (_91704d05 = new ConstantExpression(new DecimalNumber.Text("10.000")));
        /// <summary>81..84</summary>
        protected IExpression E918569FE => _918569fe ?? (_918569fe = new RangeExpression(E894EFD50, EEB3A32E1));
        /// <summary>25</summary>
        protected IExpression E91C3082F => _91c3082f ?? (_91c3082f = new ConstantExpression(new DecimalNumber.Text("25")));
        /// <summary>31</summary>
        protected IExpression E91C546C6 => _91c546c6 ?? (_91c546c6 = new ConstantExpression(new DecimalNumber.Text("31")));
        /// <summary>3.00</summary>
        protected IExpression E91FC58FF => _91fc58ff ?? (_91fc58ff = new ConstantExpression(new DecimalNumber.Text("3.00")));
        /// <summary>i=0 or i % 10=6 or i % 100=40,60,90</summary>
        protected IExpression E91FCEE49 => _91fcee49 ?? (_91fcee49 = new BinaryOpExpression(BinaryOp.LogicalOr, E49A7BFB8, ECC6E7E03));
        /// <summary>n % 100!=11,71,91</summary>
        protected IExpression E92108A80 => _92108a80 ?? (_92108a80 = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E4B0CF081));
        /// <summary>n % 100!=12,13</summary>
        protected IExpression E92120075 => _92120075 ?? (_92120075 = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E03C3A4B8));
        /// <summary>122.0</summary>
        protected IExpression E9272C33F => _9272c33f ?? (_9272c33f = new ConstantExpression(new DecimalNumber.Text("122.0")));
        /// <summary>26</summary>
        protected IExpression E92C309C2 => _92c309c2 ?? (_92c309c2 = new ConstantExpression(new DecimalNumber.Text("26")));
        /// <summary>30</summary>
        protected IExpression E92C54859 => _92c54859 ?? (_92c54859 = new ConstantExpression(new DecimalNumber.Text("30")));
        /// <summary>54</summary>
        protected IExpression E92D452E3 => _92d452e3 ?? (_92d452e3 = new ConstantExpression(new DecimalNumber.Text("54")));
        /// <summary>n % 10=2..4</summary>
        protected IExpression E92FA4C69 => _92fa4c69 ?? (_92fa4c69 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, ECD654849));
        /// <summary>n % 100=11..19</summary>
        protected IExpression E9338945F => _9338945f ?? (_9338945f = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, EA77C4BB7));
        /// <summary>80</summary>
        protected IExpression E934C92A5 => _934c92a5 ?? (_934c92a5 = new ConstantExpression(new DecimalNumber.Long(80)));
        /// <summary>27</summary>
        protected IExpression E93C30B55 => _93c30b55 ?? (_93c30b55 = new ConstantExpression(new DecimalNumber.Text("27")));
        /// <summary>37</summary>
        protected IExpression E93C549EC => _93c549ec ?? (_93c549ec = new ConstantExpression(new DecimalNumber.Text("37")));
        /// <summary>57</summary>
        protected IExpression E93D45476 => _93d45476 ?? (_93d45476 = new ConstantExpression(new DecimalNumber.Text("57")));
        /// <summary>12</summary>
        protected IExpression E93D79EE8 => _93d79ee8 ?? (_93d79ee8 = new ConstantExpression(new DecimalNumber.Long(12)));
        /// <summary>28</summary>
        protected IExpression E94C30CE8 => _94c30ce8 ?? (_94c30ce8 = new ConstantExpression(new DecimalNumber.Text("28")));
        /// <summary>36</summary>
        protected IExpression E94C54B7F => _94c54b7f ?? (_94c54b7f = new ConstantExpression(new DecimalNumber.Text("36")));
        /// <summary>2.0000</summary>
        protected IExpression E94D1C0CA => _94d1c0ca ?? (_94d1c0ca = new ConstantExpression(new DecimalNumber.Text("2.0000")));
        /// <summary>56</summary>
        protected IExpression E94D45609 => _94d45609 ?? (_94d45609 = new ConstantExpression(new DecimalNumber.Text("56")));
        /// <summary>f % 10=2..4</summary>
        protected IExpression E951A8CA1 => _951a8ca1 ?? (_951a8ca1 = new BinaryOpExpression(BinaryOp.Equal, ECAB3E756, ECD654849));
        /// <summary>15.0</summary>
        protected IExpression E9536218A => _9536218a ?? (_9536218a = new ConstantExpression(new DecimalNumber.Text("15.0")));
        /// <summary>142.0</summary>
        protected IExpression E9565B229 => _9565b229 ?? (_9565b229 = new ConstantExpression(new DecimalNumber.Text("142.0")));
        /// <summary>i=2 and v=0</summary>
        protected IExpression E95A4B850 => _95a4b850 ?? (_95a4b850 = new BinaryOpExpression(BinaryOp.LogicalAnd, EED63B9B6, E5BCFC261));
        /// <summary>29</summary>
        protected IExpression E95C30E7B => _95c30e7b ?? (_95c30e7b = new ConstantExpression(new DecimalNumber.Text("29")));
        /// <summary>35</summary>
        protected IExpression E95C54D12 => _95c54d12 ?? (_95c54d12 = new ConstantExpression(new DecimalNumber.Text("35")));
        /// <summary>19</summary>
        protected IExpression E95C9CA40 => _95c9ca40 ?? (_95c9ca40 = new ConstantExpression(new DecimalNumber.Text("19")));
        /// <summary>51</summary>
        protected IExpression E95D4579C => _95d4579c ?? (_95d4579c = new ConstantExpression(new DecimalNumber.Text("51")));
        /// <summary>9</summary>
        protected IExpression E96A80AE2 => _96a80ae2 ?? (_96a80ae2 = new ConstantExpression(new DecimalNumber.Long(9)));
        /// <summary>11..99</summary>
        protected IExpression E96BDB99F => _96bdb99f ?? (_96bdb99f = new RangeExpression(E8DF33FDF, E980FD23F));
        /// <summary>34</summary>
        protected IExpression E96C54EA5 => _96c54ea5 ?? (_96c54ea5 = new ConstantExpression(new DecimalNumber.Text("34")));
        /// <summary>18</summary>
        protected IExpression E96C9CBD3 => _96c9cbd3 ?? (_96c9cbd3 = new ConstantExpression(new DecimalNumber.Text("18")));
        /// <summary>50</summary>
        protected IExpression E96D4592F => _96d4592f ?? (_96d4592f = new ConstantExpression(new DecimalNumber.Text("50")));
        /// <summary>i % 1000</summary>
        protected IExpression E970D5EB3 => _970d5eb3 ?? (_970d5eb3 = new BinaryOpExpression(BinaryOp.Modulo, EBCBACB59, EFC934342));
        /// <summary>2..19</summary>
        protected IExpression E97B930AD => _97b930ad ?? (_97b930ad = new RangeExpression(E7C9B4215, ECDEDFC57));
        /// <summary>53</summary>
        protected IExpression E97D45AC2 => _97d45ac2 ?? (_97d45ac2 = new ConstantExpression(new DecimalNumber.Text("53")));
        /// <summary>99</summary>
        protected IExpression E980FD23F => _980fd23f ?? (_980fd23f = new ConstantExpression(new DecimalNumber.Long(99)));
        /// <summary>900</summary>
        protected IExpression E984A4D02 => _984a4d02 ?? (_984a4d02 = new ConstantExpression(new DecimalNumber.Long(900)));
        /// <summary>4.000</summary>
        protected IExpression E984F1EAC => _984f1eac ?? (_984f1eac = new ConstantExpression(new DecimalNumber.Text("4.000")));
        /// <summary>8</summary>
        protected IExpression E98A8B99F => _98a8b99f ?? (_98a8b99f = new ConstantExpression(new DecimalNumber.Long(8)));
        /// <summary>52</summary>
        protected IExpression E98D45C55 => _98d45c55 ?? (_98d45c55 = new ConstantExpression(new DecimalNumber.Text("52")));
        /// <summary>i % 100=2</summary>
        protected IExpression E995B85E0 => _995b85e0 ?? (_995b85e0 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, E7C9B4215));
        /// <summary>39</summary>
        protected IExpression E99C5535E => _99c5535e ?? (_99c5535e = new ConstantExpression(new DecimalNumber.Text("39")));
        /// <summary>v=2 and f % 10=1 and f % 100!=11</summary>
        protected IExpression E9A283E93 => _9a283e93 ?? (_9a283e93 = new BinaryOpExpression(BinaryOp.LogicalAnd, EAD21FD82, EF42B1EC6));
        /// <summary>7.3</summary>
        protected IExpression E9A308830 => _9a308830 ?? (_9a308830 = new ConstantExpression(new DecimalNumber.Text("7.3")));
        /// <summary>24.0</summary>
        protected IExpression E9A924772 => _9a924772 ?? (_9a924772 = new ConstantExpression(new DecimalNumber.Text("24.0")));
        /// <summary>38</summary>
        protected IExpression E9AC554F1 => _9ac554f1 ?? (_9ac554f1 = new ConstantExpression(new DecimalNumber.Text("38")));
        /// <summary>7.2</summary>
        protected IExpression E9B3089C3 => _9b3089c3 ?? (_9b3089c3 = new ConstantExpression(new DecimalNumber.Text("7.2")));
        /// <summary>31.0</summary>
        protected IExpression E9B845998 => _9b845998 ?? (_9b845998 = new ConstantExpression(new DecimalNumber.Text("31.0")));
        /// <summary>7.1</summary>
        protected IExpression E9C308B56 => _9c308b56 ?? (_9c308b56 = new ConstantExpression(new DecimalNumber.Text("7.1")));
        /// <summary>2.00</summary>
        protected IExpression E9CCC1A7A => _9ccc1a7a ?? (_9ccc1a7a = new ConstantExpression(new DecimalNumber.Text("2.00")));
        /// <summary>1003.0</summary>
        protected IExpression E9D2807A8 => _9d2807a8 ?? (_9d2807a8 = new ConstantExpression(new DecimalNumber.Text("1003.0")));
        /// <summary>7.0</summary>
        protected IExpression E9D308CE9 => _9d308ce9 ?? (_9d308ce9 = new ConstantExpression(new DecimalNumber.Text("7.0")));
        /// <summary>11</summary>
        protected IExpression E9DC9D6D8 => _9dc9d6d8 ?? (_9dc9d6d8 = new ConstantExpression(new DecimalNumber.Text("11")));
        /// <summary>59</summary>
        protected IExpression E9DD46434 => _9dd46434 ?? (_9dd46434 = new ConstantExpression(new DecimalNumber.Text("59")));
        /// <summary>n % 10=4</summary>
        protected IExpression E9E05D853 => _9e05d853 ?? (_9e05d853 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, EC0AA13F3));
        /// <summary>n % 10=0</summary>
        protected IExpression E9E1154F7 => _9e1154f7 ?? (_9e1154f7 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, ED8BCA617));
        /// <summary>100..106</summary>
        protected IExpression E9E176829 => _9e176829 ?? (_9e176829 = new RangeExpression(E52BF9741, E54BF9A67));
        /// <summary>3.0000</summary>
        protected IExpression E9E712C17 => _9e712c17 ?? (_9e712c17 = new ConstantExpression(new DecimalNumber.Text("3.0000")));
        /// <summary>10</summary>
        protected IExpression E9EC9D86B => _9ec9d86b ?? (_9ec9d86b = new ConstantExpression(new DecimalNumber.Text("10")));
        /// <summary>58</summary>
        protected IExpression E9ED465C7 => _9ed465c7 ?? (_9ed465c7 = new ConstantExpression(new DecimalNumber.Text("58")));
        /// <summary>3.000</summary>
        protected IExpression E9F3FCDDD => _9f3fcddd ?? (_9f3fcddd = new ConstantExpression(new DecimalNumber.Text("3.000")));
        /// <summary>13</summary>
        protected IExpression E9FC9D9FE => _9fc9d9fe ?? (_9fc9d9fe = new ConstantExpression(new DecimalNumber.Text("13")));
        /// <summary>12</summary>
        protected IExpression EA0C9DB91 => _a0c9db91 ?? (_a0c9db91 = new ConstantExpression(new DecimalNumber.Text("12")));
        /// <summary>13..19</summary>
        protected IExpression EA0D9AEEB => _a0d9aeeb ?? (_a0d9aeeb = new RangeExpression(E9FC9D9FE, E95C9CA40));
        /// <summary>88</summary>
        protected IExpression EA0DB24B2 => _a0db24b2 ?? (_a0db24b2 = new ConstantExpression(new DecimalNumber.Text("88")));
        /// <summary>7.4</summary>
        protected IExpression EA1309335 => _a1309335 ?? (_a1309335 = new ConstantExpression(new DecimalNumber.Text("7.4")));
        /// <summary>15</summary>
        protected IExpression EA1C9DD24 => _a1c9dd24 ?? (_a1c9dd24 = new ConstantExpression(new DecimalNumber.Text("15")));
        /// <summary>100..102</summary>
        protected IExpression EA2176E75 => _a2176e75 ?? (_a2176e75 = new RangeExpression(E52BF9741, E50BF941B));
        /// <summary>0.0..0.9</summary>
        protected IExpression EA23B1BEC => _a23b1bec ?? (_a23b1bec = new RangeExpression(EE005B1B8, ED905A6B3));
        /// <summary>14</summary>
        protected IExpression EA2C9DEB7 => _a2c9deb7 ?? (_a2c9deb7 = new ConstantExpression(new DecimalNumber.Text("14")));
        /// <summary>8.00</summary>
        protected IExpression EA34F39D0 => _a34f39d0 ?? (_a34f39d0 = new ConstantExpression(new DecimalNumber.Text("8.00")));
        /// <summary>17</summary>
        protected IExpression EA3C9E04A => _a3c9e04a ?? (_a3c9e04a = new ConstantExpression(new DecimalNumber.Text("17")));
        /// <summary>87</summary>
        protected IExpression EA3DB296B => _a3db296b ?? (_a3db296b = new ConstantExpression(new DecimalNumber.Text("87")));
        /// <summary>2..16</summary>
        protected IExpression EA42EBC10 => _a42ebc10 ?? (_a42ebc10 = new RangeExpression(EC6E67040, EA4C9E1DD));
        /// <summary>0.0..1.0</summary>
        protected IExpression EA4AD7B30 => _a4ad7b30 ?? (_a4ad7b30 = new RangeExpression(EE005B1B8, E46E2169F));
        /// <summary>16</summary>
        protected IExpression EA4C9E1DD => _a4c9e1dd ?? (_a4c9e1dd = new ConstantExpression(new DecimalNumber.Text("16")));
        /// <summary>84</summary>
        protected IExpression EA4DB2AFE => _a4db2afe ?? (_a4db2afe = new ConstantExpression(new DecimalNumber.Text("84")));
        /// <summary>200</summary>
        protected IExpression EA501A4AD => _a501a4ad ?? (_a501a4ad = new ConstantExpression(new DecimalNumber.Long(200)));
        /// <summary>2..17</summary>
        protected IExpression EA52EBDA3 => _a52ebda3 ?? (_a52ebda3 = new RangeExpression(EC6E67040, EA3C9E04A));
        /// <summary>83</summary>
        protected IExpression EA537C1EA => _a537c1ea ?? (_a537c1ea = new ConstantExpression(new DecimalNumber.Long(83)));
        /// <summary>82</summary>
        protected IExpression EA6DB2E24 => _a6db2e24 ?? (_a6db2e24 = new ConstantExpression(new DecimalNumber.Text("82")));
        /// <summary>v=0 and i % 100=12..14</summary>
        protected IExpression EA71FCD3A => _a71fcd3a ?? (_a71fcd3a = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EC87535B4));
        /// <summary>i=2..4</summary>
        protected IExpression EA72E2716 => _a72e2716 ?? (_a72e2716 = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, ECD654849));
        /// <summary>82</summary>
        protected IExpression EA73870A7 => _a73870a7 ?? (_a73870a7 = new ConstantExpression(new DecimalNumber.Long(82)));
        /// <summary>11..19</summary>
        protected IExpression EA77C4BB7 => _a77c4bb7 ?? (_a77c4bb7 = new RangeExpression(E8DF33FDF, ECDEDFC57));
        /// <summary>83</summary>
        protected IExpression EA7DB2FB7 => _a7db2fb7 ?? (_a7db2fb7 = new ConstantExpression(new DecimalNumber.Text("83")));
        /// <summary>n % 10=3..4,9</summary>
        protected IExpression EA80FF1DB => _a80ff1db ?? (_a80ff1db = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E0D003EFB));
        /// <summary>22..29</summary>
        protected IExpression EA88DC95C => _a88dc95c ?? (_a88dc95c = new RangeExpression(E8EC30376, E95C30E7B));
        /// <summary>80</summary>
        protected IExpression EA8DB314A => _a8db314a ?? (_a8db314a = new ConstantExpression(new DecimalNumber.Text("80")));
        /// <summary>0.0..1.5</summary>
        protected IExpression EA9AD830F => _a9ad830f ?? (_a9ad830f = new RangeExpression(EE005B1B8, E41E20EC0));
        /// <summary>81</summary>
        protected IExpression EA9DB32DD => _a9db32dd ?? (_a9db32dd = new ConstantExpression(new DecimalNumber.Text("81")));
        /// <summary>2..10</summary>
        protected IExpression EAA2EC582 => _aa2ec582 ?? (_aa2ec582 = new RangeExpression(EC6E67040, E9EC9D86B));
        /// <summary>v=0 and i % 10=2..4</summary>
        protected IExpression EAA70F92A => _aa70f92a ?? (_aa70f92a = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EC3ADD6C4));
        /// <summary>n!=0..10</summary>
        protected IExpression EAA7D95F9 => _aa7d95f9 ?? (_aa7d95f9 = new BinaryOpExpression(BinaryOp.NotEqual, EBDBACCEC, ED6A8739A));
        /// <summary>41..44</summary>
        protected IExpression EAAB8D1EE => _aab8d1ee ?? (_aab8d1ee = new RangeExpression(E75DC4B6C, E77B6BA85));
        /// <summary>6.0000</summary>
        protected IExpression EAB377D66 => _ab377d66 ?? (_ab377d66 = new ConstantExpression(new DecimalNumber.Text("6.0000")));
        /// <summary>123.0</summary>
        protected IExpression EAB9594D8 => _ab9594d8 ?? (_ab9594d8 = new ConstantExpression(new DecimalNumber.Text("123.0")));
        /// <summary>23.0</summary>
        protected IExpression EABBF99CF => _abbf99cf ?? (_abbf99cf = new ConstantExpression(new DecimalNumber.Text("23.0")));
        /// <summary>n % 100=3,23,43,63,83</summary>
        protected IExpression EABEE7EEC => _abee7eec ?? (_abee7eec = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, E06589D34));
        /// <summary>0.0..0.3</summary>
        protected IExpression EAC3B2BAA => _ac3b2baa ?? (_ac3b2baa = new RangeExpression(EE005B1B8, EE305B671));
        /// <summary>9.0000</summary>
        protected IExpression EAC83CD3D => _ac83cd3d ?? (_ac83cd3d = new ConstantExpression(new DecimalNumber.Text("9.0000")));
        /// <summary>v=2 and f % 10=1</summary>
        protected IExpression EAD21FD82 => _ad21fd82 ?? (_ad21fd82 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5B8C4D33, E486AF022));
        /// <summary>22..24</summary>
        protected IExpression EAD8DD13B => _ad8dd13b ?? (_ad8dd13b = new RangeExpression(E8EC30376, E90C3069C));
        /// <summary>604</summary>
        protected IExpression EAE93F940 => _ae93f940 ?? (_ae93f940 = new ConstantExpression(new DecimalNumber.Text("604")));
        /// <summary>60000</summary>
        protected IExpression EAF1B43A1 => _af1b43a1 ?? (_af1b43a1 = new ConstantExpression(new DecimalNumber.Long(60000)));
        /// <summary>1000..20000,40000,60000,80000</summary>
        protected IExpression EAF780C38 => _af780c38 ?? (_af780c38 = new GroupExpression(ECDBBDE88, EB2B04063, EAF1B43A1, E7B2EB74F));
        /// <summary>605</summary>
        protected IExpression EAF93FAD3 => _af93fad3 ?? (_af93fad3 = new ConstantExpression(new DecimalNumber.Text("605")));
        /// <summary>5.000</summary>
        protected IExpression EAFF7A8CB => _aff7a8cb ?? (_aff7a8cb = new ConstantExpression(new DecimalNumber.Text("5.000")));
        /// <summary>600</summary>
        protected IExpression EB293FF8C => _b293ff8c ?? (_b293ff8c = new ConstantExpression(new DecimalNumber.Text("600")));
        /// <summary>40000</summary>
        protected IExpression EB2B04063 => _b2b04063 ?? (_b2b04063 = new ConstantExpression(new DecimalNumber.Long(40000)));
        /// <summary>601</summary>
        protected IExpression EB394011F => _b394011f ?? (_b394011f = new ConstantExpression(new DecimalNumber.Text("601")));
        /// <summary>1001</summary>
        protected IExpression EB39B6550 => _b39b6550 ?? (_b39b6550 = new ConstantExpression(new DecimalNumber.Text("1001")));
        /// <summary>100.4</summary>
        protected IExpression EB39DB0AB => _b39db0ab ?? (_b39db0ab = new ConstantExpression(new DecimalNumber.Text("100.4")));
        /// <summary>602</summary>
        protected IExpression EB49402B2 => _b49402b2 ?? (_b49402b2 = new ConstantExpression(new DecimalNumber.Text("602")));
        /// <summary>1000</summary>
        protected IExpression EB49B66E3 => _b49b66e3 ?? (_b49b66e3 = new ConstantExpression(new DecimalNumber.Text("1000")));
        /// <summary>603</summary>
        protected IExpression EB5940445 => _b5940445 ?? (_b5940445 = new ConstantExpression(new DecimalNumber.Text("603")));
        /// <summary>1003</summary>
        protected IExpression EB59B6876 => _b59b6876 ?? (_b59b6876 = new ConstantExpression(new DecimalNumber.Text("1003")));
        /// <summary>12..14</summary>
        protected IExpression EB647BEC9 => _b647bec9 ?? (_b647bec9 = new RangeExpression(E93D79EE8, E77E9A35E));
        /// <summary>1002</summary>
        protected IExpression EB69B6A09 => _b69b6a09 ?? (_b69b6a09 = new ConstantExpression(new DecimalNumber.Text("1002")));
        /// <summary>100.1</summary>
        protected IExpression EB69DB564 => _b69db564 ?? (_b69db564 = new ConstantExpression(new DecimalNumber.Text("100.1")));
        /// <summary>100000.0</summary>
        protected IExpression EB6B8E1CD => _b6b8e1cd ?? (_b6b8e1cd = new ConstantExpression(new DecimalNumber.Text("100000.0")));
        /// <summary>1005</summary>
        protected IExpression EB79B6B9C => _b79b6b9c ?? (_b79b6b9c = new ConstantExpression(new DecimalNumber.Text("1005")));
        /// <summary>100.0</summary>
        protected IExpression EB79DB6F7 => _b79db6f7 ?? (_b79db6f7 = new ConstantExpression(new DecimalNumber.Text("100.0")));
        /// <summary>6.00</summary>
        protected IExpression EB87B46B6 => _b87b46b6 ?? (_b87b46b6 = new ConstantExpression(new DecimalNumber.Text("6.00")));
        /// <summary>102..107</summary>
        protected IExpression EB87FA440 => _b87fa440 ?? (_b87fa440 = new RangeExpression(E50BF941B, E53BF98D4));
        /// <summary>v!=2 and f % 10=1</summary>
        protected IExpression EB887E6E3 => _b887e6e3 ?? (_b887e6e3 = new BinaryOpExpression(BinaryOp.LogicalAnd, EEDD26642, E486AF022));
        /// <summary>1004</summary>
        protected IExpression EB89B6D2F => _b89b6d2f ?? (_b89b6d2f = new ConstantExpression(new DecimalNumber.Text("1004")));
        /// <summary>100.3</summary>
        protected IExpression EB89DB88A => _b89db88a ?? (_b89db88a = new ConstantExpression(new DecimalNumber.Text("100.3")));
        /// <summary>n=1..4 or n % 100=1..4,21..24,41..44,61..64,81..84</summary>
        protected IExpression EB8D55BD0 => _b8d55bd0 ?? (_b8d55bd0 = new BinaryOpExpression(BinaryOp.LogicalOr, E4FE21A9C, E2549312E));
        /// <summary>1007</summary>
        protected IExpression EB99B6EC2 => _b99b6ec2 ?? (_b99b6ec2 = new ConstantExpression(new DecimalNumber.Text("1007")));
        /// <summary>100.2</summary>
        protected IExpression EB99DBA1D => _b99dba1d ?? (_b99dba1d = new ConstantExpression(new DecimalNumber.Text("100.2")));
        /// <summary>7..10</summary>
        protected IExpression EB9E2DDE1 => _b9e2dde1 ?? (_b9e2dde1 = new RangeExpression(E7AC0D2FC, E3FF8DD22));
        /// <summary>1006</summary>
        protected IExpression EBA9B7055 => _ba9b7055 ?? (_ba9b7055 = new ConstantExpression(new DecimalNumber.Text("1006")));
        /// <summary>0..3</summary>
        protected IExpression EBAEFEB76 => _baefeb76 ?? (_baefeb76 = new RangeExpression(EC8E67366, EC7E671D3));
        /// <summary>n!=0 and n % 1000000=100000</summary>
        protected IExpression EBB5C4B5D => _bb5c4b5d ?? (_bb5c4b5d = new BinaryOpExpression(BinaryOp.LogicalAnd, EC1B3E81C, E5E12E1FA));
        /// <summary>i % 1000=100,200,300,400,500,600,700,800,900</summary>
        protected IExpression EBBDB662B => _bbdb662b ?? (_bbdb662b = new BinaryOpExpression(BinaryOp.Equal, E970D5EB3, E404FB29E));
        /// <summary>0..2</summary>
        protected IExpression EBBEFED09 => _bbefed09 ?? (_bbefed09 = new RangeExpression(EC8E67366, EC6E67040));
        /// <summary>i=1,2,3</summary>
        protected IExpression EBC4F8F1C => _bc4f8f1c ?? (_bc4f8f1c = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, EE915A0DF));
        /// <summary>i</summary>
        protected IExpression EBCBACB59 => _bcbacb59 ?? (_bcbacb59 = new ArgumentNameExpression("i"));
        /// <summary>0..5</summary>
        protected IExpression EBCEFEE9C => _bcefee9c ?? (_bcefee9c = new RangeExpression(EC8E67366, ECDE67B45));
        /// <summary>t=0 and i % 10=1</summary>
        protected IExpression EBD021D9B => _bd021d9b ?? (_bd021d9b = new BinaryOpExpression(BinaryOp.LogicalAnd, E2B74B6BB, E0FC06C63));
        /// <summary>v=2 and f % 10=1 and f % 100!=11 or v!=2 and f % 10=1</summary>
        protected IExpression EBD43AE72 => _bd43ae72 ?? (_bd43ae72 = new BinaryOpExpression(BinaryOp.LogicalOr, E9A283E93, EB887E6E3));
        /// <summary>n=1</summary>
        protected IExpression EBD4F1E10 => _bd4f1e10 ?? (_bd4f1e10 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ED6BF1D5A));
        /// <summary>n</summary>
        protected IExpression EBDBACCEC => _bdbaccec ?? (_bdbaccec = new ArgumentNameExpression("n"));
        /// <summary>121</summary>
        protected IExpression EBDC4BCE0 => _bdc4bce0 ?? (_bdc4bce0 = new ConstantExpression(new DecimalNumber.Text("121")));
        /// <summary>1000000</summary>
        protected IExpression EBDD71B71 => _bdd71b71 ?? (_bdd71b71 = new ConstantExpression(new DecimalNumber.Text("1000000")));
        /// <summary>400</summary>
        protected IExpression EBE1B127B => _be1b127b ?? (_be1b127b = new ConstantExpression(new DecimalNumber.Long(400)));
        /// <summary>5</summary>
        protected IExpression EBEAFB136 => _beafb136 ?? (_beafb136 = new ConstantExpression(new DecimalNumber.Long(5)));
        /// <summary>120</summary>
        protected IExpression EBEC4BE73 => _bec4be73 ?? (_bec4be73 = new ConstantExpression(new DecimalNumber.Text("120")));
        /// <summary>0..7</summary>
        protected IExpression EBEEFF1C2 => _beeff1c2 ?? (_beeff1c2 = new RangeExpression(EC8E67366, ECBE6781F));
        /// <summary>n=2..10</summary>
        protected IExpression EBF96A952 => _bf96a952 ?? (_bf96a952 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EC9AAA8CC));
        /// <summary>123</summary>
        protected IExpression EBFC4C006 => _bfc4c006 ?? (_bfc4c006 = new ConstantExpression(new DecimalNumber.Text("123")));
        /// <summary>n % 1000000</summary>
        protected IExpression EC01D5C76 => _c01d5c76 ?? (_c01d5c76 = new BinaryOpExpression(BinaryOp.Modulo, EBDBACCEC, EEEA9287A));
        /// <summary>4</summary>
        protected IExpression EC0AA13F3 => _c0aa13f3 ?? (_c0aa13f3 = new ConstantExpression(new DecimalNumber.Long(4)));
        /// <summary>122</summary>
        protected IExpression EC0C4C199 => _c0c4c199 ?? (_c0c4c199 = new ConstantExpression(new DecimalNumber.Text("122")));
        /// <summary>2..4</summary>
        protected IExpression EC1002565 => _c1002565 ?? (_c1002565 = new RangeExpression(EC6E67040, ECCE679B2));
        /// <summary>n=3</summary>
        protected IExpression EC153A18A => _c153a18a ?? (_c153a18a = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E72B6DCC0));
        /// <summary>n=0 or n % 100=2..10</summary>
        protected IExpression EC15E4BFF => _c15e4bff ?? (_c15e4bff = new BinaryOpExpression(BinaryOp.LogicalOr, EC3516619, EE9809404));
        /// <summary>5..17</summary>
        protected IExpression EC1ACAAAC => _c1acaaac ?? (_c1acaaac = new RangeExpression(ECDE67B45, EA3C9E04A));
        /// <summary>n!=0</summary>
        protected IExpression EC1B3E81C => _c1b3e81c ?? (_c1b3e81c = new BinaryOpExpression(BinaryOp.NotEqual, EBDBACCEC, ED8BCA617));
        /// <summary>n % 10=1</summary>
        protected IExpression EC20D803A => _c20d803a ?? (_c20d803a = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, ED6BF1D5A));
        /// <summary>n % 10=9</summary>
        protected IExpression EC2554682 => _c2554682 ?? (_c2554682 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E96A80AE2));
        /// <summary>n=0</summary>
        protected IExpression EC3516619 => _c3516619 ?? (_c3516619 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ED8BCA617));
        /// <summary>300..302</summary>
        protected IExpression EC3A787A5 => _c3a787a5 ?? (_c3a787a5 = new RangeExpression(E7590FD4B, E77910071));
        /// <summary>i % 10=2..4</summary>
        protected IExpression EC3ADD6C4 => _c3add6c4 ?? (_c3add6c4 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, ECD654849));
        /// <summary>n % 10=1,2 and n % 100!=11,12</summary>
        protected IExpression EC3E62A5A => _c3e62a5a ?? (_c3e62a5a = new BinaryOpExpression(BinaryOp.LogicalAnd, E323EC34A, EF40E857B));
        /// <summary>2..9</summary>
        protected IExpression EC4002A1E => _c4002a1e ?? (_c4002a1e = new RangeExpression(EC6E67040, ED1E68191));
        /// <summary>22.0</summary>
        protected IExpression EC4449AA8 => _c4449aa8 ?? (_c4449aa8 = new ConstantExpression(new DecimalNumber.Text("22.0")));
        /// <summary>5..8</summary>
        protected IExpression EC4B11118 => _c4b11118 ?? (_c4b11118 = new RangeExpression(ECDE67B45, ED0E67FFE));
        /// <summary>20000</summary>
        protected IExpression EC535AD25 => _c535ad25 ?? (_c535ad25 = new ConstantExpression(new DecimalNumber.Long(20000)));
        /// <summary>33.0</summary>
        protected IExpression EC536ACCE => _c536acce ?? (_c536acce = new ConstantExpression(new DecimalNumber.Text("33.0")));
        /// <summary>f</summary>
        protected IExpression EC5BAD984 => _c5bad984 ?? (_c5bad984 = new ArgumentNameExpression("f"));
        /// <summary>2</summary>
        protected IExpression EC6E67040 => _c6e67040 ?? (_c6e67040 = new ConstantExpression(new DecimalNumber.Text("2")));
        /// <summary>5..19</summary>
        protected IExpression EC7ACB41E => _c7acb41e ?? (_c7acb41e = new RangeExpression(ECDE67B45, E95C9CA40));
        /// <summary>n!=1</summary>
        protected IExpression EC7BC7C25 => _c7bc7c25 ?? (_c7bc7c25 = new BinaryOpExpression(BinaryOp.NotEqual, EBDBACCEC, ED6BF1D5A));
        /// <summary>143</summary>
        protected IExpression EC7C949CC => _c7c949cc ?? (_c7c949cc = new ConstantExpression(new DecimalNumber.Text("143")));
        /// <summary>3</summary>
        protected IExpression EC7E671D3 => _c7e671d3 ?? (_c7e671d3 = new ConstantExpression(new DecimalNumber.Text("3")));
        /// <summary>i % 100=12..14</summary>
        protected IExpression EC87535B4 => _c87535b4 ?? (_c87535b4 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, EB647BEC9));
        /// <summary>142</summary>
        protected IExpression EC8C94B5F => _c8c94b5f ?? (_c8c94b5f = new ConstantExpression(new DecimalNumber.Text("142")));
        /// <summary>0</summary>
        protected IExpression EC8E67366 => _c8e67366 ?? (_c8e67366 = new ConstantExpression(new DecimalNumber.Text("0")));
        /// <summary>1.000</summary>
        protected IExpression EC8E86837 => _c8e86837 ?? (_c8e86837 = new ConstantExpression(new DecimalNumber.Text("1.000")));
        /// <summary>81.0</summary>
        protected IExpression EC915FF3B => _c915ff3b ?? (_c915ff3b = new ConstantExpression(new DecimalNumber.Text("81.0")));
        /// <summary>0,7,8,9</summary>
        protected IExpression EC988B523 => _c988b523 ?? (_c988b523 = new GroupExpression(ED8BCA617, E7AC0D2FC, E98A8B99F, E96A80AE2));
        /// <summary>n % 100!=12..14</summary>
        protected IExpression EC99DFE4C => _c99dfe4c ?? (_c99dfe4c = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, EB647BEC9));
        /// <summary>2..10</summary>
        protected IExpression EC9AAA8CC => _c9aaa8cc ?? (_c9aaa8cc = new RangeExpression(E7C9B4215, E3FF8DD22));
        /// <summary>141</summary>
        protected IExpression EC9C94CF2 => _c9c94cf2 ?? (_c9c94cf2 = new ConstantExpression(new DecimalNumber.Text("141")));
        /// <summary>1</summary>
        protected IExpression EC9E674F9 => _c9e674f9 ?? (_c9e674f9 = new ConstantExpression(new DecimalNumber.Text("1")));
        /// <summary>f=1</summary>
        protected IExpression ECA854908 => _ca854908 ?? (_ca854908 = new BinaryOpExpression(BinaryOp.Equal, EC5BAD984, ED6BF1D5A));
        /// <summary>f % 10</summary>
        protected IExpression ECAB3E756 => _cab3e756 ?? (_cab3e756 = new BinaryOpExpression(BinaryOp.Modulo, EC5BAD984, E3FF8DD22));
        /// <summary>v=0 and i % 10=1 and i % 100!=11</summary>
        protected IExpression ECAB49249 => _cab49249 ?? (_cab49249 = new BinaryOpExpression(BinaryOp.LogicalAnd, E3E805D25, ED77C08CF));
        /// <summary>140</summary>
        protected IExpression ECAC94E85 => _cac94e85 ?? (_cac94e85 = new ConstantExpression(new DecimalNumber.Text("140")));
        /// <summary>6</summary>
        protected IExpression ECAE6768C => _cae6768c ?? (_cae6768c = new ConstantExpression(new DecimalNumber.Text("6")));
        /// <summary>n=2,12</summary>
        protected IExpression ECB109DEE => _cb109dee ?? (_cb109dee = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E76BF91A4));
        /// <summary>11,8,80,800</summary>
        protected IExpression ECBD249FB => _cbd249fb ?? (_cbd249fb = new GroupExpression(E8DF33FDF, E98A8B99F, E934C92A5, E63AEFA47));
        /// <summary>7</summary>
        protected IExpression ECBE6781F => _cbe6781f ?? (_cbe6781f = new ConstantExpression(new DecimalNumber.Text("7")));
        /// <summary>n % 10=2 and n % 100!=12,72,92</summary>
        protected IExpression ECC2A9E1D => _cc2a9e1d ?? (_cc2a9e1d = new BinaryOpExpression(BinaryOp.LogicalAnd, E0E48E275, E2CB43C67));
        /// <summary>i % 10=6 or i % 100=40,60,90</summary>
        protected IExpression ECC6E7E03 => _cc6e7e03 ?? (_cc6e7e03 = new BinaryOpExpression(BinaryOp.LogicalOr, E81BC9ACC, EE41128B9));
        /// <summary>v=0 and i=1,2,3</summary>
        protected IExpression ECC93277E => _cc93277e ?? (_cc93277e = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EBC4F8F1C));
        /// <summary>4</summary>
        protected IExpression ECCE679B2 => _cce679b2 ?? (_cce679b2 = new ConstantExpression(new DecimalNumber.Text("4")));
        /// <summary>n % 10=3 and n % 100!=13</summary>
        protected IExpression ECD38005F => _cd38005f ?? (_cd38005f = new BinaryOpExpression(BinaryOp.LogicalAnd, EEA123C60, E52ADA190));
        /// <summary>f % 100=1</summary>
        protected IExpression ECD396A5A => _cd396a5a ?? (_cd396a5a = new BinaryOpExpression(BinaryOp.Equal, E779038EE, ED6BF1D5A));
        /// <summary>2..4</summary>
        protected IExpression ECD654849 => _cd654849 ?? (_cd654849 = new RangeExpression(E7C9B4215, EC0AA13F3));
        /// <summary>1000..20000</summary>
        protected IExpression ECDBBDE88 => _cdbbde88 ?? (_cdbbde88 = new RangeExpression(EFC934342, EC535AD25));
        /// <summary>5</summary>
        protected IExpression ECDE67B45 => _cde67b45 ?? (_cde67b45 = new ConstantExpression(new DecimalNumber.Text("5")));
        /// <summary>19</summary>
        protected IExpression ECDEDFC57 => _cdedfc57 ?? (_cdedfc57 = new ConstantExpression(new DecimalNumber.Long(19)));
        /// <summary>52.0</summary>
        protected IExpression ECE227343 => _ce227343 ?? (_ce227343 = new ConstantExpression(new DecimalNumber.Text("52.0")));
        /// <summary>1.2..1.7</summary>
        protected IExpression ECE48CF20 => _ce48cf20 ?? (_ce48cf20 = new RangeExpression(E48E219C5, E43E211E6));
        /// <summary>i % 100=1</summary>
        protected IExpression ECF98944B => _cf98944b ?? (_cf98944b = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, ED6BF1D5A));
        /// <summary>42..44</summary>
        protected IExpression ECFC1F627 => _cfc1f627 ?? (_cfc1f627 = new RangeExpression(E0ED14480, E14D14DF2));
        /// <summary>8</summary>
        protected IExpression ED0E67FFE => _d0e67ffe ?? (_d0e67ffe = new ConstantExpression(new DecimalNumber.Text("8")));
        /// <summary>n=0..1 or n=11..99</summary>
        protected IExpression ED1051BB7 => _d1051bb7 ?? (_d1051bb7 = new BinaryOpExpression(BinaryOp.LogicalOr, E3987DFFC, E033F3325));
        /// <summary>n % 10=1 and n % 100!=11..19</summary>
        protected IExpression ED141DA67 => _d141da67 ?? (_d141da67 = new BinaryOpExpression(BinaryOp.LogicalAnd, EC20D803A, E5C674AF6));
        /// <summary>1.2..1.4</summary>
        protected IExpression ED148D3D9 => _d148d3d9 ?? (_d148d3d9 = new RangeExpression(E48E219C5, E42E21053));
        /// <summary>11..14</summary>
        protected IExpression ED1809B3E => _d1809b3e ?? (_d1809b3e = new RangeExpression(E8DF33FDF, E77E9A35E));
        /// <summary>24</summary>
        protected IExpression ED1884D73 => _d1884d73 ?? (_d1884d73 = new ConstantExpression(new DecimalNumber.Long(24)));
        /// <summary>9</summary>
        protected IExpression ED1E68191 => _d1e68191 ?? (_d1e68191 = new ConstantExpression(new DecimalNumber.Text("9")));
        /// <summary>16.0</summary>
        protected IExpression ED261C63B => _d261c63b ?? (_d261c63b = new ConstantExpression(new DecimalNumber.Text("16.0")));
        /// <summary>t=0 and i % 10=1 and i % 100!=11 or t!=0</summary>
        protected IExpression ED296C19F => _d296c19f ?? (_d296c19f = new BinaryOpExpression(BinaryOp.LogicalOr, EFB3F0513, E7DF0645A));
        /// <summary>3..6</summary>
        protected IExpression ED2D743DE => _d2d743de ?? (_d2d743de = new RangeExpression(E72B6DCC0, E84C18E51));
        /// <summary>0..1</summary>
        protected IExpression ED2F330E2 => _d2f330e2 ?? (_d2f330e2 = new RangeExpression(ED8BCA617, ED6BF1D5A));
        /// <summary>i % 100=11..14</summary>
        protected IExpression ED42D8187 => _d42d8187 ?? (_d42d8187 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, ED1809B3E));
        /// <summary>1,5,7,8,9,10</summary>
        protected IExpression ED4488666 => _d4488666 ?? (_d4488666 = new GroupExpression(ED6BF1D5A, EBEAFB136, E7AC0D2FC, E98A8B99F, E96A80AE2, E3FF8DD22));
        /// <summary>1..4,21..24,41..44,61..64,81..84</summary>
        protected IExpression ED4E966F6 => _d4e966f6 ?? (_d4e966f6 = new GroupExpression(E734BD952, E45D73A52, EAAB8D1EE, E4937FFBA, E918569FE));
        /// <summary>121.0</summary>
        protected IExpression ED547E80E => _d547e80e ?? (_d547e80e = new ConstantExpression(new DecimalNumber.Text("121.0")));
        /// <summary>v</summary>
        protected IExpression ED5BAF2B4 => _d5baf2b4 ?? (_d5baf2b4 = new ArgumentNameExpression("v"));
        /// <summary>n % 10=3..4,9 and n % 100!=10..19,70..79,90..99</summary>
        protected IExpression ED6218FB7 => _d6218fb7 ?? (_d6218fb7 = new BinaryOpExpression(BinaryOp.LogicalAnd, EA80FF1DB, E8B42049B));
        /// <summary>n % 10=6,9</summary>
        protected IExpression ED689B45A => _d689b45a ?? (_d689b45a = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E262A66BA));
        /// <summary>0..10</summary>
        protected IExpression ED6A8739A => _d6a8739a ?? (_d6a8739a = new RangeExpression(ED8BCA617, E3FF8DD22));
        /// <summary>1</summary>
        protected IExpression ED6BF1D5A => _d6bf1d5a ?? (_d6bf1d5a = new ConstantExpression(new DecimalNumber.Long(1)));
        /// <summary>i % 100!=11</summary>
        protected IExpression ED77C08CF => _d77c08cf ?? (_d77c08cf = new BinaryOpExpression(BinaryOp.NotEqual, E84E93E2B, E8DF33FDF));
        /// <summary>3,13</summary>
        protected IExpression ED7AF0040 => _d7af0040 ?? (_d7af0040 = new GroupExpression(E72B6DCC0, E51D5343D));
        /// <summary>t</summary>
        protected IExpression ED7BAF5DA => _d7baf5da ?? (_d7baf5da = new ArgumentNameExpression("t"));
        /// <summary>41.0</summary>
        protected IExpression ED7D9A4DF => _d7d9a4df ?? (_d7d9a4df = new ConstantExpression(new DecimalNumber.Text("41.0")));
        /// <summary>3.2..3.4</summary>
        protected IExpression ED7DCAE69 => _d7dcae69 ?? (_d7dcae69 = new RangeExpression(E6FD19B2F, E6DD19809));
        /// <summary>0.8</summary>
        protected IExpression ED805A520 => _d805a520 ?? (_d805a520 = new ConstantExpression(new DecimalNumber.Text("0.8")));
        /// <summary>0</summary>
        protected IExpression ED8BCA617 => _d8bca617 ?? (_d8bca617 = new ConstantExpression(new DecimalNumber.Long(0)));
        /// <summary>0.9</summary>
        protected IExpression ED905A6B3 => _d905a6b3 ?? (_d905a6b3 = new ConstantExpression(new DecimalNumber.Text("0.9")));
        /// <summary>n=0,1 or i=0 and f=1</summary>
        protected IExpression EDA9A4B53 => _da9a4b53 ?? (_da9a4b53 = new BinaryOpExpression(BinaryOp.LogicalOr, E23E9AF52, E82F74277));
        /// <summary>103.0</summary>
        protected IExpression EDAC25B46 => _dac25b46 ?? (_dac25b46 = new ConstantExpression(new DecimalNumber.Text("103.0")));
        /// <summary>n % 100=5</summary>
        protected IExpression EDB39458E => _db39458e ?? (_db39458e = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, EBEAFB136));
        /// <summary>n=0 or n % 100=2..19</summary>
        protected IExpression EDB46A3FA => _db46a3fa ?? (_db46a3fa = new BinaryOpExpression(BinaryOp.LogicalOr, EC3516619, E5D6BFBF5));
        /// <summary>i % 10!=4,6,9</summary>
        protected IExpression EDB4C9660 => _db4c9660 ?? (_db4c9660 = new BinaryOpExpression(BinaryOp.NotEqual, E5E3CEC93, EF6ED1FC8));
        /// <summary>10.00</summary>
        protected IExpression EDB7FF637 => _db7ff637 ?? (_db7ff637 = new ConstantExpression(new DecimalNumber.Text("10.00")));
        /// <summary>n!=1 and n % 100=1,21,41,61,81</summary>
        protected IExpression EDC252060 => _dc252060 ?? (_dc252060 = new BinaryOpExpression(BinaryOp.LogicalAnd, EC7BC7C25, EFC942D8E));
        /// <summary>1.2..1.9</summary>
        protected IExpression EDC48E52A => _dc48e52a ?? (_dc48e52a = new RangeExpression(E48E219C5, E4DE221A4));
        /// <summary>i % 10=5..9</summary>
        protected IExpression EDC7E3086 => _dc7e3086 ?? (_dc7e3086 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, E2D16DECF));
        /// <summary>n % 10=0 or n % 10=5..9 or n % 100=11..14</summary>
        protected IExpression EDD8F92EE => _dd8f92ee ?? (_dd8f92ee = new BinaryOpExpression(BinaryOp.LogicalOr, E9E1154F7, EE74F39B3));
        /// <summary>10..19</summary>
        protected IExpression EDF429CF6 => _df429cf6 ?? (_df429cf6 = new RangeExpression(E3FF8DD22, ECDEDFC57));
        /// <summary>20,50,70,80</summary>
        protected IExpression EDF6A33C5 => _df6a33c5 ?? (_df6a33c5 = new GroupExpression(EE997B997, EECAA6DB6, E0A191120, E934C92A5));
        /// <summary>1,11</summary>
        protected IExpression EDF7A1DD4 => _df7a1dd4 ?? (_df7a1dd4 = new GroupExpression(ED6BF1D5A, E8DF33FDF));
        /// <summary>0.0</summary>
        protected IExpression EE005B1B8 => _e005b1b8 ?? (_e005b1b8 = new ConstantExpression(new DecimalNumber.Text("0.0")));
        /// <summary>1000000.0</summary>
        protected IExpression EE103CB87 => _e103cb87 ?? (_e103cb87 = new ConstantExpression(new DecimalNumber.Text("1000000.0")));
        /// <summary>0.1</summary>
        protected IExpression EE105B34B => _e105b34b ?? (_e105b34b = new ConstantExpression(new DecimalNumber.Text("0.1")));
        /// <summary>f % 100=11..19</summary>
        protected IExpression EE14996B7 => _e14996b7 ?? (_e14996b7 = new BinaryOpExpression(BinaryOp.Equal, E779038EE, EA77C4BB7));
        /// <summary>f % 100!=12..14</summary>
        protected IExpression EE18D12A4 => _e18d12a4 ?? (_e18d12a4 = new BinaryOpExpression(BinaryOp.NotEqual, E779038EE, EB647BEC9));
        /// <summary>0.2</summary>
        protected IExpression EE205B4DE => _e205b4de ?? (_e205b4de = new ConstantExpression(new DecimalNumber.Text("0.2")));
        /// <summary>n=10</summary>
        protected IExpression EE214F988 => _e214f988 ?? (_e214f988 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E3FF8DD22));
        /// <summary>n % 100=2,22,42,62,82 or n % 1000=0 and n % 100000=1000..20000,40000,60000,80000 or n!=0 and n % 1000000=100000</summary>
        protected IExpression EE2596300 => _e2596300 ?? (_e2596300 = new BinaryOpExpression(BinaryOp.LogicalOr, E201C05C9, E24919EFF));
        /// <summary>600</summary>
        protected IExpression EE2927129 => _e2927129 ?? (_e2927129 = new ConstantExpression(new DecimalNumber.Long(600)));
        /// <summary>17,18</summary>
        protected IExpression EE2DE4C1A => _e2de4c1a ?? (_e2de4c1a = new GroupExpression(E39FB4E19, E7FF0739A));
        /// <summary>0.3</summary>
        protected IExpression EE305B671 => _e305b671 ?? (_e305b671 = new ConstantExpression(new DecimalNumber.Text("0.3")));
        /// <summary>0.4</summary>
        protected IExpression EE405B804 => _e405b804 ?? (_e405b804 = new ConstantExpression(new DecimalNumber.Text("0.4")));
        /// <summary>i % 100=40,60,90</summary>
        protected IExpression EE41128B9 => _e41128b9 ?? (_e41128b9 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, E65358758));
        /// <summary>0.5</summary>
        protected IExpression EE505B997 => _e505b997 ?? (_e505b997 = new ConstantExpression(new DecimalNumber.Text("0.5")));
        /// <summary>i=0,1</summary>
        protected IExpression EE5C8BDA7 => _e5c8bda7 ?? (_e5c8bda7 = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, EFBCA1EB0));
        /// <summary>n % 1000=0</summary>
        protected IExpression EE5F5EB57 => _e5f5eb57 ?? (_e5f5eb57 = new BinaryOpExpression(BinaryOp.Equal, E32301BAE, ED8BCA617));
        /// <summary>0.6</summary>
        protected IExpression EE605BB2A => _e605bb2a ?? (_e605bb2a = new ConstantExpression(new DecimalNumber.Text("0.6")));
        /// <summary>n=1,11</summary>
        protected IExpression EE6FFFCD6 => _e6fffcd6 ?? (_e6fffcd6 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EDF7A1DD4));
        /// <summary>0.7</summary>
        protected IExpression EE705BCBD => _e705bcbd ?? (_e705bcbd = new ConstantExpression(new DecimalNumber.Text("0.7")));
        /// <summary>2..9</summary>
        protected IExpression EE749D68C => _e749d68c ?? (_e749d68c = new RangeExpression(E7C9B4215, E96A80AE2));
        /// <summary>n % 10=5..9 or n % 100=11..14</summary>
        protected IExpression EE74F39B3 => _e74f39b3 ?? (_e74f39b3 = new BinaryOpExpression(BinaryOp.LogicalOr, E32F6A02F, E173DAD66));
        /// <summary>1,2,5,7,8</summary>
        protected IExpression EE79986FE => _e79986fe ?? (_e79986fe = new GroupExpression(ED6BF1D5A, E7C9B4215, EBEAFB136, E7AC0D2FC, E98A8B99F));
        /// <summary>21</summary>
        protected IExpression EE79D56DA => _e79d56da ?? (_e79d56da = new ConstantExpression(new DecimalNumber.Long(21)));
        /// <summary>100</summary>
        protected IExpression EE7AD2EBA => _e7ad2eba ?? (_e7ad2eba = new ConstantExpression(new DecimalNumber.Long(100)));
        /// <summary>i % 100=0,20,40,60,80</summary>
        protected IExpression EE82B0586 => _e82b0586 ?? (_e82b0586 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, EE912F243));
        /// <summary>n % 10=2..4 and n % 100!=12..14</summary>
        protected IExpression EE89E9716 => _e89e9716 ?? (_e89e9716 = new BinaryOpExpression(BinaryOp.LogicalAnd, E92FA4C69, EC99DFE4C));
        /// <summary>0,20,40,60,80</summary>
        protected IExpression EE912F243 => _e912f243 ?? (_e912f243 = new GroupExpression(ED8BCA617, EE997B997, E7FD9E0C1, E63E94A2B, E934C92A5));
        /// <summary>1,2,3</summary>
        protected IExpression EE915A0DF => _e915a0df ?? (_e915a0df = new GroupExpression(ED6BF1D5A, E7C9B4215, E72B6DCC0));
        /// <summary>v=0 and n!=0..10 and n % 10=0</summary>
        protected IExpression EE978F1CF => _e978f1cf ?? (_e978f1cf = new BinaryOpExpression(BinaryOp.LogicalAnd, E837EC443, E9E1154F7));
        /// <summary>n % 100=2..10</summary>
        protected IExpression EE9809404 => _e9809404 ?? (_e9809404 = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, EC9AAA8CC));
        /// <summary>i % 10=2</summary>
        protected IExpression EE994C828 => _e994c828 ?? (_e994c828 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, E7C9B4215));
        /// <summary>20</summary>
        protected IExpression EE997B997 => _e997b997 ?? (_e997b997 = new ConstantExpression(new DecimalNumber.Long(20)));
        /// <summary>n % 10=0 or n % 100=11..19 or v=2 and f % 100=11..19</summary>
        protected IExpression EEA009B9F => _ea009b9f ?? (_ea009b9f = new BinaryOpExpression(BinaryOp.LogicalOr, E9E1154F7, EFE8E107E));
        /// <summary>n % 10=3</summary>
        protected IExpression EEA123C60 => _ea123c60 ?? (_ea123c60 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E72B6DCC0));
        /// <summary>1000000.000</summary>
        protected IExpression EEB11DD3F => _eb11dd3f ?? (_eb11dd3f = new ConstantExpression(new DecimalNumber.Text("1000000.000")));
        /// <summary>84</summary>
        protected IExpression EEB3A32E1 => _eb3a32e1 ?? (_eb3a32e1 = new ConstantExpression(new DecimalNumber.Long(84)));
        /// <summary>17.0</summary>
        protected IExpression EEB8497D4 => _eb8497d4 ?? (_eb8497d4 = new ConstantExpression(new DecimalNumber.Text("17.0")));
        /// <summary>50</summary>
        protected IExpression EECAA6DB6 => _ecaa6db6 ?? (_ecaa6db6 = new ConstantExpression(new DecimalNumber.Long(50)));
        /// <summary>i=2</summary>
        protected IExpression EED63B9B6 => _ed63b9b6 ?? (_ed63b9b6 = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, E7C9B4215));
        /// <summary>1.1..2.6</summary>
        protected IExpression EED76E557 => _ed76e557 ?? (_ed76e557 = new RangeExpression(E45E2150C, E8CF47314));
        /// <summary>i % 10=0</summary>
        protected IExpression EEDC2B146 => _edc2b146 ?? (_edc2b146 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, ED8BCA617));
        /// <summary>v!=2</summary>
        protected IExpression EEDD26642 => _edd26642 ?? (_edd26642 = new BinaryOpExpression(BinaryOp.NotEqual, ED5BAF2B4, E7C9B4215));
        /// <summary>n % 10=6</summary>
        protected IExpression EEE0AD371 => _ee0ad371 ?? (_ee0ad371 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E84C18E51));
        /// <summary>1000000</summary>
        protected IExpression EEEA9287A => _eea9287a ?? (_eea9287a = new ConstantExpression(new DecimalNumber.Long(1000000)));
        /// <summary>n=5 or n % 100=5</summary>
        protected IExpression EEED9E998 => _eed9e998 ?? (_eed9e998 = new BinaryOpExpression(BinaryOp.LogicalOr, E3D5E8A34, EDB39458E));
        /// <summary>7..9</summary>
        protected IExpression EEEDDEBA1 => _eeddeba1 ?? (_eeddeba1 = new RangeExpression(E7AC0D2FC, E96A80AE2));
        /// <summary>i % 10=0..1</summary>
        protected IExpression EEEFDDF4B => _eefddf4b ?? (_eefddf4b = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, ED2F330E2));
        /// <summary>1000000.00</summary>
        protected IExpression EEFF9B115 => _eff9b115 ?? (_eff9b115 = new ConstantExpression(new DecimalNumber.Text("1000000.00")));
        /// <summary>6.000</summary>
        protected IExpression EF01004F2 => _f01004f2 ?? (_f01004f2 = new ConstantExpression(new DecimalNumber.Text("6.000")));
        /// <summary>v=0 and i % 100=2 or f % 100=2</summary>
        protected IExpression EF0ACC361 => _f0acc361 ?? (_f0acc361 = new BinaryOpExpression(BinaryOp.LogicalOr, E19B77C72, E737D7515));
        /// <summary>52..54</summary>
        protected IExpression EF0B17619 => _f0b17619 ?? (_f0b17619 = new RangeExpression(E98D45C55, E92D452E3));
        /// <summary>v=0 and i % 10=5..9 or v=0 and i % 100=11..14</summary>
        protected IExpression EF0E2AA5F => _f0e2aa5f ?? (_f0e2aa5f = new BinaryOpExpression(BinaryOp.LogicalOr, E519BA030, E50AE9FFD));
        /// <summary>5.1</summary>
        protected IExpression EF22F4FC0 => _f22f4fc0 ?? (_f22f4fc0 = new ConstantExpression(new DecimalNumber.Text("5.1")));
        /// <summary>3..10,13..19</summary>
        protected IExpression EF2E9CDC1 => _f2e9cdc1 ?? (_f2e9cdc1 = new GroupExpression(E305E95D5, E15AB5885));
        /// <summary>5.0</summary>
        protected IExpression EF32F5153 => _f32f5153 ?? (_f32f5153 = new ConstantExpression(new DecimalNumber.Text("5.0")));
        /// <summary>i % 100=20,50,70,80</summary>
        protected IExpression EF33A65F8 => _f33a65f8 ?? (_f33a65f8 = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, EDF6A33C5));
        /// <summary>n % 100!=11,12</summary>
        protected IExpression EF40E857B => _f40e857b ?? (_f40e857b = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, EF5DC663A));
        /// <summary>f % 100!=11</summary>
        protected IExpression EF42B1EC6 => _f42b1ec6 ?? (_f42b1ec6 = new BinaryOpExpression(BinaryOp.NotEqual, E779038EE, E8DF33FDF));
        /// <summary>5.3</summary>
        protected IExpression EF42F52E6 => _f42f52e6 ?? (_f42f52e6 = new ConstantExpression(new DecimalNumber.Text("5.3")));
        /// <summary>i % 10=7,8 and i % 100!=17,18</summary>
        protected IExpression EF4449A1A => _f4449a1a ?? (_f4449a1a = new BinaryOpExpression(BinaryOp.LogicalAnd, E12AFEB73, EF98ECFCA));
        /// <summary>4..16</summary>
        protected IExpression EF468FAB6 => _f468fab6 ?? (_f468fab6 = new RangeExpression(ECCE679B2, EA4C9E1DD));
        /// <summary>5.2</summary>
        protected IExpression EF52F5479 => _f52f5479 ?? (_f52f5479 = new ConstantExpression(new DecimalNumber.Text("5.2")));
        /// <summary>4..17</summary>
        protected IExpression EF568FC49 => _f568fc49 ?? (_f568fc49 = new RangeExpression(ECCE679B2, EA3C9E04A));
        /// <summary>8.000</summary>
        protected IExpression EF5B81BA0 => _f5b81ba0 ?? (_f5b81ba0 = new ConstantExpression(new DecimalNumber.Text("8.000")));
        /// <summary>11..26</summary>
        protected IExpression EF5CA20BF => _f5ca20bf ?? (_f5ca20bf = new RangeExpression(E9DC9D6D8, E92C309C2));
        /// <summary>11,12</summary>
        protected IExpression EF5DC663A => _f5dc663a ?? (_f5dc663a = new GroupExpression(E8DF33FDF, E93D79EE8));
        /// <summary>4..10</summary>
        protected IExpression EF668FDDC => _f668fddc ?? (_f668fddc = new RangeExpression(ECCE679B2, E9EC9D86B));
        /// <summary>11..25</summary>
        protected IExpression EF6CA2252 => _f6ca2252 ?? (_f6ca2252 = new RangeExpression(E9DC9D6D8, E91C3082F));
        /// <summary>11..15</summary>
        protected IExpression EF6CC60E9 => _f6cc60e9 ?? (_f6cc60e9 = new RangeExpression(E9DC9D6D8, EA1C9DD24));
        /// <summary>4,6,9</summary>
        protected IExpression EF6ED1FC8 => _f6ed1fc8 ?? (_f6ed1fc8 = new GroupExpression(EC0AA13F3, E84C18E51, E96A80AE2));
        /// <summary>200</summary>
        protected IExpression EF6F99720 => _f6f99720 ?? (_f6f99720 = new ConstantExpression(new DecimalNumber.Text("200")));
        /// <summary>5.4</summary>
        protected IExpression EF72F579F => _f72f579f ?? (_f72f579f = new ConstantExpression(new DecimalNumber.Text("5.4")));
        /// <summary>f % 10=2..4 and f % 100!=12..14</summary>
        protected IExpression EF796F8D6 => _f796f8d6 ?? (_f796f8d6 = new BinaryOpExpression(BinaryOp.LogicalAnd, E951A8CA1, EE18D12A4));
        /// <summary>11..24</summary>
        protected IExpression EF7CA23E5 => _f7ca23e5 ?? (_f7ca23e5 = new RangeExpression(E9DC9D6D8, E90C3069C));
        /// <summary>201</summary>
        protected IExpression EF7F998B3 => _f7f998b3 ?? (_f7f998b3 = new ConstantExpression(new DecimalNumber.Text("201")));
        /// <summary>i=0..1</summary>
        protected IExpression EF81CFE45 => _f81cfe45 ?? (_f81cfe45 = new BinaryOpExpression(BinaryOp.Equal, EBCBACB59, ED2F330E2));
        /// <summary>v=0 and i % 10=5..9 or v=0 and i % 100=12..14</summary>
        protected IExpression EF8320D9C => _f8320d9c ?? (_f8320d9c = new BinaryOpExpression(BinaryOp.LogicalOr, E519BA030, EA71FCD3A));
        /// <summary>202</summary>
        protected IExpression EF8F99A46 => _f8f99a46 ?? (_f8f99a46 = new ConstantExpression(new DecimalNumber.Text("202")));
        /// <summary>i % 100!=17,18</summary>
        protected IExpression EF98ECFCA => _f98ecfca ?? (_f98ecfca = new BinaryOpExpression(BinaryOp.NotEqual, E84E93E2B, EE2DE4C1A));
        /// <summary>203</summary>
        protected IExpression EF9F99BD9 => _f9f99bd9 ?? (_f9f99bd9 = new ConstantExpression(new DecimalNumber.Text("203")));
        /// <summary>i % 100=3..4</summary>
        protected IExpression EFA66671D => _fa66671d ?? (_fa66671d = new BinaryOpExpression(BinaryOp.Equal, E84E93E2B, E82BF64C0));
        /// <summary>204</summary>
        protected IExpression EFAF99D6C => _faf99d6c ?? (_faf99d6c = new ConstantExpression(new DecimalNumber.Text("204")));
        /// <summary>n % 100000</summary>
        protected IExpression EFB32828E => _fb32828e ?? (_fb32828e = new BinaryOpExpression(BinaryOp.Modulo, EBDBACCEC, E7D140262));
        /// <summary>t=0 and i % 10=1 and i % 100!=11</summary>
        protected IExpression EFB3F0513 => _fb3f0513 ?? (_fb3f0513 = new BinaryOpExpression(BinaryOp.LogicalAnd, EBD021D9B, ED77C08CF));
        /// <summary>n % 10=4 and n % 100!=14</summary>
        protected IExpression EFB5AAB57 => _fb5aab57 ?? (_fb5aab57 = new BinaryOpExpression(BinaryOp.LogicalAnd, E9E05D853, E5CCEB86F));
        /// <summary>n % 10=1 and n % 100!=11,71,91</summary>
        protected IExpression EFBB24E31 => _fbb24e31 ?? (_fbb24e31 = new BinaryOpExpression(BinaryOp.LogicalAnd, EC20D803A, E92108A80));
        /// <summary>7.000</summary>
        protected IExpression EFBBB4831 => _fbbb4831 ?? (_fbbb4831 = new ConstantExpression(new DecimalNumber.Text("7.000")));
        /// <summary>0,1</summary>
        protected IExpression EFBCA1EB0 => _fbca1eb0 ?? (_fbca1eb0 = new GroupExpression(ED8BCA617, ED6BF1D5A));
        /// <summary>205</summary>
        protected IExpression EFBF99EFF => _fbf99eff ?? (_fbf99eff = new ConstantExpression(new DecimalNumber.Text("205")));
        /// <summary>n % 10=2,3</summary>
        protected IExpression EFC313A7C => _fc313a7c ?? (_fc313a7c = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E911AF69C));
        /// <summary>1000</summary>
        protected IExpression EFC934342 => _fc934342 ?? (_fc934342 = new ConstantExpression(new DecimalNumber.Long(1000)));
        /// <summary>n % 100=1,21,41,61,81</summary>
        protected IExpression EFC942D8E => _fc942d8e ?? (_fc942d8e = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, E20199C06));
        /// <summary>5.0000</summary>
        protected IExpression EFDDF0321 => _fddf0321 ?? (_fddf0321 = new ConstantExpression(new DecimalNumber.Text("5.0000")));
        /// <summary>4..18</summary>
        protected IExpression EFE690A74 => _fe690a74 ?? (_fe690a74 = new RangeExpression(ECCE679B2, E96C9CBD3));
        /// <summary>i % 10=1 and i % 100!=11</summary>
        protected IExpression EFE72E477 => _fe72e477 ?? (_fe72e477 = new BinaryOpExpression(BinaryOp.LogicalAnd, E0FC06C63, ED77C08CF));
        /// <summary>n % 100=11..19 or v=2 and f % 100=11..19</summary>
        protected IExpression EFE8E107E => _fe8e107e ?? (_fe8e107e = new BinaryOpExpression(BinaryOp.LogicalOr, E9338945F, E4B83D017));
        /// <summary>21..36</summary>
        protected IExpression EFF1A15EF => _ff1a15ef ?? (_ff1a15ef = new RangeExpression(E8DC301E3, E94C54B7F));
        /// <summary>4..19</summary>
        protected IExpression EFF690C07 => _ff690c07 ?? (_ff690c07 = new RangeExpression(ECCE679B2, E95C9CA40));
        /// <summary>10..13</summary>
        protected IExpression EFFC48D42 => _ffc48d42 ?? (_ffc48d42 = new RangeExpression(E9EC9D86B, E9FC9D9FE));
        /// <summary>i=1 and v=0</summary>
        protected IExpression EFFFC2537 => _fffc2537 ?? (_fffc2537 = new BinaryOpExpression(BinaryOp.LogicalAnd, E49A58121, E5BCFC261));
        /// <summary>11.00</summary>
        protected IExpression E00EAA432 => _00eaa432 ?? (_00eaa432 = new ConstantExpression(new DecimalNumber.Text("11.00")));
        /// <summary>32..34</summary>
        protected IExpression E0156F279 => _0156f279 ?? (_0156f279 = new RangeExpression(E90C54533, E96C54EA5));
        /// <summary>43.0</summary>
        protected IExpression E02C92C95 => _02c92c95 ?? (_02c92c95 = new ConstantExpression(new DecimalNumber.Text("43.0")));
        /// <summary>11..19</summary>
        protected IExpression E02CC73CD => _02cc73cd ?? (_02cc73cd = new RangeExpression(E9DC9D6D8, E95C9CA40));
        /// <summary>n=11..99</summary>
        protected IExpression E033F3325 => _033f3325 ?? (_033f3325 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E96BDB99F));
        /// <summary>12,13</summary>
        protected IExpression E03C3A4B8 => _03c3a4b8 ?? (_03c3a4b8 = new GroupExpression(E93D79EE8, E51D5343D));
        /// <summary>n % 10=2..9 and n % 100!=11..19</summary>
        protected IExpression E03CAF549 => _03caf549 ?? (_03caf549 = new BinaryOpExpression(BinaryOp.LogicalAnd, E42E3F86C, E5C674AF6));
        /// <summary>141.0</summary>
        protected IExpression E03E9776C => _03e9776c ?? (_03e9776c = new ConstantExpression(new DecimalNumber.Text("141.0")));
        /// <summary>n % 100000=1000..20000,40000,60000,80000</summary>
        protected IExpression E05775918 => _05775918 ?? (_05775918 = new BinaryOpExpression(BinaryOp.Equal, EFB32828E, EAF780C38));
        /// <summary>f % 100=3..4</summary>
        protected IExpression E058073C0 => _058073c0 ?? (_058073c0 = new BinaryOpExpression(BinaryOp.Equal, E779038EE, E82BF64C0));
        /// <summary>v!=0 or n=0 or n % 100=2..19</summary>
        protected IExpression E062AAF64 => _062aaf64 ?? (_062aaf64 = new BinaryOpExpression(BinaryOp.LogicalOr, E7D97B2C4, EDB46A3FA));
        /// <summary>3,23,43,63,83</summary>
        protected IExpression E06589D34 => _06589d34 ?? (_06589d34 = new GroupExpression(E72B6DCC0, E83951640, E51E459A6, E1DE6D934, EA537C1EA));
        /// <summary>4.4</summary>
        protected IExpression E07B44BE0 => _07b44be0 ?? (_07b44be0 = new ConstantExpression(new DecimalNumber.Text("4.4")));
        /// <summary>62</summary>
        protected IExpression E07E43C29 => _07e43c29 ?? (_07e43c29 = new ConstantExpression(new DecimalNumber.Long(62)));
        /// <summary>111..117</summary>
        protected IExpression E0971950D => _0971950d ?? (_0971950d = new RangeExpression(E4DBD50CB, E53BD5A3D));
        /// <summary>1.0..1.3</summary>
        protected IExpression E09B8F622 => _09b8f622 ?? (_09b8f622 = new RangeExpression(E46E2169F, E47E21832));
        /// <summary>i=0 or n=1</summary>
        protected IExpression E0A18C452 => _0a18c452 ?? (_0a18c452 = new BinaryOpExpression(BinaryOp.LogicalOr, E49A7BFB8, EBD4F1E10));
        /// <summary>70</summary>
        protected IExpression E0A191120 => _0a191120 ?? (_0a191120 = new ConstantExpression(new DecimalNumber.Long(70)));
        /// <summary>404</summary>
        protected IExpression E0B73AAD6 => _0b73aad6 ?? (_0b73aad6 = new ConstantExpression(new DecimalNumber.Text("404")));
        /// <summary>4.0</summary>
        protected IExpression E0BB4522C => _0bb4522c ?? (_0bb4522c = new ConstantExpression(new DecimalNumber.Text("4.0")));
        /// <summary>v=0 and i!=1</summary>
        protected IExpression E0C3E27DE => _0c3e27de ?? (_0c3e27de = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, E3805C12C));
        /// <summary>405</summary>
        protected IExpression E0C73AC69 => _0c73ac69 ?? (_0c73ac69 = new ConstantExpression(new DecimalNumber.Text("405")));
        /// <summary>v=0 and i!=1 and i % 10=0..1 or v=0 and i % 10=5..9 or v=0 and i % 100=12..14</summary>
        protected IExpression E0CAFD5D4 => _0cafd5d4 ?? (_0cafd5d4 = new BinaryOpExpression(BinaryOp.LogicalOr, E4ABBA532, EF8320D9C));
        /// <summary>4.1</summary>
        protected IExpression E0CB453BF => _0cb453bf ?? (_0cb453bf = new ConstantExpression(new DecimalNumber.Text("4.1")));
        /// <summary>3..4,9</summary>
        protected IExpression E0D003EFB => _0d003efb ?? (_0d003efb = new GroupExpression(E82BF64C0, E96A80AE2));
        /// <summary>402</summary>
        protected IExpression E0D73ADFC => _0d73adfc ?? (_0d73adfc = new ConstantExpression(new DecimalNumber.Text("402")));
        /// <summary>n=5,6</summary>
        protected IExpression E0D7D2C88 => _0d7d2c88 ?? (_0d7d2c88 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E2049F8B2));
        /// <summary>4.2</summary>
        protected IExpression E0DB45552 => _0db45552 ?? (_0db45552 = new ConstantExpression(new DecimalNumber.Text("4.2")));
        /// <summary>n % 10=2</summary>
        protected IExpression E0E48E275 => _0e48e275 ?? (_0e48e275 = new BinaryOpExpression(BinaryOp.Equal, E6ECCDE4E, E7C9B4215));
        /// <summary>403</summary>
        protected IExpression E0E73AF8F => _0e73af8f ?? (_0e73af8f = new ConstantExpression(new DecimalNumber.Text("403")));
        /// <summary>9.00</summary>
        protected IExpression E0EB2D035 => _0eb2d035 ?? (_0eb2d035 = new ConstantExpression(new DecimalNumber.Text("9.00")));
        /// <summary>4.3</summary>
        protected IExpression E0EB456E5 => _0eb456e5 ?? (_0eb456e5 = new ConstantExpression(new DecimalNumber.Text("4.3")));
        /// <summary>42</summary>
        protected IExpression E0ED14480 => _0ed14480 ?? (_0ed14480 = new ConstantExpression(new DecimalNumber.Text("42")));
        /// <summary>90</summary>
        protected IExpression E0EDE1073 => _0ede1073 ?? (_0ede1073 = new ConstantExpression(new DecimalNumber.Text("90")));
        /// <summary>400</summary>
        protected IExpression E0F73B122 => _0f73b122 ?? (_0f73b122 = new ConstantExpression(new DecimalNumber.Text("400")));
        /// <summary>i % 10=1</summary>
        protected IExpression E0FC06C63 => _0fc06c63 ?? (_0fc06c63 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, ED6BF1D5A));
        /// <summary>43</summary>
        protected IExpression E0FD14613 => _0fd14613 ?? (_0fd14613 = new ConstantExpression(new DecimalNumber.Text("43")));
        /// <summary>401</summary>
        protected IExpression E1073B2B5 => _1073b2b5 ?? (_1073b2b5 = new ConstantExpression(new DecimalNumber.Text("401")));
        /// <summary>40</summary>
        protected IExpression E10D147A6 => _10d147a6 ?? (_10d147a6 = new ConstantExpression(new DecimalNumber.Text("40")));
        /// <summary>41</summary>
        protected IExpression E11D14939 => _11d14939 ?? (_11d14939 = new ConstantExpression(new DecimalNumber.Text("41")));
        /// <summary>n % 1000000=0</summary>
        protected IExpression E120EA07F => _120ea07f ?? (_120ea07f = new BinaryOpExpression(BinaryOp.Equal, EC01D5C76, ED8BCA617));
        /// <summary>i % 10=7,8</summary>
        protected IExpression E12AFEB73 => _12afeb73 ?? (_12afeb73 = new BinaryOpExpression(BinaryOp.Equal, E5E3CEC93, E8F44C9D2));
        /// <summary>46</summary>
        protected IExpression E12D14ACC => _12d14acc ?? (_12d14acc = new ConstantExpression(new DecimalNumber.Text("46")));
        /// <summary>47</summary>
        protected IExpression E13D14C5F => _13d14c5f ?? (_13d14c5f = new ConstantExpression(new DecimalNumber.Text("47")));
        /// <summary>v=0 and i % 100=3..4</summary>
        protected IExpression E13D1C717 => _13d1c717 ?? (_13d1c717 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, EFA66671D));
        /// <summary>t!=0 and i=0,1</summary>
        protected IExpression E14278FAE => _14278fae ?? (_14278fae = new BinaryOpExpression(BinaryOp.LogicalAnd, E7DF0645A, EE5C8BDA7));
        /// <summary>44</summary>
        protected IExpression E14D14DF2 => _14d14df2 ?? (_14d14df2 = new ConstantExpression(new DecimalNumber.Text("44")));
        /// <summary>n % 10=9 or n % 10=0 and n!=0</summary>
        protected IExpression E14E33184 => _14e33184 ?? (_14e33184 = new BinaryOpExpression(BinaryOp.LogicalOr, EC2554682, E20046CF0));
        /// <summary>13..19</summary>
        protected IExpression E15AB5885 => _15ab5885 ?? (_15ab5885 = new RangeExpression(E51D5343D, ECDEDFC57));
        /// <summary>1,5</summary>
        protected IExpression E15BCEDF1 => _15bcedf1 ?? (_15bcedf1 = new GroupExpression(ED6BF1D5A, EBEAFB136));
        /// <summary>n % 100=11..14</summary>
        protected IExpression E173DAD66 => _173dad66 ?? (_173dad66 = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, ED1809B3E));
        /// <summary>i % 100!=12</summary>
        protected IExpression E17AAFBF8 => _17aafbf8 ?? (_17aafbf8 = new BinaryOpExpression(BinaryOp.NotEqual, E84E93E2B, E93D79EE8));
        /// <summary>103..110</summary>
        protected IExpression E18686237 => _18686237 ?? (_18686237 = new RangeExpression(E4FBF9288, E4CBD4F38));
        /// <summary>48</summary>
        protected IExpression E18D1543E => _18d1543e ?? (_18d1543e = new ConstantExpression(new DecimalNumber.Text("48")));
        /// <summary>n=1,5</summary>
        protected IExpression E19478C3F => _19478c3f ?? (_19478c3f = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E15BCEDF1));
        /// <summary>300</summary>
        protected IExpression E1966E0D0 => _1966e0d0 ?? (_1966e0d0 = new ConstantExpression(new DecimalNumber.Long(300)));
        /// <summary>n % 10=2 and n % 100!=12</summary>
        protected IExpression E199BF7AF => _199bf7af ?? (_199bf7af = new BinaryOpExpression(BinaryOp.LogicalAnd, E0E48E275, E5CAB36E5));
        /// <summary>v=0 and i % 100=2</summary>
        protected IExpression E19B77C72 => _19b77c72 ?? (_19b77c72 = new BinaryOpExpression(BinaryOp.LogicalAnd, E5BCFC261, E995B85E0));
        /// <summary>0.00..0.04</summary>
        protected IExpression E19CCBF15 => _19ccbf15 ?? (_19ccbf15 = new RangeExpression(E30F67918, E34F67F64));
        /// <summary>49</summary>
        protected IExpression E19D155D1 => _19d155d1 ?? (_19d155d1 = new ConstantExpression(new DecimalNumber.Text("49")));
        /// <summary>63.0</summary>
        protected IExpression E1BBDF62B => _1bbdf62b ?? (_1bbdf62b = new ConstantExpression(new DecimalNumber.Text("63.0")));
        /// <summary>42.0</summary>
        protected IExpression E1BEBFE2E => _1bebfe2e ?? (_1bebfe2e = new ConstantExpression(new DecimalNumber.Text("42.0")));
        /// <summary>68</summary>
        protected IExpression E1CCCDD5C => _1cccdd5c ?? (_1cccdd5c = new ConstantExpression(new DecimalNumber.Text("68")));
        /// <summary>1,5,7..9</summary>
        protected IExpression E1CF15395 => _1cf15395 ?? (_1cf15395 = new GroupExpression(ED6BF1D5A, EBEAFB136, EEEDDEBA1));
        /// <summary>7..10</summary>
        protected IExpression E1D2676CB => _1d2676cb ?? (_1d2676cb = new RangeExpression(ECBE6781F, E9EC9D86B));
        /// <summary>69</summary>
        protected IExpression E1DCCDEEF => _1dccdeef ?? (_1dccdeef = new ConstantExpression(new DecimalNumber.Text("69")));
        /// <summary>1,3</summary>
        protected IExpression E1DCCF45B => _1dccf45b ?? (_1dccf45b = new GroupExpression(ED6BF1D5A, E72B6DCC0));
        /// <summary>63</summary>
        protected IExpression E1DE6D934 => _1de6d934 ?? (_1de6d934 = new ConstantExpression(new DecimalNumber.Long(63)));
        /// <summary>n=3,4</summary>
        protected IExpression E1E3A2A10 => _1e3a2a10 ?? (_1e3a2a10 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E63DB2C0A));
        /// <summary>13.0</summary>
        protected IExpression E1F372AC0 => _1f372ac0 ?? (_1f372ac0 = new ConstantExpression(new DecimalNumber.Text("13.0")));
        /// <summary>500</summary>
        protected IExpression E1F472B36 => _1f472b36 ?? (_1f472b36 = new ConstantExpression(new DecimalNumber.Long(500)));
        /// <summary>2.000</summary>
        protected IExpression E1F4D627E => _1f4d627e ?? (_1f4d627e = new ConstantExpression(new DecimalNumber.Text("2.000")));
        /// <summary>90..99</summary>
        protected IExpression E1F6682E6 => _1f6682e6 ?? (_1f6682e6 = new RangeExpression(E2A0D05FA, E980FD23F));
        /// <summary>n % 10=0 and n!=0</summary>
        protected IExpression E20046CF0 => _20046cf0 ?? (_20046cf0 = new BinaryOpExpression(BinaryOp.LogicalAnd, E9E1154F7, EC1B3E81C));
        /// <summary>1,21,41,61,81</summary>
        protected IExpression E20199C06 => _20199c06 ?? (_20199c06 = new GroupExpression(ED6BF1D5A, EE79D56DA, E75DC4B6C, E61EEE76E, E894EFD50));
        /// <summary>n % 100=2,22,42,62,82</summary>
        protected IExpression E201C05C9 => _201c05c9 ?? (_201c05c9 = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, E7432AAA1));
        /// <summary>5,6</summary>
        protected IExpression E2049F8B2 => _2049f8b2 ?? (_2049f8b2 = new GroupExpression(EBEAFB136, E84C18E51));
        /// <summary>64</summary>
        protected IExpression E20CCE3A8 => _20cce3a8 ?? (_20cce3a8 = new ConstantExpression(new DecimalNumber.Text("64")));
        /// <summary>79</summary>
        protected IExpression E21CF23D2 => _21cf23d2 ?? (_21cf23d2 = new ConstantExpression(new DecimalNumber.Text("79")));
        /// <summary>78</summary>
        protected IExpression E22CF2565 => _22cf2565 ?? (_22cf2565 = new ConstantExpression(new DecimalNumber.Text("78")));
        /// <summary>1..4</summary>
        protected IExpression E22D74F20 => _22d74f20 ?? (_22d74f20 = new RangeExpression(EC9E674F9, ECCE679B2));
        /// <summary>n % 100!=11</summary>
        protected IExpression E22DAC12E => _22dac12e ?? (_22dac12e = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E8DF33FDF));
        /// <summary>…</summary>
        protected IExpression E233EEC59 => _233eec59 ?? (_233eec59 = new InfiniteExpression());
        /// <summary>10000</summary>
        protected IExpression E23A2DE29 => _23a2de29 ?? (_23a2de29 = new ConstantExpression(new DecimalNumber.Text("10000")));
        /// <summary>67</summary>
        protected IExpression E23CCE861 => _23cce861 ?? (_23cce861 = new ConstantExpression(new DecimalNumber.Text("67")));
        /// <summary>77</summary>
        protected IExpression E23CF26F8 => _23cf26f8 ?? (_23cf26f8 = new ConstantExpression(new DecimalNumber.Text("77")));
        /// <summary>n=0,1</summary>
        protected IExpression E23E9AF52 => _23e9af52 ?? (_23e9af52 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, EFBCA1EB0));
        /// <summary>n % 1000=0 and n % 100000=1000..20000,40000,60000,80000 or n!=0 and n % 1000000=100000</summary>
        protected IExpression E24919EFF => _24919eff ?? (_24919eff = new BinaryOpExpression(BinaryOp.LogicalOr, E68397994, EBB5C4B5D));
        /// <summary>60</summary>
        protected IExpression E24CCE9F4 => _24cce9f4 ?? (_24cce9f4 = new ConstantExpression(new DecimalNumber.Text("60")));
        /// <summary>n % 100=1..4,21..24,41..44,61..64,81..84</summary>
        protected IExpression E2549312E => _2549312e ?? (_2549312e = new BinaryOpExpression(BinaryOp.Equal, E3FFFE6B6, ED4E966F6));
        /// <summary>0.1..1.6</summary>
        protected IExpression E254B891D => _254b891d ?? (_254b891d = new RangeExpression(EE105B34B, E44E21379));
        /// <summary>61</summary>
        protected IExpression E25CCEB87 => _25cceb87 ?? (_25cceb87 = new ConstantExpression(new DecimalNumber.Text("61")));
        /// <summary>v=0 and i % 100=3..4 or v!=0</summary>
        protected IExpression E2617C675 => _2617c675 ?? (_2617c675 = new BinaryOpExpression(BinaryOp.LogicalOr, E13D1C717, E7D97B2C4));
        /// <summary>6,9</summary>
        protected IExpression E262A66BA => _262a66ba ?? (_262a66ba = new GroupExpression(E84C18E51, E96A80AE2));
        /// <summary>i=2..4 and v=0</summary>
        protected IExpression E264ACB7C => _264acb7c ?? (_264acb7c = new BinaryOpExpression(BinaryOp.LogicalAnd, EA72E2716, E5BCFC261));
        /// <summary>n=1 or t!=0 and i=0,1</summary>
        protected IExpression E26C25720 => _26c25720 ?? (_26c25720 = new BinaryOpExpression(BinaryOp.LogicalOr, EBD4F1E10, E14278FAE));
        /// <summary>62</summary>
        protected IExpression E26CCED1A => _26cced1a ?? (_26cced1a = new ConstantExpression(new DecimalNumber.Text("62")));
        /// <summary>74</summary>
        protected IExpression E26CF2BB1 => _26cf2bb1 ?? (_26cf2bb1 = new ConstantExpression(new DecimalNumber.Text("74")));
        /// <summary>12.0</summary>
        protected IExpression E27020E27 => _27020e27 ?? (_27020e27 = new ConstantExpression(new DecimalNumber.Text("12.0")));
        /// <summary>n=2,3</summary>
        protected IExpression E27804086 => _27804086 ?? (_27804086 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, E911AF69C));
        /// <summary>63</summary>
        protected IExpression E27CCEEAD => _27cceead ?? (_27cceead = new ConstantExpression(new DecimalNumber.Text("63")));
        /// <summary>73</summary>
        protected IExpression E27CF2D44 => _27cf2d44 ?? (_27cf2d44 = new ConstantExpression(new DecimalNumber.Text("73")));
        /// <summary>9.000</summary>
        protected IExpression E287D77DF => _287d77df ?? (_287d77df = new ConstantExpression(new DecimalNumber.Text("9.000")));
        /// <summary>10.0000</summary>
        protected IExpression E28C98A6F => _28c98a6f ?? (_28c98a6f = new ConstantExpression(new DecimalNumber.Text("10.0000")));
        /// <summary>72</summary>
        protected IExpression E28CF2ED7 => _28cf2ed7 ?? (_28cf2ed7 = new ConstantExpression(new DecimalNumber.Text("72")));
        /// <summary>n=3..6</summary>
        protected IExpression E2951DF68 => _2951df68 ?? (_2951df68 = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ED2D743DE));
        /// <summary>71</summary>
        protected IExpression E29CF306A => _29cf306a ?? (_29cf306a = new ConstantExpression(new DecimalNumber.Text("71")));
        /// <summary>90</summary>
        protected IExpression E2A0D05FA => _2a0d05fa ?? (_2a0d05fa = new ConstantExpression(new DecimalNumber.Long(90)));
        /// <summary>1000.4</summary>
        protected IExpression E2A548A39 => _2a548a39 ?? (_2a548a39 = new ConstantExpression(new DecimalNumber.Text("1000.4")));
        /// <summary>200..202</summary>
        protected IExpression E2AB277E1 => _2ab277e1 ?? (_2ab277e1 = new RangeExpression(EF6F99720, EF8F99A46));
        /// <summary>70</summary>
        protected IExpression E2ACF31FD => _2acf31fd ?? (_2acf31fd = new ConstantExpression(new DecimalNumber.Text("70")));
        /// <summary>1000.3</summary>
        protected IExpression E2B548BCC => _2b548bcc ?? (_2b548bcc = new ConstantExpression(new DecimalNumber.Text("1000.3")));
        /// <summary>t=0</summary>
        protected IExpression E2B74B6BB => _2b74b6bb ?? (_2b74b6bb = new BinaryOpExpression(BinaryOp.Equal, ED7BAF5DA, ED8BCA617));
        /// <summary>1011.0</summary>
        protected IExpression E2BCFA0CB => _2bcfa0cb ?? (_2bcfa0cb = new ConstantExpression(new DecimalNumber.Text("1011.0")));
        /// <summary>64</summary>
        protected IExpression E2BFBA9EF => _2bfba9ef ?? (_2bfba9ef = new ConstantExpression(new DecimalNumber.Long(64)));
        /// <summary>32.0</summary>
        protected IExpression E2C1311B5 => _2c1311b5 ?? (_2c1311b5 = new ConstantExpression(new DecimalNumber.Text("32.0")));
        /// <summary>705</summary>
        protected IExpression E2C2B5DD8 => _2c2b5dd8 ?? (_2c2b5dd8 = new ConstantExpression(new DecimalNumber.Text("705")));
        /// <summary>1000.2</summary>
        protected IExpression E2C548D5F => _2c548d5f ?? (_2c548d5f = new ConstantExpression(new DecimalNumber.Text("1000.2")));
        /// <summary>n % 100!=12,72,92</summary>
        protected IExpression E2CB43C67 => _2cb43c67 ?? (_2cb43c67 = new BinaryOpExpression(BinaryOp.NotEqual, E3FFFE6B6, E63E4D29A));
        /// <summary>5..9</summary>
        protected IExpression E2D16DECF => _2d16decf ?? (_2d16decf = new RangeExpression(EBEAFB136, E96A80AE2));
        /// <summary>704</summary>
        protected IExpression E2D2B5F6B => _2d2b5f6b ?? (_2d2b5f6b = new ConstantExpression(new DecimalNumber.Text("704")));
        /// <summary>1000.1</summary>
        protected IExpression E2D548EF2 => _2d548ef2 ?? (_2d548ef2 = new ConstantExpression(new DecimalNumber.Text("1000.1")));
        /// <summary>34.0</summary>
        protected IExpression E2D718E33 => _2d718e33 ?? (_2d718e33 = new ConstantExpression(new DecimalNumber.Text("34.0")));
        /// <summary>n=11,8,80,800</summary>
        protected IExpression E2E2A901D => _2e2a901d ?? (_2e2a901d = new BinaryOpExpression(BinaryOp.Equal, EBDBACCEC, ECBD249FB));
        /// <summary>1000.0</summary>
        protected IExpression E2E549085 => _2e549085 ?? (_2e549085 = new ConstantExpression(new DecimalNumber.Text("1000.0")));
        /// <summary>143.0</summary>
        protected IExpression E2E894D42 => _2e894d42 ?? (_2e894d42 = new ConstantExpression(new DecimalNumber.Text("143.0")));
        /// <summary>101.0</summary>
        protected IExpression E2FD39D10 => _2fd39d10 ?? (_2fd39d10 = new ConstantExpression(new DecimalNumber.Text("101.0")));
        /// <summary>41..44</summary>
        protected IExpression E2FF66220 => _2ff66220 ?? (_2ff66220 = new RangeExpression(E11D14939, E14D14DF2));
        /// <summary>701</summary>
        protected IExpression E302B6424 => _302b6424 ?? (_302b6424 = new ConstantExpression(new DecimalNumber.Text("701")));
        /// <summary>3..10</summary>
        protected IExpression E305E95D5 => _305e95d5 ?? (_305e95d5 = new RangeExpression(E72B6DCC0, E3FF8DD22));
        /// <summary>0.00</summary>
        protected IExpression E30F67918 => _30f67918 ?? (_30f67918 = new ConstantExpression(new DecimalNumber.Text("0.00")));

        /// <summary></summary>
        protected ISamplesExpression _32e02e0d, _37be0a31, _3fa7ca56, _42e4c727, _4320fac8, _441a5a7e, _463cd75d, _47a544d8, _48928607, _49a0c5a9, _4b5d780d, _4cab1716, _4cbe7cf4, _4d1ceb1a, _4e67f0dd, _50d6947e, _534b3b37, _53eafb8f, _571d9611, _580d87b4, _5ee15a4e, _5f659133, _620ab079, _62152d88, _63345f3f, _6440295c, _64d7705c, _6584da98, _68b1e329, _6b373360, _6bad1fb4, _6bb4a5dd, _6e411094, _70ef10e9, _713d3c18, _7143b999, _7421e24a, _74312601, _78162d52, _791dd688, _7aef0f49, _7b6dddf3, _7bc3e6a8, _7d97aa09, _7fb07c93, _7fef7670, _8148790b, _818ead2b, _81f8fa2d, _8aa9704b, _8b31405b, _8e8728df, _90131c27, _915ff900, _95a531b4, _982833e3, _98da6edc, _99f271af, _9a0e44b4, _9b8d06b5, _a02b52df, _a02da6ba, _a26dc2f6, _a6a336dc, _a7d89e52, _a7f98eb0, _a8538a54, _ac450d5f, _b152352e, _b1cdea56, _b2143050, _b3062ecf, _b4143376, _b614369c, _b714382f, _b73d72fe, _b81439c2, _b84947e1, _b8582b29, _b9143b55, _bc300370, _bea662f3, _bfea5c82, _c0345761, _c0bda170, _c3ca1b79, _c437968e, _c4967a7e, _c91bca52, _c9d009ea, _cd444539, _ce306b2b, _ce70a2d4, _d3125ee4, _d379571c, _d3a073f0, _d48dc621, _d71f448b, _d7798032, _dbd924c8, _dd06ef4e, _dd390f19, _e2a81867, _e30b61d6, _e6cb67ef, _e783df46, _e7caae43, _e8ce77d1, _e9057c68, _eb8a4639, _ec063218, _edcab7b5, _f0161866, _f13564ad, _f35205fb, _f419640f, _f4866074, _f7179b11, _f758082a, _fa63af5a, _fc992fde, _fd50e885, _0060d900, _00ce1ee9, _032410e9, _043d1268, _0742db5f, _0943d865, _128ce41a, _12d0cc0e, _14ff0595, _15e3d309, _16fdb0a4, _17dd8cf2, _1b284677, _1c8a142b, _1ccb859a, _1f378bf0, _20acf1f6, _214176c0, _21525782, _2170f568, _228c2fd7, _24e4bf2d, _260f6011, _263c5779, _26c984b0, _26f9940b, _2b0c4a40, _2b1d626b, _2c0a6ad4, _2c671de9, _2ed8b14f;
        /// <summary>@integer 0, 5, 7~20, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E32E02E0D => _32e02e0d ?? (_32e02e0d = new SamplesExpression("integer", EC8E67366, ECDE67B45, E8B1ED530, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 0.0, 1.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0</summary>
        protected ISamplesExpression E37BE0A31 => _37be0a31 ?? (_37be0a31 = new SamplesExpression("decimal", EE005B1B8, E46E2169F, E4A26B276, E27020E27, E1F372AC0, E7C134FF1, E9536218A, ED261C63B, EEB8497D4, E59F58855, E731859EE, E6F3358DE, E55712385, EC4449AA8, EABBF99CF, E9A924772));
        /// <summary>@decimal 0.2~0.4, 1.2~1.4, 2.2~2.4, 3.2~3.4, 4.2~4.4, 5.2, 10.2, 100.2, 1000.2, …</summary>
        protected ISamplesExpression E3FA7CA56 => _3fa7ca56 ?? (_3fa7ca56 = new SamplesExpression("decimal", E69114891, ED148D3D9, E85F92CD1, ED7DCAE69, E6D12B621, EF52F5479, E4E64AC57, EB99DBA1D, E2C548D5F, E233EEC59));
        /// <summary>@integer 7~10</summary>
        protected ISamplesExpression E42E4C727 => _42e4c727 ?? (_42e4c727 = new SamplesExpression("integer", E1D2676CB));
        /// <summary>@decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E4320FAC8 => _4320fac8 ?? (_4320fac8 = new SamplesExpression("decimal", EA9AD830F, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 1, 101, 201, 301, 401, 501, 601, 701, 1001, …</summary>
        protected ISamplesExpression E441A5A7E => _441a5a7e ?? (_441a5a7e = new SamplesExpression("integer", EC9E674F9, E51BF95AE, EF7F998B3, E7490FBB8, E1073B2B5, E8E484A3A, EB394011F, E302B6424, EB39B6550, E233EEC59));
        /// <summary>@integer 11~26, 111, 1011, …</summary>
        protected ISamplesExpression E463CD75D => _463cd75d ?? (_463cd75d = new SamplesExpression("integer", EF5CA20BF, E4DBD50CB, E4F98894D, E233EEC59));
        /// <summary>@decimal 2.0~3.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E47A544D8 => _47a544d8 ?? (_47a544d8 = new SamplesExpression("decimal", E83B791EF, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 0.1, 1.0, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</summary>
        protected ISamplesExpression E48928607 => _48928607 ?? (_48928607 = new SamplesExpression("decimal", EE105B34B, E46E2169F, E45E2150C, E8BF47181, E70D19CC2, E0CB453BF, EF22F4FC0, E37542995, E9C308B56, E4F64ADEA, EB69DB564, E2D548EF2, E233EEC59));
        /// <summary>@decimal 0.0~0.9, 1.1~1.6, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E49A0C5A9 => _49a0c5a9 ?? (_49a0c5a9 = new SamplesExpression("decimal", EA23B1BEC, E58E826EE, E4F64ADEA, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E4B5D780D => _4b5d780d ?? (_4b5d780d = new SamplesExpression("integer", EC8E67366, EA42EBC10, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0, 20, 40, 60, 80, 100, 120, 140, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E4CAB1716 => _4cab1716 ?? (_4cab1716 = new SamplesExpression("integer", EC8E67366, E8CC30050, E10D147A6, E24CCE9F4, EA8DB314A, E52BF9741, EBEC4BE73, ECAC94E85, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 0.1~1.6</summary>
        protected ISamplesExpression E4CBE7CF4 => _4cbe7cf4 ?? (_4cbe7cf4 = new SamplesExpression("decimal", E254B891D));
        /// <summary>@integer 100~102, 200~202, 300~302, 400~402, 500~502, 600, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E4D1CEB1A => _4d1ceb1a ?? (_4d1ceb1a = new SamplesExpression("integer", EA2176E75, E2AB277E1, EC3A787A5, E50FB55E9, E6FFAE315, EB293FF8C, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0~3, 5, 7, 8, 10~13, 15, 17, 18, 20, 21, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E4E67F0DD => _4e67f0dd ?? (_4e67f0dd = new SamplesExpression("integer", EBAEFEB76, ECDE67B45, ECBE6781F, ED0E67FFE, EFFC48D42, EA1C9DD24, EA3C9E04A, E96C9CBD3, E8CC30050, E8DC301E3, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 0.0~0.3, 0.5, 0.7, 0.8, 1.0~1.3, 1.5, 1.7, 1.8, 2.0, 2.1, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E50D6947E => _50d6947e ?? (_50d6947e = new SamplesExpression("decimal", EAC3B2BAA, EE505B997, EE705BCBD, ED805A520, E09B8F622, E41E20EC0, E43E211E6, E4EE22337, E8AF46FEE, E8BF47181, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 0, 3~17, 101, 1001, …</summary>
        protected ISamplesExpression E534B3B37 => _534b3b37 ?? (_534b3b37 = new SamplesExpression("integer", EC8E67366, E34F3F456, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 4~19, 100, 1000000, …</summary>
        protected ISamplesExpression E53EAFB8F => _53eafb8f ?? (_53eafb8f = new SamplesExpression("integer", EFF690C07, E52BF9741, EBDD71B71, E233EEC59));
        /// <summary>@integer 1, 11, 21, 31, 41, 51, 61, 71, 101, 1001, …</summary>
        protected ISamplesExpression E571D9611 => _571d9611 ?? (_571d9611 = new SamplesExpression("integer", EC9E674F9, E9DC9D6D8, E8DC301E3, E91C546C6, E11D14939, E95D4579C, E25CCEB87, E29CF306A, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 3~6</summary>
        protected ISamplesExpression E580D87B4 => _580d87b4 ?? (_580d87b4 = new SamplesExpression("integer", E8803BD20));
        /// <summary>@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000000.0, …</summary>
        protected ISamplesExpression E5EE15A4E => _5ee15a4e ?? (_5ee15a4e = new SamplesExpression("decimal", E320DA9FF, E59E82881, E5064AF7D, EB79DB6F7, EE103CB87, E233EEC59));
        /// <summary>@integer 0, 10~24, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E5F659133 => _5f659133 ?? (_5f659133 = new SamplesExpression("integer", EC8E67366, E88BD1628, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 3~10, 13~19, 23, 103, 1003, …</summary>
        protected ISamplesExpression E620AB079 => _620ab079 ?? (_620ab079 = new SamplesExpression("integer", E37F3F90F, EA0D9AEEB, E8FC30509, E4FBF9288, EB59B6876, E233EEC59));
        /// <summary>@integer 1, 11</summary>
        protected ISamplesExpression E62152D88 => _62152d88 ?? (_62152d88 = new SamplesExpression("integer", EC9E674F9, E9DC9D6D8));
        /// <summary>@integer 1, 21, 31, 41, 51, 61, 81, 101, 1001, …</summary>
        protected ISamplesExpression E63345F3F => _63345f3f ?? (_63345f3f = new SamplesExpression("integer", EC9E674F9, E8DC301E3, E91C546C6, E11D14939, E95D4579C, E25CCEB87, EA9DB32DD, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 3, 4</summary>
        protected ISamplesExpression E6440295C => _6440295c ?? (_6440295c = new SamplesExpression("integer", EC7E671D3, ECCE679B2));
        /// <summary>@integer 4, 24, 34, 44, 54, 64, 74, 84, 104, 1004, …</summary>
        protected ISamplesExpression E64D7705C => _64d7705c ?? (_64d7705c = new SamplesExpression("integer", ECCE679B2, E90C3069C, E96C54EA5, E14D14DF2, E92D452E3, E20CCE3A8, E26CF2BB1, EA4DB2AFE, E56BF9D8D, EB89B6D2F, E233EEC59));
        /// <summary>@decimal 3.0, 4.0, 5.0, 6.0, 3.00, 4.00, 5.00, 6.00, 3.000, 4.000, 5.000, 6.000, 3.0000, 4.0000, 5.0000, 6.0000</summary>
        protected ISamplesExpression E6584DA98 => _6584da98 ?? (_6584da98 = new SamplesExpression("decimal", E71D19E55, E0BB4522C, EF32F5153, E36542802, E91FC58FF, E88DD4214, E367D1ED9, EB87B46B6, E9F3FCDDD, E984F1EAC, EAFF7A8CB, EF01004F2, E9E712C17, E608D2F94, EFDDF0321, EAB377D66));
        /// <summary>@decimal 1.0, 21.0, 31.0, 41.0, 51.0, 61.0, 71.0, 81.0, 101.0, 1001.0, …</summary>
        protected ISamplesExpression E68B1E329 => _68b1e329 ?? (_68b1e329 = new SamplesExpression("decimal", E46E2169F, E55712385, E9B845998, ED7D9A4DF, E91468072, E46AD7DE1, E8B837F74, EC915FF3B, E2FD39D10, E4816C5DE, E233EEC59));
        /// <summary>@decimal 0.0, 0.00, 0.000, 0.0000</summary>
        protected ISamplesExpression E6B373360 => _6b373360 ?? (_6b373360 = new SamplesExpression("decimal", EE005B1B8, E30F67918, E3C00B9F8, E3D2475D8));
        /// <summary>@integer 2~10, 100~106, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E6BAD1FB4 => _6bad1fb4 ?? (_6bad1fb4 = new SamplesExpression("integer", EAA2EC582, E9E176829, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 11~19, 111~117, 1011, …</summary>
        protected ISamplesExpression E6BB4A5DD => _6bb4a5dd ?? (_6bb4a5dd = new SamplesExpression("integer", E02CC73CD, E0971950D, E4F98894D, E233EEC59));
        /// <summary>@decimal 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00</summary>
        protected ISamplesExpression E6E411094 => _6e411094 ?? (_6e411094 = new SamplesExpression("decimal", E8AF46FEE, E71D19E55, E0BB4522C, EF32F5153, E36542802, E9D308CE9, E371340C0, E3EDE2427, E5064AF7D, E9CCC1A7A, E91FC58FF, E88DD4214, E367D1ED9, EB87B46B6, E4C6DB99B, EA34F39D0));
        /// <summary>@decimal 21.0, 41.0, 61.0, 81.0, 101.0, 121.0, 141.0, 161.0, 1001.0, …</summary>
        protected ISamplesExpression E70EF10E9 => _70ef10e9 ?? (_70ef10e9 = new SamplesExpression("decimal", E55712385, ED7D9A4DF, E46AD7DE1, EC915FF3B, E2FD39D10, ED547E80E, E03E9776C, E468378EA, E4816C5DE, E233EEC59));
        /// <summary>@decimal 7.0, 8.0, 9.0, 10.0, 7.00, 8.00, 9.00, 10.00, 7.000, 8.000, 9.000, 10.000, 7.0000, 8.0000, 9.0000, 10.0000</summary>
        protected ISamplesExpression E713D3C18 => _713d3c18 ?? (_713d3c18 = new SamplesExpression("decimal", E9D308CE9, E371340C0, E3EDE2427, E5064AF7D, E4C6DB99B, EA34F39D0, E0EB2D035, EDB7FF637, EFBBB4831, EF5B81BA0, E287D77DF, E91704D05, E48D25993, E60D363B0, EAC83CD3D, E28C98A6F));
        /// <summary>@decimal 0.0~1.0, 0.00~0.04</summary>
        protected ISamplesExpression E7143B999 => _7143b999 ?? (_7143b999 = new SamplesExpression("decimal", EA4AD7B30, E19CCBF15));
        /// <summary>@integer 3, 4, 9, 23, 24, 29, 33, 34, 39, 43, 44, 49, 103, 1003, …</summary>
        protected ISamplesExpression E7421E24A => _7421e24a ?? (_7421e24a = new SamplesExpression("integer", EC7E671D3, ECCE679B2, ED1E68191, E8FC30509, E90C3069C, E95C30E7B, E8FC543A0, E96C54EA5, E99C5535E, E0FD14613, E14D14DF2, E19D155D1, E4FBF9288, EB59B6876, E233EEC59));
        /// <summary>@integer 6, 9, 10, 16, 19, 26, 29, 36, 39, 106, 1006, …</summary>
        protected ISamplesExpression E74312601 => _74312601 ?? (_74312601 = new SamplesExpression("integer", ECAE6768C, ED1E68191, E9EC9D86B, EA4C9E1DD, E95C9CA40, E92C309C2, E95C30E7B, E94C54B7F, E99C5535E, E54BF9A67, EBA9B7055, E233EEC59));
        /// <summary>@integer 5, 105, 205, 305, 405, 505, 605, 705, 1005, …</summary>
        protected ISamplesExpression E78162D52 => _78162d52 ?? (_78162d52 = new SamplesExpression("integer", ECDE67B45, E55BF9BFA, EFBF99EFF, E78910204, E0C73AC69, E8A4843EE, EAF93FAD3, E2C2B5DD8, EB79B6B9C, E233EEC59));
        /// <summary>@decimal 0.0, 0.1, 1.0, 0.00, 0.01, 1.00, 0.000, 0.001, 1.000, 0.0000, 0.0001, 1.0000</summary>
        protected ISamplesExpression E791DD688 => _791dd688 ?? (_791dd688 = new SamplesExpression("decimal", EE005B1B8, EE105B34B, E46E2169F, E30F67918, E31F67AAB, E44E9B57D, E3C00B9F8, E3D00BB8B, EC8E86837, E3D2475D8, E3E24776B, E4CDBC305));
        /// <summary>@decimal 0.1~1.6, 10.1, 100.1, 1000.1, …</summary>
        protected ISamplesExpression E7AEF0F49 => _7aef0f49 ?? (_7aef0f49 = new SamplesExpression("decimal", E254B891D, E4F64ADEA, EB69DB564, E2D548EF2, E233EEC59));
        /// <summary>@integer 0, 10~20, 30, 40, 50, 60, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E7B6DDDF3 => _7b6dddf3 ?? (_7b6dddf3 = new SamplesExpression("integer", EC8E67366, E8CBD1C74, E92C54859, E10D147A6, E96D4592F, E24CCE9F4, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 2, 22, 32, 42, 52, 62, 72, 82, 102, 1002, …</summary>
        protected ISamplesExpression E7BC3E6A8 => _7bc3e6a8 ?? (_7bc3e6a8 = new SamplesExpression("integer", EC6E67040, E8EC30376, E90C54533, E0ED14480, E98D45C55, E26CCED1A, E28CF2ED7, EA6DB2E24, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@decimal 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E7D97AA09 => _7d97aa09 ?? (_7d97aa09 = new SamplesExpression("decimal", EE005B1B8, E8AF46FEE, E71D19E55, E0BB4522C, EF32F5153, E36542802, E9D308CE9, E371340C0, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 0, 2~16, 102, 1002, …</summary>
        protected ISamplesExpression E7FB07C93 => _7fb07c93 ?? (_7fb07c93 = new SamplesExpression("integer", EC8E67366, EA42EBC10, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@integer 0, 5~8, 10~20, 100, 1000, 10000, 100000, …</summary>
        protected ISamplesExpression E7FEF7670 => _7fef7670 ?? (_7fef7670 = new SamplesExpression("integer", EC8E67366, EC4B11118, E8CBD1C74, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, E233EEC59));
        /// <summary>@integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E8148790B => _8148790b ?? (_8148790b = new SamplesExpression("integer", EC8E67366, EC7ACB41E, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 6, 9, 10, 16, 19, 20, 26, 29, 30, 36, 39, 40, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E818EAD2B => _818ead2b ?? (_818ead2b = new SamplesExpression("integer", ECAE6768C, ED1E68191, E9EC9D86B, EA4C9E1DD, E95C9CA40, E8CC30050, E92C309C2, E95C30E7B, E92C54859, E94C54B7F, E99C5535E, E10D147A6, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 3~10, 13~19</summary>
        protected ISamplesExpression E81F8FA2D => _81f8fa2d ?? (_81f8fa2d = new SamplesExpression("integer", E37F3F90F, EA0D9AEEB));
        /// <summary>@integer 11~26, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E8AA9704B => _8aa9704b ?? (_8aa9704b = new SamplesExpression("integer", EF5CA20BF, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 103.0, 1003.0, …</summary>
        protected ISamplesExpression E8B31405B => _8b31405b ?? (_8b31405b = new SamplesExpression("decimal", E71D19E55, E0BB4522C, EF32F5153, E36542802, E9D308CE9, E371340C0, E3EDE2427, E5064AF7D, EDAC25B46, E9D2807A8, E233EEC59));
        /// <summary>@decimal 2.0, 22.0, 42.0, 62.0, 82.0, 102.0, 122.0, 142.0, 1002.0, …</summary>
        protected ISamplesExpression E8E8728DF => _8e8728df ?? (_8e8728df = new SamplesExpression("decimal", E8AF46FEE, EC4449AA8, E1BEBFE2E, E35802B84, E8BEA5A8A, E619EF28D, E9272C33F, E9565B229, E84A306CF, E233EEC59));
        /// <summary>@integer 20~35, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E90131C27 => _90131c27 ?? (_90131c27 = new SamplesExpression("integer", E841885E3, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 1000000.0, 1000000.00, 1000000.000, …</summary>
        protected ISamplesExpression E915FF900 => _915ff900 ?? (_915ff900 = new SamplesExpression("decimal", EE103CB87, EEFF9B115, EEB11DD3F, E233EEC59));
        /// <summary>@integer 1~4</summary>
        protected ISamplesExpression E95A531B4 => _95a531b4 ?? (_95a531b4 = new SamplesExpression("integer", E22D74F20));
        /// <summary>@integer 0~15, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E982833E3 => _982833e3 ?? (_982833e3 = new SamplesExpression("integer", E86AAD6FF, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 0.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E98DA6EDC => _98da6edc ?? (_98da6edc = new SamplesExpression("decimal", EE005B1B8, EF32F5153, E36542802, E9D308CE9, E371340C0, E3EDE2427, E5064AF7D, E4A26B276, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 0.3, 0.4, 1.3, 1.4, 2.3, 2.4, 3.3, 3.4, 4.3, 4.4, 5.3, 5.4, 6.3, 6.4, 7.3, 7.4, 10.3, 100.3, 1000.3, …</summary>
        protected ISamplesExpression E99F271AF => _99f271af ?? (_99f271af = new SamplesExpression("decimal", EE305B671, EE405B804, E47E21832, E42E21053, E89F46E5B, E8EF4763A, E6ED1999C, E6DD19809, E0EB456E5, E07B44BE0, EF42F52E6, EF72F579F, E3554266F, E325421B6, E9A308830, EA1309335, E4D64AAC4, EB89DB88A, E2B548BCC, E233EEC59));
        /// <summary>@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E9A0E44B4 => _9a0e44b4 ?? (_9a0e44b4 = new SamplesExpression("decimal", EA23B1BEC, E58E826EE, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 0, 4~18, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E9B8D06B5 => _9b8d06b5 ?? (_9b8d06b5 = new SamplesExpression("integer", EC8E67366, EFE690A74, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0~7, 9, 10, 12~17, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EA02B52DF => _a02b52df ?? (_a02b52df = new SamplesExpression("integer", EBEEFF1C2, ED1E68191, E9EC9D86B, E55759678, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 2, 3</summary>
        protected ISamplesExpression EA02DA6BA => _a02da6ba ?? (_a02da6ba = new SamplesExpression("integer", EC6E67040, EC7E671D3));
        /// <summary>@integer 3, 23, 33, 43, 53, 63, 73, 83, 103, 1003, …</summary>
        protected ISamplesExpression EA26DC2F6 => _a26dc2f6 ?? (_a26dc2f6 = new SamplesExpression("integer", EC7E671D3, E8FC30509, E8FC543A0, E0FD14613, E97D45AC2, E27CCEEAD, E27CF2D44, EA7DB2FB7, E4FBF9288, EB59B6876, E233EEC59));
        /// <summary>@integer 2, 22, 42, 62, 82, 102, 122, 142, 1002, …</summary>
        protected ISamplesExpression EA6A336DC => _a6a336dc ?? (_a6a336dc = new SamplesExpression("integer", EC6E67040, E8EC30376, E0ED14480, E26CCED1A, EA6DB2E24, E50BF941B, EC0C4C199, EC8C94B5F, EB69B6A09, E233EEC59));
        /// <summary>@integer 2, 12</summary>
        protected ISamplesExpression EA7D89E52 => _a7d89e52 ?? (_a7d89e52 = new SamplesExpression("integer", EC6E67040, EA0C9DB91));
        /// <summary>@integer 3, 13</summary>
        protected ISamplesExpression EA7F98EB0 => _a7f98eb0 ?? (_a7f98eb0 = new SamplesExpression("integer", EC7E671D3, E9FC9D9FE));
        /// <summary>@integer 9, 10, 19, 29, 30, 39, 49, 59, 69, 79, 109, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EA8538A54 => _a8538a54 ?? (_a8538a54 = new SamplesExpression("integer", ED1E68191, E9EC9D86B, E95C9CA40, E95C30E7B, E92C54859, E99C5535E, E19D155D1, E9DD46434, E1DCCDEEF, E21CF23D2, E49BF8916, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 10~25, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EAC450D5F => _ac450d5f ?? (_ac450d5f = new SamplesExpression("integer", E89BD17BB, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 1, 21, 31, 41, 51, 61, 71, 81, 101, 1001, …</summary>
        protected ISamplesExpression EB152352E => _b152352e ?? (_b152352e = new SamplesExpression("integer", EC9E674F9, E8DC301E3, E91C546C6, E11D14939, E95D4579C, E25CCEB87, E29CF306A, EA9DB32DD, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 0, 7~9</summary>
        protected ISamplesExpression EB1CDEA56 => _b1cdea56 ?? (_b1cdea56 = new SamplesExpression("integer", EC8E67366, E508BE571));
        /// <summary>@integer 6</summary>
        protected ISamplesExpression EB2143050 => _b2143050 ?? (_b2143050 = new SamplesExpression("integer", ECAE6768C));
        /// <summary>@integer 0, 3~17, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EB3062ECF => _b3062ecf ?? (_b3062ecf = new SamplesExpression("integer", EC8E67366, E34F3F456, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 4</summary>
        protected ISamplesExpression EB4143376 => _b4143376 ?? (_b4143376 = new SamplesExpression("integer", ECCE679B2));
        /// <summary>@integer 2</summary>
        protected ISamplesExpression EB614369C => _b614369c ?? (_b614369c = new SamplesExpression("integer", EC6E67040));
        /// <summary>@integer 3</summary>
        protected ISamplesExpression EB714382F => _b714382f ?? (_b714382f = new SamplesExpression("integer", EC7E671D3));
        /// <summary>@decimal 0.0, 2.0~3.4, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression EB73D72FE => _b73d72fe ?? (_b73d72fe = new SamplesExpression("decimal", EE005B1B8, E82B7905C, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 0</summary>
        protected ISamplesExpression EB81439C2 => _b81439c2 ?? (_b81439c2 = new SamplesExpression("integer", EC8E67366));
        /// <summary>@integer 8, 11, 80, 800</summary>
        protected ISamplesExpression EB84947E1 => _b84947e1 ?? (_b84947e1 = new SamplesExpression("integer", ED0E67FFE, E9DC9D6D8, EA8DB314A, E4B0EE30E));
        /// <summary>@integer 2, 102, 202, 302, 402, 502, 602, 702, 1002, …</summary>
        protected ISamplesExpression EB8582B29 => _b8582b29 ?? (_b8582b29 = new SamplesExpression("integer", EC6E67040, E50BF941B, EF8F99A46, E77910071, E0D73ADFC, E8D4848A7, EB49402B2, E332B68DD, EB69B6A09, E233EEC59));
        /// <summary>@integer 1</summary>
        protected ISamplesExpression EB9143B55 => _b9143b55 ?? (_b9143b55 = new SamplesExpression("integer", EC9E674F9));
        /// <summary>@integer 5, 6</summary>
        protected ISamplesExpression EBC300370 => _bc300370 ?? (_bc300370 = new SamplesExpression("integer", ECDE67B45, ECAE6768C));
        /// <summary>@integer 7, 8, 27, 28, 37, 38, 47, 48, 57, 58, 67, 68, 77, 78, 87, 88, 107, 1007, …</summary>
        protected ISamplesExpression EBEA662F3 => _bea662f3 ?? (_bea662f3 = new SamplesExpression("integer", ECBE6781F, ED0E67FFE, E93C30B55, E94C30CE8, E93C549EC, E9AC554F1, E13D14C5F, E18D1543E, E93D45476, E9ED465C7, E23CCE861, E1CCCDD5C, E23CF26F8, E22CF2565, EA3DB296B, EA0DB24B2, E53BF98D4, EB99B6EC2, E233EEC59));
        /// <summary>@integer 0~5, 7, 8, 11~15, 17, 18, 21, 101, 1001, …</summary>
        protected ISamplesExpression EBFEA5C82 => _bfea5c82 ?? (_bfea5c82 = new SamplesExpression("integer", EBCEFEE9C, ECBE6781F, ED0E67FFE, EF6CC60E9, EA3C9E04A, E96C9CBD3, E8DC301E3, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 1, 5, 7~9</summary>
        protected ISamplesExpression EC0345761 => _c0345761 ?? (_c0345761 = new SamplesExpression("integer", EC9E674F9, ECDE67B45, E508BE571));
        /// <summary>@decimal 1.1~2.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression EC0BDA170 => _c0bda170 ?? (_c0bda170 = new SamplesExpression("decimal", EED76E557, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 10.0, 102.0, 1002.0, …</summary>
        protected ISamplesExpression EC3CA1B79 => _c3ca1b79 ?? (_c3ca1b79 = new SamplesExpression("decimal", EE005B1B8, E8AF46FEE, E71D19E55, E0BB4522C, EF32F5153, E36542802, E9D308CE9, E371340C0, E5064AF7D, E619EF28D, E84A306CF, E233EEC59));
        /// <summary>@integer 21, 41, 61, 81, 101, 121, 141, 161, 1001, …</summary>
        protected ISamplesExpression EC437968E => _c437968e ?? (_c437968e = new SamplesExpression("integer", E8DC301E3, E11D14939, E25CCEB87, EA9DB32DD, E51BF95AE, EBDC4BCE0, EC9C94CF2, E55CEA684, EB39B6550, E233EEC59));
        /// <summary>@integer 0, 3~6, 9~19, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EC4967A7E => _c4967a7e ?? (_c4967a7e = new SamplesExpression("integer", EC8E67366, E8803BD20, E4FD11ACA, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 2, 22, 32, 42, 52, 62, 82, 102, 1002, …</summary>
        protected ISamplesExpression EC91BCA52 => _c91bca52 ?? (_c91bca52 = new SamplesExpression("integer", EC6E67040, E8EC30376, E90C54533, E0ED14480, E98D45C55, E26CCED1A, EA6DB2E24, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@decimal 0.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression EC9D009EA => _c9d009ea ?? (_c9d009ea = new SamplesExpression("decimal", EE005B1B8, E5064AF7D, E4A26B276, E27020E27, E1F372AC0, E7C134FF1, E9536218A, ED261C63B, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 2.0, 3.0, 4.0, 22.0, 23.0, 24.0, 32.0, 33.0, 102.0, 1002.0, …</summary>
        protected ISamplesExpression ECD444539 => _cd444539 ?? (_cd444539 = new SamplesExpression("decimal", E8AF46FEE, E71D19E55, E0BB4522C, EC4449AA8, EABBF99CF, E9A924772, E2C1311B5, EC536ACCE, E619EF28D, E84A306CF, E233EEC59));
        /// <summary>@integer 0, 4~10, 14~21, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression ECE306B2B => _ce306b2b ?? (_ce306b2b = new SamplesExpression("integer", EC8E67366, EF668FDDC, E800B9663, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 0.2~0.9, 1.1~1.8, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression ECE70A2D4 => _ce70a2d4 ?? (_ce70a2d4 = new SamplesExpression("decimal", E641140B2, E52E81D7C, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 0.1~0.9, 1.1~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression ED3125EE4 => _d3125ee4 ?? (_d3125ee4 = new SamplesExpression("decimal", E320DA9FF, E59E82881, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 2.0, 12.0, 2.00, 12.00, 2.000, 12.000, 2.0000</summary>
        protected ISamplesExpression ED379571C => _d379571c ?? (_d379571c = new SamplesExpression("decimal", E8AF46FEE, E27020E27, E9CCC1A7A, E7F3C2E35, E1F4D627E, E50BC71DF, E94D1C0CA));
        /// <summary>@decimal 6.0, 6.00, 6.000, 6.0000</summary>
        protected ISamplesExpression ED3A073F0 => _d3a073f0 ?? (_d3a073f0 = new SamplesExpression("decimal", E36542802, EB87B46B6, EF01004F2, EAB377D66));
        /// <summary>@decimal 0.0, 0.5~1.0, 1.5~2.0, 2.5~2.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression ED48DC621 => _d48dc621 ?? (_d48dc621 = new SamplesExpression("decimal", EE005B1B8, E32F3F2D7, E43D64505, E47ED7465, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 2, 12, 22, 32, 42, 52, 62, 72, 102, 1002, …</summary>
        protected ISamplesExpression ED71F448B => _d71f448b ?? (_d71f448b = new SamplesExpression("integer", EC6E67040, EA0C9DB91, E8EC30376, E90C54533, E0ED14480, E98D45C55, E26CCED1A, E28CF2ED7, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@decimal 0.0~1.5</summary>
        protected ISamplesExpression ED7798032 => _d7798032 ?? (_d7798032 = new SamplesExpression("decimal", EA9AD830F));
        /// <summary>@integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, …</summary>
        protected ISamplesExpression EDBD924C8 => _dbd924c8 ?? (_dbd924c8 = new SamplesExpression("integer", EC1002565, EAD8DD13B, E0156F279, ECFC1F627, EF0B17619, E26CCED1A, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, …</summary>
        protected ISamplesExpression EDD06EF4E => _dd06ef4e ?? (_dd06ef4e = new SamplesExpression("decimal", EA23B1BEC, E58E826EE, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, E233EEC59));
        /// <summary>@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression EDD390F19 => _dd390f19 ?? (_dd390f19 = new SamplesExpression("decimal", E320DA9FF, E59E82881, E4F64ADEA, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 0.1~0.9, 1.1~1.7, 10.1, 100.1, 1000.1, …</summary>
        protected ISamplesExpression EE2A81867 => _e2a81867 ?? (_e2a81867 = new SamplesExpression("decimal", E320DA9FF, E59E82881, E4F64ADEA, EB69DB564, E2D548EF2, E233EEC59));
        /// <summary>@integer 0, 2~10, 102~107, 1002, …</summary>
        protected ISamplesExpression EE30B61D6 => _e30b61d6 ?? (_e30b61d6 = new SamplesExpression("integer", EC8E67366, EAA2EC582, EB87FA440, EB69B6A09, E233EEC59));
        /// <summary>@integer 2~17, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EE6CB67EF => _e6cb67ef ?? (_e6cb67ef = new SamplesExpression("integer", EA52EBDA3, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0, 6~20, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EE783DF46 => _e783df46 ?? (_e783df46 = new SamplesExpression("integer", EC8E67366, E3C30DB95, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 1, 3</summary>
        protected ISamplesExpression EE7CAAE43 => _e7caae43 ?? (_e7caae43 = new SamplesExpression("integer", EC9E674F9, EC7E671D3));
        /// <summary>@integer 3, 23, 43, 63, 83, 103, 123, 143, 1003, …</summary>
        protected ISamplesExpression EE8CE77D1 => _e8ce77d1 ?? (_e8ce77d1 = new SamplesExpression("integer", EC7E671D3, E8FC30509, E0FD14613, E27CCEEAD, EA7DB2FB7, E4FBF9288, EBFC4C006, EC7C949CC, EB59B6876, E233EEC59));
        /// <summary>@decimal 1.0, 1.00, 1.000, 1.0000</summary>
        protected ISamplesExpression EE9057C68 => _e9057c68 ?? (_e9057c68 = new SamplesExpression("decimal", E46E2169F, E44E9B57D, EC8E86837, E4CDBC305));
        /// <summary>@integer 20, 30, 40, 50, 60, 70, 80, 90, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EEB8A4639 => _eb8a4639 ?? (_eb8a4639 = new SamplesExpression("integer", E8CC30050, E92C54859, E10D147A6, E96D4592F, E24CCE9F4, E2ACF31FD, EA8DB314A, E0EDE1073, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 3.0, 3.00, 3.000, 3.0000</summary>
        protected ISamplesExpression EEC063218 => _ec063218 ?? (_ec063218 = new SamplesExpression("decimal", E71D19E55, E91FC58FF, E9F3FCDDD, E9E712C17));
        /// <summary>@integer 1, 5</summary>
        protected ISamplesExpression EEDCAB7B5 => _edcab7b5 ?? (_edcab7b5 = new SamplesExpression("integer", EC9E674F9, ECDE67B45));
        /// <summary>@integer 4, 6, 9, 14, 16, 19, 24, 26, 104, 1004, …</summary>
        protected ISamplesExpression EF0161866 => _f0161866 ?? (_f0161866 = new SamplesExpression("integer", ECCE679B2, ECAE6768C, ED1E68191, EA2C9DEB7, EA4C9E1DD, E95C9CA40, E90C3069C, E92C309C2, E56BF9D8D, EB89B6D2F, E233EEC59));
        /// <summary>@decimal 0.1, 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 10.1, 100.1, 1000.1, …</summary>
        protected ISamplesExpression EF13564AD => _f13564ad ?? (_f13564ad = new SamplesExpression("decimal", EE105B34B, E45E2150C, E8BF47181, E70D19CC2, E0CB453BF, EF22F4FC0, E37542995, E9C308B56, E4F64ADEA, EB69DB564, E2D548EF2, E233EEC59));
        /// <summary>@integer 1, 5, 7~10</summary>
        protected ISamplesExpression EF35205FB => _f35205fb ?? (_f35205fb = new SamplesExpression("integer", EC9E674F9, ECDE67B45, E1D2676CB));
        /// <summary>@integer 0, 20~34, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EF419640F => _f419640f ?? (_f419640f = new SamplesExpression("integer", EC8E67366, E83188450, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 3, 4, 13, 14, 23, 24, 33, 34, 43, 44, 53, 54, 63, 64, 73, 74, 100, 1003, …</summary>
        protected ISamplesExpression EF4866074 => _f4866074 ?? (_f4866074 = new SamplesExpression("integer", EC7E671D3, ECCE679B2, E9FC9D9FE, EA2C9DEB7, E8FC30509, E90C3069C, E8FC543A0, E96C54EA5, E0FD14613, E14D14DF2, E97D45AC2, E92D452E3, E27CCEEAD, E20CCE3A8, E27CF2D44, E26CF2BB1, E52BF9741, EB59B6876, E233EEC59));
        /// <summary>@integer 4, 5, 7~20, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EF7179B11 => _f7179b11 ?? (_f7179b11 = new SamplesExpression("integer", ECCE679B2, ECDE67B45, E8B1ED530, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0, 1</summary>
        protected ISamplesExpression EF758082A => _f758082a ?? (_f758082a = new SamplesExpression("integer", EC8E67366, EC9E674F9));
        /// <summary>@integer 0, 1, 4~17, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression EFA63AF5A => _fa63af5a ?? (_fa63af5a = new SamplesExpression("integer", EC8E67366, EC9E674F9, EF568FC49, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0, 1, 11~24</summary>
        protected ISamplesExpression EFC992FDE => _fc992fde ?? (_fc992fde = new SamplesExpression("integer", EC8E67366, EC9E674F9, EF7CA23E5));
        /// <summary>@integer 1~4, 21~24, 41~44, 61~64, 101, 1001, …</summary>
        protected ISamplesExpression EFD50E885 => _fd50e885 ?? (_fd50e885 = new SamplesExpression("integer", E22D74F20, E7B17078C, E2FF66220, E67ADBEBC, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 0, 2~4, 6~17, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E0060D900 => _0060d900 ?? (_0060d900 = new SamplesExpression("integer", EC8E67366, EC1002565, E45332857, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 22.0, 102.0, 1002.0, …</summary>
        protected ISamplesExpression E00CE1EE9 => _00ce1ee9 ?? (_00ce1ee9 = new SamplesExpression("decimal", E8AF46FEE, E71D19E55, E0BB4522C, EF32F5153, E36542802, E9D308CE9, E371340C0, E3EDE2427, EC4449AA8, E619EF28D, E84A306CF, E233EEC59));
        /// <summary>@decimal 1.1~1.9, 2.1~2.7, 10.1, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E032410E9 => _032410e9 ?? (_032410e9 = new SamplesExpression("decimal", E53E81F0F, E73D9E621, E4F64ADEA, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@integer 0, 6, 16, 26, 36, 40, 46, 56, 106, 1006, …</summary>
        protected ISamplesExpression E043D1268 => _043d1268 ?? (_043d1268 = new SamplesExpression("integer", EC8E67366, ECAE6768C, EA4C9E1DD, E92C309C2, E94C54B7F, E10D147A6, E12D14ACC, E94D45609, E54BF9A67, EBA9B7055, E233EEC59));
        /// <summary>@decimal 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 3.00</summary>
        protected ISamplesExpression E0742DB5F => _0742db5f ?? (_0742db5f = new SamplesExpression("decimal", E71D19E55, E0BB4522C, EF32F5153, E36542802, E9D308CE9, E371340C0, E3EDE2427, E5064AF7D, E1F372AC0, E7C134FF1, E9536218A, ED261C63B, EEB8497D4, E59F58855, E731859EE, E91FC58FF));
        /// <summary>@decimal 2.0, 22.0, 32.0, 42.0, 52.0, 62.0, 82.0, 102.0, 1002.0, …</summary>
        protected ISamplesExpression E0943D865 => _0943d865 ?? (_0943d865 = new SamplesExpression("decimal", E8AF46FEE, EC4449AA8, E2C1311B5, E1BEBFE2E, ECE227343, E35802B84, E8BEA5A8A, E619EF28D, E84A306CF, E233EEC59));
        /// <summary>@decimal 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 111.0, 1011.0, …</summary>
        protected ISamplesExpression E128CE41A => _128ce41a ?? (_128ce41a = new SamplesExpression("decimal", E4A26B276, E27020E27, E1F372AC0, E7C134FF1, E9536218A, ED261C63B, EEB8497D4, E59F58855, E8AFD045D, E2BCFA0CB, E233EEC59));
        /// <summary>@decimal 3.0, 23.0, 43.0, 63.0, 83.0, 103.0, 123.0, 143.0, 1003.0, …</summary>
        protected ISamplesExpression E12D0CC0E => _12d0cc0e ?? (_12d0cc0e = new SamplesExpression("decimal", E71D19E55, EABBF99CF, E02C92C95, E1BBDF62B, E72C788F1, EDAC25B46, EAB9594D8, E2E894D42, E9D2807A8, E233EEC59));
        /// <summary>@integer 0~5, 7, 8, 11~15, 17, 18, 20, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E14FF0595 => _14ff0595 ?? (_14ff0595 = new SamplesExpression("integer", EBCEFEE9C, ECBE6781F, ED0E67FFE, EF6CC60E9, EA3C9E04A, E96C9CBD3, E8CC30050, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 2~4</summary>
        protected ISamplesExpression E15E3D309 => _15e3d309 ?? (_15e3d309 = new SamplesExpression("integer", EC1002565));
        /// <summary>@integer 3, 4, 103, 104, 203, 204, 303, 304, 403, 404, 503, 504, 603, 604, 703, 704, 1003, …</summary>
        protected ISamplesExpression E16FDB0A4 => _16fdb0a4 ?? (_16fdb0a4 = new SamplesExpression("integer", EC7E671D3, ECCE679B2, E4FBF9288, E56BF9D8D, EF9F99BD9, EFAF99D6C, E7690FEDE, E79910397, E0E73AF8F, E0B73AAD6, E8C484714, E8B484581, EB5940445, EAE93F940, E322B674A, E2D2B5F6B, EB59B6876, E233EEC59));
        /// <summary>@decimal 0.2~0.9, 1.2~1.9, 10.2, 100.2, 1000.2, …</summary>
        protected ISamplesExpression E17DD8CF2 => _17dd8cf2 ?? (_17dd8cf2 = new SamplesExpression("decimal", E641140B2, EDC48E52A, E4E64AC57, EB99DBA1D, E2C548D5F, E233EEC59));
        /// <summary>@decimal 0.4, 0.6, 0.9, 1.4, 1.6, 1.9, 2.4, 2.6, 10.4, 100.4, 1000.4, …</summary>
        protected ISamplesExpression E1B284677 => _1b284677 ?? (_1b284677 = new SamplesExpression("decimal", EE405B804, EE605BB2A, ED905A6B3, E42E21053, E44E21379, E4DE221A4, E8EF4763A, E8CF47314, E4C64A931, EB39DB0AB, E2A548A39, E233EEC59));
        /// <summary>@integer 21~36, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E1C8A142B => _1c8a142b ?? (_1c8a142b = new SamplesExpression("integer", EFF1A15EF, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 0, 2, 3, 5~17, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E1CCB859A => _1ccb859a ?? (_1ccb859a = new SamplesExpression("integer", EC8E67366, EC6E67040, EC7E671D3, EC1ACAAAC, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 1.0, 21.0, 31.0, 41.0, 51.0, 61.0, 81.0, 101.0, 1001.0, …</summary>
        protected ISamplesExpression E1F378BF0 => _1f378bf0 ?? (_1f378bf0 = new SamplesExpression("decimal", E46E2169F, E55712385, E9B845998, ED7D9A4DF, E91468072, E46AD7DE1, EC915FF3B, E2FD39D10, E4816C5DE, E233EEC59));
        /// <summary>@integer 2~10</summary>
        protected ISamplesExpression E20ACF1F6 => _20acf1f6 ?? (_20acf1f6 = new SamplesExpression("integer", EAA2EC582));
        /// <summary>@decimal 0.0, 1.0, 0.00, 1.00, 0.000, 1.000, 0.0000, 1.0000</summary>
        protected ISamplesExpression E214176C0 => _214176c0 ?? (_214176c0 = new SamplesExpression("decimal", EE005B1B8, E46E2169F, E30F67918, E44E9B57D, E3C00B9F8, EC8E86837, E3D2475D8, E4CDBC305));
        /// <summary>@decimal 0.2, 1.2, 2.2, 3.2, 4.2, 5.2, 6.2, 7.2, 10.2, 100.2, 1000.2, …</summary>
        protected ISamplesExpression E21525782 => _21525782 ?? (_21525782 = new SamplesExpression("decimal", EE205B4DE, E48E219C5, E88F46CC8, E6FD19B2F, E0DB45552, EF52F5479, E345424DC, E9B3089C3, E4E64AC57, EB99DBA1D, E2C548D5F, E233EEC59));
        /// <summary>@integer 1, 2, 5, 7, 8, 11, 12, 15, 17, 18, 20~22, 25, 101, 1001, …</summary>
        protected ISamplesExpression E2170F568 => _2170f568 ?? (_2170f568 = new SamplesExpression("integer", EC9E674F9, EC6E67040, ECDE67B45, ECBE6781F, ED0E67FFE, E9DC9D6D8, EA0C9DB91, EA1C9DD24, EA3C9E04A, E96C9CBD3, E8B1ACF7F, E91C3082F, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@decimal 1.0, 11.0, 1.00, 11.00, 1.000, 11.000, 1.0000</summary>
        protected ISamplesExpression E228C2FD7 => _228c2fd7 ?? (_228c2fd7 = new SamplesExpression("decimal", E46E2169F, E4A26B276, E44E9B57D, E00EAA432, EC8E86837, E73602F26, E4CDBC305));
        /// <summary>@integer 2~9, 22~29, 102, 1002, …</summary>
        protected ISamplesExpression E24E4BF2D => _24e4bf2d ?? (_24e4bf2d = new SamplesExpression("integer", EC4002A1E, EA88DC95C, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@integer 1000000, …</summary>
        protected ISamplesExpression E260F6011 => _260f6011 ?? (_260f6011 = new SamplesExpression("integer", EBDD71B71, E233EEC59));
        /// <summary>@integer 0, 11~25, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E263C5779 => _263c5779 ?? (_263c5779 = new SamplesExpression("integer", EC8E67366, EF6CA2252, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@decimal 2.0, 2.00, 2.000, 2.0000</summary>
        protected ISamplesExpression E26C984B0 => _26c984b0 ?? (_26c984b0 = new SamplesExpression("decimal", E8AF46FEE, E9CCC1A7A, E1F4D627E, E94D1C0CA));
        /// <summary>@integer 1, 2, 21, 22, 31, 32, 41, 42, 51, 52, 61, 62, 71, 72, 81, 82, 101, 1001, …</summary>
        protected ISamplesExpression E26F9940B => _26f9940b ?? (_26f9940b = new SamplesExpression("integer", EC9E674F9, EC6E67040, E8DC301E3, E8EC30376, E91C546C6, E90C54533, E11D14939, E0ED14480, E95D4579C, E98D45C55, E25CCEB87, E26CCED1A, E29CF306A, E28CF2ED7, EA9DB32DD, EA6DB2E24, E51BF95AE, EB39B6550, E233EEC59));
        /// <summary>@integer 0~2, 4~16, 100, 1000, 10000, 100000, 1000000, …</summary>
        protected ISamplesExpression E2B0C4A40 => _2b0c4a40 ?? (_2b0c4a40 = new SamplesExpression("integer", EBBEFED09, EF468FAB6, E52BF9741, EB49B66E3, E23A2DE29, E3263A15B, EBDD71B71, E233EEC59));
        /// <summary>@integer 2, 3, 22, 23, 32, 33, 42, 43, 52, 53, 62, 63, 72, 73, 82, 83, 102, 1002, …</summary>
        protected ISamplesExpression E2B1D626B => _2b1d626b ?? (_2b1d626b = new SamplesExpression("integer", EC6E67040, EC7E671D3, E8EC30376, E8FC30509, E90C54533, E8FC543A0, E0ED14480, E0FD14613, E98D45C55, E97D45AC2, E26CCED1A, E27CCEEAD, E28CF2ED7, E27CF2D44, EA6DB2E24, EA7DB2FB7, E50BF941B, EB69B6A09, E233EEC59));
        /// <summary>@integer 3~10, 103~110, 1003, …</summary>
        protected ISamplesExpression E2C0A6AD4 => _2c0a6ad4 ?? (_2c0a6ad4 = new SamplesExpression("integer", E37F3F90F, E18686237, EB59B6876, E233EEC59));
        /// <summary>@decimal 0.0, 0.2~1.0, 1.2~1.7, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …</summary>
        protected ISamplesExpression E2C671DE9 => _2c671de9 ?? (_2c671de9 = new SamplesExpression("decimal", EE005B1B8, E65961D56, ECE48CF20, E5064AF7D, EB79DB6F7, E2E549085, E31AAB26F, EB6B8E1CD, EE103CB87, E233EEC59));
        /// <summary>@decimal 3.0, 4.0, 9.0, 23.0, 24.0, 29.0, 33.0, 34.0, 103.0, 1003.0, …</summary>
        protected ISamplesExpression E2ED8B14F => _2ed8b14f ?? (_2ed8b14f = new SamplesExpression("decimal", E71D19E55, E0BB4522C, E3EDE2427, EABBF99CF, E9A924772, E4C7FAE6D, EC536ACCE, E2D718E33, EDAC25B46, E9D2807A8, E233EEC59));

    }

}
