using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using g3;

namespace gs
{
    public class GCodeToPlanarComplex : IGCodeListener
    {
        public PlanarComplex Complex = new PlanarComplex();

        Vector2d P;


        public void Begin()
        {
            P = Vector2d.Zero;
        }


        public void LinearMoveToAbsolute(Vector2d v)
        {
            Vector2d P2 = v;
            Complex.Add(new Segment2d(P, P2));
            P = P2;
        }

        public void LinearMoveToRelative(Vector2d v)
        {
            Vector2d P2 = P + v;
            Complex.Add(new Segment2d(P, P2));
            P = P2;
        }



		int find_arc_centers(Vector2d p1, Vector2d p2, double radius, out Vector2d c0, out Vector2d c1)
		{
			c0 = c1 = Vector2d.Zero;

			double dist2 = p1.DistanceSquared(p2);
			double diam2 = 4 * radius * radius;
			Debug.Assert(diam2 < dist2 );	// otherwise solution is impossible

			Vector2d midpoint = 0.5 * (p1 + p2);
			if ( MathUtil.EpsilonEqual(dist2, diam2, MathUtil.ZeroTolerance) ) {
				c0 = midpoint;
				return 1;
			}

			double d = Math.Sqrt(radius * radius - dist2 / 4);
			double distance = Math.Sqrt(dist2);
			double ox = d * (p2.x - p1.x) / distance;
			double oy = d * (p2.y - p1.y) / distance;
			c0 = new Vector2d(midpoint.x - oy, midpoint.y + ox);
			c1 = new Vector2d(midpoint.x + oy, midpoint.y - ox);
			return 2;
		}


		double arc_angle_deg(Vector2d p, Vector2d c) {
			Vector2d v = p - c;
			v.Normalize();
			double angle = Math.Atan2(v.y, v.x);
			return angle * MathUtil.Rad2Deg;
		}


		bool arc_is_cw( Vector2d p1, Vector2d p2, Vector2d c ) {
			return false;
		}


		public void ArcToRelative(Vector2d v, double radius, bool clockwise)
        {
			Vector2d P2 = P + v;

			Vector2d c0,c1;
			int nCenters = find_arc_centers(P, P2, radius, out c0, out c1);

			Vector2d c = c0;
			if (nCenters == 2 && clockwise == false )
				c = c1;

			bool reverse = false;
			//if (radius < 0)
			//	reverse = true;
			radius = Math.Abs(radius);

			double start_angle = arc_angle_deg(P, c);
			double end_angle = arc_angle_deg(P2, c);


			Arc2d arc = new Arc2d(c, radius, start_angle, end_angle);
			if ( clockwise == false )
				arc.Reverse();
			Complex.Add(arc);

			P = P2;
        }



    }
}
