using System.Text.RegularExpressions;

namespace WhatWhereWhenGame.Helpers
{
    /// <summary>
    /// Methods to remove HTML from strings.
    /// </summary>
    public static class HtmlRemoval
    {
        /// <summary>
        /// Remove char codes from string with Regex.
        /// </summary>
        public static string StripCharCodes(string source)
        {
            string outp = source;
            outp = outp.Replace("\n", " ");
            outp = outp.Replace("&nbsp;", " ");
            outp = outp.Replace("&mdash;", "-");
            return outp;
        }
        public static string StripExtraCodes(string source)
        {
            string outp = source;
            outp = outp.Replace("\n", " ");
            outp = outp.Replace("&nbsp;", " ");
            outp = outp.Replace("&mdash;", "-");
            while (outp.Contains("  "))
                outp = outp.Replace("  ", " ");
            outp = outp.Trim();
            return outp;
        }
        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        private static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            source = StripCharCodes(source);
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            string s = new string(array, 0, arrayIndex);
            s = s.Trim();
            if (s.StartsWith("."))
                s = s.Substring(1);
            return s;
        }
    }
}