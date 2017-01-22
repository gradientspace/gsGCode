using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gs
{
    public class GCodeFile
    {
        public enum LineType
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



        public enum ParamType
        {
            Code,
            DoubleValue,
            IntegerValue,
            TextValue,
            Unknown
        }


        public struct Parameter
        {
            public ParamType type;
            public string identifier;

            public double doubleValue;      
            public int intValue {
                get { return (int)doubleValue; }    // we can store [-2^54, 2^54] precisely in a double
                set { doubleValue = value; }
            }
            public string textValue;
        }


        public struct Line
        {
            public int lineNumber;

            public LineType type;
            public string orig_string;

            public int N;       // N number of line
            public int code;    // G or M code
            public Parameter[] parameters;      // arguments/parameters

            public string comment;

            public Line(int num, LineType type)
            {
                lineNumber = num;
                this.type = type;

                orig_string = null;
                N = code = -1;
                parameters = null;
                comment = null;
            }
        }



        List<Line> lines;



        public GCodeFile()
        {
            lines = new List<Line>();
        }


        public void AppendLine(Line l)
        {
            lines.Add(l);
        }


    }
}
