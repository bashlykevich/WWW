using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using WhatWhereWhenGame.db.chgk.info;

namespace WhatWhereWhenGame.Games.si
{
    public partial class SISettings : PhoneApplicationPage
    {
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

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            HtmlNode.ElementsFlags.Remove("option");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);

            RandomSettings settings = new RandomSettings();
            HtmlNode selbox = doc.GetElementbyId("edit-complexity");

            HtmlNodeCollection nodes = selbox.ChildNodes;
            foreach (HtmlNode opt in nodes)
            {
                string item = opt.InnerText.Contains(" -- ") ? "Любой" : opt.InnerText;
                settings.ComplexityList.Add(item);
            }

            int dfd = -1;
            int dfm = -1;
            int dfy = -1;
            HtmlNodeCollection nodes1 = doc.GetElementbyId("edit-from-date-day").ChildNodes;
            foreach (HtmlNode node in nodes1)
            {
                dfd++;
                if (node.Attributes.Where(a => a.Name == "selected").Count() > 0)
                    break;
            }
            HtmlNodeCollection nodes2 = doc.GetElementbyId("edit-from-date-month").ChildNodes;
            foreach (HtmlNode node in nodes2)
            {
                dfm++;
                if (node.Attributes.Where(a => a.Name == "selected").Count() > 0)
                    break;
            }
            HtmlNodeCollection nodes3 = doc.GetElementbyId("edit-from-date-year").ChildNodes;
            foreach (HtmlNode node in nodes2)
            {
                dfy++;
                if (node.Attributes.Where(a => a.Name == "selected").Count() > 0)
                    break;
            }
            settings.DateFrom = new DateTime(1989 + dfy, dfm, dfd);

            int dtd = -1;
            int dtm = -1;
            int dty = -1;
            nodes1 = doc.GetElementbyId("edit-to-date-day").ChildNodes;
            foreach (HtmlNode node in nodes1)
            {
                dtd++;
                if (node.Attributes.Where(a => a.Name == "selected").Count() > 0)
                    break;
            }
            nodes2 = doc.GetElementbyId("edit-to-date-month").ChildNodes;
            foreach (HtmlNode node in nodes2)
            {
                dtm++;
                if (node.Attributes.Where(a => a.Name == "selected").Count() > 0)
                    break;
            }
            nodes3 = doc.GetElementbyId("edit-to-date-year").ChildNodes;
            foreach (HtmlNode node in nodes2)
            {
                dty++;
                if (node.Attributes.Where(a => a.Name == "selected").Count() > 0)
                    break;
            }
            settings.DateTo = new DateTime(1989 + dty, dtm, dtd);
            settings.Quantity = Int32.Parse(doc.GetElementbyId("edit-limit").Attributes["value"].Value);

            //settings.DateFrom = new DateTime(1990, 1, 1);
            //settings.DateTo = DateTime.Now;
            //settings.Quantity = 24;

            try
            {
                Action action = () =>
                    {
                        edtQ.Text = settings.Quantity.ToString();
                        foreach (string s in settings.ComplexityList)
                            edtLevel.Items.Add(s);
                        edtDateStart.Value = settings.DateFrom;
                        edtDateEnd.Value = settings.DateTo;
                    };
                Dispatcher.BeginInvoke(action);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InitizlizeQuestions()
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
            HtmlNode.ElementsFlags.Remove("option");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(e.Result);
            if (!doc.DocumentNode.InnerHtml.Contains("random_question"))
            {
                MessageBox.Show("Вопросов не найдено!");
                return;
            }
            var foos = from foo in doc.DocumentNode.SelectNodes("//div[@class='random_question']") select foo;
            foreach (HtmlNode random_question in foos)
            {
                string nodetext = random_question.InnerHtml;

                ThemeSI q = new ThemeSI();

                /*
                public List<string> answers;
                public List<string> questions;
                */
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
                        qs = qs.Replace("\n", "");
                        qs = qs.Replace("&mdash;", "-");
                        q.Questions.Add(qs);
                    }
                    string searchPattern5 = "&nbsp;&nbsp;&nbsp;&nbsp;5";
                    start = txt.IndexOf(searchPattern5) + searchPattern5.Length;
                    tmp = txt.Substring(start);
                    length = tmp.IndexOf("<div class='collapsible ");
                    string qs5 = tmp.Substring(0, length);
                    qs5 = qs5.Replace("\n", "");
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

                        qs = qs.Replace("\n", "");
                        qs = qs.Replace("&mdash;", "-");
                        qs = qs.Trim();
                        if (qs.StartsWith("."))
                            qs = qs.Substring(1);
                        if (qs.EndsWith("."))
                            qs = qs.Substring(0, qs.Length - 1);
                        qs = qs.Trim();

                        q.answers.Add(qs);
                    }
                    string searchPattern5 = "&nbsp;&nbsp;&nbsp;&nbsp;5";
                    start = txt.IndexOf(searchPattern5) + searchPattern5.Length;
                    string s = txt.Substring(start);
                    s = s.Replace("\n", "");
                    s = s.Replace("&mdash;", "-");
                    s = s.Trim();
                    if (s.StartsWith("."))
                        s = s.Substring(1);
                    if (s.EndsWith("."))
                        s = s.Substring(0, s.Length - 1);
                    s = s.Trim();
                    q.answers.Add(s);
                }

                /*
                start = nodetext.IndexOf("</strong>")+9;
                length = nodetext.IndexOf("<div class='collapsible collapsed'>") - start;
                string qw = nodetext.Substring(start, length);
                q.Question = qw;

                start = nodetext.IndexOf("Ответ:</strong>") + 15;
                string tmp1 = nodetext.Substring(start);
                length = tmp1.IndexOf("</p>");
                string qa = tmp1.Substring(0, length);
                q.Answer = qa;*/

                if (nodetext.Contains("<strong>Источник(и):</strong>"))
                {
                    start = nodetext.IndexOf("<strong>Источник(и):</strong>") + 29;
                    tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.source = txt;
                }
                if (nodetext.Contains("<strong>Автор:</strong>"))
                {
                    start = nodetext.IndexOf("<strong>Автор:</strong>") + 24;
                    tmp = nodetext.Substring(start);
                    length = tmp.IndexOf("</p>");
                    string txt = tmp.Substring(0, length);
                    q.author = txt;
                }

                GameSI.Instance.Themes.Add(q);
            }

            // redirect to game
            NavigationService.Navigate(new Uri(@"/Games/si/SIGameQuestion.xaml", UriKind.Relative));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            InitizlizeQuestions();
        }
    }
}