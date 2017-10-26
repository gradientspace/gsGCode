﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;
using gs;

namespace gs
{

	public class RepRapAssembler : BaseDepositionAssembler
    {
        public static BaseDepositionAssembler Factory(GCodeBuilder builder, SingleMaterialFFFSettings settings) {
            return new RepRapAssembler(builder, settings);
        }


		public SingleMaterialFFFSettings Settings;


		public RepRapAssembler(GCodeBuilder useBuilder, SingleMaterialFFFSettings settings) : base(useBuilder)
        {
			Settings = settings;
		}


		public override void BeginRetract(Vector3d pos, double feedRate, double extrudeDist, string comment = null) {
            base.BeginRetract(pos, feedRate, extrudeDist, comment);
		}

		public override void EndRetract(Vector3d pos, double feedRate, double extrudeDist = -9999, string comment = null) {
            base.EndRetract(pos, feedRate, extrudeDist, comment);
		}


        public override void UpdateProgress(int i) {
			// not supported on reprap?
			//Builder.BeginMLine(73).AppendI("P",i);
		}

		public override void ShowMessage(string s)
		{
			Builder.BeginMLine(117).AppendL(s);
		}

		public override void EnableFan() {
			// [TODO] fan speed configuration?
			Builder.BeginMLine(106, "fan on").AppendI("S", 255);
		}
		public override void DisableFan() {
			Builder.BeginMLine(107, "fan off");
		}






		public override void AppendHeader() {
			AppendHeader_StandardRepRap();
		}
		void AppendHeader_StandardRepRap() {

			Builder.AddCommentLine("; Print Settings");
			Builder.AddCommentLine("; Model: " + Settings.Machine.ManufacturerName + " " + Settings.Machine.ModelIdentifier);
			Builder.AddCommentLine("; Layer Height: " + Settings.LayerHeightMM);
			Builder.AddCommentLine("; Nozzle Diameter: " + Settings.NozzleDiamMM + "  Filament Diameter: " + Settings.FilamentDiamMM);
			Builder.AddCommentLine("; Extruder Temp: " + Settings.ExtruderTempC + " Bed Temp: " + Settings.HeatedBedTempC);

			double LayerHeight = Settings.LayerHeightMM;

			//Vector2d BackRight = new Vector2d(152,75);
			//Vector2d FrontLeft = new Vector2d(-141,-74);

			//Vector2d PrimeFrontRight = new Vector2d(105.4, -74);
			//double PrimeHeight = 0.270;


			// M109
			SetExtruderTargetTempAndWait(Settings.ExtruderTempC);

			// M190
			if (Settings.Machine.HasHeatedBed && Settings.HeatedBedTempC > 0)
				SetBedTargetTempAndWait(Settings.HeatedBedTempC);

			Builder.BeginGLine(21, "units=mm");
			Builder.BeginGLine(90, "absolute positions");
			Builder.BeginMLine(82, "absolute extruder position");

			DisableFan();

			Builder.BeginGLine(28, "home x/y").AppendI("X", 0).AppendI("Y", 0);
			currentPos.x = currentPos.y = 0;
			PositionShift = Settings.BedSizeMM * 0.5;
				
			Builder.BeginGLine(28, "home z").AppendI("Z", 0);
			currentPos.z = 0;

			Builder.BeginGLine(1, "move platform down").AppendF("Z", 15).AppendI("F", 9000);
			currentPos.z = 15;

			Builder.BeginGLine(92, "reset extruded length").AppendI("E", 0);
			extruderA = 0;
			Builder.BeginGLine(1, "extrude blob").AppendI("F", 200).AppendI("E", 3);
			Builder.BeginGLine(92, "reset extruded length again").AppendI("E", 0);
			extruderA = 0;
			Builder.BeginGLine(1, "reset speed").AppendI("F", 9000);

			// move to z=0
			Builder.BeginGLine(1).AppendI("Z", 0).AppendI("F", 9000);
			currentPos.z = 0;

			ShowMessage("Print Started");

			Builder.BeginMLine(136, "(enable build)");

			in_retract = false;
			in_travel = false;

			UpdateProgress(0);
		}





		public override void AppendFooter() {
			AppendFooter_StandardRepRap();
		}
		void AppendFooter_StandardRepRap() {
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