using System.Collections.Generic;

namespace WhatWhereWhenGame.db.chgk.info
{
    public class GameWWW
    {
        private static readonly GameWWW instance = new GameWWW();

        private int currentIndex = 0;

        private List<QuestionWWW> questions = new List<QuestionWWW>();

        private int score = 0;

        protected GameWWW() { }

        public static GameWWW Instance
        {
            get { return instance; }
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
            set { currentIndex = value; }
        }

        public List<QuestionWWW> Questions
        {
            get { return questions; }
            set { questions = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
    }
}