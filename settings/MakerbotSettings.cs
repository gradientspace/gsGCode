using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs
{
	public static class Makerbot
	{
		public enum Models {
			Replicator2
		};

	}


	public class MakerbotSettings : SingleMaterialFFFSettings
	{
		public Makerbot.Models Model;


		public MakerbotSettings() {
			Model = Makerbot.Models.Replicator2;
			BedSizeMM = new Vector2d(100,100);
			MaxHeightMM = 100;

			ExtruderTempC = 230;
			NozzleDiamMM = 0.4;
			FilamentDiamMM = 1.75;



		}
	}

}