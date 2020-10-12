using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Formatting {
    
    public static class NumberFormatting {

        public static string ToFormattedString(this int integer) {
            string str = integer.ToString();
            str += (str[^1]) switch
            {
                '1' => (str.CompareTo("11") != 0) ? "st" : "th",
                '2' => (str.CompareTo("12") != 0) ? "nd" : "th",
                '3' => (str.CompareTo("13") != 0) ? "rd" : "th",
                _ => "th",
            };
            return str;
        }

    }

}
