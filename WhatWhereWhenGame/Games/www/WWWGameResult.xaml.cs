using System;
using System.Windows;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.www
{
    public partial class WWWGameResult : PhoneApplicationPage
    {
        private int index;
        private bool RightAnswer = false;

        public WWWGameResult()
        {
            InitializeComponent();
            index = GameWWW.Instance.CurrentIndex;
            QuestionWWW q = GameWWW.Instance.Questions[index];
            edtAnswer.Text = "Ответ: " + q.Answer;
            edtAuthor.Text = "Автор: " + q.author;
            if (!String.IsNullOrEmpty(q.Comments))
                edtComments.Text = "Комментарий: " + q.Comments;
            if (q.userAnswer.ToUpper() == q.Answer.ToUpper())
            {
                GameWWW.Instance.Score++;
                edtMessage.Text = "Правильно, поздравляем!";
                RightAnswer = true;
            }
            else
            {
                edtMessage.Text = "Увы, вы ошиблись.";
            }

            edtScore.Text = GameWWW.Instance.Score.ToString() + ":" + (index + 1 - GameWWW.Instance.Score).ToString();
            if (index + 1 == GameWWW.Instance.Questions.Count)
            {
                btnOk.Content = "Главное меню";
                edtMessage.Text = "Игра закончена, спасибо за игру!";
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (index + 1 == GameWWW.Instance.Questions.Count)
            {
                // FINISH
                NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                // NEXT
                if (GameWWW.Instance.Questions.Count > index + 1)
                    GameWWW.Instance.CurrentIndex = index + 1;
                NavigationService.Navigate(new Uri(@"/Games/www/WWWGameQuestion.xaml?ind=" + GameWWW.Instance.CurrentIndex.ToString(), UriKind.Relative));
            }
        }

        private void btnScoreMyAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!RightAnswer)
            {
                GameWWW.Instance.Score++;
                edtMessage.Text = "Правильно, поздравляем!";
                edtScore.Text = GameWWW.Instance.Score.ToString() + ":" + (index + 1 - GameWWW.Instance.Score).ToString();
                RightAnswer = true;
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
        }
    }
}