using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void LinearArcTo(Vector2d pos, double radius)
        {
            throw new NotImplementedException();
        }



    }
}
