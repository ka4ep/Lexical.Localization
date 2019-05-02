// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Lexical.Localization
{
    ///// <summary>
    ///// Basic line part.
    ///// </summary>
    //public class StringLocalizerPart : LinePart, ILinePart, IStringLocalizerKey, IStringLocalizer, IStringLocalizerFactory
    //{
    //    /// <summary>
    //    /// Previous part.
    //    /// </summary>
    //    public ILinePart PreviousPart { get; protected set; }

    //    /// <summary>
    //    /// Part appender
    //    /// </summary>
    //    public ILinePartAppender Appender { get; protected set; }

    //    /// <summary>
    //    /// Create string localizer part.
    //    /// </summary>
    //    /// <param name="previousPart"></param>
    //    /// <param name="appender"></param>
    //    public StringLocalizerPart(ILinePart previousPart, ILinePartAppender appender) : base(previousPart, appender) { }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Value => "";

    //    //static RuntimeConstructor<IAssetKey, _Type> typeConstructor = new RuntimeConstructor<IAssetKey, _Type>(typeof(_Type<>));

    //    /// <summary>
    //    /// Create Resource key part.
    //    /// </summary>
    //    /// <param name="basename"></param>
    //    /// <param name="location"></param>
    //    /// <returns></returns>
    //    public IStringLocalizer Create(string basename, string location) => null; // new _Resource(new _Assembly(this, location), basename);

    //    /// <summary>
    //    /// Create Type key part.
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public IStringLocalizer Create(Type type) => null; // typeConstructor.Create(type, this);

    //    /// <summary>
    //    /// Create Culture key part.
    //    /// </summary>
    //    /// <param name="newCulture"></param>
    //    /// <returns></returns>
    //    public IStringLocalizer WithCulture(CultureInfo newCulture)
    //    {
    //        return null;
    //        /*
    //        // Find culture key
    //        IAssetKey oldCultureKey = this.FindCultureKey();
    //        // No culture key, create new
    //        if (oldCultureKey == null) return newCulture == null ? this : new _Culture(this, null, newCulture);
    //        // Old culture matches the new, return as is
    //        if (oldCultureKey?.Name == newCulture?.Name) return this;

    //        // Replace culture
    //        IAssetKey beforeCultureKey = oldCultureKey?.GetPreviousKey();
    //        if (beforeCultureKey == null) throw new InvalidOperationException("Cannot change culture when culture is the root key.");
    //        // Read parameters
    //        List<(string, string)> parameters = new List<(string, string)>();
    //        for (IAssetKey k = this; k != oldCultureKey; k = k.GetPreviousKey())
    //            if (k is IAssetKeyParameterAssigned parameterKey && parameterKey.ParameterName != null)
    //                parameters.Add((parameterKey.ParameterName, parameterKey.Name));
    //        // Assign new culture
    //        IAssetKey result = newCulture == null ? beforeCultureKey : beforeCultureKey.Culture(newCulture);
    //        // Apply parameters
    //        for (int i = parameters.Count - 1; i >= 0; i--)
    //            result = result.AppendParameter(parameters[i].Item1, parameters[i].Item2);
    //        return (IStringLocalizer)result;*/
    //    }

    //    /// <summary>
    //    /// Get localized string.
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <returns></returns>
    //    public LocalizedString this[string name]
    //    {
    //        get
    //        {
    //            return default;
    //            /*
    //            ILocalizationKey key = this.Key(name);
    //            LocalizationString printedString = key.ResolveFormulatedString();
    //            if (printedString.Value == null)
    //                return new LocalizedString(name, key.BuildName(), true);
    //            else
    //                return new LocalizedString(name, printedString.Value);
    //                */
    //        }
    //    }

    //    /// <summary>
    //    /// Create localized string with format arguments.
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <param name="arguments"></param>
    //    /// <returns></returns>
    //    public LocalizedString this[string name, params object[] arguments]
    //    {
    //        get
    //        {
    //            ILocalizationKey key = this.Key(name).Format(arguments);
    //            LocalizationString printedString = key.ResolveFormulatedString();
    //            if (printedString.Value == null)
    //                return new LocalizedString(name, key.BuildName(), true);
    //            else
    //                return new LocalizedString(name, printedString.Value);
    //        }
    //    }

    //    /// <summary>
    //    /// Get all strings as localized strings.
    //    /// </summary>
    //    /// <param name="includeParentCultures"></param>
    //    /// <returns></returns>
    //    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    //    {
    //        ILocalizationStringLinesEnumerable collections = this.FindAsset() as ILocalizationStringLinesEnumerable;
    //        if (collections == null) return null;

    //        CultureInfo ci = null;
    //        if (includeParentCultures && ((ci = this.FindCulture()) != null))
    //        {
    //            IEnumerable<LocalizedString> result = null;
    //            while (true)
    //            {
    //                IEnumerable<KeyValuePair<string, IFormulationString>> strs = collections?.GetAllStringLines(this);
    //                if (strs != null)
    //                {
    //                    IEnumerable<LocalizedString> converted = ConvertStrings(strs);
    //                    result = result == null ? converted : result.Concat(converted);
    //                }

    //                if (ci.Parent == ci || ci.Parent == null || ci.Name == ci.Parent?.Name) break;
    //            }
    //            return result;
    //        }
    //        else
    //        {
    //            IEnumerable<KeyValuePair<string, IFormulationString>> strs = collections?.GetAllStringLines(this);
    //            return strs == null ? null : ConvertStrings(strs);
    //        }
    //    }

    //    /// <summary>
    //    /// Convert strings to localized strings.
    //    /// </summary>
    //    /// <param name="lines"></param>
    //    /// <returns></returns>
    //    IEnumerable<LocalizedString> ConvertStrings(IEnumerable<KeyValuePair<string, IFormulationString>> lines)
    //    {
    //        foreach (var kp in lines)
    //        {
    //            string value = kp.Value.Text; // <- What kind of value is expected? Is formulation expected?
    //            yield return new LocalizedString(kp.Key, value);
    //        }
    //    }
    //}

    ///// <summary>
    ///// Appender that appends parts that implement <see cref="IStringLocalizer"/>.
    ///// </summary>
    //public class StringLocalizerPartAppender : LinePartAppender
    //{
    //    private readonly static ILinePartAppender instance = new StringLocalizerPartAppender().ReadOnly();

    //    /// <summary>
    //    /// Default instance
    //    /// </summary>
    //    public static ILinePartAppender Instance => instance;

    //    /// <summary>
    //    /// Create new part appender
    //    /// </summary>
    //    public StringLocalizerPartAppender()
    //    {
            
    //    }
    //}

}
