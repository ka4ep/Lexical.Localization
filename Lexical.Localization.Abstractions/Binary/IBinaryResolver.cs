// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Binary;
using System;

namespace Lexical.Localization.Binary
{
    /// <summary>
    /// Resolver that resolves <see cref="ILine"/> into binary resource. 
    /// </summary>
    public interface IBinaryResolver
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
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryBytes.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoBinaryResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        LineBinaryBytes ResolveBytes(ILine line);

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineBinaryStream"/>.
        /// If Stream (<see cref="LineBinaryStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryStream.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoBinaryResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        LineBinaryStream ResolveStream(ILine line);
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
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryBytes.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoBinaryResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        public static LineBinaryBytes ResolveBytes(this ILine line)
        {
            LineBinaryBytes result = new LineBinaryBytes(line, (Exception)null, LineStatus.ResolveFailedNoBinaryResolver);
            for (ILine k = line; k != null; k = k.GetPreviousPart())
            {
                IBinaryResolver resolver;
                if (k is ILineBinaryResolver resolverAssigned && ((resolver = resolverAssigned.BinaryResolver) != null))
                {
                    LineBinaryBytes str = resolver.ResolveBytes(line);

                    // Return bytes
                    if (str.Value != null) return str;

                    // Got better code, move on
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineBinaryStream"/>.
        /// If Stream (<see cref="LineBinaryStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryStream.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoBinaryResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result status</returns>
        public static LineBinaryStream ResolveStream(this ILine line)
        {
            LineBinaryStream result = new LineBinaryStream(line, (Exception)null, LineStatus.ResolveFailedNoBinaryResolver);
            for (ILine k = line; k != null; k = k.GetPreviousPart())
            {
                IBinaryResolver resolver;
                if (k is ILineBinaryResolver resolverAssigned && ((resolver = resolverAssigned.BinaryResolver) != null))
                {
                    LineBinaryStream str = resolver.ResolveStream(line);

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