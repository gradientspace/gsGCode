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


		public RepRapAssembler(GCodeBuilder useBuilder, SingleMaterialFFFSettings settings) : base(useBuilder, settings.Machine)
        {
			Settings = settings;

			OmitDuplicateZ = true;
			OmitDuplicateF = true;
			OmitDuplicateE = true;

            HomeSequenceF =  StandardHomeSequence;
        }


		//public override void BeginRetract(Vector3d pos, double feedRate, double extrudeDist, string comment = null) {
  //          base.BeginRetract(pos, feedRate, extrudeDist, comment);
		//}

		//public override void EndRetract(Vector3d pos, double feedRate, double extrudeDist = -9999, string comment = null) {
  //          base.EndRetract(pos, feedRate, extrudeDist, comment);
		//}


        public override void UpdateProgress(int i) {
			// not supported on reprap?
			//Builder.BeginMLine(73).AppendI("P",i);
		}

		public override void ShowMessage(string s)
		{
			Builder.BeginMLine(117).AppendL(s);
		}

		public override void EnableFan() {
            int fan_speed = (int)(Settings.FanSpeedX * 255.0);
			Builder.BeginMLine(106, "fan on").AppendI("S", fan_speed);
		}
		public override void DisableFan() {
			Builder.BeginMLine(107, "fan off");
		}


        /// <summary>
        /// Replace this to run your own home sequence
        /// </summary>
        public Action<GCodeBuilder> HomeSequenceF;


        public enum HeaderState
		{
			AfterComments,
			AfterTemperature,
			BeforeHome,
			BeforePrime
		};
		public Action<HeaderState, GCodeBuilder> HeaderCustomizerF = (state, builder) => { };




		public override void AppendHeader() {
			AppendHeader_StandardRepRap();
		}
		void AppendHeader_StandardRepRap() {

            base.AddStandardHeader(Settings);

            DisableFan();

            HeaderCustomizerF(HeaderState.AfterComments, Builder);

            /*
             * Configure temperatures
             */

            // do this first so it happens while bed heats
            SetExtruderTargetTemp(Settings.ExtruderTempC);

            // M190
            if (Settings.Machine.HasHeatedBed) {
                if (Settings.HeatedBedTempC > 0)
                    SetBedTargetTempAndWait(Settings.HeatedBedTempC);
                else
                    SetBedTargetTemp(0, "disable heated bed");
            }

            // M109
            SetExtruderTargetTempAndWait(Settings.ExtruderTempC);


            HeaderCustomizerF(HeaderState.AfterTemperature, Builder);

			Builder.BeginGLine(21, "units=mm");
			Builder.BeginGLine(90, "absolute positions");
			Builder.BeginMLine(82, "absolute extruder position");

			HeaderCustomizerF(HeaderState.BeforeHome, Builder);

            HomeSequenceF(Builder);

            currentPos = Vector3d.Zero;

			HeaderCustomizerF(HeaderState.BeforePrime, Builder);

            base.AddPrimeLine(Settings);

            // [RMS] this does not extrude very much and does not seem to work?
            //Builder.BeginGLine(1, "move platform down").AppendF("Z", 15).AppendI("F", 9000);
            //currentPos.z = 15;
            //Builder.BeginGLine(92, "reset extruded length").AppendI("E", 0);
            //extruderA = 0;
            //Builder.BeginGLine(1, "extrude blob").AppendI("F", 200).AppendI("E", 3);
            //Builder.BeginGLine(92, "reset extruded length again").AppendI("E", 0);
            //extruderA = 0;
            //Builder.BeginGLine(1, "reset speed").AppendI("F", 9000);

            // move to z=0
            BeginTravel();
            AppendMoveTo(new Vector3d(NozzlePosition.x,NozzlePosition.y,0), Settings.ZTravelSpeed, "reset z");
            EndTravel();

            ShowMessage("Print Started");

			in_retract = false;
			in_travel = false;

            EnableFan();
            UpdateProgress(0);
		}




		public override void AppendFooter() {
			AppendFooter_StandardRepRap();
		}
		void AppendFooter_StandardRepRap() {

            UpdateProgress(100);

            Builder.AddCommentLine("End of print");
            ShowMessage("Done!");

            DisableFan();
            SetExtruderTargetTemp(0, "extruder off");
            SetBedTargetTemp(0, "bed off");

            BeginRetractRelativeDist(currentPos, 300, -1, "final retract");

            Vector3d zup = currentPos;
            zup.z = Math.Min(Settings.Machine.MaxHeightMM, zup.z + 50);
            AppendMoveToE(zup, 9000, ExtruderA - 5.0, "move up and retract");

            Builder.BeginGLine(28, "home x/y").AppendI("X", 0).AppendI("Y", 0);
            currentPos.x = currentPos.y = 0;

            Builder.BeginMLine(84, "turn off steppers");

			Builder.EndLine();		// need to force this
		}




        public virtual void StandardHomeSequence(GCodeBuilder builder)
        {
            Builder.BeginGLine(28, "home x/y").AppendI("X", 0).AppendI("Y", 0);
            Builder.BeginGLine(28, "home z").AppendI("Z", 0);
        }


	}


}