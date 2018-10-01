using System;
using g3;

namespace gs
{
    public enum MachineClass
    {
        Unknown,
        PlasticFFFPrinter,
        MetalSLSPrinter
    }


    public abstract class MachineInfo
    {
        protected static string UnknownUUID = "00000000-0000-0000-0000-000000000000";

        public string ManufacturerName = "Unknown";
        public string ManufacturerUUID = UnknownUUID;
        public string ModelIdentifier = "Machine";
        public string ModelUUID = UnknownUUID;
        public MachineClass Class = MachineClass.Unknown;

        public double BedSizeXMM = 100;
        public double BedSizeYMM = 100;
        public double MaxHeightMM = 100;


        public abstract T CloneAs<T>() where T : class;
        protected virtual void CopyFieldsTo(MachineInfo to)
        {
            to.ManufacturerName = this.ManufacturerName;
            to.ManufacturerUUID = this.ManufacturerUUID;
            to.ModelIdentifier = this.ModelIdentifier;
            to.ModelUUID = this.ModelUUID;
            to.Class = this.Class;
            to.BedSizeXMM = this.BedSizeXMM;
            to.BedSizeYMM = this.BedSizeYMM;
            to.MaxHeightMM = this.MaxHeightMM;
        }
    }



    public class FFFMachineInfo : MachineInfo
    {
        /*
         * printer mechanics
         */
        public double NozzleDiamMM = 0.4;
        public double FilamentDiamMM = 1.75;

        public double MinLayerHeightMM = 0.2;
        public double MaxLayerHeightMM = 0.2;

        /*
         * Temperatures
         */

        public int MinExtruderTempC = 20;
        public int MaxExtruderTempC = 230;

        public bool HasHeatedBed = false;
        public int MinBedTempC = 0;
        public int MaxBedTempC = 0;

        /*
         * All units are mm/min = (mm/s * 60)
         */
        public int MaxExtrudeSpeedMMM = 50 * 60;
        public int MaxTravelSpeedMMM = 100 * 60;
        public int MaxZTravelSpeedMMM = 20 * 60;
        public int MaxRetractSpeedMMM = 20 * 60;


        /*
         *  bed levelling
         */
        public bool HasAutoBedLeveling = false;
        public bool EnableAutoBedLeveling = false;


        /*
         * Hacks?
         */

        public double MinPointSpacingMM = 0.1;          // Avoid emitting gcode extrusion points closer than this spacing.
                                                        // This is a workaround for the fact that many printers do not gracefully
                                                        // handle very tiny sequential extrusion steps. This setting could be
                                                        // configured using CalibrationModelGenerator.MakePrintStepSizeTest() with
                                                        // all other cleanup steps disabled.
                                                        // [TODO] this is actually speed-dependent...

        public override T CloneAs<T>()
        {
            FFFMachineInfo fi = new FFFMachineInfo();
            this.CopyFieldsTo(fi);
            return fi as T;
        }
        protected virtual void CopyFieldsTo(FFFMachineInfo to)
        {
            base.CopyFieldsTo(to);

            to.NozzleDiamMM = this.NozzleDiamMM;
            to.FilamentDiamMM = this.FilamentDiamMM;
            to.MinLayerHeightMM = this.MinLayerHeightMM;
            to.MaxHeightMM = this.MaxHeightMM;
            to.MinExtruderTempC = this.MaxExtruderTempC;
            to.HasHeatedBed = this.HasHeatedBed;
            to.MinBedTempC = this.MinBedTempC;
            to.MaxBedTempC = this.MaxBedTempC;
            to.MaxExtrudeSpeedMMM = this.MaxExtrudeSpeedMMM;
            to.MaxTravelSpeedMMM = this.MaxTravelSpeedMMM;
            to.MaxZTravelSpeedMMM = this.MaxZTravelSpeedMMM;
            to.MaxRetractSpeedMMM = this.MaxRetractSpeedMMM;
            to.MinPointSpacingMM = this.MinPointSpacingMM;

            to.EnableAutoBedLeveling = this.EnableAutoBedLeveling;
            to.HasAutoBedLeveling = this.HasAutoBedLeveling;

            to.ManufacturerName = this.ManufacturerName;
            to.ManufacturerUUID = this.ManufacturerUUID;
            to.ModelIdentifier = this.ModelIdentifier;
            to.ModelUUID = this.ModelUUID;
            to.Class = this.Class;
            to.BedSizeXMM = this.BedSizeXMM;
            to.BedSizeYMM = this.BedSizeYMM;
            to.MaxHeightMM = this.MaxHeightMM;
        }
    }


