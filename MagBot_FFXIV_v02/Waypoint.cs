using System;
using System.Xml.Linq;

namespace MagBot_FFXIV_v02
{
    public struct Waypoint : IEquatable<Waypoint> //IEquatable allows us to override .Equals, in order to check equality between two instances in a custom way
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        //:this() allows you to call one constructor from another within the same class
        public Waypoint(float x, float y, float z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Waypoint(XElement xml)
            : this(Convert.ToSingle(xml.Attribute("X").Value),
                   Convert.ToSingle(xml.Attribute("Y").Value),
                   Convert.ToSingle(xml.Attribute("Z").Value)) //Calls the base constructor with these parameters
        { }

        public double Distance(Waypoint to)
        {
            return Distance(to.X, to.Y, to.Z);
        }

        private double Distance(float toX, float toY, float toZ)
        {
            float dX = X - toX;
            float dY = Y - toY;
            float dZ = Z - toZ;
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        public double Angle(Waypoint to)
        {
            return Angle(to.X, to.Y);
        }

        private double Angle(float toX, float toY)
        {
            float dX = X - toX;
            float dY = Y - toY;

            return Math.Atan2(dY, dX);
        }

        public XElement GetXml()
        {
            return new XElement("Point", new XAttribute("X", X), new XAttribute("Y", Y), new XAttribute("Z", Z));
        }

        public bool Equals(Waypoint other)
        {
            const double tolerance = 0.001;
            return Math.Abs(other.X - X) < tolerance && Math.Abs(other.Y - Y) < tolerance && Math.Abs(other.Z - Z) < tolerance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Waypoint && Equals((Waypoint)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Waypoint left, Waypoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Waypoint left, Waypoint right)
        {
            return !left.Equals(right);
        }
    }
}
