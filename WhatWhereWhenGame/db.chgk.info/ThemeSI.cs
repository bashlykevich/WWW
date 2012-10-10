using System.Collections.Generic;

namespace WhatWhereWhenGame.db.chgk.info
{
    public class ThemeSI
    {
        public string author;
        public string description;
        public string source;
        public string url;
        public string comments;
        public string name;
        public List<string> userAnswers = new List<string>();
        public List<string> answers = new List<string>();
        private List<string> questions = new List<string>();

        public List<string> Questions
        {
            get { return questions; }
            set { questions = value; }
        }
    }
}