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






	public class MakerbotAssembler : IDepositionAssembler
	{
		public GCodeBuilder Builder;
		public MakerbotSettings Settings;


		public MakerbotAssembler(GCodeBuilder useBuilder, MakerbotSettings settings) {
			Builder = useBuilder;
			Settings = settings;
			currentPos = Vector3d.Zero;
			extruderA = 0;
		}



		Vector3d currentPos;
		public Vector3d NozzlePosition
		{
			get { return currentPos; }
		}

		double extruderA;
		public double ExtruderA 
		{
			get { return extruderA; }
		}

		bool in_retract;
		double retractA;
		public bool InRetract
		{
			get { return in_retract; }
		}

		bool in_travel;
		public bool InTravel 
		{
			get { return in_travel; }
		}



		public virtual void AppendMoveTo(Vector3d pos, double f, string comment = null) {
			AppendMoveTo(pos.x, pos.y, pos.z, f, comment);
		}
		public virtual void AppendMoveTo(double x, double y, double z, double f, string comment = null) {
			Builder.BeginGLine(1, comment).
			       AppendF("X",x).AppendF("Y",y).AppendF("Z",z).AppendF("F",f);
			currentPos = new Vector3d(x, y, z);
		}

		public virtual void AppendMoveToE(double x, double y, double z, double f, double e, string comment = null) {
			Builder.BeginGLine(1, comment).
			       AppendF("X",x).AppendF("Y",y).AppendF("Z",z).AppendF("F",f).AppendF("E",e);
			currentPos = new Vector3d(x, y, z);
			extruderA = e;
		}

		public virtual void AppendMoveToA(Vector3d pos, double f, double a, string comment = null) {
			AppendMoveToA(pos.x, pos.y, pos.z, f, a, comment);
		}
		public virtual void AppendMoveToA(double x, double y, double z, double f, double a, string comment = null) {
			Builder.BeginGLine(1, comment).
			       AppendF("X",x).AppendF("Y",y).AppendF("Z",z).AppendF("F",f).AppendF("A",a);
			currentPos = new Vector3d(x, y, z);
			extruderA = a;
		}


		public virtual void BeginRetract(Vector3d pos, double f, double a) {
			if (in_retract)
				throw new Exception("MakerbotAssembler.BeginRetract: already in retract!");
			if (a > extruderA)
				throw new Exception("MakerbotAssembler.BeginRetract: retract extrudeA is forward motion!");
			retractA = extruderA;
			AppendMoveToA(pos, f, a, "Retract");
			// [RMS] makerbot does this...but does it do anything??
			AppendMoveTo(pos, 3000, "Retract 2?");
			in_retract = true;
		}

		public virtual void EndRetract(Vector3d pos, double f, double a = -9999) {
			if (! in_retract)
				throw new Exception("MakerbotAssembler.EndRetract: already in retract!");
			if (a != -9999 && a != retractA)
				throw new Exception("MakerbotAssembler.EndRetract: restart position is not same as start of retract!");
			if (a == -9999)
				a = retractA;
			AppendMoveToA(pos, f, a, "Restart");
			in_retract = false;			
		}

		public virtual void BeginTravel() {
			if (in_travel)
				throw new Exception("MakerbotAssembler.BeginTravel: already in travel!");
			in_travel = true;
		}

		public virtual void EndTravel()
		{
			if (in_travel == false)
				throw new Exception("MakerbotAssembler.EndTravel: not in travel!");
			in_travel = false;
		}

		public virtual void AppendTravelTo(double x, double y, double z, double f)
		{
			//G1 X-7.198 Y-12.470 Z0.200 F1500 A1.85684; Retract
			//G1 X-7.198 Y-12.470 Z0.200 F3000; Retract
			//G1 X-7.399 Y-12.818 Z0.200 F9000; Travel Move
			//G1 X-7.399 Y-12.818 Z0.200 F1500 A3.15684; Restart
		}


		public virtual void UpdateProgress(int i) {
			Builder.BeginMLine(73).AppendI("P",i);
		}


		public virtual void EnableFan() {
			Builder.BeginMLine(126).AppendI("T",0);
		}
		public virtual void DisableFan() {
			Builder.BeginMLine(127).AppendI("T",0);
		}








		public void AppendHeader() {
			AppendHeader_Replicator2();
		}
		void AppendHeader_Replicator2() {

			Builder.AddCommentLine("; Print Settings");
			Builder.AddCommentLine("; Model: Makerbot " + Settings.Model.ToString());
			Builder.AddCommentLine("; Layer Height: " + Settings.LayerHeightMM);
			Builder.AddCommentLine("; Nozzle Diameter: " + Settings.NozzleDiamMM + "  Filament Diameter: " + Settings.FilamentDiamMM);
			Builder.AddCommentLine("; Extruder Temp: " + Settings.ExtruderTempC);

			double LayerHeight = Settings.LayerHeightMM;

			Vector2d BackRight = new Vector2d(152,75);
			Vector2d FrontLeft = new Vector2d(-141,-74);

			Vector2d PrimeFrontRight = new Vector2d(105.4, -74);
			double PrimeHeight = 0.270;



			Builder.BeginMLine(136, "(enable build)");

			// reset build percentage
			UpdateProgress(0);

			Builder.BeginGLine(162, "(home XY axes maximum)")
			       .AppendL("X").AppendL("Y").AppendI("F", 2000);

			Builder.BeginGLine(161, "(home Z axis minimum)")
			       .AppendL("Z").AppendI("F", 900);

			Builder.BeginGLine(92, "(set Z to -5)").
			       AppendI("X",0).AppendI("Y",0).AppendI("Z",-5).AppendI("A",0).AppendI("B",0);
			currentPos = new Vector3d(0, 0, -5);
			extruderA = 0;			

			Builder.BeginGLine(1, "(move Z to '0')")
			       .AppendI("Z", 0).AppendI("F", 900);
			currentPos.z = 0;		

			Builder.BeginGLine(161, "(home Z axis minimum)")
			       .AppendL("Z").AppendI("F", 100);

			Builder.BeginMLine(132, "(Recall stored home offsets for XYZAB axis)").
			       AppendL("X").AppendL("Y").AppendL("Z").AppendL("A").AppendL("B");

			Builder.BeginGLine(92).
			       AppendF("X",BackRight.x).AppendF("Y",BackRight.y).AppendI("Z",0).AppendI("A",0).AppendI("B",0);
			currentPos = new Vector3d(BackRight.x, BackRight.y, 0);
			extruderA = 0;

			Builder.BeginGLine(1, "(move to waiting position)").
			       AppendF("X",FrontLeft.x).AppendF("Y",FrontLeft.y).AppendI("Z",40).AppendI("F",3300);
			currentPos = new Vector3d(FrontLeft.x, FrontLeft.y, 40);

			Builder.BeginGLine(130, "(Lower stepper Vrefs while heating)").
			       AppendI("X",20).AppendI("Y",20).AppendI("A",20).AppendI("B",20);

			// set tool
			Builder.BeginMLine(135).AppendI("T",0);

			// set target temperature
			Builder.BeginMLine(104).AppendI("S",Settings.ExtruderTempC).AppendI("T",0);

			// wait to heat
			Builder.BeginMLine(133).AppendI("T",0);

			Builder.BeginGLine(130, "(Set Stepper motor Vref to defaults)").
			       AppendI("X",127).AppendI("Y",127).AppendI("A",127).AppendI("B",127);


			// thick line along front of bed, at start of print
			AppendMoveTo(PrimeFrontRight.x, PrimeFrontRight.y, PrimeHeight, 9000, "(Extruder Prime Dry Move)");
			AppendMoveToE(FrontLeft.x, FrontLeft.y, PrimeHeight, 1800, 25, "(Extruder Prime Start)");

			Builder.BeginGLine(92,"(Reset after prime)").AppendI("A",0).AppendI("B",0);
			extruderA = 0;

			// move to z=0
			Builder.BeginGLine(1).AppendI("Z",0).AppendI("F",1000);
			currentPos.z = 0;

			// move to front-left corner
			AppendMoveToE(FrontLeft.x, FrontLeft.y, 0, 1000, 0);

			// reset E/A stepper 
			Builder.BeginGLine(92).AppendI("E",0);
			extruderA = 0;

			// should do this at higher level...
			//AppendMoveToA(FrontLeft.x, FrontLeft.y, 0, 1500, -1.3, "Retract");
			//AppendMoveTo(FrontLeft.x, FrontLeft.y, 0, 3000);		// what is this line for??
			//AppendMoveTo(FrontLeft.x, FrontLeft.y, LayerHeight, 1380, "Next Layer");

			in_retract = false;
			in_travel = false;

			UpdateProgress(0);
		}





		public void AppendFooter() {
			AppendFooter_Replicator2();
		}
		void AppendFooter_Replicator2() {
			double MaxHeight = 155;

			Builder.AddCommentLine("End of print");

			//G1 X-9.119 Y10.721 Z0.200 F1500 A61.36007; Retract

			Builder.BeginMLine(127, "(Fan Off)").AppendI("T",0);
			Builder.BeginMLine(18, "(Turn off A and B Steppers)").AppendL("A").AppendL("B");

			// move bed to max height
			Builder.BeginGLine(1).AppendF("Z",MaxHeight).AppendI("F",900);

			// home steppers and turn off
			Builder.BeginGLine(162).AppendL("X").AppendL("Y").AppendI("F", 2000);
			Builder.BeginMLine(18,"(Turn off steppers)").AppendL("X").AppendL("Y").AppendL("Z");

			// set temp to 0
			Builder.BeginMLine(104).AppendI("S",0).AppendI("T",0);

			// set built-in status message
			Builder.BeginMLine(70, "(We <3 Making Things!)").AppendI("P",5);

			// skip song
			//Builder.BeginMLine(72).AppendI("P",1);

			UpdateProgress(100);

			Builder.BeginMLine(137,"(build end notification)");

			Builder.EndLine();		// need to force this
		}

	}


}