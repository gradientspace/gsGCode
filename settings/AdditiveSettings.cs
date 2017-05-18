using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs
{
	public class PlanarAdditiveSettings
	{
		public Vector2d BedSizeMM = new Vector2d(100,100);
		public double MaxHeightMM = 100;

		public double LayerHeightMM = 0.2;

	}




	public class SingleMaterialFFFSettings : PlanarAdditiveSettings
	{
		public int ExtruderTempC = 210;

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

		// these are all in untis of millimeters/minute
		public double RetractSpeed = 25 * 60;   // 1500

		public double ZTravelSpeed = 23 * 60;   // 1380

		public double RapidTravelSpeed = 150 * 60;  // 9000

		public double FirstLayerExtrudeSpeed = 30 * 60;  // 1800
		public double RapidExtrudeSpeed = 90 * 60;		 // 5400
	}

}