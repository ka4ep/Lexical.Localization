// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.IO;
using System.Text;

namespace Lexical.Localization.Binary
{
    /// <summary>
    /// Result of an operation that resolves a <see cref="ILine"/> into a string within an executing context, such as one that includes current active culture.
    /// </summary>
    public struct LineBinaryBytes
    {
        /// <summary>
        /// Converts to bytes by reading the stream and returning the position.
        /// 
        /// Does not dispose <paramref name="str"/>.
        /// 
        /// If position could not be returned, then leaves it were it is.
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator LineBinaryBytes(LineBinaryStream str)
        {
            if (str.Value == null) return new LineBinaryStream(str.Line, str.Exception, str.Status);

            try
            {
                // Get position.
                long pos = str.Value.Position;
                try
                {
                    return ReadFully(str);
                } finally
                {
                    // Return position
                    try
                    {
                        str.Value.Position = pos;
                    } catch (IOException)
                    {
                        // Could not return position.
                    }
                }
            }
            catch(IOException)
            {
                // Cannot get position. 
                return ReadFully(str);
            }
        }

        /// <summary>
        /// Return bytes.
        /// </summary>
        /// <param name="bytes"></param>
        public static implicit operator byte[](LineBinaryBytes bytes)
            => bytes.Value;

        /// <summary>
        /// Convert bytes to <see cref="LineBinaryBytes"/>.
        /// </summary>
        /// <param name="data"></param>
        public static implicit operator LineBinaryBytes(byte[] data)
            => data == null ?
                new LineBinaryBytes(null, data, LineStatus.ResourceFailedNull) :
                new LineBinaryBytes(null, data, LineStatus.ResourceOk);

        /// <summary>
        /// Return status.
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator LineStatus(LineBinaryBytes str)
            => str.Status;

        /// <summary>
        /// Convert from status code.
        /// </summary>
        /// <param name="status"></param>
        public static implicit operator LineBinaryBytes(LineStatus status)
            => new LineBinaryBytes(null, status);


        /// <summary>
        /// Status code
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// (optional) The line that was requested to be resolved.
        /// </summary>
        public ILine Line;

        /// <summary>
        /// Resolved resource.
        /// 
        /// Null, if value was not available.
        /// </summary>
        public byte[] Value;

        /// <summary>
        /// Related unexpeced exception.
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// Tests if there is a result, be that successful or an error.
        /// </summary>
        public bool HasResult => Status != LineStatus.NoResult;

        /// <summary>
        /// Highest severity value out of each category.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity Severity => Status.Severity();

        /// <summary>
        /// Result has ok state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Produced ok value.
        /// </summary>
        public bool Ok => Status.Ok();

        /// <summary>
        /// Result has warning state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Warning state has a value, but there was something occured during the resolve that may need attention.
        /// </summary>
        public bool Warning => Status.Warning();

        /// <summary>
        /// Result has error state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Error state has some kind of fallback value, but it is bad quality.
        /// </summary>
        public bool Error => Status.Error();

        /// <summary>
        /// Result has failed state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Failed state has no value.
        /// </summary>
        public bool Failed => Status.Failed();

        /// <summary>
        /// Create resource result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="value">resolved bytes</param>
        /// <param name="status">resolve reslut</param>
        public LineBinaryBytes(ILine line, byte[] value, LineStatus status)
        {
            Line = line;
            Value = value;
            Status = status;
            Exception = null;
        }

        /// <summary>
        /// Create resource result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="error">error</param>
        /// <param name="status">resolve reslut</param>
        public LineBinaryBytes(ILine line, Exception error, LineStatus status)
        {
            Line = line;
            Value = null;
            Status = status;
            Exception = error;
        }

        /// <summary>
        /// Create resource result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="status">resolve reslut</param>
        public LineBinaryBytes(ILine line, LineStatus status)
        {
            Line = line;
            Value = null;
            Status = status;
            Exception = null;
        }

        /// <summary>
        /// Return Value or ""
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => DebugInfo;

        /// <summary>
        /// Print debug information about the formatting result.
        /// </summary>
        /// <returns></returns>
        public string DebugInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                // Append status
                Status.AppendFlags(sb);

                // Append key
                if (Line != null)
                {
                    sb.Append(" ");
                    StructList12<ILineParameter> list = new StructList12<ILineParameter>();
                    Line.GetParameterParts<StructList12<ILineParameter>>(ref list);
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var parameter = list[i];
                        if (parameter.ParameterName == "String") continue;
                        if (i < list.Count - 1) sb.Append(':');
                        sb.Append(parameter.ParameterName);
                        sb.Append(':');
                        sb.Append(parameter.ParameterValue);
                    }
                }

                // Append result
                if (Value != null)
                {
                    sb.Append(" ");
                    sb.Append(Value.Length);
                    sb.Append(Value.Length == 1 ? " byte" : " bytes");
                }

                // Compile string
                return sb.ToString();
            }
        }

        /// <summary>
        /// Read bytes from <paramref name="str"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static LineBinaryBytes ReadFully(LineBinaryStream str)
        {
            Stream s = str.Value;
            if (s == null) return new LineBinaryBytes(str.Line, str.Exception, str.Status.Up_(LineStatus.ResourceFailedNull));

            try
            {
                // Get length
                long length;
                try
                {
                    length = s.Length;
                }
                catch (NotSupportedException)
                {
                    // Cannot get length
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);
                        return ms.ToArray();
                }

                if (length > int.MaxValue) return new LineBinaryBytes(str.Line, str.Exception, str.Status.Up_(LineStatus.ResourceFailed2GBLimit));

                int _len = (int)length;
                byte[] data = new byte[_len];

                // Read chunks
                int ix = 0;
                while (ix < _len)
                {
                    int count = s.Read(data, ix, _len - ix);

                    // "returns zero (0) if the end of the stream has been reached."
                    if (count == 0) break;

                    ix += count;
                }
                if (ix == _len) return data;

                // Failed to read stream fully
                return new LineBinaryBytes(str.Line, str.Exception, str.Status.Up_(LineStatus.ResourceFailedConversionError));
            }
            catch (IOException e)
            {
                return new LineBinaryBytes(str.Line, e, str.Status.Up_(LineStatus.ResourceFailedException));
            }
        }
    }

}
