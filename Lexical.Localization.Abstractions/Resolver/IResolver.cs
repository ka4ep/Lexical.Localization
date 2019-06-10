// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           16.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.Resolver
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
    public interface IGenericResolver : IResolver
    {
        /// <summary>
        /// Resolve <paramref name="identifier"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="identifier"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns><typeparamref name="T"/> or null</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        /// <exception cref="LocalizationException">If resolve fails.</exception>
        T Resolve<T>(string identifier);

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="result"></param>
        /// <returns>true if was resolved with result</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        bool TryResolve<T>(string identifier, out T result);
    }

    /// <summary>
    /// This interface signals that the resolver can resolve parameters into line arguments, which can be instantiated to line parts.
    /// </summary>
    public interface IParameterResolver : IResolver
    {
        /// <summary>
        /// The parameters this resolver is capable of resolving.
        /// </summary>
        string[] ParameterNames { get; }

        /// <summary>
        /// Try to resolve parameter into line arguments. 
        /// 
        /// For example parameter "Culture" is resolved to <see cref="ILineArgument"/> that produces <see cref="ILineCulture"/> with <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="previous">(optional) previous parts</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLineArgument"></param>
        /// <returns></returns>
        bool TryResolveParameter(ILine previous, string parameterName, string parameterValue, out ILineArgument resolvedLineArgument);
    }

    /// <summary>
    /// Composition of resolvers that can enumerate component resolvers.
    /// </summary>
    public interface IResolverEnumerable : IResolver, IEnumerable<IResolver>
    {
    }
}

namespace Lexical.Localization
{
    using Lexical.Localization.Resolver;

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
            if (resolver is IGenericResolver generic_) return generic_.Resolve<T>(identifier);

            throw new LocalizationException($"Doesn't implement {nameof(IResolver)}<{nameof(T)}>");

            bool TryGeneric(out T r)
            {
                if (resolver is IGenericResolver generic) { r = generic.Resolve<T>(identifier); return true; }
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
            if (resolver is IGenericResolver generic && generic.TryResolve<T>(identifier, out result)) return true;
            result = default; return false;
        }

        /// <summary>
        /// Try to resolve parameter into line arguments. 
        /// 
        /// For example parameter "Culture" is resolved to <see cref="ILineArgument"/> that produces <see cref="ILineCulture"/> with <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="previous">(optional) previous parts</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLineArgument"></param>
        /// <returns></returns>
        public static bool TryResolveParameter(this IResolver resolver, ILine previous, string parameterName, string parameterValue, out ILineArgument resolvedLineArgument)
        {
            if (resolver is IParameterResolver parameterResolver) return parameterResolver.TryResolveParameter(previous, parameterName, parameterValue, out resolvedLineArgument);
            resolvedLineArgument = default;
            return false;
        }

    }

}
