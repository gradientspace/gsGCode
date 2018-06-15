using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs.info
{
	public class GenericPrinterSettings : GenericRepRapSettings
    {
        public override AssemblerFactoryF AssemblerType() {
            return RepRapAssembler.Factory;
        }

        public string ManufacturerName;
        public string ManufacturerUUID;
        public string DefaultMachineUUID;

		public GenericPrinterSettings(string mfgName, string mfgUUID, string defaultMachineUUID) {
            ManufacturerName = mfgName;
            ManufacturerUUID = mfgUUID;
            DefaultMachineUUID = defaultMachineUUID;

            configure_unknown();
        }


        public override T CloneAs<T>() {
            GenericPrinterSettings copy = new GenericPrinterSettings(ManufacturerName, ManufacturerUUID, DefaultMachineUUID);
            this.CopyFieldsTo(copy);
            return copy as T;
        }


        void configure_unknown()
        {
            Machine.ManufacturerName = ManufacturerName;
            Machine.ManufacturerUUID = ManufacturerUUID;
            Machine.ModelIdentifier = "Unknown";
            Machine.ModelUUID = DefaultMachineUUID;
            Machine.Class = MachineClass.PlasticFFFPrinter;
            Machine.BedSizeXMM = 80;
            Machine.BedSizeYMM = 80;
            Machine.MaxHeightMM = 55;
            Machine.NozzleDiamMM = 0.4;
            Machine.FilamentDiamMM = 1.75;

            Machine.MaxExtruderTempC = 230;
            Machine.HasHeatedBed = false;
            Machine.MaxBedTempC = 60;

            Machine.MaxExtrudeSpeedMMM = 50 * 60;
            Machine.MaxTravelSpeedMMM = 150 * 60;
            Machine.MaxZTravelSpeedMMM = 100 * 60;
            Machine.MaxRetractSpeedMMM = 40 * 60;
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


    }

}