﻿//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Internal
{
    public static class UtilityExtensions
    {
        public static Encoding Encoding = new UTF8Encoding(false);

        /// <summary>
        /// Read content in <paramref name="srcText"/> and write to memory stream snapshot.
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns>stream that doesn't need to be disposed</returns>
        public static MemoryStream ToStream(this TextReader srcText)
        {
            if (srcText == null) return null;
            byte[] data = Encoding.GetBytes(srcText.ReadToEnd());
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Flush();
            ms.Position = 0L;
            return ms;
        }

        /// <summary>
        /// Read content in <paramref name="s"/> and decode into string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>string reader that need to be disposed</returns>
        public static TextReader ToTextReader(this Stream s)
            => new StreamReader(s, Encoding, true, 32 * 1024);

        public static byte[] ReadFully(this Stream s)
        {
            if (s == null) return null;

            // Try to read stream completely.
            int len_ = (int)s.Length;
            if (len_ > 2147483647) throw new IOException("File size over 2GB");
            byte[] data = new byte[len_];

            // Read chunks
            int ix = 0;
            while (ix < len_)
            {
                int count = s.Read(data, ix, len_ - ix);

                // "returns zero (0) if the end of the stream has been reached."
                if (count == 0) break;

                ix += count;
            }
            if (ix == len_) return data;
            throw new AssetException("Failed to read stream fully");
        }
        /// <summary>
        /// Write contents in <paramref name="ms"/> into <paramref name="dstText"/>.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="dstText"></param>
        public static void WriteText(this MemoryStream ms, TextWriter dstText)
        {
            ms.Position = 0L;
            dstText.Write(Encoding.GetString(ms.GetBuffer()));
            dstText.Flush();
        }

        /// <summary>
        /// Create writer that converts text to stream.
        /// Result must be flushed and disposed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>writer that must be disposed.</returns>
        public static TextWriter ToTextWriter(this Stream s)
            => new StreamWriter(s, Encoding, 16 * 1024, true);
    }

}
