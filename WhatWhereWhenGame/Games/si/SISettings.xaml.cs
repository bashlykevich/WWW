﻿using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.si
{
    public partial class SISettings : PhoneApplicationPage
    {
        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("ShowProgress", typeof(bool), typeof(SISettings), new PropertyMetadata(false));

        public SISettings()
        {
            InitializeComponent();

            //WebClient client = new WebClient();
            //client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            //client.DownloadStringAsync(new Uri(@"http://db.chgk.info/random"));

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
    
        private void InitializeQuestions()
        {
            string url = @"http://db.chgk.info/random";
            string fromDate = @"/from_" + edtDateStart.Value.Value.ToString("yyyy-MM-dd");
            string toDate = @"/to_" + edtDateEnd.Value.Value.ToString("yyyy-MM-dd");
            string types = @"/types5";
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
            if (!doc.DocumentNode.InnerHtml.Contains("random_question"))
            {
                MessageBox.Show("Вопросов не найдено!");
                NavigationService.Navigate(new Uri(@"/MainPage.xaml", UriKind.Relative));
                return;
            }
            var foos = from foo in doc.DocumentNode.SelectNodes("//div[@class='random_question']") select foo;
            foreach (HtmlNode random_question in foos)
            {
                string nodetext = random_question.InnerHtml;

                ThemeSI q = new ThemeSI();

                Regex r = new Regex(@"<a.*?>(.*?)</a>.*?");
                string a = r.Match(random_question.InnerHtml).Value;
                string descr = Regex.Match(a, @">(.*?)</").Value.Replace(">", "").Replace(@"</", "");
                int start = a.IndexOf("f=\"") + 3;
                int length = a.IndexOf("\">") - start;
                string url = a.Substring(start, length);
                q.description = descr;
                q.url = url;

                start = nodetext.IndexOf("<strong>Вопрос") + 27;
                string tmp = nodetext.Substring(start);
                length = tmp.IndexOf("<br>") - 1;
                if (length > 0)
                {
                    string th = tmp.Substring(0, length);
                    if (th.EndsWith("."))
                        th = th.Substring(0, th.Length - 1);
                    q.name = th;
                }
                else
                    q.name = q.description;

                // parse questions
                {
                    string p = "&nbsp;&nbsp;&nbsp;&nbsp;1";
                    int s1 = nodetext.IndexOf(p);
                    if (s1 < 0)
                        continue;
                    int e1 = nodetext.IndexOf("<div class='collapsible ");
                    string txt = nodetext.Substring(s1, e1);
                    for (int i = 1; i < 5; i++)
                    {
                        string searchPattern = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        start = txt.IndexOf(searchPattern + i.ToString()) + searchPattern.Length + 2;
                        tmp = txt.Substring(start);
                        length = tmp.IndexOf("<br>");
                        string qs = tmp.Substring(0, length);
                                        
                        qs = Helpers.HtmlRemoval.StripTagsCharArray(qs);
                        
                        q.Questions.Add(qs);
                    }
                    string searchPattern5 = "&nbsp;&nbsp;&nbsp;&nbsp;5";
                    start = txt.IndexOf(searchPattern5) + searchPattern5.Length;
                    tmp = txt.Substring(start);
                    length = tmp.IndexOf("<div class='collapsible ");
                    string qs5 = tmp.Substring(0, length);
                    
                    qs5 =  Helpers.HtmlRemoval.StripTagsCharArray(qs5);                    
                    
                    q.Questions.Add(qs5);
                }

                // parse answers
                {
                    string p = "Ответ:</strong> <br>";
                    int s1 = nodetext.IndexOf(p);
                    string tmpq = nodetext.Substring(s1);
                    int e1 = tmpq.IndexOf("</p>");
                    string txt = tmpq.Substring(0, e1);
                    for (int i = 1; i < 5; i++)
                    {
                        string searchPattern = "&nbsp;&nbsp;&nbsp;&nbsp;";
                        start = txt.IndexOf(searchPattern + i.ToString()) + searchPattern.Length + 2;
                        tmp = txt.Substring(start);
                        length = tmp.IndexOf("<br>");
                        string qs = tmp.Substring(0, length);                        
                        qs = Helpers.HtmlRemoval.StripTagsCharArray(qs);
                        if (qs.EndsWith("."))
                            qs = qs.Substring(0, qs.Length - 1);
                        q.answers.Add(qs);
                    }
                    string searchPattern5 = "&nbsp;&nbsp;&nbsp;&nbsp;5";
                    start = txt.IndexOf(searchPattern5) + searchPattern5.Length;
                    string s = txt.Substring(start);
                    s = Helpers.HtmlRemoval.StripTagsCharArray(s);                    
                    if (s.EndsWith("."))
                        s = s.Substring(0, s.Length-1);
                    q.answers.Add(s);
                }

                if (nodetext.Contains("<strong>Источник(и):</strong>"))
                {
                    start = nodetext.IndexOf("<strong>Источник(и):</strong>") + 29;
                    tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.source = Helpers.HtmlRemoval.StripTagsCharArray(txt);              
                }
                if (nodetext.Contains("<strong>Автор:</strong>"))
                {
                    start = nodetext.IndexOf("<strong>Автор:</strong>") + 24;
                    tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.author = Helpers.HtmlRemoval.StripTagsCharArray(txt); ;
                }

                GameSI.Instance.Themes.Add(q);
            }

            // stop progress bar
            ShowProgress = false;
            ContentPanel.Visibility = System.Windows.Visibility.Visible;

            // redirect to game
            NavigationService.Navigate(new Uri(@"/Games/si/SIGameQuestion.xaml", UriKind.Relative));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LoadingData();
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
    }
}