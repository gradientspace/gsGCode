using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace gs
{
    class GenericGCodeParser
    {

        public void Parse(TextReader input)
        {
            GCodeFile file = new GCodeFile();

            int lines = 0;
            while ( input.Peek() >= 0 ) {
                string line = input.ReadLine();
                int nLineNum = lines++;

                GCodeFile.Line l = ParseLine(line, nLineNum);
                file.AppendLine(l);
            }

        }



        public GCodeFile.Line ParseLine(string line, int nLineNum)
        {
            if (line.Length == 0)
                return make_blank(nLineNum);
            if (line[0] == ';')
                return make_comment(line, nLineNum);

            string[] tokens = line.Split( (char[])null , StringSplitOptions.RemoveEmptyEntries);

            // handle extra spaces at start...?
            if (tokens.Length == 0)
                return make_blank(nLineNum);
            if (tokens[0][0] == ';')
                return make_comment(line, nLineNum);

            if ( tokens[0][0] == 'N' ) 
                return make_code_line(line, tokens, nLineNum);

            if (tokens[0][0] == ':')
                return make_control_line(line, tokens, nLineNum);

            return make_string_line(line, nLineNum);
        }




        
        // N### lines
        public GCodeFile.Line make_code_line(string line, string[] tokens, int nLineNum)
        {
            GCodeFile.LineType eType = GCodeFile.LineType.UnknownCode;
            if (tokens[1][0] == 'G')
                eType = GCodeFile.LineType.GCode;
            else if (tokens[1][0] == 'M')
                eType = GCodeFile.LineType.MCode;

            GCodeFile.Line l = new GCodeFile.Line(nLineNum, eType);
            l.orig_string = line;

            l.N = int.Parse(tokens[0].Substring(1));

            // [TODO] comments

            if (eType == GCodeFile.LineType.UnknownCode) {
                if (tokens.Length > 1)
                    l.parameters = parse_parameters(tokens, 1);
            } else {
                l.code = int.Parse(tokens[1].Substring(1));
                if (tokens.Length > 2)
                    l.parameters = parse_parameters(tokens, 2);
            }

            return l;
        }






        // any line we can't understand
        public GCodeFile.Line make_string_line(string line, int nLineNum)
        {
            GCodeFile.Line l = new GCodeFile.Line(nLineNum, GCodeFile.LineType.UnknownString);
            l.orig_string = line;
            return l;
        }



        // :IF, :ENDIF, :ELSE
        public GCodeFile.Line make_control_line(string line, string[] tokens, int nLineNum)
        {
            // figure out command type
            string command = tokens[0].Substring(1);
            GCodeFile.LineType eType = GCodeFile.LineType.UnknownControl;
            if (command.Equals("if", StringComparison.OrdinalIgnoreCase))
                eType = GCodeFile.LineType.If;
            else if (command.Equals("else", StringComparison.OrdinalIgnoreCase))
                eType = GCodeFile.LineType.Else;
            else if (command.Equals("endif", StringComparison.OrdinalIgnoreCase))
                eType = GCodeFile.LineType.EndIf;

            GCodeFile.Line l = new GCodeFile.Line(nLineNum, eType);
            l.orig_string = line;

            if (tokens.Length > 1)
                l.parameters = parse_parameters(tokens, 1);

            return l;
        }



        // line starting with ;
        public GCodeFile.Line make_comment(string line, int nLineNum)
        {
            GCodeFile.Line l = new GCodeFile.Line(nLineNum, GCodeFile.LineType.Comment);

            l.orig_string = line;
            int iStart = line.IndexOf(';');
            l.comment = line.Substring(iStart);
            return l;
        }


        // line with no text at all
        public GCodeFile.Line make_blank(int nLineNum)
        {
            return new GCodeFile.Line(nLineNum, GCodeFile.LineType.Blank);
        }







        public GCodeFile.Parameter[] parse_parameters(string[] tokens, int iStart, int iEnd = -1)
        {
            if (iEnd == -1)
                iEnd = tokens.Length;

            int N = iEnd - iStart;
            GCodeFile.Parameter[] paramList = new GCodeFile.Parameter[N];

            for ( int i = iStart; i < iEnd; ++i ) {
                if ( tokens[i].Contains('=') ) {
                    parse_value_parameter(tokens[i], ref paramList[i]);

                } else if ( tokens[i][0] == 'G' || tokens[i][0] == 'M' ) {
                    parse_code_parameter(tokens[i], ref paramList[i]);

                } else {
                    paramList[i].type = GCodeFile.ParamType.Unknown;
                    paramList[i].identifier = tokens[i];
                }
            }

            return paramList;
        }



        public bool parse_code_parameter(string token, ref GCodeFile.Parameter param)
        {
            param.type = GCodeFile.ParamType.Code;
            param.identifier = token;

            string value = token.Substring(1);
            GCodeParseUtil.NumberType numType = GCodeParseUtil.GetNumberType(value);
            if (numType == GCodeParseUtil.NumberType.Integer)
                param.intValue = int.Parse(value);

            return true;
        }



        public bool parse_value_parameter(string token, ref GCodeFile.Parameter param)
        {
            int i = token.IndexOf('=');

            param.identifier = token.Substring(0, i);

            string value = token.Substring(i + 1, token.Length - i);

            try {

                GCodeParseUtil.NumberType numType = GCodeParseUtil.GetNumberType(value);
                if (numType == GCodeParseUtil.NumberType.Decimal) {
                    param.type = GCodeFile.ParamType.DoubleValue;
                    param.doubleValue = double.Parse(value);
                    return true;
                } else if (numType == GCodeParseUtil.NumberType.Integer) {
                    param.type = GCodeFile.ParamType.IntegerValue;
                    param.intValue = int.Parse(value);
                    return true;
                }
            } catch {
                // just continue on and do generic string param
            }

            param.type = GCodeFile.ParamType.TextValue;
            param.textValue = value;
            return true;
        }



    }
}
