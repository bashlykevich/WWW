using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.www
{
    public partial class WWWSettings : PhoneApplicationPage
    {
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("ShowProgress", typeof(bool), typeof(WWWSettings), new PropertyMetadata(false));

        public WWWSettings()
        {
            InitializeComponent();

            RandomSettings settings = new RandomSettings();
            settings.DateFrom = new DateTime(1990, 1, 1);
            settings.DateTo = DateTime.Now;
            settings.Quantity = 24;
            settings.ComplexityList.Add("Любой");
            settings.ComplexityList.Add("Очень простой");
            settings.ComplexityList.Add("Простой");
            settings.ComplexityList.Add("Средний");
            settings.ComplexityList.Add("Сложный");
            settings.ComplexityList.Add("Очень сложный");
            settings.ComplexityIndex = 0;

            edtQ.Text = settings.Quantity.ToString();

            foreach (string s in settings.ComplexityList)
                edtLevel.Items.Add(s);
            edtLevel.SelectedIndex = 0;
            edtDateStart.Value = settings.DateFrom;
            edtDateEnd.Value = settings.DateTo;
        }

        private void LoadingData()
        {
            ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
            ShowProgress = true;

            ThreadPool.QueueUserWorkItem(
                (o) =>
                {
                    this.Dispatcher.BeginInvoke(InitializeQuestions);
                });
        }     

        private void InitializeQuestions()
        {
            string url = @"http://db.chgk.info/random";
            string fromDate = @"/from_" + edtDateStart.Value.Value.ToString("yyyy-MM-dd");
            string toDate = @"/to_" + edtDateEnd.Value.Value.ToString("yyyy-MM-dd");
            string types = @"/types1";
            string complexity = edtLevel.SelectedIndex > 0 ? (@"/complexity" + edtLevel.SelectedIndex) : "";
            string limit = @"/limit" + edtQ.Text;
            Random r = new Random();
            string random = @"/" + r.Next().ToString();
            url += fromDate + toDate + types + complexity + random + limit;

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(QuestionsDownloadStringCompleted);
            client.DownloadStringAsync(new Uri(url));
        }

        private void QuestionsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Ошибка подключения к базе вопросов. Проверьте соединение с интернетом.");
                NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
                return;
            }

            HtmlNode.ElementsFlags.Remove("option");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);

            var foos = from foo in doc.DocumentNode.SelectNodes("//div[@class='random_question']") select foo;
            foreach (HtmlNode random_question in foos)
            {
                string nodetext = random_question.InnerHtml;

                if (nodetext.Contains("razdatka") || nodetext.Contains("<img"))
                    continue;

                QuestionWWW q = new QuestionWWW();

                Regex r = new Regex(@"<a.*?>(.*?)</a>.*?");
                string a = r.Match(random_question.InnerHtml).Value;
                string descr = Regex.Match(a, @">(.*?)</").Value.Replace(">", "").Replace(@"</", "");
                int start = a.IndexOf("f=\"") + 3;
                int length = a.IndexOf("\">") - start;
                string url = a.Substring(start, length);
                q.description = descr;
                q.url = url;

                start = nodetext.IndexOf("</strong>") + 9;
                length = nodetext.IndexOf("<div class='collapsible collapsed'>") - start;
                string qw = nodetext.Substring(start, length);
                q.Question = Helpers.HtmlRemoval.StripTagsCharArray(qw);

                start = nodetext.IndexOf("Ответ:</strong>") + 15;
                string tmp1 = nodetext.Substring(start);
                length = tmp1.IndexOf("</p>");
                string qa = tmp1.Substring(0, length);
                q.Answer = Helpers.HtmlRemoval.StripTagsCharArray(qa);

                if (nodetext.Contains("<strong>Комментарий:</strong>"))
                {
                    start = nodetext.IndexOf("Комментарий:</strong>") + 21;
                    string tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.Comments = Helpers.HtmlRemoval.StripTagsCharArray(txt);
                }
                if (nodetext.Contains("<strong>Источник(и):</strong>"))
                {
                    start = nodetext.IndexOf("<strong>Источник(и):</strong>") + 29;
                    string tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.source = Helpers.HtmlRemoval.StripTagsCharArray(txt);
                }
                if (nodetext.Contains("<strong>Автор:</strong>"))
                {
                    start = nodetext.IndexOf("<strong>Автор:</strong>") + 24;
                    string tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.author = Helpers.HtmlRemoval.StripTagsCharArray(txt);
                }

                GameWWW.Instance.Questions.Add(q);
            }

            // stop progress bar
            ShowProgress = false;
            ContentPanel.Visibility = System.Windows.Visibility.Visible;

            // redirect to game
            NavigationService.Navigate(new Uri(@"/Games/www/WWWGameQuestion.xaml", UriKind.Relative));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LoadingData();
        }
    }
}