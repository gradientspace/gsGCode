﻿using System;
using g3;

namespace gs
{
    public class MachineInfo
    {
        public string ManufacturerName = "Unknown";
        public string ModelIdentifier = "Machine";

        public int MinExtruderTempC = 20;
        public int MaxExtruderTempC = 230;

        public bool HasHeatedBed = false;
        public int MinBedTempC = 0;
        public int MaxBedTempC = 0;

        public double MinLayerHeightMM = 0.2;
        public double MaxLayerHeightMM = 0.2;

        /*
         * All units are mm/min = (mm/s * 60)
         */
        public int MaxExtrudeSpeedMMM = 50 * 60;
        public int MaxTravelSpeedMMM = 100 * 60;
        public int MaxZTravelSpeedMMM = 20 * 60;
        public int MaxRetractSpeedMMM = 20 * 60;

        public double FilamentDiamenterMM = 1.75;
    }



    public class PlanarAdditiveSettings
	{
		public Vector2d BedSizeMM = new Vector2d(100,100);
		public double MaxHeightMM = 100;

		public double LayerHeightMM = 0.2;

	}



	public class SingleMaterialFFFSettings : PlanarAdditiveSettings
	{
        // This is a bit of an odd place for this, but settings are where we actually
        // know what assembler we should be using...
        public virtual AssemblerFactoryF AssemblerType() {
            throw new NotImplementedException("Settings.AssemblerType() not provided");
        }


        public MachineInfo Machine = new MachineInfo();

        public int ExtruderTempC = 210;
        public int HeatedBedTempC = 0;

		/*
		 * Distances.
		 * All units are mm
		 */

		public double NozzleDiamMM = 0.4;
		public double FilamentDiamMM = 1.75;

		public double FillPathSpacingMM = 0.4;

		public double RetractDistanceMM = 1.3;


		/*
		 * Speeds. 
		 * All units are mm/min = (mm/s * 60)
		 */

		// these are all in units of millimeters/minute
		public double RetractSpeed = 25 * 60;   // 1500

		public double ZTravelSpeed = 23 * 60;   // 1380

		public double RapidTravelSpeed = 150 * 60;  // 9000

		public double CarefulExtrudeSpeed = 30 * 60;  	// 1800
		public double RapidExtrudeSpeed = 90 * 60;      // 5400

		public double OuterPerimeterSpeedX = 0.5;


        /*
         * Shells
         */
        public int Shells = 2;
        public int InteriorSolidRegionShells = 1;       // how many shells to add around interior solid regions (eg roof/floor)

		/*
		 * Roof/Floors
		 */
		public int RoofLayers = 2;
		public int FloorLayers = 2;


		/*
		 * Sparse infill settings
		 */
		public double SparseLinearInfillStepX = 3.0;      // this is a multiplier on FillPathSpacingMM


        /*
         * Toolpath filtering options
         */
        public bool ClipSelfOverlaps = false;            // if true, try to remove portions of toolpaths that will self-overlap
        public double SelfOverlapToleranceX = 0.75;      // what counts as 'self-overla'. this is a multiplier on NozzleDiamMM


        /*
         * Debug/Utility options
         */

        public Interval1i LayerRangeFilter = new Interval1i(0, 999999999);   // only compute slices in this range
	}




    // just for naming...
    public class RepRapSettings : SingleMaterialFFFSettings
    {
        public override AssemblerFactoryF AssemblerType() {
            return RepRapAssembler.Factory;
        }

    }

}