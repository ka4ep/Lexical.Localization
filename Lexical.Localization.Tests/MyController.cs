using System;
using System.Runtime.Serialization;

namespace ConsoleApp1
{
    [Serializable]
    public class MyController : ISerializable
    {
        public MyController(SerializationInfo info, StreamingContext context) { }
        public void GetObjectData(SerializationInfo info, StreamingContext context) { }
    }
}