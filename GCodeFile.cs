using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gs
{



    public struct GCodeParam
    {
        public enum PType
        {
            Code,
            DoubleValue,
            IntegerValue,
            TextValue,
            Unknown
        }

        public PType type;
        public string identifier;

        public double doubleValue;      
        public int intValue {
            get { return (int)doubleValue; }    // we can store [-2^54, 2^54] precisely in a double
            set { doubleValue = value; }
        }
        public string textValue;
    }




    // ugh..class...dangerous!!
    public class GCodeLine
    {
        public enum LType
        {
            GCode,
            MCode, 
            UnknownCode,

            Comment,
            UnknownString,
            Blank,

            If,
            EndIf,
            Else,
            UnknownControl
        }




        public int lineNumber;

        public LType type;
        public string orig_string;

        public int N;       // N number of line
        public int code;    // G or M code
        public GCodeParam[] parameters;      // arguments/parameters

        public string comment;

        public GCodeLine(int num, LType type)
        {
            lineNumber = num;
            this.type = type;

            orig_string = null;
            N = code = -1;
            parameters = null;
            comment = null;
        }
    }



    public class GCodeFile
    {

        List<GCodeLine> lines;



        public GCodeFile()
        {
            lines = new List<GCodeLine>();
        }


        public void AppendLine(GCodeLine l)
        {
            lines.Add(l);
        }


        public IEnumerable<GCodeLine> AllLines()
        {
            int N = lines.Count;
            for (int i = 0; i < N; ++i) {
                yield return lines[i];
            }
        }


        public IEnumerable<GCodeLine> AllLinesOfType(GCodeLine.LType eType)
        {
            int N = lines.Count;
            for (int i = 0; i < N; ++i) {
                if ( lines[i].type == eType )
                    yield return lines[i];
            }
        }



    }
}
