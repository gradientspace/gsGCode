using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gs
{
    static public class GCodeParseUtil
    {

        public enum NumberType
        {
            Integer, Decimal, NotANumber
        }

        // doesn't handle e^ numbers
        // doesn't allow commas
        static public NumberType GetNumberType(string s)
        {
            int N = s.Length;

            bool saw_digit = false;
            bool saw_dot = false;
            bool saw_sign = false;
            for (int i = 0; i < N; ++i) {
                char c = s[i];
                if ( c == '-' ) {
                    if (saw_digit || saw_dot || saw_sign)
                        return NumberType.NotANumber;
                    saw_sign = true;
                } else if ( c == '.' ) {
                    if (saw_dot)
                        return NumberType.NotANumber;
                    saw_dot = true;
                } else if (Char.IsDigit(c)) {
                    saw_digit = true;
                } else {
                    return NumberType.NotANumber;
                }
            }
            return (saw_dot) ? NumberType.Decimal : NumberType.Integer;
        }


    }
}
