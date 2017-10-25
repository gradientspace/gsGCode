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


	public class MonopriceSettings : SingleMaterialFFFSettings
	{
		public Monoprice.Models Model;

		public MonopriceSettings(Monoprice.Models model) {
			Model = model;

            if (model == Monoprice.Models.MP_Select_Mini_V2)
                configure_MP_Select_Mini_V2();
        }


        void configure_MP_Select_Mini_V2()
        {
            Limits.MaxExtruderTempC = 250;
            Limits.HasHeatedBed = true;
            Limits.MaxBedTempC = 60;
            Limits.MaxExtrudeSpeedMMM = 55 * 60;
            Limits.MaxTravelSpeedMMM = 150 * 60;
            Limits.MaxZTravelSpeedMMM = 100 * 60;
            Limits.MaxRetractSpeedMMM = 40 * 60;
            Limits.MinLayerHeightMM = 0.1;
            Limits.MaxLayerHeightMM = 0.3;

            BedSizeMM = new Vector2d(120, 120);
            MaxHeightMM = 120;
            LayerHeightMM = 0.2;

            ExtruderTempC = 210;
            HeatedBedTempC = 30;

            NozzleDiamMM = 0.4;
            FilamentDiamMM = 1.75;
            FillPathSpacingMM = 0.4;
            RetractDistanceMM = 4.5;

            RetractSpeed = Limits.MaxRetractSpeedMMM;
            ZTravelSpeed = Limits.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Limits.MaxZTravelSpeedMMM;
            CarefulExtrudeSpeed = 20 * 60;
            RapidExtrudeSpeed = Limits.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }


    }

}