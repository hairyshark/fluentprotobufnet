using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FluentProtobufNet.Tests
{
    [DataContract]
    public class ToyBox
    {
        [DataMember]
        public List<Shape> Toys { get; set; }
    }

    [DataContract]
    public abstract class Shape
    {
        [DataMember]
        public double Area { get; set; }
    }


    [DataContract]
    public class Circle : Shape
    {
        [DataMember]
        public double Radius { get; set; }
    }
}
