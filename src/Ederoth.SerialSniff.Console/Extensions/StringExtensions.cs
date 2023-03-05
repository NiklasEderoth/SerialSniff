using System.Text;

namespace Ederoth.SerialSniffer;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static string ToShotClock(this ReadOnlySpan<char> str)
    {
        //include milli
        if (str[2] == '.')
        {
            return str.ToString();
        }       
        return str.ToString().Replace(" ", string.Empty); 
    }

    public static string ToGameClock(this ReadOnlySpan<char> str)
    {
        //include milli
        if (str[2] == '.')
        {
            return str.ToString();
        }
        if (str.Length == 4)
        {
            return $"{str[0]}{str[1]}.{str[2]}{str[3]}";
        }
        return $"{str[0]}.{str[1]}{str[2]}";
    }
}