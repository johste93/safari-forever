using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class StringExtensions
{
    public static string FirstLetterToUpper(this string str)
    {
        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    public static void CopyToClipboard(this string s)
    {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }

    public static byte[] ToByte(this string s)
    {
        return Encoding.UTF8.GetBytes(s);
    }
}