    public interface IPlanarAdditiveSettings
    {
        string Identifier { get; set; }
        double LayerHeightMM { get; set; }
        T CloneAs<T>() where T : IPlanarAdditiveSettings, new();

    }

    public abstract class PlanarAdditiveSettings : IPlanarAdditiveSettings
    {
        /// <summary>
        /// This is the "name" of this settings (eg user identifier)
        /// </summary>
        private string identifier = "Defaults";
        public string Identifier { get => identifier; set => identifier = value; }

        public double LayerHeightMM { get; set; } = 0.2;


        public string ClassTypeName
        {
            get { return GetType().ToString(); }
        }


        public abstract MachineInfo BaseMachine { get; set; }

        public virtual T CloneAs<T>() where T : IPlanarAdditiveSettings, new()
        {
            T to = new T();
            this.CopyFieldsTo(to);
            return to;
        }

        protected virtual void CopyFieldsTo(IPlanarAdditiveSettings to)
        {
            to.Identifier = this.Identifier;
            to.LayerHeightMM = this.LayerHeightMM;
        }
    }

    public interface ISingleMaterialFFFSettings : IPlanarAdditiveSettings
    {
        FFFMachineInfo Machine { get; set; }
        double RapidTravelSpeed { get; set; }
        double CarefulExtrudeSpeed { get; set; }
        double RapidExtrudeSpeed { get; set; }
        double MinExtrudeSpeed { get; set; }
        double OuterPerimeterSpeedX { get; set; }
        double BridgeExtrudeSpeedX { get; set; }
        Interval1i LayerRangeFilter { get; set; }
        int FloorLayers { get; set; }
        int RoofLayers { get; set; }
        double ZTravelSpeed { get; set; }
        bool OuterShellLast { get; set; }
        double MinLayerTime { get; set; }
        double SparseLinearInfillStepX { get; set; }
        double SparseFillBorderOverlapX { get; set; }
        bool EnableSupportShell { get; set; }
        double SupportSpacingStepX { get; set; }
        bool EnableBridging { get; set; }
        int Shells { get; set; }
        int InteriorSolidRegionShells { get; set; }
        bool ClipSelfOverlaps { get; set; }
        double SelfOverlapToleranceX { get; set; }
        double SolidFillBorderOverlapX { get; set; }
        bool GenerateSupport { get; set; }
        double SupportOverhangAngleDeg { get; set; }
        double SupportRegionJoinTolX { get; set; }
        double SupportSolidSpace { get; set; }
        double SupportAreaOffsetX { get; set; }
        double SupportMinDimension { get; set; }
        double SupportPointDiam { get; set; }
        int SupportPointSides { get; set; }
        double MaxBridgeWidthMM { get; set; }
        double RetractSpeed { get; set; }
        bool EnableRetraction { get; set; }
        double RetractDistanceMM { get; set; }
        double MinRetractTravelLength { get; set; }
        double SupportVolumeScale { get; set; }
        double BridgeVolumeScale { get; set; }
        bool SupportMinZTips { get; set; }
        int ExtruderTempC { get; set; }
        int HeatedBedTempC { get; set; }
        double FanSpeedX { get; set; }
        double ShellsFillNozzleDiamStepX { get; set; }
        double SolidFillNozzleDiamStepX { get; set; }
        int StartLayers { get; set; }
        double StartLayerHeightMM { get; set; }
        bool EnableSupportReleaseOpt { get; set; }
        double SupportReleaseGap { get; set; }
        double BridgeFillNozzleDiamStepX { get; set; }

