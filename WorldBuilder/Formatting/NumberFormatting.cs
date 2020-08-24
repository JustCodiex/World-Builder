using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Formatting {
    
    public static class NumberFormatting {
    
        public static string ToFormattedString(this int integer) {
            string str = integer.ToString();
            str += (str[^0]) switch
            {
                '1' => "st",
                '2' => "nd",
                '3' => "rd",
                _ => "th",
            };
            return str;
        }

    }

}
