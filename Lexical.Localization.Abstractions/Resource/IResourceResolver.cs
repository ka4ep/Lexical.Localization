// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;
using System;

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Resolver that resolves <see cref="ILine"/> into binary resource. 
    /// </summary>
    public interface IResourceResolver
    {
        /// <summary>
        /// Resolve <paramref name="line"/> into bytes.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromKey"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineResourceBytes.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResourceResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        LineResourceBytes ResolveBytes(ILine line);

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineResourceStream"/>.
        /// If Stream (<see cref="LineResourceStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromKey"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineResourceStream.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResourceResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        LineResourceStream ResolveStream(ILine line);
    }
}

namespace Lexical.Localization
{
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Resolve <paramref name="line"/> into bytes.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromKey"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineResourceBytes.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResourceResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        public static LineResourceBytes ResolveBytes(this ILine line)
        {
            LineResourceBytes result = new LineResourceBytes(line, (Exception)null, LineStatus.ResolveFailedNoResourceResolver);
            for (ILine k = line; k != null; k = k.GetPreviousPart())
            {
                IResourceResolver resolver;
                if (k is ILineResourceResolver resolverAssigned && ((resolver = resolverAssigned.ResourceResolver) != null))
                {
                    LineResourceBytes str = resolver.ResolveBytes(line);

                    // Return bytes
                    if (str.Value != null) return str;

                    // Got better code, move on
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineResourceStream"/>.
        /// If Stream (<see cref="LineResourceStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromKey"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineResourceStream.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResourceResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        public static LineResourceStream ResolveStream(this ILine line)
        {
            LineResourceStream result = new LineResourceStream(line, (Exception)null, LineStatus.ResolveFailedNoResourceResolver);
            for (ILine k = line; k != null; k = k.GetPreviousPart())
            {
                IResourceResolver resolver;
                if (k is ILineResourceResolver resolverAssigned && ((resolver = resolverAssigned.ResourceResolver) != null))
                {
                    LineResourceStream str = resolver.ResolveStream(line);

                    // Return an open stream
                    if (str.Value != null) return str;

                    // Got better code, move on
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

    }

}