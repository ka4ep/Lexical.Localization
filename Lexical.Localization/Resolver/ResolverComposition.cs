using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Resolver
{
    /// <summary>
    /// Composition of <see cref="IResolver"/>.
    /// </summary>
    public class ResolverComposition : IResolver, IGenericResolver, IParameterResolver, IResolverEnumerable
    {
        /// <summary>
        /// Component resolvers.
        /// </summary>
        protected List<IResolver> resolvers = new List<IResolver>();

        /// <summary>
        /// Resolvers by resolved type
        /// </summary>
        protected MapList<Type, IResolver> resolversByType = new MapList<Type, IResolver>();

        /// <summary>
        /// Resolvers by parameter name.
        /// </summary>
        protected MapList<string, IParameterResolver> resolversByParameterName = new MapList<string, IParameterResolver>();

        /// <summary>
        /// Generic resolvers.
        /// </summary>
        protected List<IGenericResolver> genericResolvers = new List<IGenericResolver>();

        /// <summary>
        /// The parameters this resolver is capable of resolving.
        /// </summary>
        public string[] ParameterNames { get; protected set; }

        /// <summary>
        /// Is in read-only state.
        /// </summary>
        bool isReadonly;

        /// <summary>
        /// Add resolver to composition.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public ResolverComposition Add(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (isReadonly) throw new InvalidOperationException("read-only");

            resolvers.Add(resolver);

            // Add to resolvers by type
            Type[] intfs = resolver.GetType().GetInterfaces();
            foreach (Type itype in intfs.Where(i => i.IsGenericType && typeof(IResolver<>).Equals(i.GetGenericTypeDefinition())))
            {
                Type[] paramTypes = itype.GetGenericArguments();
                var resolveType = paramTypes[0];
                resolversByType.Add(resolveType, resolver);
            }

            // Add to resolvers by parameter name
            if (resolver is IParameterResolver parameterResolver_)
            {
                foreach (string parameterName in parameterResolver_.ParameterNames)
                    resolversByParameterName.Add(parameterName, parameterResolver_);
            }

            // Add to generic resolvers
            if (resolver is IGenericResolver genericResolver)
                genericResolvers.Add(genericResolver);

            // Update parameter names
            List<string> names = new List<string>();
            foreach (var r in resolvers)
                if (r is IParameterResolver parameterResolver)
                    names.AddRange(parameterResolver.ParameterNames);
            ParameterNames = names.Distinct().ToArray();
            return this;
        }

        /// <summary>
        /// Set composition to readonly state
        /// </summary>
        /// <returns></returns>
        public ResolverComposition ReadOnly()
        {
            isReadonly = true;
            return this;
        }

        /// <summary>
        /// Resolve <paramref name="identifier"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="identifier"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns><typeparamref name="T"/> or null</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        /// <exception cref="LocalizationException">If resolve fails.</exception>
        public T Resolve<T>(string identifier)
        {
            StructList4<Exception> errors = new StructList4<Exception>();

            List<IResolver> list = resolversByType.TryGetList(typeof(T));
            if (list != null)
                foreach (IResolver resolver in list)
                {
                    try
                    {
                        return ((IResolver<T>)resolver).Resolve(identifier);
                    } catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

            foreach (IGenericResolver genericResolver in genericResolvers)
                try
                {
                    return genericResolver.Resolve<T>(identifier);
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }

            string msg = $"Failed to resolve {identifier} to {typeof(T).FullName}.";
            if (errors.Count == 0) throw new LocalizationException(msg);
            throw new LocalizationException(msg, new AggregateException(errors));
        }

        /// <summary>
        /// Try resolve into value.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>true if was resolved with result</returns>
        /// <exception cref="ObjectDisposedException">resolver is disposed</exception>
        public bool TryResolve<T>(string identifier, out T result)
        {
            List<IResolver> list = resolversByType.TryGetList(typeof(T));
            if (list != null)
                foreach(IResolver resolver in list)
                {
                    if (((IResolver<T>)resolver).TryResolve(identifier, out result)) return true;
                }

            foreach (IGenericResolver genericResolver in genericResolvers)
                if (genericResolver.TryResolve<T>(identifier, out result)) return true;

            result = default;
            return false;
        }

        /// <summary>
        /// Try to resolve parameter into line arguments. 
        /// </summary>
        /// <param name="previous">(optional) previous parts</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLineArguments"></param>
        /// <returns></returns>
        public bool TryResolveParameter(ILine previous, string parameterName, string parameterValue, out ILineArguments resolvedLineArguments)
        {
            List<IParameterResolver> list = resolversByParameterName.TryGetList(parameterName);
            if (list != null)
                foreach (IParameterResolver resolver in list)
                    if (resolver.TryResolveParameter(previous, parameterName, parameterValue, out resolvedLineArguments)) return true;

            resolvedLineArguments = default;
            return false;
        }

        /// <summary>
        /// Get component resolvers
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IResolver> GetEnumerator()
            => resolvers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => resolvers.GetEnumerator();
    }
}
