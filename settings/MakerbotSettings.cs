using System.Collections.Generic;
using System.Linq;
using System.Text;
using g3;

namespace gs
{
	public static class Makerbot
	{
		public enum Models {
            Unknown,
			Replicator2
		};

	}


	public class MakerbotSettings : SingleMaterialFFFSettings
	{
        public Makerbot.Models Model;

        public override AssemblerFactoryF AssemblerType() {
            return MakerbotAssembler.Factory;
        }

        public MakerbotSettings(Makerbot.Models model = Makerbot.Models.Replicator2) {
			Model = model;

            Machine.ManufacturerName = "Makerbot";
            Machine.ModelIdentifier = model.ToString();

			BedSizeMM = new Vector2d(285, 153);
			MaxHeightMM = 155;

			ExtruderTempC = 230;
			NozzleDiamMM = 0.4;
			FilamentDiamMM = 1.75;

		}
	}

}