using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs.info
{
	public static class Prusa
	{
        public const string UUID = "4530287d-cd23-416c-b74b-4876c385750a";

        public enum Models {
            Unknown = 0,
            i3_MK3 = 1
        };

        public const string UUID_Unknown = "b7498970-b847-4a24-8e5c-2e7c0c4d417f";
        public const string UUID_I3_MK3 = "fc09f99c-aee0-45e9-bca3-3e088ccb0b55";
    }


	public class PrusaSettings : GenericRepRapSettings
    {
		public Prusa.Models ModelEnum;

        public override AssemblerFactoryF AssemblerType() {
			return MakePrusaAssembler;
        }


		public PrusaSettings(Prusa.Models model) {
			ModelEnum = model;

            if (model == Prusa.Models.i3_MK3)
                configure_i3_MK3();
            else
                configure_unknown();
        }

        public override T CloneAs<T>() {
			PrusaSettings copy = new PrusaSettings(this.ModelEnum);
            this.CopyFieldsTo(copy);
            return copy as T;
        }


        public static IEnumerable<SingleMaterialFFFSettings> EnumerateDefaults()
        {
            yield return new PrusaSettings(Prusa.Models.i3_MK3);
            yield return new PrusaSettings(Prusa.Models.Unknown);
        }


        void configure_i3_MK3()
        {
            Machine.ManufacturerName = "Prusa";
			Machine.ManufacturerUUID = Prusa.UUID;
            Machine.ModelIdentifier = "i3 MK3";
			Machine.ModelUUID = Prusa.UUID_I3_MK3;
            Machine.Class = MachineClass.PlasticFFFPrinter;
            Machine.BedSizeXMM = 250;
            Machine.BedSizeYMM = 210;
            Machine.MaxHeightMM = 200;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 300;
            Machine.HasHeatedBed = true;
            Machine.MaxBedTempC = 120;

            Machine.HasAutoBedLeveling = true;
            Machine.EnableAutoBedLeveling = true;

            Machine.MaxExtrudeSpeedMMM = 80 * 60;
            Machine.MaxTravelSpeedMMM = 120 * 60;
            Machine.MaxZTravelSpeedMMM = 250 * 60;
            Machine.MaxRetractSpeedMMM = 35 * 60;
            Machine.MinLayerHeightMM = 0.05;
            Machine.MaxLayerHeightMM = 0.35;

            LayerHeightMM = 0.2;

            ExtruderTempC = 200;
            HeatedBedTempC = 60;

            SolidFillNozzleDiamStepX = 1.0;
            RetractDistanceMM = 0.7;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxTravelSpeedMMM;
            CarefulExtrudeSpeed = 20 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }




        void configure_unknown()
        {
            Machine.ManufacturerName = "Prusa";
            Machine.ManufacturerUUID = Prusa.UUID;
            Machine.ModelIdentifier = "(Unknown)";
            Machine.ModelUUID = Prusa.UUID_Unknown;
            Machine.Class = MachineClass.PlasticFFFPrinter;
            Machine.BedSizeXMM = 100;
            Machine.BedSizeYMM = 100;
            Machine.MaxHeightMM = 100;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = false;
            Machine.MaxBedTempC = 0;

            Machine.HasAutoBedLeveling = false;
            Machine.EnableAutoBedLeveling = false;

            Machine.MaxExtrudeSpeedMMM = 60 * 60;
            Machine.MaxTravelSpeedMMM = 80 * 60;
            Machine.MaxZTravelSpeedMMM = 23 * 60;
            Machine.MaxRetractSpeedMMM = 20 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;

            LayerHeightMM = 0.2;

            ExtruderTempC = 200;
            HeatedBedTempC = 0;

            SolidFillNozzleDiamStepX = 1.0;
            RetractDistanceMM = 1.0;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxTravelSpeedMMM;
            CarefulExtrudeSpeed = 20 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }





        public BaseDepositionAssembler MakePrusaAssembler(
			GCodeBuilder builder, SingleMaterialFFFSettings settings)
		{
			var asm = new RepRapAssembler(builder, settings);
            asm.HomeSequenceF = this.HomeSequence;
			asm.HeaderCustomizerF = HeaderCustomF;
            asm.TravelGCode = 1;
            return asm;
		}


        protected virtual void HomeSequence(GCodeBuilder builder)
        {
            if (Machine.HasAutoBedLeveling && Machine.EnableAutoBedLeveling) {
                builder.BeginGLine(28).AppendL("W").AppendComment("home all without bed level");
                builder.BeginGLine(80, "auto-level bed");
            } else {
                // standard home sequenece
                builder.BeginGLine(28, "home x/y").AppendI("X", 0).AppendI("Y", 0);
                builder.BeginGLine(28, "home z").AppendI("Z", 0);
            }

        }


		protected virtual void HeaderCustomF(RepRapAssembler.HeaderState state, GCodeBuilder Builder)
		{
            if (state == RepRapAssembler.HeaderState.AfterComments ) {

                if ( ModelEnum == Prusa.Models.i3_MK3 ) {
                    Builder.BeginMLine(201)
                        .AppendI("X",1000).AppendI("Y",1000).AppendI("Z",200).AppendI("E",5000)
                        .AppendComment("Set maximum accelleration in mm/sec^2");
                    Builder.BeginMLine(203)
                        .AppendI("X", 200).AppendI("Y", 200).AppendI("Z", 12).AppendI("E", 120)
                        .AppendComment("Set maximum feedrates in mm/sec");
                    Builder.BeginMLine(204)
                        .AppendI("S", 1250).AppendI("T", 1250)
                        .AppendComment("Set acceleration for moves (S) and retract (T)");
                    Builder.BeginMLine(205)
                        .AppendF("X", 10).AppendF("Y", 10).AppendF("Z", 0.4).AppendF("E", 2.5)
                        .AppendComment("Set jerk limits in mm/sec");
                    Builder.BeginMLine(205)
                        .AppendI("S", 0).AppendI("T", 0)
                        .AppendComment("Set minimum extrude and travel feed rate in mm/sec");
                }


            } 
		}

    }

}