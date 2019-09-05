// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           13.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Lexical.Localization.Internal;
using Lexical.Localization.Binary;

namespace Lexical.Localization
{
    /// <summary>
    /// <see cref="DynamicMetaObject" /> implementation for <see cref="ILine" /> interfaces.
    /// </summary>
    public class LineDynamicMetaObject : DynamicMetaObject
    {
        /// <summary>
        /// Library of mehtods
        /// </summary>
        protected DynamicObjectLibrary library;

        /// <summary>
        /// Create dynamic meta object for <see cref="ILine"/>.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="expression"></param>
        /// <param name="restrictions"></param>
        public LineDynamicMetaObject(DynamicObjectLibrary library, Expression expression, BindingRestrictions restrictions) : base(expression, restrictions)
        {
            this.library = library;
        }

        /// <summary>
        /// Create dynamic meta object for <see cref="ILine"/>.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="expression"></param>
        /// <param name="restrictions"></param>
        /// <param name="value"></param>
        public LineDynamicMetaObject(DynamicObjectLibrary library, Expression expression, BindingRestrictions restrictions, object value) : base(expression, restrictions, value)
        {
            this.library = library;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            => library.BindGetMember(binder, LimitType ?? this.Value?.GetType(), Expression) ?? createKeyMember(binder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            => library.BindInvokeMember(binder, args, LimitType ?? this.Value?.GetType(), Expression) ?? BindInlineInvokeMember(binder, args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] args)
            => library.BindGetIndex(binder, args, LimitType ?? this.Value?.GetType(), Expression) ?? base.BindGetIndex(binder, args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            var result_ = BindResolveResourceConvert(binder);
            if (result_ != null) return result_;

            MethodInfo mi = null;

            // (string) maps to obj.ToString()
            if (typeof(String).IsAssignableFrom(binder.ReturnType))
            {
                mi = (HasValue ? Value.GetType() : LimitType).GetMethods().Where(_ => _.Name == "ToString" && _.GetParameters().Length == 0).FirstOrDefault();
            }

            // (byte[]) maps to LineExtensions.GetResource(obj)
            if (typeof(byte[]).IsAssignableFrom(binder.ReturnType))
            {
                if (miGetResource == null) miGetResource = typeof(ILineExtensions).GetMethod(nameof(ILineExtensions.GetBytes));
                mi = miGetResource;
            }

            if (mi != null)
            {
                if (mi.IsStatic)
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    Type parameterType = pis[0].ParameterType;
                    Expression selfExpression = Expression;
                    if (!parameterType.IsAssignableFrom(selfExpression.Type)) selfExpression = Expression.Convert(selfExpression, parameterType);

                    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                    Expression exp = Expression.Call(mi, selfExpression);
                    return new DynamicMetaObject(exp, restrictions);
                } else
                {
                    Expression selfExpression = Expression;
                    if (!mi.ReflectedType.IsAssignableFrom(selfExpression.Type)) selfExpression = Expression.Convert(selfExpression, mi.ReflectedType);
                    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                    Expression exp = Expression.Call(selfExpression, mi);
                    return new DynamicMetaObject(exp, restrictions);
                }
            }

            return base.BindConvert(binder);
        }

        /// <summary>
        /// Method Info 
        /// </summary>
        protected static MethodInfo miBuildName, miGetResource;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        protected virtual DynamicMetaObject createKeyMember(GetMemberBinder binder)
        {            
            MethodInfo mi = typeof(ILineExtensions).GetMethod("Key", new Type[] { typeof(ILine), typeof(string) });
            BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
            Expression selfExp = Expression.Convert(Expression, mi.GetParameters()[0].ParameterType);
            Expression keyNameExp = Expression.Constant(binder.Name);
            return new DynamicMetaObject(Expression.Call(mi, selfExp, keyNameExp), restrictions);
        }

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
                if (inlineMethod == null) inlineMethod = typeof(LineExtensions).GetMethod(nameof(LineExtensions.InlineCulture), new Type[] { typeof(ILine), typeof(string), typeof(string) });
                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                restrictions.Merge(BindingRestrictions.GetTypeRestriction(args[0].Expression, typeof(String)));

                Expression selfExp = Expression.Convert(Expression, typeof(ILine));
                Expression cultureExp = Expression.Constant((binder.Name.Contains("_") ? binder.Name.Replace('_', '-') : binder.Name));
                Expression textExp = args[0].HasValue ? Expression.Constant(args[0].Value.ToString()) : (Expression)Expression.Convert(args[0].Expression, typeof(String));

                return new DynamicMetaObject(Expression.Call(inlineMethod, selfExp, cultureExp, textExp), restrictions);
            }
            return null;
        }
        static MethodInfo inlineMethod;
        static Regex lang_region_pattern = new Regex(@"^([a-z]{2,3})(_([A-Za-z]{2,7}))?$");

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
                if (miResolveResource == null) miResolveResource = typeof(ILineExtensions).GetMethod(nameof(ILineExtensions.ResolveBytes));
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
                FieldInfo fi = typeof(LineBinaryBytes).GetField(nameof(LineBinaryBytes.Value));
                exp = Expression.Field(exp, fi);
                return new DynamicMetaObject(exp, restrictions);
            }

            return null;
        }
        static MethodInfo miResolveResource;

    }

}
