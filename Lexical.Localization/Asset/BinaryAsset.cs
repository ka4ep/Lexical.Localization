// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Lexical.Localization.Binary;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryAsset : IBinaryAsset
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineBinaryBytes GetBytes(ILine key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineBinaryStream GetStream(ILine key)
        {
            throw new NotImplementedException();
        }
    }
}
