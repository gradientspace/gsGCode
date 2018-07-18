﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;
using gs;

namespace gs
{

	public class GenericMillingAssembler : BaseMillingAssembler
    {
        public static BaseMillingAssembler Factory(GCodeBuilder builder, SingleMaterialFFFSettings settings) {
            return new GenericMillingAssembler(builder, settings);
        }


		public SingleMaterialFFFSettings Settings;


		public GenericMillingAssembler(GCodeBuilder useBuilder, SingleMaterialFFFSettings settings) : base(useBuilder, settings.Machine)
        {
			Settings = settings;

			OmitDuplicateZ = true;
			OmitDuplicateF = true;

            HomeSequenceF =  StandardHomeSequence;
        }


        public override void UpdateProgress(int i) {
			// not supported on reprap?
			//Builder.BeginMLine(73).AppendI("P",i);
		}

		public override void ShowMessage(string s)
		{
            Builder.AddCommentLine(s);
		}


        /// <summary>
        /// Replace this to run your own home sequence
        /// </summary>
        public Action<GCodeBuilder> HomeSequenceF;


        public enum HeaderState
		{
			AfterComments,
			BeforeHome
		};
		public Action<HeaderState, GCodeBuilder> HeaderCustomizerF = (state, builder) => { };




		public override void AppendHeader() {
			AppendHeader_StandardRepRap();
		}
		void AppendHeader_StandardRepRap() {

            base.AddStandardHeader(Settings);

            HeaderCustomizerF(HeaderState.AfterComments, Builder);

			Builder.BeginGLine(21, "units=mm");
			Builder.BeginGLine(90, "absolute positions");

			HeaderCustomizerF(HeaderState.BeforeHome, Builder);

            HomeSequenceF(Builder);

            // retract to retract height
            //Builder.BeginGLine(0, "retract to top").AppendI("X", 0).AppendI("Y", 0).AppendF("Z", Settings.RetractDistanceMM);

            //PositionShift = 0.5 * new Vector2d(Settings.Machine.BedSizeXMM, Settings.Machine.BedSizeYMM);
            //PositionShift = Vector2d.Zero;
            currentPos = Vector3d.Zero;

            ShowMessage("Cut Started");

			in_travel = false;

            UpdateProgress(0);
		}




		public override void AppendFooter() {
			AppendFooter_StandardRepRap();
		}
		void AppendFooter_StandardRepRap() {

            UpdateProgress(100);

            Builder.AddCommentLine("End of print");
            ShowMessage("Done!");

            Builder.BeginMLine(30, "end program");

			Builder.EndLine();		// need to force this
		}




        public virtual void StandardHomeSequence(GCodeBuilder builder)
        {
            //Builder.BeginGLine(0, "home x/y/z").AppendI("X", 0).AppendI("Y", 0).AppendI("Z", 0);
        }


	}


}