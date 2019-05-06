// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    internal class LineArguments<Intf> : ILineArguments<Intf> { }

    internal class LineArguments<Intf, A0> : ILineArguments<Intf, A0>
    {
        public A0 Argument0 { get; set; }
        public LineArguments(A0 a0)
        {
            this.Argument0 = a0;
        }
    }

    internal class LineArguments<Intf, A0, A1> : ILineArguments<Intf, A0, A1>
    {
        public A0 Argument0 { get; set; }
        public A1 Argument1 { get; set; }
        public LineArguments(A0 a0, A1 a1)
        {
            this.Argument0 = a0;
            this.Argument1 = a1;
        }
    }

    internal class LineArguments<Intf, A0, A1, A2> : ILineArguments<Intf, A0, A1, A2>
    {
        public A0 Argument0 { get; set; }
        public A1 Argument1 { get; set; }
        public A2 Argument2 { get; set; }
        public LineArguments(A0 a0, A1 a1, A2 a2)
        {
            this.Argument0 = a0;
            this.Argument1 = a1;
            this.Argument2 = a2;
        }
    }

}
