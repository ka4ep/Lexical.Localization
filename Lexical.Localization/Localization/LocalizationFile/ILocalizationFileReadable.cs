//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.LocalizationFile
{
    #region ILocalizationFileReadable
    /// <summary>
    /// A structured document that contains a localization file.
    /// </summary>
    public interface ILocalizationFileReadable : IDisposable
    {
        /// <summary>
        /// Return associated name policy. 
        /// 
        /// Name policy is either innate to the implemented file format, or it was
        /// given to the class in its constructor.
        /// </summary>
        IAssetKeyNamePolicy NamePolicy { get; }

        /// <summary>
        /// Read file as a stream of elements that describe a hierarchical structure.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TextElement> Read();
    }

    public struct TextElement
    {
        /// <summary>
        /// Token type
        /// </summary>
        public enum TokenKind
        {
            BeginSection,
            EndSection,
            KeyValue
        }

        /// <summary>
        /// Create element that describes beginning of a new section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static TextElement Begin(string section) => new TextElement { Token = TokenKind.BeginSection, Value = section };

        /// <summary>
        /// Create element that describes ending of a section.
        /// </summary>
        /// <returns></returns>
        public static TextElement End() => new TextElement { Token = TokenKind.EndSection };

        /// <summary>
        /// Create element that describes key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TextElement KeyValue(string key, string value) => new TextElement { Token = TokenKind.KeyValue, Key = key, Value = value };

        /// <summary>
        /// Element type.
        /// </summary>
        public TokenKind Token;

        /// <summary>
        /// Key
        /// </summary>
        public string Key;

        /// <summary>
        /// Value
        /// </summary>
        public string Value;

        /// <summary>
        /// Print
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Token)
            {
                case TokenKind.BeginSection: return $"BeginSection {Value}";
                case TokenKind.EndSection: return $"EndSection {Value}";
                case TokenKind.KeyValue: return $"{Key}={Value}";
            }
            return "";
        }
    }
    #endregion ILocalizationFileReadable

    public static class LocalizationFileReadableExtensions
    {
        /// <summary>
        /// Decorate the enumerable of text elements.
        /// </summary>
        /// <param name="readable"></param>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public static ILocalizationFileReadable Decorate(this ILocalizationFileReadable readable, Func<IEnumerable<TextElement>, IEnumerable<TextElement>> adapter)
            => new LocalizationFileReadableDecorator(readable, adapter);

        /// <summary>
        /// Decorate the enumerable of text elements.
        /// </summary>
        /// <param name="readable"></param>
        /// <param name=initialSections"></param>
        /// <returns></returns>
        public static ILocalizationFileReadable DecorateSections(this ILocalizationFileReadable readable, IReadOnlyDictionary<string, string> initialSections)
            => initialSections == null ? readable : new InitialSections(readable, initialSections);

        /// <summary>
        /// Convert <see cref="ILocalizationFileReadable"/> to key-values.
        /// </summary>
        /// <param name="textFile"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, string> ToDictionary(this ILocalizationFileReadable textFile, IDictionary<string, string> dst = default, string sectionSeparator = default)
        {
            if (dst == null) dst = new Dictionary<string, string>();
            if (sectionSeparator == null) sectionSeparator = ":";
            WriteToDictionary(textFile, dst, sectionSeparator);
            return (IReadOnlyDictionary<string, string>)dst;
        }

        public static IDictionary<string, string> WriteToDictionary(this ILocalizationFileReadable textFile, IDictionary<string, string> dictionary, string sectionSeparator)
        {
            List<string> stack = new List<string>();
            foreach (var element in textFile.Read())
            {
                switch (element.Token)
                {
                    case TextElement.TokenKind.BeginSection: stack.Add(element.Value); break;
                    case TextElement.TokenKind.EndSection: if (stack.Count > 0) stack.RemoveAt(stack.Count - 1); break;
                    case TextElement.TokenKind.KeyValue:
                        string full_key = stack.Count == 0 ? element.Key : string.Join(sectionSeparator, stack) + (string.IsNullOrEmpty(element.Key) ? "" : sectionSeparator + element.Key);
                        dictionary[full_key] = element.Value;
                        break;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Convert <see cref="ILocalizationFileReadable"/> to key-values.
        /// </summary>
        /// <param name="textFile"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, string> ToDictionaryAndClose(this ILocalizationFileReadable textFile, IDictionary<string, string> dst = default, string sectionSeparator = default)
        {
            if (dst == null) dst = new Dictionary<string, string>();
            if (sectionSeparator == null) sectionSeparator = ":";
            WriteToDictionary(textFile, dst, sectionSeparator);
            (textFile as IDisposable)?.Dispose();
            return (IReadOnlyDictionary<string, string>)dst;
        }

        /// <summary>
        /// Convert <see cref="ILocalizationFileReadable"/> to <see cref="IAsset"/>.
        /// </summary>
        /// <param name="textFile"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this ILocalizationFileReadable textFile)
            => new LocalizationStringDictionary(textFile.ToDictionary(), textFile.NamePolicy);

        /// <summary>
        /// Convert <see cref="ILocalizationFileReadable"/> to <see cref="IAsset"/>.
        /// </summary>
        /// <param name="textFile"></param>
        /// <returns></returns>
        public static IAsset ToAssetAndClose(this ILocalizationFileReadable textFile)
            => new LocalizationStringDictionary(
                  source:     textFile.ToDictionaryAndClose(dst: null, sectionSeparator: null), 
                  namePolicy: textFile.NamePolicy
               );
    }


    class LocalizationFileReadableDecorator : ILocalizationFileReadable
    {
        ILocalizationFileReadable source;
        Func<IEnumerable<TextElement>, IEnumerable<TextElement>> adapter;

        public LocalizationFileReadableDecorator(ILocalizationFileReadable source, Func<IEnumerable<TextElement>, IEnumerable<TextElement>> adapter)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(adapter));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }
        public IAssetKeyNamePolicy NamePolicy => source.NamePolicy;
        public void Dispose() => source.Dispose();
        public IEnumerable<TextElement> Read() => adapter(source.Read());
    }

    class InitialSections : ILocalizationFileReadable
    {
        ILocalizationFileReadable source;
        IReadOnlyDictionary<string, string> initialSections;
        public InitialSections(ILocalizationFileReadable source, IReadOnlyDictionary<string, string> initialSections)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.initialSections = initialSections ?? throw new ArgumentNullException(nameof(initialSections));
        }
        public IAssetKeyNamePolicy NamePolicy => source.NamePolicy;
        public void Dispose() => source.Dispose();
        public IEnumerable<TextElement> Read()
        {
            string[] keys = initialSections.Keys.ToArray();
            Array.Sort(keys);
            foreach (string key in keys) yield return TextElement.Begin(initialSections[key]);
            foreach (TextElement te in source.Read()) yield return te;
            foreach (string key in keys) yield return TextElement.End();
        }
    }


}
