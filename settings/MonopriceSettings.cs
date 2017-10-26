using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs
{
	public static class Monoprice
	{
		public enum Models {
            MP_Select_Mini_V2
        };

	}


	public class MonopriceSettings : RepRapSettings
    {
		public Monoprice.Models Model;

        public override AssemblerFactoryF AssemblerType() {
            return RepRapAssembler.Factory;
        }


		public MonopriceSettings(Monoprice.Models model) {
			Model = model;

            if (model == Monoprice.Models.MP_Select_Mini_V2)
                configure_MP_Select_Mini_V2();
        }


        void configure_MP_Select_Mini_V2()
        {
            Machine.ManufacturerName = "Monoprice";
            Machine.ModelIdentifier = "MP Select Mini V2";
            Machine.MaxExtruderTempC = 250;
            Machine.HasHeatedBed = true;
            Machine.MaxBedTempC = 60;
            Machine.MaxExtrudeSpeedMMM = 55 * 60;
            Machine.MaxTravelSpeedMMM = 150 * 60;
            Machine.MaxZTravelSpeedMMM = 100 * 60;
            Machine.MaxRetractSpeedMMM = 100 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;

            BedSizeMM = new Vector2d(120, 120);
            MaxHeightMM = 120;
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