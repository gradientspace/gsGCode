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

		public double NozzleDiamMM = 0.4;
		public double FilamentDiamMM = 1.75;
	}

}