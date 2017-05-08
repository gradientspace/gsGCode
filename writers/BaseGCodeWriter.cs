using System;
using System.IO;
using g3;

namespace gs 
{
	public abstract class BaseGCodeWriter 
	{

		public virtual void WriteFile(GCodeFile file, StreamWriter outStream) 
		{
			foreach ( var line in file.AllLines() )
				WriteLine(line, outStream);
		}


		public abstract void WriteLine(GCodeLine line, StreamWriter outStream);

	}
}