        double SolidFillPathSpacingMM();
        double ShellsFillPathSpacingMM();
        double BridgeFillPathSpacingMM();

        new T CloneAs<T>() where T : ISingleMaterialFFFSettings, new();
    }

    public class SingleMaterialFFFSettings : PlanarAdditiveSettings, ISingleMaterialFFFSettings
    {
        // This is a bit of an odd place for this, but settings are where we actually
        // know what assembler we should be using...
        public virtual AssemblerFactoryF AssemblerType()
        {
            throw new NotImplementedException("Settings.AssemblerType() not provided");
        }


        protected FFFMachineInfo machineInfo;
        public FFFMachineInfo Machine
        {
            get { if (machineInfo == null) machineInfo = new FFFMachineInfo(); return machineInfo; }
            set { machineInfo = value; }
        }


        public override MachineInfo BaseMachine
        {
            get { return Machine; }
            set
            {
                if (value is FFFMachineInfo)
                    machineInfo = value as FFFMachineInfo;
                else
                    throw new Exception("SingleMaterialFFFSettings.Machine.set: type is not FFFMachineInfo!");
            }
        }

        /*
         * Temperatures
         */

        public int ExtruderTempC { get; set; } = 210;
        public int HeatedBedTempC { get; set; } = 0;

        /*
		 * Distances.
		 * All units are mm
		 */

        public bool EnableRetraction { get; set; } = true;
        public double RetractDistanceMM { get; set; } = 1.3;
        public double MinRetractTravelLength { get; set; } = 2.5;     // don't retract if we are travelling less than this distance


        /*
		 * Speeds. 
		 * All units are mm/min = (mm/s * 60)
		 */

        // these are all in units of millimeters/minute
        public double RetractSpeed { get; set; } = 25 * 60;   // 1500

        public double ZTravelSpeed { get; set; } = 23 * 60;   // 1380

        public double RapidTravelSpeed { get; set; } = 150 * 60;  // 9000
        public double CarefulExtrudeSpeed { get; set; } = 30 * 60;      // 1800
        public double RapidExtrudeSpeed { get; set; } = 90 * 60;      // 5400
        public double MinExtrudeSpeed { get; set; } = 20 * 60;        // 600

        public double OuterPerimeterSpeedX { get; set; } = 0.5;

        public double FanSpeedX { get; set; } = 1.0;                  // default fan speed, fraction of max speed (generally unknown)

        /*
         * Shells
         */
        public int Shells { get; set; } = 2;
        public int InteriorSolidRegionShells { get; set; } = 0;       // how many shells to add around interior solid regions (eg roof/floor)
        public bool OuterShellLast { get; set; } = false;               // do outer shell last (better quality but worse precision)

        /*
		 * Roof/Floors
		 */
        public int RoofLayers { get; set; } = 2;
        public int FloorLayers { get; set; } = 2;

        /*
         *  Solid fill settings
         */
        public double ShellsFillNozzleDiamStepX { get; set; } = 1.0;      // multipler on Machine.NozzleDiamMM, defines spacing between adjacent
                                                                          // nested shells/perimeters. If < 1, they overlap.
        public double SolidFillNozzleDiamStepX { get; set; } = 1.0;       // multipler on Machine.NozzleDiamMM, defines spacing between adjacent
                                                                          // solid fill parallel lines. If < 1, they overlap.
        public double SolidFillBorderOverlapX { get; set; } = 0.25f;      // this is a multiplier on Machine.NozzleDiamMM, defines how far we
                                                                          // overlap solid fill onto border shells (if 0, no overlap)

        /*
		 * Sparse infill settings
		 */
        public double SparseLinearInfillStepX { get; set; } = 5.0;      // this is a multiplier on FillPathSpacingMM

