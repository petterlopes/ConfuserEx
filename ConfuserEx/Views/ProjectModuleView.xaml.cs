﻿using ConfuserEx.ViewModel;
using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace ConfuserEx.Views
{
    public partial class ProjectModuleView : Window
    {
        private readonly ProjectModuleVM module;

        public ProjectModuleView(ProjectModuleVM module)
        {
            InitializeComponent();
            this.module = module;
            DataContext = module;
            PwdBox.IsEnabled = !string.IsNullOrEmpty(PathBox.Text);
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PwdBox.IsEnabled = !string.IsNullOrEmpty(PathBox.Text);
        }

        private void ChooseSNKey(object sender, RoutedEventArgs e)
        {
            var ofd = new VistaOpenFileDialog();
            ofd.Filter = "Supported Key Files (*.snk, *.pfx)|*.snk;*.pfx|All Files (*.*)|*.*";
            if (ofd.ShowDialog() ?? false)
            {
                module.SNKeyPath = ofd.FileName;
            }
        }
    }
}