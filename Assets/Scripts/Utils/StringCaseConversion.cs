using System.Text.RegularExpressions;

public static class StringCaseConversion
{
    static Regex Pattern = new Regex(@"([A-Z][a-z0-9]*)|[0-9]+");
    /// <summary>
    /// Converts "PascalCase" to "pascal-case"
    /// </summary>
    /// <param name="pascal"></param>
    /// <returns></returns>
    public static string ToLowerKebabCase(this string pascal)
    {
        MatchCollection matches = Pattern.Matches(pascal);
        bool addHyphen = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (Match match in matches)
        {
            if (addHyphen)
            {
                sb.Append("-");
            }
            else
            {
                addHyphen = true;
            }
            sb.Append(match.Value.ToLower());
        }
        return sb.ToString();
    }
}