        public double SparseFillBorderOverlapX { get; set; } = 0.25f;     // this is a multiplier on Machine.NozzleDiamMM, defines how far we
                                                                          // overlap solid fill onto border shells (if 0, no overlap)

        /*
         * Start layer controls
         */
        public int StartLayers { get; set; } = 0;                      // number of start layers, special handling
        public double StartLayerHeightMM { get; set; } = 0;            // height of start layers. If 0, same as regular layers


        /*
         * Support settings
         */
        public bool GenerateSupport { get; set; } = true;              // should we auto-generate support
        public double SupportOverhangAngleDeg { get; set; } = 35;      // standard "support angle"
        public double SupportSpacingStepX { get; set; } = 5.0;         // usage depends on support technique?           
        public double SupportVolumeScale { get; set; } = 1.0;          // multiplier on extrusion volume
        public bool EnableSupportShell { get; set; } = true;           // should we print a shell around support areas
        public double SupportAreaOffsetX { get; set; } = -0.5;         // 2D inset/outset added to support regions. Multiplier on Machine.NozzleDiamMM.
        public double SupportSolidSpace { get; set; } = 0.35f;         // how much space to leave between model and support
        public double SupportRegionJoinTolX { get; set; } = 2.0;		 // support regions within this distance will be merged via topological dilation. Multiplier on NozzleDiamMM.
        public bool EnableSupportReleaseOpt { get; set; } = true;      // should we use support release optimization
        public double SupportReleaseGap { get; set; } = 0.2f;          // how much space do we leave
        public double SupportMinDimension { get; set; } = 1.5;         // minimal size of support polygons
        public bool SupportMinZTips { get; set; } = true;              // turn on/off detection of support 'tip' regions, ie tiny islands.
        public double SupportPointDiam { get; set; } = 2.5f;           // width of per-layer support "points" (keep larger than SupportMinDimension!)
        public int SupportPointSides { get; set; } = 4;                // number of vertices for support-point polygons (circles)


        /*
		 * Bridging settings
		 */
        public bool EnableBridging { get; set; } = true;
        public double MaxBridgeWidthMM { get; set; } = 10.0;
        public double BridgeFillNozzleDiamStepX { get; set; } = 0.85;  // multiplier on FillPathSpacingMM
        public double BridgeVolumeScale { get; set; } = 1.0;           // multiplier on extrusion volume
        public double BridgeExtrudeSpeedX { get; set; } = 0.5;		 // multiplier on CarefulExtrudeSpeed


        /*
         * Toolpath filtering options
         */
        public double MinLayerTime { get; set; } = 5.0;                // minimum layer time in seconds
        public bool ClipSelfOverlaps { get; set; } = false;            // if true, try to remove portions of toolpaths that will self-overlap
        public double SelfOverlapToleranceX { get; set; } = 0.75;      // what counts as 'self-overlap'. this is a multiplier on NozzleDiamMM

        /*
         * Debug/Utility options
         */

        public Interval1i LayerRangeFilter { get; set; } = new Interval1i(0, 999999999);   // only compute slices in this range




        /*
         * functions that calculate derived values
         * NOTE: these cannot be properties because then they will be json-serialized!
         */
        public double ShellsFillPathSpacingMM()
        {
            return Machine.NozzleDiamMM * ShellsFillNozzleDiamStepX;
        }
        public double SolidFillPathSpacingMM()
        {
            return Machine.NozzleDiamMM * SolidFillNozzleDiamStepX;
        }
        public double BridgeFillPathSpacingMM()
        {
            return Machine.NozzleDiamMM * BridgeFillNozzleDiamStepX;
        }


