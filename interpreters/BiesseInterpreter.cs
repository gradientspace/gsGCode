using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using g3;

namespace gs
{
    public class BiesseInterpreter : IGCodeInterpreter
    {
        IGCodeListener listener = null;
        Dictionary<int, Action<GCodeLine>> GCodeMap = new Dictionary<int, Action<GCodeLine>>();


        public BiesseInterpreter()
        {
            build_maps();
        }



        public virtual void AddListener(IGCodeListener listener) {
            if (this.listener != null)
                throw new Exception("Only one listener supported!");
            this.listener = listener;
        }

        public virtual void Interpret(GCodeFile file, InterpretArgs args)
        {
            IEnumerable<GCodeLine> lines_enum =
                (args.HasTypeFilter) ? file.AllLines() : file.AllLinesOfType(args.eTypeFilter);

            listener.Begin();

            foreach(GCodeLine line in lines_enum) {

                if ( line.type == GCodeLine.LType.GCode ) {
                    Action<GCodeLine> parseF;
                    if (GCodeMap.TryGetValue(line.code, out parseF))
                        parseF(line);
                }

            }
        }








        void emit_linear(GCodeLine line)
        {
            Debug.Assert(line.code == 1);

            double dx = 0, dy = 0;
            bool brelx = GCodeUtil.TryFindParamNum(line.parameters, "XI", ref dx);
            bool brely = GCodeUtil.TryFindParamNum(line.parameters, "YI", ref dy);

            if (brelx || brely) {
                listener.LinearMoveToRelative(new Vector2d(dx, dy));
                return;
            }

            double x = 0, y = 0;
            bool absx = GCodeUtil.TryFindParamNum(line.parameters, "X", ref x);
            bool absy = GCodeUtil.TryFindParamNum(line.parameters, "Y", ref y);
            if ( absx && absy ) {
                listener.LinearMoveToAbsolute(new Vector2d(x, y));
                return;
            }

            // [RMS] can we have this??
            if (absx || absy)
                System.Diagnostics.Debug.Assert(false);
        }






        void build_maps()
        {

            // G1 = linear move
            GCodeMap[1] = emit_linear;
        }

    }
}
