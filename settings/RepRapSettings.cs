using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs
{
	public static class RepRap
	{
		public enum Models {
            Unknown
        };

	}


	public class RepRapSettings : GenericRepRapSettings
    {
		public RepRap.Models Model;

        public override AssemblerFactoryF AssemblerType() {
            return RepRapAssembler.Factory;
        }


		public RepRapSettings(RepRap.Models model) {
			Model = model;

            if (model == RepRap.Models.Unknown)
                configure_unknown();
        }


        void configure_unknown()
        {
            Machine.ManufacturerName = "RepRap";
            Machine.ModelIdentifier = "Unknown";
            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = false;
            Machine.MaxBedTempC = 60;
            Machine.MaxExtrudeSpeedMMM = 50 * 60;
            Machine.MaxTravelSpeedMMM = 150 * 60;
            Machine.MaxZTravelSpeedMMM = 100 * 60;
            Machine.MaxRetractSpeedMMM = 40 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;

            BedSizeMM = new Vector2d(80, 80);
            MaxHeightMM = 55;
            LayerHeightMM = 0.2;

            ExtruderTempC = 200;
            HeatedBedTempC = 0;

            NozzleDiamMM = 0.4;
            FilamentDiamMM = 1.75;
            FillPathSpacingMM = 0.4;
            RetractDistanceMM = 4.5;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxZTravelSpeedMMM;
            CarefulExtrudeSpeed = 20 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }


    }

}