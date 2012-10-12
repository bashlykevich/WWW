using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.br
{
    public partial class BRGameQuestion : PhoneApplicationPage
    {
        private DispatcherTimer timer = new DispatcherTimer();

        private QuestionBR q;
        private int index = 0;

        private int timeToRead = 10;
        private int timeToThink = 30;
        private int timeToAnswer = 10;

        public BRGameQuestion()
        {
            InitializeComponent();
            index = GameBR.Instance.CurrentIndex;
            q = GameBR.Instance.Questions[index];

            timeToRead = q.Question.Length / 20;
            timeToAnswer = q.Answer.Length * 2;

            edtNumber.Text = "Вопрос №" + (index + 1).ToString();
            edtQuestion.Text = q.Question.Replace("\n", " ");

            //            edtAnswer.Text = q.answer;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            timer.Start();
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
            if (timeToRead >= 0)
            {
                edtTimer.Text = GetTimeString(timeToRead);
                timeToRead--;
                if (timeToRead == 0)
                {
                    edtStatus.Text = "...";
                }
            }
            else if (timeToThink >= 0)
            {
                edtTimer.Text = GetTimeString(timeToThink);
                timeToThink--;
                if (timeToThink == 0)
                    edtStatus.Text = "Время на ввод ответа";
            }
            else if (timeToAnswer >= 0)
            {
                edtTimer.Text = GetTimeString(timeToAnswer);
                timeToAnswer--;
            }
            else
                Answer();
        }

        private void Answer()
        {
            edtStatus.Text = "...";
            timer.Stop();

            GameBR.Instance.Questions[index].userAnswer = edtAnswer.Text;
            NavigationService.Navigate(new Uri(@"/Games/br/BRGameResult.xaml?ind=" + GameBR.Instance.CurrentIndex.ToString(), UriKind.Relative));
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            Answer();
        }

        private void btnAnswer_Click(object sender, RoutedEventArgs e)
        {
            Answer();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
        }
    }
}