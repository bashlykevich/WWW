using System.Collections.Generic;

namespace WhatWhereWhenGame.db.chgk.info
{
    public class GameSI
    {
        private static readonly GameSI instance = new GameSI();
        private int currentIndex = 0;
        bool isTraining = false;

        public bool IsTraining
        {
            get { return isTraining; }
            set { isTraining = value; }
        }

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
            string s = Helpers.HtmlRemoval.StripExtraCodes(str);

            // Question Delimiter
            string QuestionDelemiter = ". ";
            if(s.Contains("10.")
                && s.Contains("20.")
                && s.Contains("30.")
                && s.Contains("40.")
                && s.Contains("50."))
            {
                QuestionDelemiter = "0. ";
            }
            
            // URL, DESCR
            string a = Substring(s, "<a ", "</a>", true);
            string url = Substring(a, "href=\"", "\">");
            string descr = Substring(a, "\">", "</a>");

            res.url = url;
            res.description = descr;

            // cut start
            s = s.Substring(s.IndexOf("<strong>Вопрос"));

            // theme name
            string n = Substring(s, "</strong>", " 1" + QuestionDelemiter);
            if (n.Contains("<br>"))
                n = n.Substring(0, n.IndexOf("<br>"));
            res.name = n;

            //questions
            string qs = Substring(s, n, "div class='collapsible collapsed'>");
            for (int i = 1; i < 6; i++)
            {
                string q = Substring(qs, i.ToString() + QuestionDelemiter, "<");
                res.Questions.Add(q);                
            }

            // cut start
            s = s.Substring(s.IndexOf("<strong>Ответ:</strong>"));

            // answers
            for (int i = 1; i < 6; i++)
            {
                string q = Substring(s, i.ToString() + QuestionDelemiter, "<");
                res.answers.Add(q);
            }
                
            if (s.Contains("<strong>Источник(и):</strong>"))
            {
                res.source = Helpers.HtmlRemoval.StripTagsCharArray(Substring(s, "<strong>Источник(и):</strong>", "</p>"));
            }
            if (s.Contains("<strong>Автор:</strong>"))
            {
                res.source = Helpers.HtmlRemoval.StripTagsCharArray(Substring(s, "<strong>Автор:</strong>", "</p>"));
            }
            if (s.Contains("<strong>Авторы:</strong>"))
            {
                res.source = Helpers.HtmlRemoval.StripTagsCharArray(Substring(s, "<strong>Авторы:</strong>", "</p>"));
            }
            return res;
        }
        private static string Substring(string src, string startChars, string endChars, bool includeLimiters=false)
        {
            int start = src.IndexOf(startChars);
            if (!includeLimiters)
                start += startChars.Length;
            
            src = src.Substring(start);
               
            int length = src.IndexOf(endChars);
            if (includeLimiters)
                length += endChars.Length;
            string res = src.Substring(0, length);
            return res;
        }
    }
}