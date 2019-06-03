// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
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
        /// Resolve <paramref name="line"/> into <see cref="LineResourceStream"/>.
        /// If Stream is provided, then its open. Caller must close the stream.
        /// 
        /// If <paramref name="line"/> has <see cref="ICulturePolicy"/>, the applies the culture that is active in the
        /// executing context.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result</returns>
        LineResourceStream ResolveStream(ILine line);

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineResourceBytes"/> with arguments applied.
        /// 
        /// If <paramref name="line"/> has <see cref="ICulturePolicy"/>, the applies the culture that is active in the
        /// executing context.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>reslut</returns>
        LineResourceBytes ResolveBytes(ILine line);
    }
}

namespace Lexical.Localization
{
    public static partial class ILineExtensions
    {

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineResourceStream"/>.
        /// If Stream is provided, then its open. Caller must close the stream.
        /// 
        /// If <paramref name="line"/> has <see cref="ICulturePolicy"/>, the applies the culture that is active in the
        /// executing context.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result</returns>
        public static LineResourceStream ResolveStream(this ILine line)
        {
            LineResourceStream result = new LineResourceStream(line, (Exception)null, LineStatus.ResolveFailedNoResourceResolver | LineStatus.CultureFailedNoResult | LineStatus.ResourceFailedNoResult);
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

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineResourceBytes"/>.
        /// If Bytes is provided, then its open. Caller must close the stream.
        /// 
        /// If <paramref name="line"/> has <see cref="ICulturePolicy"/>, the applies the culture that is active in the
        /// executing context.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result</returns>
        public static LineResourceBytes ResolveBytes(this ILine line)
        {
            LineResourceBytes result = new LineResourceBytes(line, (Exception)null, LineStatus.ResolveFailedNoResourceResolver | LineStatus.CultureFailedNoResult | LineStatus.ResourceFailedNoResult);
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

    }
}