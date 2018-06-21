using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs.info
{
	public static class Flashforge
	{
        public const string UUID = "5974064e-8751-4048-a700-73a023add74f";

        public enum Models {
            Unknown,
			CreatorPro
		};

        public const string UUID_Unknown = "f387a3d4-ba39-4a60-9e68-93ce7af4d92a";
        public const string UUID_CreatorPro = "0dd884e1-9b19-436e-9558-e4cb5fb27f7d";
    }


	public class FlashforgeSettings : SingleMaterialFFFSettings, ISailfishSettings
    {
        public Flashforge.Models ModelEnum;

        public override AssemblerFactoryF AssemblerType() {
            return MakerbotAssembler.Factory;
        }

        public FlashforgeSettings(Flashforge.Models model = Flashforge.Models.CreatorPro) {
			ModelEnum = model;

            if (model == Flashforge.Models.CreatorPro)
                configure_CreatorPro();
            else
                configure_unknown();

        }


        public override T CloneAs<T>() {
            FlashforgeSettings copy = new FlashforgeSettings(this.ModelEnum);
            this.CopyFieldsTo(copy);
            return copy as T;
        }


        public static IEnumerable<SingleMaterialFFFSettings> EnumerateDefaults()
        {
            yield return new FlashforgeSettings(Flashforge.Models.CreatorPro);
            yield return new FlashforgeSettings(Flashforge.Models.Unknown);
        }


        public string GPXModelFlag {
            get {
                if (ModelEnum == Flashforge.Models.CreatorPro)
                    return "-m fcp";
                else
                    return "";
            }
        }


        void configure_CreatorPro()
        {
            Machine.ManufacturerName = "Flashforge";
            Machine.ManufacturerUUID = Flashforge.UUID;
            Machine.ModelIdentifier = "Creator Pro";
            Machine.ModelUUID = Flashforge.UUID_CreatorPro;
            Machine.Class = MachineClass.PlasticFFFPrinter;
            Machine.BedSizeXMM = 227;
            Machine.BedSizeYMM = 148;
            Machine.MaxHeightMM = 150;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = true;
            Machine.MaxBedTempC = 105;

            Machine.MaxExtrudeSpeedMMM = 60 * 60;
            Machine.MaxTravelSpeedMMM = 80 * 60;
            Machine.MaxZTravelSpeedMMM = 23 * 60;
            Machine.MaxRetractSpeedMMM = 20 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;

            LayerHeightMM = 0.2;

            ExtruderTempC = 230;
            HeatedBedTempC = 25;

            SolidFillNozzleDiamStepX = 1.0;
            RetractDistanceMM = 1.3;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxTravelSpeedMMM;
            CarefulExtrudeSpeed = 30 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }


        void configure_unknown()
        {
            Machine.ManufacturerName = "Flashforge";
            Machine.ManufacturerUUID = Flashforge.UUID;
            Machine.ModelIdentifier = "(Unknown)";
            Machine.ModelUUID = Flashforge.UUID_Unknown;
            Machine.Class = MachineClass.PlasticFFFPrinter;

            Machine.BedSizeXMM = 100;
            Machine.BedSizeYMM = 100;
            Machine.MaxHeightMM = 130;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = false;
            Machine.MaxBedTempC = 0;

            Machine.MaxExtrudeSpeedMMM = 60 * 60;
            Machine.MaxTravelSpeedMMM = 80 * 60;
            Machine.MaxZTravelSpeedMMM = 23 * 60;
            Machine.MaxRetractSpeedMMM = 20 * 60;
            Machine.MinLayerHeightMM = 0.1;
            Machine.MaxLayerHeightMM = 0.3;

            LayerHeightMM = 0.2;

            ExtruderTempC = 230;
            HeatedBedTempC = 0;

            SolidFillNozzleDiamStepX = 1.0;
            RetractDistanceMM = 1.3;

            RetractSpeed = Machine.MaxRetractSpeedMMM;
            ZTravelSpeed = Machine.MaxZTravelSpeedMMM;
            RapidTravelSpeed = Machine.MaxTravelSpeedMMM;
            CarefulExtrudeSpeed = 30 * 60;
            RapidExtrudeSpeed = Machine.MaxExtrudeSpeedMMM;
            OuterPerimeterSpeedX = 0.5;
        }

    }

}