//---------------------------------------------------------
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
        /// Create writer that converts text to stream.
        /// Result must be flushed and disposed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>writer that must be disposed.</returns>
        public static TextWriter ToTextWriter(this Stream s)
            => new StreamWriter(s, Encoding, 16 * 1024, true);


        /// <summary>
        /// Read content in <paramref name="srcText"/> and write to memory stream snapshot.
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns>stream that doesn't need to be disposed</returns>
        public static MemoryStream ReadStream(this TextReader srcText)
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
        /// Read content in <paramref name="filename"/> and write to memory stream snapshot.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>stream that doesn't need to be disposed</returns>
        public static MemoryStream ReadStream(this string filename)
        {
            if (filename == null) return null;            
            byte[] data = File.ReadAllBytes(filename);
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Flush();
            ms.Position = 0L;
            return ms;
        }

        /// <summary>
        /// Read content in <paramref name="s"/> and decode into string.
        /// </summary>
        /// <param name="s">(optional) stream to read</param>
        /// <returns>string reader that doesn't need disposing or null</returns>
        public static TextReader ReadText(this Stream s)
        {
            if (s == null) return null;
            using (var sr = new StreamReader(s, Encoding, true, 32 * 1024))
                return new StringReader(sr.ReadToEnd());
        }

        /// <summary>
        /// Read file completely into memory
        /// </summary>
        /// <param name="filename">(optional)</param>
        /// <returns>memory reader or null</returns>
        public static TextReader ReadText(this string filename)
        {
            if (filename == null) return null;
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                string text = sr.ReadToEnd();
                return new StringReader(text);
            }
        }

        /// <summary>
        /// Open or create <paramref name="filename"/> and return <see cref="System.IO.TextWriter"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>text writer that must be disposed</returns>
        public static TextWriter TextWriter(this string filename)
        {
            FileStream fs = new FileStream(filename, File.Exists(filename) ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.ReadWrite);
            fs.SetLength(0L);
            fs.Position = 0L;
            StreamWriter sw = new StreamWriter(fs, Encoding, 16 * 1024, false);
            return sw;
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
        /// Write contents in <paramref name="ms"/> into <paramref name="filename"/>.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        public static void WriteText(this MemoryStream ms, string filename)
        {
            ms.Position = 0L;
            using (var fs = new FileStream(filename, File.Exists(filename) ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.ReadWrite))
            {
                fs.SetLength(0L);
                fs.Position = 0L;
                ms.CopyTo(fs);
                fs.Flush();
            }
        }

    }

}
