﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;
using gs;

namespace gs
{

	public interface IMillingAssembler : IGCodeAssembler
    {
	}


    public delegate BaseMillingAssembler MillingAssemblerFactoryF(GCodeBuilder builder, SingleMaterialFFFSettings settings);


    /// <summary>
    /// Assembler translates high-level commands from Compiler (eg MoveTo, BeginRetract, etc)
    /// into GCode instructions, which it passes to GCodeBuilder instance.
    /// 
    /// To do this, Assembler maintains state machine for things like current tool position
    /// 
    /// TODO:
    ///   - support relative mode
    /// 
    /// </summary>
	public abstract class BaseMillingAssembler : IMillingAssembler
    {
		public GCodeBuilder Builder;

		/// <summary>
		/// To keep things simple, we use the absolute coordinates of the slice polygons
		/// at the higher levels. However the printer often operates in some other coordinate
		/// system, for example relative to front-left corner. PositionShift is added to all x/y
		/// coordinates before they are passed to the GCodeBuilder.
		/// </summary>
		public Vector2d PositionShift = Vector2d.Zero;


        /// <summary>
        /// check that all points lie within bounds
        /// </summary>
        public bool EnableBoundsChecking = true;

        /// <summary>
        /// if EnableBoundsChecking=true, will assert if we try to move outside these bounds
        /// </summary>
        public AxisAlignedBox2d PositionBounds = AxisAlignedBox2d.Infinite;

		public bool OmitDuplicateZ = false;
		public bool OmitDuplicateF = false;

        // threshold for omitting "duplicate" Z/F parameters
        public double MoveEpsilon = 0.00001;

        public BaseMillingAssembler(GCodeBuilder useBuilder, FFFMachineInfo machineInfo) 
		{
			Builder = useBuilder;
            currentPos = Vector3d.Zero;
			currentFeed = 0;
        }


        /*
         * Subclasses must implement these
         */

        public abstract void AppendHeader();
        public abstract void AppendFooter();
		public abstract void UpdateProgress(int i);
		public abstract void ShowMessage(string s);


        /*
         * Position commands
         */

        protected Vector3d currentPos;
        public Vector3d ToolPosition {
			get { return currentPos; }
		}

		protected double currentFeed;
		public double FeedRate  {
			get { return currentFeed; }
		}


        protected bool in_retract;
        public bool InRetract {
            get { return in_retract; }
        }

        protected bool in_travel;
		public bool InTravel  {
			get { return in_travel; }
		}

        public bool InCut {
            get { return InTravel == false; }
        }




        // actually emit travel move gcode
        protected virtual void emit_travel(Vector3d toPos, double feedRate, string comment)
        {
            double write_x = toPos.x + PositionShift.x;
            double write_y = toPos.y + PositionShift.y;

            Builder.BeginGLine(0, comment).
                   AppendF("X", write_x).AppendF("Y", write_y);

            if (OmitDuplicateZ == false || MathUtil.EpsilonEqual(currentPos.z, toPos.z, MoveEpsilon) == false) {
                Builder.AppendF("Z", toPos.z);
            }
            if (OmitDuplicateF == false || MathUtil.EpsilonEqual(currentFeed, feedRate, MoveEpsilon) == false) {
                Builder.AppendF("F", feedRate);
            }

            currentPos = toPos;
            currentFeed = feedRate;
        }


        // emit gcode for an cut move
        protected virtual void emit_cut(Vector3d toPos, double feedRate, string comment = null)
        {
            double write_x = toPos.x + PositionShift.x;
            double write_y = toPos.y + PositionShift.y;
            Builder.BeginGLine(1, comment).
                   AppendF("X", write_x).AppendF("Y", write_y);

            if (OmitDuplicateZ == false || MathUtil.EpsilonEqual(toPos.z, currentPos.z, MoveEpsilon) == false) {
                Builder.AppendF("Z", toPos.z);
            }
            if (OmitDuplicateF == false || MathUtil.EpsilonEqual(feedRate, currentFeed, MoveEpsilon) == false) {
                Builder.AppendF("F", feedRate);
            }

            currentPos = toPos;
            currentFeed = feedRate;
        }




