using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs.info
{
	public static class Monoprice
	{
        public const string UUID = "860432eb-dec6-4b20-8f97-3643a50daf1d";

        public enum Models {
            Unknown = 0,
            MP_Select_Mini_V2 = 1
        };

        public const string UUID_Unknown = "409e843b-c1d0-45fc-82d1-d7cc5635b8ee";
        public const string UUID_MP_Select_Mini_V2 = "4a498843-9080-4c97-aa82-b587f415ab1f";
    }


	public class MonopriceSettings : GenericRepRapSettings
    {
		public Monoprice.Models ModelEnum;

        public override AssemblerFactoryF AssemblerType() {
            return RepRapAssembler.Factory;
        }


		public MonopriceSettings(Monoprice.Models model) {
			ModelEnum = model;

            if (model == Monoprice.Models.MP_Select_Mini_V2)
                configure_MP_Select_Mini_V2();
            else
                configure_unknown();
        }

        public override T CloneAs<T>() {
            MonopriceSettings copy = new MonopriceSettings(this.ModelEnum);
            this.CopyFieldsTo(copy);
            return copy as T;
        }

        public static IEnumerable<SingleMaterialFFFSettings> EnumerateDefaults()
        {
            yield return new MonopriceSettings(Monoprice.Models.MP_Select_Mini_V2);
            yield return new MonopriceSettings(Monoprice.Models.Unknown);
        }



        void configure_MP_Select_Mini_V2()
        {
            Machine.ManufacturerName = "Monoprice";
            Machine.ManufacturerUUID = Monoprice.UUID;
            Machine.ModelIdentifier = "MP Select Mini V2";
            Machine.ModelUUID = Monoprice.UUID_MP_Select_Mini_V2;
            Machine.Class = MachineClass.PlasticFFFPrinter;
            Machine.BedSizeXMM = 120;
            Machine.BedSizeYMM = 120;
            Machine.MaxHeightMM = 120;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 250;
            Machine.HasHeatedBed = true;
            Machine.MaxBedTempC = 60;

            Machine.MaxExtrudeSpeedMMM = 55 * 60;
            Machine.MaxTravelSpeedMMM = 150 * 60;
            Machine.MaxZTravelSpeedMMM = 100 * 60;
            Machine.MaxRetractSpeedMMM = 100 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;


            LayerHeightMM = 0.2;

            ExtruderTempC = 200;
            HeatedBedTempC = 0;

            SolidFillNozzleDiamStepX = 1.0;
            RetractDistanceMM = 4.5;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxTravelSpeedMMM;
            CarefulExtrudeSpeed = 20 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }




        void configure_unknown()
        {
            Machine.ManufacturerName = "Monoprice";
            Machine.ManufacturerUUID = Monoprice.UUID;
            Machine.ModelIdentifier = "(Unknown)";
            Machine.ModelUUID = Monoprice.UUID_Unknown;
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


    }

}