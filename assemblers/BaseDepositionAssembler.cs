﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;
using gs;

namespace gs
{
	public interface IDepositionAssembler
	{
	}


    public delegate BaseDepositionAssembler AssemblerFactoryF(GCodeBuilder builder, SingleMaterialFFFSettings settings);


	public abstract class BaseDepositionAssembler : IDepositionAssembler
	{
		public GCodeBuilder Builder;

        public enum ExtrudeParamType {
            ExtrudeParamA, ExtrudeParamE
        }
        ExtrudeParamType ExtrudeParam = ExtrudeParamType.ExtrudeParamE;

        public BaseDepositionAssembler(GCodeBuilder useBuilder) {
			Builder = useBuilder;
			currentPos = Vector3d.Zero;
			extruderA = 0;
		}


        /*
         * Subclasses must implement these
         */

        public abstract void AppendHeader();
        public abstract void AppendFooter();
        public abstract void UpdateProgress(int i);
        public abstract void EnableFan();
        public abstract void DisableFan();

        /*
         * Internals
         */


        protected Vector3d currentPos;
		public Vector3d NozzlePosition
		{
			get { return currentPos; }
		}

        protected double extruderA;
		public double ExtruderA 
		{
			get { return extruderA; }
		}

        protected bool in_retract;
        protected double retractA;
		public bool InRetract
		{
			get { return in_retract; }
		}

        protected bool in_travel;
		public bool InTravel 
		{
			get { return in_travel; }
		}



		public virtual void AppendMoveTo(double x, double y, double z, double f, string comment = null) {
			Builder.BeginGLine(1, comment).
			       AppendF("X",x).AppendF("Y",y).AppendF("Z",z).AppendF("F",f);
			currentPos = new Vector3d(x, y, z);
		}
        public virtual void AppendMoveTo(Vector3d pos, double f, string comment = null)
        {
            AppendMoveTo(pos.x, pos.y, pos.z, f, comment);
        }


        public virtual void AppendExtrudeTo(Vector3d pos, double feedRate, double extrudeDist, string comment = null)
        {
            if (ExtrudeParam == ExtrudeParamType.ExtrudeParamA)
                AppendMoveToA(pos, feedRate, extrudeDist, comment);
            else
                AppendMoveToE(pos, feedRate, extrudeDist, comment);
        }



        protected virtual void AppendMoveToE(double x, double y, double z, double f, double e, string comment = null) {
			Builder.BeginGLine(1, comment).
			       AppendF("X",x).AppendF("Y",y).AppendF("Z",z).AppendF("F",f).AppendF("E",e);
			currentPos = new Vector3d(x, y, z);
			extruderA = e;
		}
        protected virtual void AppendMoveToE(Vector3d pos, double f, double e, string comment = null)
        {
            AppendMoveToE(pos.x, pos.y, pos.z, f, e, comment);
        }


        protected virtual void AppendMoveToA(double x, double y, double z, double f, double a, string comment = null) {
			Builder.BeginGLine(1, comment).
			       AppendF("X",x).AppendF("Y",y).AppendF("Z",z).AppendF("F",f).AppendF("A",a);
			currentPos = new Vector3d(x, y, z);
			extruderA = a;
		}
        protected virtual void AppendMoveToA(Vector3d pos, double f, double a, string comment = null)
        {
            AppendMoveToA(pos.x, pos.y, pos.z, f, a, comment);
        }



        public virtual void BeginRetract(Vector3d pos, double feedRate, double extrudeDist, string comment = null) {
			if (in_retract)
				throw new Exception("BaseDepositionAssembler.BeginRetract: already in retract!");
			if (extrudeDist > extruderA)
				throw new Exception("BaseDepositionAssembler.BeginRetract: retract extrudeA is forward motion!");

			retractA = extruderA;
			AppendMoveToA(pos, feedRate, extrudeDist, (comment == null) ? "Retract" : comment);
			in_retract = true;
		}


		public virtual void EndRetract(Vector3d pos, double feedRate, double extrudeDist = -9999, string comment = null) {
			if (! in_retract)
				throw new Exception("BaseDepositionAssembler.EndRetract: already in retract!");
			if (extrudeDist != -9999 && MathUtil.EpsilonEqual(extrudeDist, retractA, 0.0001) == false )
				throw new Exception("BaseDepositionAssembler.EndRetract: restart position is not same as start of retract!");
			if (extrudeDist == -9999)
				extrudeDist = retractA;
			AppendMoveToA(pos, feedRate, extrudeDist, (comment == null) ? "Restart" : comment);
			in_retract = false;
		}


		public virtual void BeginTravel() {
			if (in_travel)
				throw new Exception("BaseDepositionAssembler.BeginTravel: already in travel!");
			in_travel = true;
		}


		public virtual void EndTravel()
		{
			if (in_travel == false)
				throw new Exception("BaseDepositionAssembler.EndTravel: not in travel!");
			in_travel = false;
		}


		public virtual void AppendTravelTo(double x, double y, double z, double f)
		{
            throw new NotImplementedException("BaseDepositionAssembler.AppendTravelTo");
		}





	}


}