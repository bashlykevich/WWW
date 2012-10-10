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
    }
}