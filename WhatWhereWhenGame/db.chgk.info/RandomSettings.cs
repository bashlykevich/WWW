using System;
using System.Collections.Generic;

namespace WhatWhereWhenGame.db.chgk.info
{
    public enum GameType { ChGK, BR, SI };

    public class RandomSettings
    {
        public DateTime DateFrom = DateTime.Now;
        public DateTime DateTo = DateTime.Now;
        public List<string> ComplexityList = new List<string>();
        public int ComplexityIndex = -1;
        public int Quantity = 24;
        public GameType game = GameType.ChGK;
    }
}