        //public override T CloneAs<T> () 
        //{
        //    T copy = new T();
        //    this.CopyFieldsTo(copy);
        //    return copy as T;
        //}
        protected virtual void CopyFieldsTo(ISingleMaterialFFFSettings to)
        {
            base.CopyFieldsTo(to);
            to.Machine = this.machineInfo.CloneAs<FFFMachineInfo>();

            to.ExtruderTempC = this.ExtruderTempC;
            to.HeatedBedTempC = this.HeatedBedTempC;
            to.EnableRetraction = this.EnableRetraction;
            to.RetractDistanceMM = this.RetractDistanceMM;
            to.MinRetractTravelLength = this.MinRetractTravelLength;

            to.RetractSpeed = this.RetractSpeed;
            to.ZTravelSpeed = this.ZTravelSpeed;
            to.RapidTravelSpeed = this.RapidTravelSpeed;
            to.CarefulExtrudeSpeed = this.CarefulExtrudeSpeed;
            to.RapidExtrudeSpeed = this.RapidExtrudeSpeed;
            to.MinExtrudeSpeed = this.MinExtrudeSpeed;
            to.OuterPerimeterSpeedX = this.OuterPerimeterSpeedX;
            to.FanSpeedX = this.FanSpeedX;

            to.Shells = this.Shells;
            to.InteriorSolidRegionShells = this.InteriorSolidRegionShells;
            to.OuterShellLast = this.OuterShellLast;
            to.RoofLayers = this.RoofLayers;
            to.FloorLayers = this.FloorLayers;

            to.ShellsFillNozzleDiamStepX = this.ShellsFillNozzleDiamStepX;
            to.SolidFillNozzleDiamStepX = this.SolidFillNozzleDiamStepX;
            to.SolidFillBorderOverlapX = this.SolidFillBorderOverlapX;

            to.SparseLinearInfillStepX = this.SparseLinearInfillStepX;
            to.SparseFillBorderOverlapX = this.SparseFillBorderOverlapX;

            to.StartLayers = this.StartLayers;
            to.StartLayerHeightMM = this.StartLayerHeightMM;

            to.GenerateSupport = this.GenerateSupport;
            to.SupportOverhangAngleDeg = this.SupportOverhangAngleDeg;
            to.SupportSpacingStepX = this.SupportSpacingStepX;
            to.SupportVolumeScale = this.SupportVolumeScale;
            to.EnableSupportShell = this.EnableSupportShell;
            to.SupportAreaOffsetX = this.SupportAreaOffsetX;
            to.SupportSolidSpace = this.SupportSolidSpace;
            to.SupportRegionJoinTolX = this.SupportRegionJoinTolX;
            to.EnableSupportReleaseOpt = this.EnableSupportReleaseOpt;
            to.SupportReleaseGap = this.SupportReleaseGap;
            to.SupportMinDimension = this.SupportMinDimension;
            to.SupportMinZTips = this.SupportMinZTips;
            to.SupportPointDiam = this.SupportPointDiam;
            to.SupportPointSides = this.SupportPointSides;

            to.EnableBridging = this.EnableBridging;
            to.MaxBridgeWidthMM = this.MaxBridgeWidthMM;
            to.BridgeFillNozzleDiamStepX = this.BridgeFillNozzleDiamStepX;
            to.BridgeVolumeScale = this.BridgeVolumeScale;
            to.BridgeExtrudeSpeedX = this.BridgeExtrudeSpeedX;


            to.MinLayerTime = this.MinLayerTime;
            to.ClipSelfOverlaps = this.ClipSelfOverlaps;
            to.SelfOverlapToleranceX = this.SelfOverlapToleranceX;

            to.LayerRangeFilter = this.LayerRangeFilter;
        }

        //public T CloneAs2<T>() where T : ISingleMaterialFFFSettings, new()
        //{
        //    T to = new T();
        //    this.CopyFieldsTo(to);
        //    return to;
        //}

        T ISingleMaterialFFFSettings.CloneAs<T>()
        {
            T to = new T();
            this.CopyFieldsTo(to);
            return to;
        }
    }




    // just for naming...
    public class GenericRepRapSettings : SingleMaterialFFFSettings
    {
        public override AssemblerFactoryF AssemblerType()
        {
            return RepRapAssembler.Factory;
        }


        public override T CloneAs<T>()
        {
            T copy = new T();
            this.CopyFieldsTo(copy);
            return copy;
        }


    }

}