        /*
         * Assembler API that Compiler uses
         */


        public virtual void AppendMoveTo(Vector3d pos, double f, string comment = null)
        {
            if (in_retract) {
                last_retract_z = pos.z;
                pos.z = currentPos.z;
            }

            emit_travel(pos, f, comment);
        }



        public virtual void AppendCutTo(Vector3d pos, double feedRate, string comment = null)
        {
            if (InRetract)
                throw new Exception("BaseMillingAssembler.AppendMoveTo: in retract!");

            emit_cut(pos, feedRate, comment);
        }




        double last_retract_z = 0;


        public virtual void BeginRetract(double retractHeight, double feedRate, string comment = null)
        {
            if (in_retract)
                throw new Exception("BaseMillingAssembler.BeginRetract: already in retract!");

            last_retract_z = currentPos.z;
            Vector3d pos = currentPos;
            pos.z = retractHeight;

            emit_travel(pos, feedRate, comment);

            in_retract = true;
        }


        public virtual void EndRetract(double feedRate, string comment = null)
        {
            if (!in_retract)
                throw new Exception("BaseMillingAssembler.EndRetract: already in retract!");

            Vector3d pos = currentPos;
            pos.z = last_retract_z;
            emit_travel(pos, feedRate, comment);

            in_retract = false;
        }








        public virtual void BeginTravel() {
			if (in_travel)
				throw new Exception("BaseMillingAssembler.BeginTravel: already in travel!");
			in_travel = true;
		}


		public virtual void EndTravel()
		{
			if (in_travel == false)
				throw new Exception("BaseMillingAssembler.EndTravel: not in travel!");
			in_travel = false;
		}


		public virtual void AppendTravelTo(double x, double y, double z, double f)
		{
            throw new NotImplementedException("BaseMillingAssembler.AppendTravelTo");
		}


        public virtual void AppendComment(string comment)
        {
            Builder.AddCommentLine(comment);
        }


        public virtual void AppendDwell(int milliseconds, string comment = null)
        {
            Builder.BeginGLine(4, (comment != null) ? comment : "dwell" )
                .AppendI("P", milliseconds);
        }




        protected virtual void AddStandardHeader(SingleMaterialFFFSettings Settings)
        {
            Builder.AddCommentLine(" Generated on " + DateTime.Now.ToLongDateString() + " by Gradientspace gsSlicer");
            Builder.AddCommentLine(string.Format(" Printer: {0} {1}", Settings.Machine.ManufacturerName, Settings.Machine.ModelIdentifier));
            Builder.AddCommentLine(" Print Settings");
            Builder.AddCommentLine(" Layer Height: " + Settings.LayerHeightMM);
            Builder.AddCommentLine(" Nozzle Diameter: " + Settings.Machine.NozzleDiamMM + "  Filament Diameter: " + Settings.Machine.FilamentDiamMM);
            Builder.AddCommentLine(" Extruder Temp: " + Settings.ExtruderTempC);
            Builder.AddCommentLine(string.Format(" Speeds Extrude: {0}  Travel: {1} Z: {2}", Settings.RapidExtrudeSpeed, Settings.RapidTravelSpeed, Settings.ZTravelSpeed));
            Builder.AddCommentLine(string.Format(" Retract Distance: {0}  Speed: {1}", Settings.RetractDistanceMM, Settings.RetractSpeed));
            Builder.AddCommentLine(string.Format(" Shells: {0}  InteriorShells: {1}", Settings.Shells, Settings.InteriorSolidRegionShells));
            Builder.AddCommentLine(string.Format(" InfillX: {0}", Settings.SparseLinearInfillStepX));
            Builder.AddCommentLine(string.Format(" LayerRange: {0}-{1}", Settings.LayerRangeFilter.a, Settings.LayerRangeFilter.b));
        }




    }


}