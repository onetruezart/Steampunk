using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public static class StringFormatExtencion
{
    public static string ToNormalFormat(this String str)
    {
        return str.Replace("$", @"'").Replace(@"\n", Environment.NewLine).Replace(@"\t", "   ");
    }

    public static string WithoutExtraChars(this string str)
    {
        return Regex.Replace(str.Replace(",", ""), "[ ]+", " ");
    }
}
