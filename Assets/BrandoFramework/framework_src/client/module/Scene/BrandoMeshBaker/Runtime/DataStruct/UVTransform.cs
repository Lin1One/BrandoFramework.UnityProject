using System;
using UnityEngine;

// A UV Transform is a transform considering only scale and offset it is used to represent the scaling and offset
// from UVs outside the 0,0..1,1 box and material tiling
// Rect objects are used to store the transform

namespace GameWorld
{
    public struct DVector2
    {
        static double epsilon = 10e-6;

        public double x;
        public double y;

        public static DVector2 Subtract(DVector2 a, DVector2 b)
        {
            return new DVector2(a.x - b.x, a.y - b.y);
        }

        public DVector2(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        public DVector2(DVector2 r)
        {
            x = r.x;
            y = r.y;
        }

        public Vector2 GetVector2()
        {
            return new Vector2((float)x, (float)y);
        }

        public bool IsContainedIn(DRect r)
        {
            if (x >= r.x && y >= r.y && x <= r.x + r.width && y <= r.y + r.height)
            {
                return true;
            } else
            {
                return false;
            }
        }
        
        public bool IsContainedInWithMargin(DRect r)
        {
            if (x >= r.x - epsilon && y >= r.y - epsilon && x <= r.x + r.width + epsilon && y <= r.y + r.height + epsilon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }

        public string ToString(string formatS)
        {
            return string.Format("({0},{1})", x.ToString(formatS), y.ToString(formatS));
        }

        public static double Distance(DVector2 a, DVector2 b)
        {
            double dx = b.x - a.x;
            double dy = b.y - a.y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public struct DRect
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public DRect(Rect r)
        {
            x = r.x;
            y = r.y;
            width = r.width;
            height = r.height;
        }

        public DRect(Vector2 o, Vector2 s)
        {
            x = o.x;
            y = o.y;
            width = s.x;
            height = s.y;
        }

        public DRect(DRect r)
        {
            x = r.x;
            y = r.y;
            width = r.width;
            height = r.height;
        }

        public DRect(float xx, float yy, float w, float h)
        {
            x = xx;
            y = yy;
            width = w;
            height = h;
        }

        public DRect(double xx, double yy, double w, double h)
        {
            x = xx;
            y = yy;
            width = w;
            height = h;
        }

        public Rect GetRect()
        {
            return new Rect((float)x, (float)y, (float)width, (float)height);
        }

        public DVector2 minD
        {
            get
            {
                return new DVector2(x, y);
            }
        }

        public DVector2 maxD
        {
            get
            {
                return new DVector2((x + width), (y + height));
            }
        }

        public Vector2 min
        {
            get
            {
                return new Vector2((float)x, (float)y);
            }
        }

        public Vector2 max
        {
            get
            {
                return new Vector2((float)(x + width), (float)(y + height));
            }
        }

        public Vector2 size {
            get {
                return new Vector2((float)(width), (float)(height));
            }
        }

        public DVector2 center
        {
            get
            {
                return new DVector2(x + width / 2.0, y + height / 2.0);
            }
        }

        public override bool Equals(object obj)
        {
            DRect dr = (DRect) obj;
            if (dr.x == x && dr.y == y && dr.width == width && dr.height == height)
            {
                return true;
            }
            return false;
        }

        public static bool operator ==(DRect a, DRect b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DRect a, DRect b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
                return String.Format("(x={0},y={1},w={2},h={3})", x.ToString("F5"), y.ToString("F5"), width.ToString("F5"), height.ToString("F5"));
        }

        public void Expand(float amt)
        {
            x -= amt;
            y -= amt;
            width += amt * 2;
            height += amt * 2;
        }

        public bool Encloses(DRect smallToTestIfFits)
        {
            double smnx = smallToTestIfFits.x;
            double smny = smallToTestIfFits.y;
            double smxx = smallToTestIfFits.x + smallToTestIfFits.width;
            double smxy = smallToTestIfFits.y + smallToTestIfFits.height;
            //expand slightly to deal with rounding errors
            double bmnx = this.x;
            double bmny = this.y;
            double bmxx = this.x + this.width;
            double bmxy = this.y + this.height;
            return bmnx <= smnx && smnx <= bmxx &&
                    bmnx <= smxx && smxx <= bmxx &&
                    bmny <= smny && smny <= bmxy &&
                    bmny <= smxy && smxy <= bmxy;
        }

		public override int GetHashCode ()
		{
			return x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
		}
    }
}
