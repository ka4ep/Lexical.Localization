//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
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
        /// Read file as a stream of elements that describe a hierarchical structure.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Key, string>> Read();
    }


    /// <summary>
    /// A structured document that contains a localization file.
    /// </summary>
    public interface ILocalizationFileTokenizer : IDisposable
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
        IEnumerable<Token> Read();
    }

    public struct Token
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
        public static Token Begin(string section) => new Token { Kind = TokenKind.BeginSection, Value = section };

        /// <summary>
        /// Create element that describes ending of a section.
        /// </summary>
        /// <returns></returns>
        public static Token End() => new Token { Kind = TokenKind.EndSection };

        /// <summary>
        /// Create element that describes key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Token KeyValue(string key, string value) => new Token { Kind = TokenKind.KeyValue, Key = key, Value = value };

        /// <summary>
        /// Element type.
        /// </summary>
        public TokenKind Kind;

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
            switch (Kind)
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
        public static ILocalizationFileTokenizer Decorate(this ILocalizationFileTokenizer readable, Func<IEnumerable<Token>, IEnumerable<Token>> adapter)
            => new LocalizationFileReadableDecorator(readable, adapter);

        /// <summary>
        /// Decorate the enumerable of text elements.
        /// </summary>
        /// <param name="readable"></param>
        /// <param name=initialSections"></param>
        /// <returns></returns>
        public static ILocalizationFileTokenizer DecorateSections(this ILocalizationFileTokenizer readable, IReadOnlyDictionary<string, string> initialSections)
            => initialSections == null ? readable : new InitialSections(readable, initialSections);

        /// <summary>
        /// Convert <see cref="ILocalizationFileTokenizer"/> to key-values.
        /// </summary>
        /// <param name="textFile"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, string> ToDictionary(this ILocalizationFileTokenizer textFile, IDictionary<string, string> dst = default, string sectionSeparator = default)
        {
            if (dst == null) dst = new Dictionary<string, string>();
            if (sectionSeparator == null) sectionSeparator = ":";
            WriteToDictionary(textFile, dst, sectionSeparator);
            return (IReadOnlyDictionary<string, string>)dst;
        }

        public static IDictionary<string, string> WriteToDictionary(this ILocalizationFileTokenizer textFile, IDictionary<string, string> dictionary, string sectionSeparator)
        {
            List<string> stack = new List<string>();
            foreach (var element in textFile.Read())
            {
                switch (element.Kind)
                {
                    case Token.TokenKind.BeginSection: stack.Add(element.Value); break;
                    case Token.TokenKind.EndSection: if (stack.Count > 0) stack.RemoveAt(stack.Count - 1); break;
                    case Token.TokenKind.KeyValue:
                        string full_key = stack.Count == 0 ? element.Key : string.Join(sectionSeparator, stack) + (string.IsNullOrEmpty(element.Key) ? "" : sectionSeparator + element.Key);
                        dictionary[full_key] = element.Value;
                        break;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Convert <see cref="ILocalizationFileTokenizer"/> to key-values.
        /// </summary>
        /// <param name="textFile"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, string> ToDictionaryAndClose(this ILocalizationFileTokenizer textFile, IDictionary<string, string> dst = default, string sectionSeparator = default)
        {
            if (dst == null) dst = new Dictionary<string, string>();
            if (sectionSeparator == null) sectionSeparator = ":";
            WriteToDictionary(textFile, dst, sectionSeparator);
            (textFile as IDisposable)?.Dispose();
            return (IReadOnlyDictionary<string, string>)dst;
        }

        /// <summary>
        /// Convert <see cref="ILocalizationFileTokenizer"/> to <see cref="IAsset"/>.
        /// </summary>
        /// <param name="textFile"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this ILocalizationFileTokenizer textFile)
            => new LocalizationStringAsset(textFile.ToDictionary(), textFile.NamePolicy);

        /// <summary>
        /// Convert <see cref="ILocalizationFileTokenizer"/> to <see cref="IAsset"/>.
        /// </summary>
        /// <param name="textFile"></param>
        /// <returns></returns>
        public static IAsset ToAssetAndClose(this ILocalizationFileTokenizer textFile)
            => new LocalizationStringAsset(
                  source:     textFile.ToDictionaryAndClose(dst: null, sectionSeparator: null), 
                  namePolicy: textFile.NamePolicy
               );
    }


    class LocalizationFileReadableDecorator : ILocalizationFileTokenizer
    {
        ILocalizationFileTokenizer source;
        Func<IEnumerable<Token>, IEnumerable<Token>> adapter;

        public LocalizationFileReadableDecorator(ILocalizationFileTokenizer source, Func<IEnumerable<Token>, IEnumerable<Token>> adapter)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(adapter));
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }
        public IAssetKeyNamePolicy NamePolicy => source.NamePolicy;
        public void Dispose() => source.Dispose();
        public IEnumerable<Token> Read() => adapter(source.Read());
    }

    class InitialSections : ILocalizationFileTokenizer
    {
        ILocalizationFileTokenizer source;
        IReadOnlyDictionary<string, string> initialSections;
        public InitialSections(ILocalizationFileTokenizer source, IReadOnlyDictionary<string, string> initialSections)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.initialSections = initialSections ?? throw new ArgumentNullException(nameof(initialSections));
        }
        public IAssetKeyNamePolicy NamePolicy => source.NamePolicy;
        public void Dispose() => source.Dispose();
        public IEnumerable<Token> Read()
        {
            string[] keys = initialSections.Keys.ToArray();
            Array.Sort(keys);
            foreach (string key in keys) yield return Token.Begin(initialSections[key]);
            foreach (Token te in source.Read()) yield return te;
            foreach (string key in keys) yield return Token.End();
        }
    }


}
