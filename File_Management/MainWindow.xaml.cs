using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

static class Constants
{
    public const string REGEX_WILDCARD = ".*";
    public const string BASE_DIR = "C:\\Users";
    public const string PROJ_DIR = ".";
    public const string WILDCARD = "*";
}

namespace Windows_Grep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Initialise statistics
            int wordsSearched = 0;
            int charsSearched = 0;

            // Get user input
            string searchString = string.IsNullOrEmpty(SearchBox.Text) ? Constants.REGEX_WILDCARD : SearchBox.Text;
            string dir = string.IsNullOrEmpty(DirBox.Text) ? Constants.PROJ_DIR : DirBox.Text;
            string searchPattern = string.IsNullOrEmpty(ExtBox.Text) ? Constants.WILDCARD : $"*.{ExtBox.Text}";

            // Search files
            string[] files = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories);
            List<string> matchedFiles = new List<string>();
            ProgressBar.BeginAnimation(ProgressBar.ValueProperty, null);
            int filesSearched = 0;

            foreach (string file in files)
            {
                string content = File.ReadAllText(file);

                // Adds file to matched files list if it contains the search string
                if (Regex.IsMatch(content, searchString))
                {
                    matchedFiles.Add(file);
                }

                // Update statistics
                wordsSearched += content.Split(' ').Length;
                charsSearched += content.Length;
                filesSearched++;
                ProgressBar.Value = filesSearched / files.Length * 100;
            }

            StatsTextBox.Text = $"{files.Length} files, {wordsSearched} words, {charsSearched} characters searched";
            MatchedTextBox.Text = $"({matchedFiles.Count} matches)";
            MatchedFilesListBox.ItemsSource = matchedFiles;
        }

        private void DirButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Constants.BASE_DIR;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DirBox.Text = dialog.FileName;
            }
        }

        private void MatchedFilesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? file = MatchedFilesListBox.SelectedItem.ToString();
            Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
        }
    }
}
