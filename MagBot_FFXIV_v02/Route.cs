using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MagBot_FFXIV_v02
{
    public class Route
    {
        private readonly string _from;
        private readonly string _to;
        public readonly string Name;
        public readonly List<Waypoint> Points;

        public Route(string name, string from = "na", string to = "na")
        {
            Points = new List<Waypoint>();
            Name = name;
            _from = from;
            _to = to;
        }

        public Route(XElement xml)
            : this(
                xml.Attribute("Name").Value,
                xml.Attribute("From").Value,
                xml.Attribute("To").Value)
        {
            foreach (XElement xpoint in xml.Descendants("Point"))
                Points.Add(new Waypoint(xpoint));
        }

        public XElement GetXml()
        {
            var xroute = new XElement("Route",
                new XAttribute("Name", Name),
                new XAttribute("From", _from),
                new XAttribute("To", _to));

            foreach (Waypoint point in Points)
                xroute.Add(point.GetXml());

            return xroute;
        }

        public Waypoint NextWaypoint(Waypoint wp)
        {
            if (wp == Points.Last())
            {
                return Points.First();
            }
            return Points[Points.IndexOf(wp) + 1];
        }

        public Waypoint ClosestWaypoint(Waypoint wp)
        {
            double tempDistance = Int32.MaxValue;
            Waypoint closestPoint = Points.First();

            foreach (Waypoint p in Points)
            {
                if (p.Distance(wp) < tempDistance)
                {
                    tempDistance = p.Distance(wp);
                    closestPoint = p;
                }
            }
            return closestPoint;
        }
    }
}
