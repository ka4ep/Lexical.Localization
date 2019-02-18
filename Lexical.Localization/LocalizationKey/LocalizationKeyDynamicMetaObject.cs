// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// <see cref="DynamicMetaObject" /> implementation for <see cref="ILocalizationKey" /> interfaces.
    /// 
    /// </summary>
    public class LocalizationKeyDynamicMetaObject : AssetKeyDynamicMetaObject
    {
        public LocalizationKeyDynamicMetaObject(DynamicObjectLibrary library, Expression expression, BindingRestrictions restrictions) : base(library, expression, restrictions) { }
        public LocalizationKeyDynamicMetaObject(DynamicObjectLibrary library, Expression expression, BindingRestrictions restrictions, object value) : base(library, expression, restrictions, value) { }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
            => BindResolveResourceConvert(binder) ?? base.BindConvert(binder);

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            => library.BindInvokeMember(binder, args, LimitType ?? this.Value?.GetType(), Expression) ?? BindInlineInvokeMember(binder, args) ?? base.BindInvokeMember(binder, args); //<- makes second call to library.BindInvokeMember

        /// <summary>
        /// Tries to match to key.ResolveResource() and key.ResolveString(). (instead of the super-type's key.GetResource() and key.GetString().)
        /// </summary>
        /// <param name="binder"></param>
        /// <returns>DynamicMetaObject or null</returns>
        DynamicMetaObject BindResolveResourceConvert(ConvertBinder binder)
        {
            MethodInfo mi = null;

            // (byte[]) maps to LocalizationKeyExtensions.ResolveResource(obj)
            if (typeof(byte[]).IsAssignableFrom(binder.ReturnType))
            {
                if (miResolveResource == null) miResolveResource = typeof(LocalizationKeyExtensions).GetMethod(nameof(LocalizationKeyExtensions.ResolveResource));
                mi = miResolveResource;
            }

            if (mi != null)
            {
                ParameterInfo[] pis = mi.GetParameters();
                Type parameterType = pis[0].ParameterType;
                Expression selfExpression = Expression;
                if (!parameterType.IsAssignableFrom(selfExpression.Type)) selfExpression = Expression.Convert(selfExpression, parameterType);

                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                Expression exp = Expression.Call(mi, selfExpression);
                return new DynamicMetaObject(exp, restrictions);
            }

            return null;
        }
        static MethodInfo miResolveResource;

        /// <summary>
        /// Try to bind member to inline method.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns>Inline expression bound or null</returns>
        protected DynamicMetaObject BindInlineInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            // Call inline
            Match m = null;
            if (args.Length == 1 && typeof(String).IsAssignableFrom(args[0].LimitType) && (m = lang_region_pattern.Match(binder.Name)).Success)
            {
                if (inlineMethod == null) inlineMethod = typeof(ILocalizationKeyInlineAssignable).GetMethod(nameof(ILocalizationKeyInlineAssignable.Inline));
                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                restrictions.Merge(BindingRestrictions.GetTypeRestriction(args[0].Expression, typeof(String)));

                Expression selfExp = Expression.Convert(Expression, inlineMethod.DeclaringType);
                Expression cultureExp = Expression.Constant(binder.Name.Contains("_") ? binder.Name.Replace('_', '-') : binder.Name);
                Expression textExp = args[0].HasValue ? Expression.Constant(args[0].Value.ToString()) : (Expression)Expression.Convert(args[0].Expression, typeof(String));

                return new DynamicMetaObject(Expression.Call(selfExp, inlineMethod, cultureExp, textExp), restrictions);
            }
            return null;
        }
        static MethodInfo inlineMethod;
        static Regex lang_region_pattern = new Regex(@"^([a-z]{2,3})(_([A-Za-z]{2,7}))?$");
    }
}
