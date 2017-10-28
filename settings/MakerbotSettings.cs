using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs
{
	public static class Makerbot
	{
		public enum Models {
            Unknown,
			Replicator2
		};

	}


	public class MakerbotSettings : SingleMaterialFFFSettings
	{
        public Makerbot.Models Model;

        public override AssemblerFactoryF AssemblerType() {
            return MakerbotAssembler.Factory;
        }

        public MakerbotSettings(Makerbot.Models model = Makerbot.Models.Replicator2) {
			Model = model;

            if (model == Makerbot.Models.Replicator2)
                configure_Replicator_2();
            else
                configure_unknown();

        }



        void configure_Replicator_2()
        {
            Machine.ManufacturerName = "Makerbot";
            Machine.ModelIdentifier = "Replicator 2";
            Machine.Class = MachineClass.PlasticFFFPrinter;
            Machine.BedSizeMM = new Vector2d(285, 153);
            Machine.MaxHeightMM = 155;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = false;
            Machine.MaxBedTempC = 0;

            Machine.MaxExtrudeSpeedMMM = 90 * 60;
            Machine.MaxTravelSpeedMMM = 150 * 60;
            Machine.MaxZTravelSpeedMMM = 23 * 60;
            Machine.MaxRetractSpeedMMM = 25 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;


            LayerHeightMM = 0.2;

            ExtruderTempC = 230;
            HeatedBedTempC = 0;

            FillPathSpacingMM = 0.4;
            RetractDistanceMM = 1.3;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxZTravelSpeedMMM;
            CarefulExtrudeSpeed = 30 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }


        void configure_unknown()
        {
            Machine.ManufacturerName = "Makerbot";
            Machine.ModelIdentifier = "(Unknown)";
            Machine.Class = MachineClass.PlasticFFFPrinter;

            Machine.BedSizeMM = new Vector2d(100,100);
            Machine.MaxHeightMM = 130;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = false;
            Machine.MaxBedTempC = 0;
            Machine.MaxExtrudeSpeedMMM = 90 * 60;
            Machine.MaxTravelSpeedMMM = 150 * 60;
            Machine.MaxZTravelSpeedMMM = 23 * 60;
            Machine.MaxRetractSpeedMMM = 25 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;


            LayerHeightMM = 0.2;

            ExtruderTempC = 230;
            HeatedBedTempC = 0;

            FillPathSpacingMM = 0.4;
            RetractDistanceMM = 1.3;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxZTravelSpeedMMM;
            CarefulExtrudeSpeed = 30 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }

    }

}