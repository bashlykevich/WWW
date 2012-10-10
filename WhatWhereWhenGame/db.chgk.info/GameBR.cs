using System.Collections.Generic;

namespace WhatWhereWhenGame.db.chgk.info
{
    public class GameBR
    {
        private static readonly GameBR instance = new GameBR();

        private int currentIndex = 0;

        private List<QuestionBR> questions = new List<QuestionBR>();

        private int score = 0;

        protected GameBR() { }

        public static GameBR Instance
        {
            get { return instance; }
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
            set { currentIndex = value; }
        }

        public List<QuestionBR> Questions
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