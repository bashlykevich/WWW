using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.si
{
    public partial class SIGameQuestion : PhoneApplicationPage
    {
        private DispatcherTimer timer = new DispatcherTimer();

        private ThemeSI q;
        private int index = 0;

        private int timeToAnswer = 15;
        private int currentPoints = 0;
        private TextBlock[] tb = new TextBlock[5];

        private bool RightAnswer = false;

        public SIGameQuestion()
        {
            InitializeComponent();
            tb[0] = edtPoints10;
            tb[1] = edtPoints20;
            tb[2] = edtPoints30;
            tb[3] = edtPoints40;
            tb[4] = edtPoints50;

            if (GameSI.Instance.Themes.Count == 0)
                NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));

            index = GameSI.Instance.CurrentIndex;
            q = GameSI.Instance.Themes[index];
            edtTheme.Text = "ТЕМА №" + (index + 1).ToString() + ": " + q.name;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            BindQuestion();            
        }

        private void BindQuestion()
        {
            btnOk.Visibility = System.Windows.Visibility.Visible;
            btnOk.Content = " OK ";
            btnSkip.Content = "Пропустить";

            btnAnswer.IsEnabled = true;
            btnAnswer.Visibility = System.Windows.Visibility.Visible;
            btnSkip.Visibility = System.Windows.Visibility.Visible;
            edtTimer.Visibility = System.Windows.Visibility.Visible;
            btnGo.Visibility = System.Windows.Visibility.Collapsed;
            grAnswer.Visibility = System.Windows.Visibility.Collapsed;
            edtAnswer.Text = "";
            edtAnswer.IsReadOnly = false;
            edtAnswerTitle.Visibility = System.Windows.Visibility.Collapsed;
            btnFix.Visibility = System.Windows.Visibility.Collapsed;
            RightAnswer = false;

            timeToAnswer = 15;
            edtQuestion.Text = q.Questions[currentPoints];

            DisplayScore();

            foreach (TextBlock t in tb)
                t.FontSize = 26;
            tb[currentPoints].FontSize = 50;
            timer.Start();
        }

        private void DisplayScore()
        {
            edtTotal.Text = "";
            if (GameSI.Instance.Score > 0)
                edtTotal.Text = "+";
            edtTotal.Text += GameSI.Instance.Score.ToString();
        }

        private string GetTimeString(int secs)
        {
            if (secs == 0)
                return "ВРЕМЯ!";
            string r = "";
            int min = 0;
            int sec = 0;
            if (secs > 59)
            {
                min = secs / 60;
                if (min < 10)
                    r = "0" + min.ToString();
                else
                    r = min.ToString();
            }
            else
                r = "00";
            r += ":";
            sec = secs - 60 * min;
            if (sec < 10)
                r += "0" + sec.ToString();
            else
                r += sec.ToString();
            return r;
        }

        private void OnTimerTick(object sender, EventArgs args)
        {
            if (timeToAnswer >= 0)
            {
                edtTimer.Text = GetTimeString(timeToAnswer);
                timeToAnswer--;
            }
            else
            {
                btnAnswer.Visibility = System.Windows.Visibility.Collapsed;
                btnSkip.Content = "Далее";
            }
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            edtAnswer.IsReadOnly = true;
            edtTimer.Text = "";
            edtQuestion.Text = "Ответ: " + q.answers[currentPoints] + "\n\n"+ edtQuestion.Text;
            tb[currentPoints].Text = "-";
            btnAnswer.Visibility = System.Windows.Visibility.Collapsed;
            btnSkip.Visibility = System.Windows.Visibility.Collapsed;
            btnOk.Visibility = System.Windows.Visibility.Collapsed;            
            btnFix.Visibility = System.Windows.Visibility.Collapsed;
            btnGo.Visibility = System.Windows.Visibility.Visible;            
            btnOk.Content = "Далее";            
        }
        private void btnAnswer_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            btnAnswer.Visibility = System.Windows.Visibility.Collapsed;
            btnSkip.Visibility = System.Windows.Visibility.Collapsed;
            edtTimer.Visibility = System.Windows.Visibility.Collapsed;

            if (GameSI.Instance.IsTraining)
            {
                PostAnswer();
            }
            else
            {
                grAnswer.Visibility = System.Windows.Visibility.Visible;
                edtAnswerTitle.Visibility = System.Windows.Visibility.Visible;
                edtAnswer.Focus();
            }
        }

        private void NextTheme()
        {
            if (index + 1 == GameSI.Instance.Themes.Count)
            {
                // FINISH
                NavigationService.Navigate(new Uri(@"/Games/si/SIGameResult.xaml", UriKind.Relative));
            }
            else
            {
                // NEXT
                if (GameSI.Instance.Themes.Count > index + 1)
                    GameSI.Instance.CurrentIndex = index + 1;
                NavigationService.Navigate(new Uri(@"/Games/si/SIGameQuestion.xaml?ind=" + GameSI.Instance.CurrentIndex.ToString(), UriKind.Relative));
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {

            if (currentPoints < 4)
            {                
                currentPoints++;
                BindQuestion();
            }
            else
            {
                NextTheme();
            }
        }

        private void btnFix_Click(object sender, RoutedEventArgs e)
        {
            if (!RightAnswer)
            {
                RightAnswer = true;
                tb[currentPoints].Text = "+" + (currentPoints + 1) * 10;
                GameSI.Instance.Score += 2 * (currentPoints + 1) * 10;
                GameSI.Instance.statiscticsPlus[currentPoints]++;
                GameSI.Instance.statiscticsMinus[currentPoints]--;
                DisplayScore();
            }
            btnFix.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
        }

        void PostAnswer()
        {
            btnOk.Visibility = System.Windows.Visibility.Collapsed;
            btnGo.Visibility = System.Windows.Visibility.Visible;
            edtAnswer.IsReadOnly = true;
            //edtQuestion.Text += "\n\nОтвет: " + q.answers[currentPoints];
            edtQuestion.Text = "Ответ: " + q.answers[currentPoints] + "\n\n" + edtQuestion.Text;
            btnFix.Visibility = System.Windows.Visibility.Visible;
            btnOk.Content = "Далее";
            q.userAnswers.Add(edtAnswer.Text);
            if (edtAnswer.Text.ToUpper() == q.answers[currentPoints].ToUpper())
            {
                RightAnswer = true;
                tb[currentPoints].Text = "+" + tb[currentPoints].Text;
                GameSI.Instance.Score += (currentPoints + 1) * 10;
                GameSI.Instance.statiscticsPlus[currentPoints]++;
            }
            else
            {
                tb[currentPoints].Text = "-" + tb[currentPoints].Text;
                GameSI.Instance.Score -= (currentPoints + 1) * 10;
                GameSI.Instance.statiscticsMinus[currentPoints]++;
            }
            DisplayScore();
            this.Focus();
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            PostAnswer();
        }

        private void edtAnswer_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PostAnswer();
            }
        }
    }
}