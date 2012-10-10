namespace WhatWhereWhenGame.db.chgk.info
{
    public class QuestionBR
    {
        public string author;
        public string description;
        public string source;
        public string url;
        public string userAnswer;
        private string answer;
        private string comments;
        private string question;

        public string Answer
        {
            get { return answer; }
            set
            {
                string s = value;
                s = s.Trim();
                if (s[s.Length - 1] == '.')
                    s = s.Substring(0, s.Length - 1);
                s = s.Replace("<br>", " ");
                s = s.Replace("&mdash;", "-");
                s = s.Replace("&nbsp;", " ");
                answer = s;
            }
        }

        public string Comments
        {
            get { return comments; }
            set
            {
                string s = value;
                s = s.Replace("<br>", " ");
                s = s.Replace("&mdash;", "-");
                s = s.Replace("&nbsp;", " ");
                comments = s;
            }
        }

        public string Question
        {
            get { return question; }
            set
            {
                string v = value;
                v = v.Replace("<br>", " ");
                v = v.Replace("&mdash;", "-");
                v = v.Replace("&nbsp;", " ");

                //v = v.Replace("", "");

                question = v;
            }
        }
    }
}