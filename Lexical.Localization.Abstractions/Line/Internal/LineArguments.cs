// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Line part arguments with no parameters
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    public class LineArguments<Intf> : LineArguments, ILineArguments<Intf>
    {
    }

    /// <summary>
    /// Line part arguments with one parameter.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    public class LineArguments<Intf, A0> : LineArguments, ILineArguments<Intf, A0>
    {
        /// <summary>
        /// Value of first argument.
        /// </summary>
        public A0 Argument0 { get; set; }

        /// <summary>
        /// Create part arguments.
        /// </summary>
        /// <param name="a0"></param>
        public LineArguments(A0 a0)
        {
            this.Argument0 = a0;
        }
    }

    /// <summary>
    /// Line part arguments with two parameters.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    public class LineArguments<Intf, A0, A1> : LineArguments, ILineArguments<Intf, A0, A1>
    {
        /// <summary>
        /// Value of first argument.
        /// </summary>
        public A0 Argument0 { get; set; }

        /// <summary>
        /// Value of second argument.
        /// </summary>
        public A1 Argument1 { get; set; }

        /// <summary>
        /// Create part arguments.
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        public LineArguments(A0 a0, A1 a1)
        {
            this.Argument0 = a0;
            this.Argument1 = a1;
        }
    }

    /// <summary>
    /// Line part arguments with three parameters.
    /// </summary>
    /// <typeparam name="Intf"></typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    public class LineArguments<Intf, A0, A1, A2> : LineArguments, ILineArguments<Intf, A0, A1, A2>
    {
        /// <summary>
        /// Value of first argument.
        /// </summary>
        public A0 Argument0 { get; set; }

        /// <summary>
        /// Value of second argument.
        /// </summary>
        public A1 Argument1 { get; set; }

        /// <summary>
        /// Value of third argument.
        /// </summary>
        public A2 Argument2 { get; set; }

        /// <summary>
        /// Create part arguments.
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        public LineArguments(A0 a0, A1 a1, A2 a2)
        {
            this.Argument0 = a0;
            this.Argument1 = a1;
            this.Argument2 = a2;
        }
    }

}
