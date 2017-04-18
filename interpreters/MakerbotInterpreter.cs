using System;
using System.Collections.Generic;
using System.Diagnostics;
using g3;

namespace gs 
{
	public class MakerbotInterpreter : IGCodeInterpreter
	{
		IGCodeListener listener = null;

		Dictionary<int, Action<GCodeLine>> GCodeMap = new Dictionary<int, Action<GCodeLine>>();

		double ExtrusionA = 0;
		double LastRetractA = 0;
		bool in_retract = false;

		public MakerbotInterpreter() {
			build_maps();			
		}

		public virtual void AddListener(IGCodeListener listener) 
		{
			if (this.listener != null)
				throw new Exception("Only one listener supported!");
			this.listener = listener;
		}


		public virtual void Interpret(GCodeFile file, InterpretArgs args)
		{
			IEnumerable<GCodeLine> lines_enum =
				(args.HasTypeFilter) ? file.AllLines() : file.AllLinesOfType(args.eTypeFilter);

			listener.Begin();

			ExtrusionA = 0;

			foreach(GCodeLine line in lines_enum) {

				if ( line.type == GCodeLine.LType.GCode ) {
					Action<GCodeLine> parseF;
					if (GCodeMap.TryGetValue(line.code, out parseF))
						parseF(line);
				}
			}

			listener.End();
		}



		void emit_linear(GCodeLine line)
		{
			Debug.Assert(line.code == 1);

			double x = 0, y = 0, z = 0;
			bool absx = GCodeUtil.TryFindParamNum(line.parameters, "X", ref x);
			bool absy = GCodeUtil.TryFindParamNum(line.parameters, "Y", ref y);
			bool absz = GCodeUtil.TryFindParamNum(line.parameters, "Z", ref z);

			// F is feed rate (this changes?)
			double f = 0;
			/*bool haveF =*/ GCodeUtil.TryFindParamNum(line.parameters, "F", ref f);

			// A is extrusion stepper
			double a = 0;
			bool haveA = GCodeUtil.TryFindParamNum(line.parameters, "A", ref a);

			if ( haveA == false ) {
				Debug.Assert(in_retract);
			} else if (in_retract) {
				Debug.Assert(a <= LastRetractA);
				if ( MathUtil.EpsilonEqual(a, LastRetractA, 0.00001) ) {
					in_retract = false;
					listener.BeginDeposition();
					ExtrusionA = a;
				}
			} else if ( a < ExtrusionA ) {
				in_retract = true;
				LastRetractA = ExtrusionA;
				ExtrusionA = a;
				listener.BeginTravel();
			} else {
				ExtrusionA = a;
			}

			Debug.Assert(absx && absy && absz);
			if ( absx && absy && absz ) {
				listener.LinearMoveToAbsolute3d(new g3.Vector3d(x,y,z));
				return;
			}


		}



		void build_maps()
		{

			// G1 = linear move
			GCodeMap[1] = emit_linear;

			// G4 = CCW circular
			//GCodeMap[4] = emit_ccw_arc;
			//GCodeMap[5] = emit_cw_arc;
		}


	}
}
