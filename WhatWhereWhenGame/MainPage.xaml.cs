using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace WhatWhereWhenGame
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnGameSelectWWW_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/Games/www/WWWSettings.xaml", UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/Games/br/BRSettings.xaml", UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"/Games/si/SISettings.xaml", UriKind.Relative));
        }
    }
}