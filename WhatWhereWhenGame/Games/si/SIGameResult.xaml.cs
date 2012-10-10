using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.si
{
    public partial class SIGameResult : PhoneApplicationPage
    {
        public SIGameResult()
        {
            InitializeComponent();
            edtThemesQ.Text = "Количество тем: " + GameSI.Instance.Themes.Count.ToString();
            if (GameSI.Instance.Score > 0)
                edtStatResult.Text = "+" + GameSI.Instance.Score.ToString();
            else
                edtStatResult.Text = GameSI.Instance.Score.ToString();
            TextBlock[] tb = new TextBlock[5];
            tb[0] = edtStat10;
            tb[1] = edtStat20;
            tb[2] = edtStat30;
            tb[3] = edtStat40;
            tb[4] = edtStat50;

            for (int i = 0; i < 5; i++)
            {
                int sc = 10 * (i + 1);
                tb[i].Text = sc.ToString() + ": +" + GameSI.Instance.statiscticsPlus[i].ToString() + " -" + GameSI.Instance.statiscticsMinus[i].ToString() + " = " + (sc * (GameSI.Instance.statiscticsPlus[i] - GameSI.Instance.statiscticsMinus[i])).ToString();
            }
        }

        private void btnOk_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
        }
    }
}