// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolver
    /// </summary>
    public interface IResolver
    {

    }

    /// <summary>
    /// Resolves name to <typeparamref name="T"/>.
    /// </summary>
    public interface IResolver<T> : IResolver
    {
        /// <summary>
        /// Resolve <paramref name="identifier"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns><typeparamref name="T"/> or null</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        /// <exception cref="LocalizationException">If resolve fails.</exception>
        T Resolve(string identifier);

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="result"></param>
        /// <returns>true if was resolved with result</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        bool TryResolve(string identifier, out T result);
    }

    /// <summary>
    /// Resolves identifiers to types.
    /// </summary>
    public interface IResolverGeneric : IResolver
    {
        /// <summary>
        /// Resolve <paramref name="identifier"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns><typeparamref name="T"/> or null</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        /// <exception cref="LocalizationException">If resolve fails.</exception>
        T Resolve<T>(string identifier);

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="result"></param>
        /// <returns>true if was resolved with result</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        bool TryResolve<T>(string identifier, out T result);
    }

    /// <summary>
    /// This interface signals that the resolver can resolve specific parameter name into <typeparamref name="T"/>.
    /// </summary>
    public interface IParameterResolver<T> : IResolver<T>
    {
        /// <summary>
        /// Name of parameter that can be resolved into an instance.
        /// </summary>
        String ParameterName { get; }
    }

    /// <summary>
    /// Composition of resolvers that can enumerate component resolvers.
    /// </summary>
    public interface IResolverEnumerable : IResolver, IEnumerable<IResolver>
    {
    }

    /// <summary></summary>
    public static class IResolverExtensions
    {
        /// <summary>
        /// Resolve <paramref name="identifier"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="identifier"></param>
        /// <returns><typeparamref name="T"/> or null</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        /// <exception cref="ArgumentException">If cannot be casted to <typeparamref name="T"/> resolver</exception>
        public static T Resolve<T>(this IResolver resolver, string identifier)
        {
            T result;
            if (resolver is IResolver<T> _resolver)
            {
                try
                {
                    return _resolver.Resolve(identifier);
                } catch (Exception) when(TryGeneric(out result))
                {
                    return result;
                }
            }
            if (resolver is IResolverGeneric generic_) return generic_.Resolve<T>(identifier);

            throw new LocalizationException($"Doesn't implement {nameof(IResolver)}<{nameof(T)}>");

            bool TryGeneric(out T r)
            {
                if (resolver is IResolverGeneric generic) { r = generic.Resolve<T>(identifier); return true; }
                r = default;
                return false;
            }
        }

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="identifier"></param>
        /// <param name="result"></param>
        /// <returns>true if was resolved with result</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        public static bool TryResolve<T>(this IResolver resolver, string identifier, out T result)
        {
            if (resolver is IResolver<T> _resolver && _resolver.TryResolve(identifier, out result)) return true;
            if (resolver is IResolverGeneric generic && generic.TryResolve<T>(identifier, out result)) return true;
            result = default; return false;
        }


        /// <summary>
        /// Resolves parameters and keys into instances.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is resolved to <see cref="ILineCulture"/></item>
        ///     <item>Parameter "Value" is resolved to <see cref="ILineValue"/></item>
        ///     <item>Parameter "StringFormat" is resolved to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is resolved to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is resolved to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is resolved to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// 
        /// Parts that don't need resolving may be need to be cloned. 
        /// If the line appender of <paramref name="line"/> fails cloning, then the operation 
        /// fails and returns false.
        /// 
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLine"></param>
        /// <returns>true if operation was successful, <paramref name="resolvedLine"/> contains new line. If operation failed, <paramref name="resolvedLine"/> contains the reference to <paramref name="line"/>.</returns>
        public static bool TryResolveParameters(this IResolver resolver, string parameterName, string parameterValue, ILine prevParts, out ILine resolvedLine)
        {/*
            // 1. Scan line to A) test if resolving is needed, B) if clone is possible
            bool resolvingNeeded;
            ILineFactory appender;

            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                    foreach (ILineParameter lineParameter in lineParameters)
                    {

                    }
                if (l is ILineParameter parameter)
                {

                }
            }
            */
            // TODO IMPLEMENT
            resolvedLine = null;
            return false;
        }


    }

}
