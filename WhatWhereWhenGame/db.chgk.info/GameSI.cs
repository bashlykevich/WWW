using System.Collections.Generic;

namespace WhatWhereWhenGame.db.chgk.info
{
    public class GameSI
    {
        private static readonly GameSI instance = new GameSI();
        private int currentIndex = 0;

        public int[] statiscticsPlus = { 0, 0, 0, 0, 0 };
        public int[] statiscticsMinus = { 0, 0, 0, 0, 0 };

        private List<ThemeSI> themes = new List<ThemeSI>();

        private int score = 0;

        protected GameSI() { }

        public static GameSI Instance
        {
            get { return instance; }
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
            set { currentIndex = value; }
        }

        public List<ThemeSI> Themes
        {
            get { return themes; }
            set { themes = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        public void Reset()
        {
            this.currentIndex = 0;
            this.score = 0;
            for(int i=0;i<5;i++)
            {
                statiscticsMinus[i] = 0;
                statiscticsPlus[i] = 0;
            }            
            this.themes.Clear();            
        }
        public static ThemeSI Parse(string str)
        {
            ThemeSI res = new ThemeSI();
            string s1 = Helpers.HtmlRemoval.StripExtraCodes(str);
            string s2 = s1.Remove(0, 4).Trim(); // <hr>
            string s3 = s2.Remove(0, 3).Trim(); // <p>
            string s4 = s3.Remove(0, 9).Trim(); // <a href="
            res.url = s4.Substring(0, s4.IndexOf('>') - 1); // URL
            string s5 = s4.Remove(0, res.url.Length + 2).Trim(); // ">
            res.description = s5.Substring(0, s5.IndexOf("</a></p>")).Trim();
            string s6 = s5.Remove(0, s5.IndexOf("</strong>") + 9).Trim(); // </a></p> <strong>Вопрос 1:</strong>

            string name = s6.Substring(0, s6.IndexOf("<br>")).Trim();
            if (name.EndsWith("."))
                name = name.Substring(0, name.Length - 1);
            res.name = name;

            string s7 = s6.Remove(0, res.name.Length + 4).Trim(); // <br>
            string s8 = "";
            for (int i = 0; i < 5; i++)
            {
                s8 = s7.Remove(0, s7.IndexOf('.') + 1).Trim(); // 1.
                res.Questions.Add(s8.Substring(0, s8.IndexOf("<")).Trim());
                s7 = s8.Remove(0, s8.IndexOf("<")).Trim(); // <br>                
            }

            string s9 = s7.Remove(0, s7.IndexOf("<br>") + 4).Trim();
            string s10 = "";
            for (int i = 0; i < 5; i++)
            {
                s10 = s9.Remove(0, s9.IndexOf('.') + 1).Trim(); // 1.
                string a = s10.Substring(0, s10.IndexOf("<")).Trim();
                if (a.EndsWith("."))
                    a = a.Substring(0, a.Length - 1);
                res.answers.Add(a);
                s9 = s10.Remove(0, s10.IndexOf("<")).Trim(); // <br>                
            }

            string nodetext = s9;
            if (nodetext.Contains("<strong>Источник(и):</strong>"))
            {
                int start = nodetext.IndexOf("<strong>Источник(и):</strong>") + 29;
                string tmp = nodetext.Substring(start);
                int length = tmp.IndexOf("</p>");
                string txt = tmp.Substring(0, length);
                res.source = Helpers.HtmlRemoval.StripTagsCharArray(txt);
            }
            if (nodetext.Contains("<strong>Автор:</strong>"))
            {
                int start = nodetext.IndexOf("<strong>Автор:</strong>") + 24;
                string tmp = nodetext.Substring(start);
                int length = tmp.IndexOf("</p>");
                string txt = tmp.Substring(0, length);
                res.author = Helpers.HtmlRemoval.StripTagsCharArray(txt); ;
            }
            return res;
        }
    }
}