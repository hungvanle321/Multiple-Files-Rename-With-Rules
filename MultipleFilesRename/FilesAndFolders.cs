using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using DragEventArgs = System.Windows.DragEventArgs;
using DataFormats = System.Windows.DataFormats;
using System.Windows.Controls;
using System.Collections.Generic;

namespace MultipleFilesRename
{
    public class FileInfor : INotifyPropertyChanged
    {
        public string OldName { get; set; }

        public string NewName { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public DateTime DateModified { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class MainWindow
    {
        ObservableCollection<FileInfor> _files = new();
        ObservableCollection<FileInfor> _folders = new();
        ObservableCollection<FileInfor> _subfiles = new();
        ObservableCollection<FileInfor> _subfolders = new();

        public int RowsPerPage { get; set; } = 16;
        public int TotalPages { get; set; } = 0;
        public int CurrentPage { get; set; } = 0;

        public int TotalItems { get; set; } = 0;
        public int DisplayingItems { get; set; } = 0;

        void UpdatePaging()
        {
            TotalItems = _files.Count;
            TotalPages = (TotalItems / RowsPerPage) +
                ((TotalItems % RowsPerPage == 0) ? 0 : 1);

            _subfiles = new ObservableCollection<FileInfor>(_files
                .Skip((CurrentPage - 1) * RowsPerPage)
                .Take(RowsPerPage).ToList());
            DisplayingItems = _subfiles.Count();
            dgFiles.ItemsSource = _subfiles;

            if (_subfiles.Count == 0)
                if (CurrentPage > 0)
                {
                    CurrentPage--;
                    UpdatePaging();
                }

            if (TotalPages > 1)
                PageNumberStackPanel.Visibility = Visibility.Visible;
            else
                PageNumberStackPanel.Visibility = Visibility.Collapsed;
        }

        private void AddingFilesToDatagrid()
        {
            var i = fileComboBox.SelectedIndex;
            if (i == -1) return;
            var screen = new CommonOpenFileDialog
            {
                Multiselect = true
            };

            if (i == 0)
            {
                screen.Title = "Add files";
                if (screen.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (string fileName in screen.FileNames)
                    {
                        if (_files.Any(info => info.FilePath == fileName)) continue;
                        var info = new FileInfo(fileName);
                        _files.Add(new FileInfor() { OldName = info.Name, NewName = info.Name, FilePath = fileName, FileSize = info.Length, DateModified = info.LastWriteTime });
                    }
                }
            }
            else
            {
                screen.Title = "Add directories";
                screen.IsFolderPicker = true;
                if (screen.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (string fileName in screen.FileNames)
                    {
                        var dirInfo = new DirectoryInfo(fileName);
                        FileInfo[] files = dirInfo.GetFiles();

                        foreach (FileInfo file in files)
                        {
                            if (_files.Any(info => info.FilePath == file.FullName)) continue;
                            _files.Add(new FileInfor() { OldName = file.Name, NewName = file.Name, FilePath = file.FullName, FileSize = file.Length, DateModified = file.LastWriteTime });
                        }
                    }
                }
            }
            UpdateResult();
        }

        private void fileComboBox_DropDownClosed(object sender, EventArgs e)
        {
            AddingFilesToDatagrid();
            
        }
        private void OpenThisFile_Click(object sender, RoutedEventArgs e)
        {
            int i = dgFiles.SelectedIndex + (CurrentPage - 1) * RowsPerPage;
            string path = _files[i].FilePath;
            Process.Start("explorer.exe", @path);
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            int i = dgFiles.SelectedIndex + (CurrentPage - 1) * RowsPerPage;
            var file = new FileInfo(_files[i].FilePath);
            string path = file.DirectoryName;
            Process.Start("explorer.exe", @path);
        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            int i = dgFiles.SelectedIndex + (CurrentPage - 1) * RowsPerPage;
            _files.RemoveAt(i);
            UpdatePaging();
        }

        //Kéo thả từ File Explorer vào phần mềm
        private void dgFiles_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop, true) is string[] files)
            {
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    if (!info.Exists)
                    {
                        var dirInfo = new DirectoryInfo(file);
                        FileInfo[] fileInfos = dirInfo.GetFiles();

                        foreach (FileInfo file_ in fileInfos)
                        {
                            _files.Add(new FileInfor() { OldName = file_.Name, NewName = file_.Name, FilePath = file_.FullName, FileSize = file_.Length, DateModified = file_.LastWriteTime });
                        }
                    }
                    else
                        _files.Add(new FileInfor() { OldName = info.Name, NewName = info.Name, FilePath = info.FullName, FileSize = info.Length, DateModified = info.LastWriteTime });
                }
            }
            UpdateResult();
        }
        private void ClearAllFiles(object sender, RoutedEventArgs e)
        {
            if (_files.Count == 0) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure ?", "Delete All Files Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _files.Clear();
                _subfiles.Clear();
                CurrentPage = 1;
                PageNumberStackPanel.Visibility = Visibility.Collapsed;
                IsBatchable = false;
            }
        }

        private void NextFilePage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdatePaging();
            }
        }

        private void PrevFilePage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePaging();
            }
        }

        private void FirstFilePage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage = 1;
                UpdatePaging();
            }
        }
        private void LastFilePage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage = TotalPages;
                UpdatePaging();
            }
        }

        //Folder Tab
        public int RowsPerFolderPage { get; set; } = 16;
        public int TotalFolderPages { get; set; } = 0;
        public int CurrentFolderPage { get; set; } = 0;

        public int TotalFolderItems { get; set; } = 0;
        public int DisplayingFolderItems { get; set; } = 0;
        void UpdateFolderPaging()
        {
            TotalFolderItems = _folders.Count;
            TotalFolderPages = (TotalFolderItems / RowsPerFolderPage) +
                ((TotalFolderItems % RowsPerFolderPage == 0) ? 0 : 1);

            _subfolders = new ObservableCollection<FileInfor>(_folders
                .Skip((CurrentFolderPage - 1) * RowsPerFolderPage)
                .Take(RowsPerFolderPage).ToList());
            DisplayingFolderItems = _subfolders.Count();
            dgFolders.ItemsSource = _subfolders;

            if (_subfolders.Count == 0)
                if (CurrentFolderPage > 0)
                {
                    CurrentFolderPage--;
                    UpdateFolderPaging();
                }

            if (TotalFolderPages > 1)
                FolderPageNumberStackPanel.Visibility = Visibility.Visible;
            else
                FolderPageNumberStackPanel.Visibility = Visibility.Collapsed;
        }

        public static long DirSize(DirectoryInfo dirInf)
        {
            long size = 0;

            FileInfo[] files = dirInf.GetFiles();
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }

            DirectoryInfo[] dirs = dirInf.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                size += DirSize(dir);
            }
            return size;
        }

        private void AddFolders_FolderTab_Click(object sender, RoutedEventArgs e)
        {
            var screen = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true
            };

            if (screen.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string fileName in screen.FileNames)
                {
                    if (_folders.Any(info => info.FilePath == fileName)) continue;
                    var info = new DirectoryInfo(fileName);
                    _folders.Add(new FileInfor() { OldName = info.Name, NewName = info.Name, FilePath = fileName, FileSize = DirSize(info), DateModified = info.LastWriteTime });
                }
            }
            UpdateResult();
        }

        private void dgFolders_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop, true) is string[] files)
            {
                foreach (var file in files)
                {
                    var info = new DirectoryInfo(file);
                    if (info.Exists)
                    {
                        _folders.Add(new FileInfor() { OldName = info.Name, NewName = info.Name, FilePath = info.FullName, FileSize = DirSize(info), DateModified = info.LastWriteTime });
                    }
                }
            }
            UpdateResult();
        }


        private void OpenThisFolder_Click(object sender, RoutedEventArgs e)
        {
            int i = dgFolders.SelectedIndex + (CurrentFolderPage - 1) * RowsPerFolderPage;
            string path = _folders[i].FilePath;
            Process.Start("explorer.exe", @path);
        }

        private void OpenContainingFolder_FolderTab_Click(object sender, RoutedEventArgs e)
        {
            int i = dgFolders.SelectedIndex + (CurrentFolderPage - 1) * RowsPerFolderPage;
            var dir = new DirectoryInfo(_folders[i].FilePath);
            string path = dir.Parent.FullName;
            Process.Start("explorer.exe", @path);
        }

        private void DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            int i = dgFolders.SelectedIndex + (CurrentFolderPage - 1) * RowsPerFolderPage;
            _folders.RemoveAt(i);
            UpdateFolderPaging();
        }

        private void ClearAllFolders(object sender, RoutedEventArgs e)
        {
            if (_folders.Count == 0) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure ?", "Delete All Folders Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _folders.Clear();
                _subfolders.Clear();
                CurrentFolderPage = 1;
                FolderPageNumberStackPanel.Visibility = Visibility.Collapsed;
                IsBatchable = false;
            }
        }

        private void NextFolderPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentFolderPage < TotalFolderPages)
            {
                CurrentFolderPage++;
                UpdateFolderPaging();
            }
        }

        private void PrevFolderPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentFolderPage > 1)
            {
                CurrentFolderPage--;
                UpdateFolderPaging();
            }
        }

        private void FirstFolderPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentFolderPage > 1)
            {
                CurrentFolderPage = 1;
                UpdateFolderPaging();
            }
        }
        private void LastFolderPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentFolderPage < TotalFolderPages)
            {
                CurrentFolderPage = TotalFolderPages;
                UpdateFolderPaging();
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Height > 450)
            {
                if (fileTabControl.SelectedIndex == 0)
                {
                    int newRowsCount = 16 + ((int)Height - 450) / 15;
                    RowsPerPage = newRowsCount;
                    if (_files.Count < 16) return;
                    else UpdatePaging();
                }
                else
                {
                    int newRowsCount = 16 + ((int)Height - 450) / 15;
                    RowsPerFolderPage = newRowsCount;
                    if (_folders.Count < 16) return;
                    else UpdateFolderPaging();
                }
            }
        }

        private void fileTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileTabControl.SelectedIndex == 0)
            {
                if (_files.Count == 0) IsBatchable = false;
                else CheckIfBatchable();
            }
            else
            {
                if (_folders.Count == 0) IsBatchable = false;
                else CheckIfBatchable();
            }
        }


        List<bool> IsExistingPath()
        {
            List<bool> result = new();
            if (fileTabControl.SelectedIndex == 0)
            {
                foreach (var i in _files)
                    if (File.Exists(@i.FilePath))
                        result.Add(true);
                    else
                        result.Add(false);
            }
            else
            {
                foreach (var i in _folders)
                    if (Directory.Exists(@i.FilePath))
                        result.Add(true);
                    else
                        result.Add(false);
            }
            return result;
        }

        private void RemoveAllNonexistFiles(object sender, RoutedEventArgs e)
        {
            var _existingFiles = IsExistingPath();
            var Remaining = new List<FileInfor>();
            for (int i = 0; i < _existingFiles.Count; i++)
                if (_existingFiles[i])
                    Remaining.Add(_files[i]);
            _files.Clear();
            foreach (var file in Remaining)
                _files.Add(file);
            UpdateResult();
            CheckIfBatchable();
            e.Handled = true;
        }

        private void RemoveAllNonexistFolders(object sender, RoutedEventArgs e)
        {
            var _existingFolders = IsExistingPath();
            var Remaining = new List<FileInfor>();
            for (int i = 0; i < _existingFolders.Count; i++)
                if (_existingFolders[i])
                    Remaining.Add(_folders[i]);
            _folders.Clear();
            foreach (var folder in Remaining)
                _folders.Add(folder);
            UpdateResult();
            CheckIfBatchable();
            e.Handled = true;
        }
    }
}
