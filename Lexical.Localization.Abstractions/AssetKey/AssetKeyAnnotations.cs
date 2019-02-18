// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Attribute that describes parameter name a type represents.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class AssetKeyParameterAttribute : Attribute
    {
        public readonly string ParameterName;

        public AssetKeyParameterAttribute(string parameterName)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }
    }

    /// <summary>
    /// Attribute that describes that method constructs a key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AssetKeyConstructorAttribute : Attribute
    {
        public readonly string ParameterName;

        public AssetKeyConstructorAttribute(string parameterName)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }
    }

}
