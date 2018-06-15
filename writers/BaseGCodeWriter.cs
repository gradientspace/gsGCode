using System;
using System.IO;
using System.Threading;
using System.Globalization;
using g3;

namespace gs 
{
	public abstract class BaseGCodeWriter 
	{
        /// <summary>
        /// If the mesh format we are writing is text, then the OS will write in the number style
        /// of the current language. So in Germany, numbers are written 1,00 instead of 1.00, for example.
        /// If this flag is true, we override this to always write in a consistent way.
        /// </summary>
        public bool WriteInvariantCulture = true;


        public virtual IOWriteResult WriteFile(GCodeFile file, StreamWriter outStream) 
		{
            // save current culture
            var current_culture = Thread.CurrentThread.CurrentCulture;

            try {  
                if (WriteInvariantCulture)
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                foreach ( var line in file.AllLines() )
				    WriteLine(line, outStream);

                // restore culture
                if (WriteInvariantCulture)
                    Thread.CurrentThread.CurrentCulture = current_culture;

                return IOWriteResult.Ok;

            } catch (Exception e) {
                // restore culture
                if (WriteInvariantCulture)
                    Thread.CurrentThread.CurrentCulture = current_culture;
                return new IOWriteResult(IOCode.WriterError, "Unknown error : exception : " + e.Message);
            }
        }


		public abstract void WriteLine(GCodeLine line, StreamWriter outStream);

	}
}
