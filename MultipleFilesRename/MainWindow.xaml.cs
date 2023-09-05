using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using RuleBaseInterface;
using System.IO;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
#pragma warning disable

namespace MultipleFilesRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IDropTarget
    {
        public bool IsBatchable { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SetButtonCursors(DependencyObject element)
        {
            if (element is Button button)
            {
                button.Cursor = CustomCursor1;
            }
            else
            {
                int childCount = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < childCount; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(element, i);
                    SetButtonCursors(child);
                }
            }
        }

        ObservableCollection<IRule> _activeRules = new ObservableCollection<IRule>();
        ObservableCollection<RuleView> _rules = new ObservableCollection<RuleView>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDllFile();
            LoadDefaultPresets();

            Cursor = CustomCursor2;
            SetButtonCursors(this);

            presetComboBox.ItemsSource = _preset;
            RulesComboBox.ItemsSource = _activeRules;
            nameListBox.ItemsSource = _rules;

            CurrentPage = 1;
            PageNumberStackPanel.Visibility = Visibility.Collapsed;

            CurrentFolderPage = 1;
            FolderPageNumberStackPanel.Visibility = Visibility.Collapsed;
            IsBatchable = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateResult();
        }

        private void UpdateResult()
        {
            CheckIfBatchable();
            if (fileTabControl.SelectedIndex == 0)
            {
                UpdateTabControl(_files, true);
                UpdatePaging();
            }
            else
            {
                UpdateTabControl(_folders, false);
                UpdateFolderPaging();
            }
        }

        void UpdateTabControl(ObservableCollection<FileInfor> fileInfors, bool fileType)
        {
            List<RuleView> AddCounter_RulePos = new List<RuleView>();
            if (fileInfors.Count == 0) return;
            foreach (var i in fileInfors)
            {
                string newName;
                if (_rules.Any(rule => rule.RuleName == "NoOriginalName"))
                    newName = Path.GetExtension(i.OldName);
                else
                    newName = i.OldName;
                foreach (var rule in _rules)
                {
                    if (rule.RuleName == "NoOriginalName")
                        continue;
                    if (rule.RuleName == "AddCounter")
                    {
                        AddCounter_RulePos.Add(rule);
                    }

                    if (!fileType && rule.RuleName == "ChangeExtension")
                        continue;

                    if (rule.IsCheck)
                        newName = rule.RuleTypes.Rename(newName);
                }
                i.NewName = newName;
            }

            //Set lại Start về giá trị ban đầu (lưu trong biến CurrentStart)
            if (AddCounter_RulePos.Count > 0)
            {
                for (int i = 0; i < AddCounter_RulePos.Count; i++)
                {
                    var j = AddCounter_RulePos[i].RuleTypes;
                    var CurrentStartInfo = j.GetType().GetProperty("CurrentStart");
                    var StartInfo = j.GetType().GetProperty("Start");
                    StartInfo?.SetValue(j, CurrentStartInfo?.GetValue(j));
                }
            }
        }

        private void RemoveRule_Click(object sender, RoutedEventArgs e)
        {
            RuleView local = ((sender as Button).Tag as RuleView);
            _rules.Remove(local);
            UpdateResult();
        }

        private void RulesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (RulesComboBox.SelectedIndex == -1) return;
            var i = (RulesComboBox.SelectedItem as IRule).Clone() as IRule;
            if (i.IsUnique && _rules.Any(rule => rule.RuleName == i.Name))
            {
                return;
            }

            var newRule = new RuleView() { RuleName = i.Name, RuleTypes = i, IsCheck = true, ToolTip = i.ToolTip };
            _rules.Add(newRule);
            CheckIfBatchable();
            UpdateResult();
            RulesComboBox.SelectedIndex = -1;
        }

        [Obsolete]
        private void AddRuleParam_Click(object sender, RoutedEventArgs e)
        {
            var addButton = sender as Button;
            RuleView local = ((sender as Button).Tag as RuleView);
            IRule ruleType = local.RuleTypes;
            StackPanel ruleStackPanel = (((sender as Button).Parent as StackPanel).Parent as Border).Parent as StackPanel;

            if (ruleStackPanel.Children.Count == 1)
            {
                var c = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(0, 5, 0, 5) };
                var Parameters = ruleType.Parameters();
                if (Parameters.Count == 0) return;

                var longestParamLength = Parameters.OrderByDescending(s => s.Length).First().Length * 8;
                
                for (int i = 0; i < Parameters.Count; i++)
                {
                    var stackPan = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 12, 0, 12)
                    };

                    var title = new TextBlock()
                    {
                        Text = Regex.Replace(Parameters[i], "(\\B[A-Z])", " $1") + ": ",
                        Width = longestParamLength
                    };
                    var valueTextBox = new TextBox() { Width = 100 };
                    var propBinding = new Binding();
                    propBinding.Source = ruleType;
                    propBinding.Mode = BindingMode.TwoWay;
                    var propertyInfo = ruleType.GetType().GetProperty(Parameters[i]);
                    propBinding.Path = new PropertyPath("(0)", propertyInfo);

                    valueTextBox.SetValue(Validation.ErrorTemplateProperty, FindResource("validationTemplate"));
                    valueTextBox.Style = (Style)FindResource("textBoxInError");

                    propBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        FileNameRule valid = new FileNameRule();
                        valid.longestFileNameLength = FindLongestFileNameLength();
                        propBinding.ValidationRules.Add(valid);
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        PositiveNumberRule valid = new PositiveNumberRule();
                        valid.longestFileNameLength = FindLongestFileNameLength();
                        propBinding.ValidationRules.Add(valid);
                    }

                    valueTextBox.SetBinding(TextBox.TextProperty, propBinding);

                    stackPan.Children.Add(title);
                    stackPan.Children.Add(valueTextBox);
                    valueTextBox.TextChanged += RuleParameterValue_TextChanged;

                    c.Children.Add(stackPan);
                }
                c.Visibility = Visibility.Collapsed;
                ruleStackPanel.Children.Add(c);
            }

            if (ruleStackPanel.Children[1].Visibility == Visibility.Collapsed)
            {
                ((Image)addButton.FindName("addImage")).Source = new BitmapImage(new Uri("Images/MinusMark.png", UriKind.Relative));
                ruleStackPanel.Children[1].Visibility = Visibility.Visible;
            }
            else
            {
                ((Image)addButton.FindName("addImage")).Source = new BitmapImage(new Uri("Images/PlusMark.png", UriKind.Relative));
                ruleStackPanel.Children[1].Visibility = Visibility.Collapsed;
            }
        }

        private void RuleParameterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            StackPanel local = ((sender as TextBox).Parent as StackPanel).Parent as StackPanel;
            StackPanel stackPan = local.Parent as StackPanel;
            var button = (Button)stackPan.FindName("AddRuleParam");
            RuleView rule = button.Tag as RuleView;

            List<string> param = new List<string>();
            foreach (var stackPanel in local.Children)
            {
                var stPanel = stackPanel as StackPanel;
                var valueTextBox = stPanel.Children[1] as TextBox;
                if (Validation.GetHasError(valueTextBox))
                {
                    IsBatchable = false;
                    return;
                }
                param.Add(valueTextBox.Text);
            }

            IsBatchable = true;
            //Do text của TextBox chỉ Binding với mỗi Start nên phải thay đổi CurrentStart
            if (rule.RuleName == "AddCounter")
            {
                var j = rule.RuleTypes;
                var CurrentStartInfo = j.GetType().GetProperty("CurrentStart");
                var StartInfo = j.GetType().GetProperty("Start");
                CurrentStartInfo.SetValue(j, StartInfo.GetValue(j));
            }

            UpdateResult();
        }

        private void RemoveAllRules_Click(object sender, RoutedEventArgs e)
        {
            if (_rules.Count == 0) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure ?", "Delete All Rules Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _rules.Clear();
                UpdateResult();
            }
        }

        public bool IsFileNameValid(string fileName)
        {
            int maxLength = 255; // Maximum allowed length for file name
            string pattern = @"^[a-zA-Z0-9](?:[a-zA-Z0-9 ._-]*[a-zA-Z0-9])?\.[a-zA-Z0-9_-]+$";

            return fileName.Length <= maxLength && Regex.IsMatch(fileName, pattern);
        }

        public bool IsFolderNameValid(string folderName)
        {
            string pattern = @"^[^\/:*?""<>|]+$";
            return Regex.IsMatch(folderName, pattern);
        }

        private bool CheckValidFiles(bool fileType)
        {
            var Existing = IsExistingPath();
            string message = fileType ? "files" : "folders";
            if (Existing.Any(isExist => isExist == false))
            {
                MessageBox.Show("Batch Failed! Please remove all non-exist "+ message +" before batching.", "Batch Failed Message", MessageBoxButton.OK);
                return false;
            }

            if ((fileType && _files.Any(file => !IsFileNameValid(file.NewName)))
                || (!fileType && _folders.Any(folder => !IsFolderNameValid(folder.NewName)))) {
                MessageBox.Show("Batch Failed! Invalid new " + message + "' names", "Batch Failed Message", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private void StartBatch_Click(object sender, RoutedEventArgs e)
        {
            if (_rules.Count == 0 || !IsBatchable) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure ?", "Batch All File Names Confirmation", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (fileTabControl.SelectedIndex == 0)
                {
                    if (_files.Count == 0 || !CheckValidFiles(true)) return;

                    var tempDirectory = Directory.GetCurrentDirectory() + "\\" + "Temp";
                    Directory.CreateDirectory(tempDirectory);
                    int count = 0;

                    //Move vào 1 thư mục rỗng trước rồi mới move từ thư mục đó ra path ban đầu (tránh trùng tên giữa các file ở path gốc)
                    foreach (var i in _files)
                    {
                        var info = new FileInfo(i.FilePath);
                        string tempPath = tempDirectory + "\\" + count.ToString() + info.Extension;
                        File.Move(i.FilePath, tempPath);
                        count++;
                    }

                    count = 0;
                    foreach (var i in _files)
                    {
                        var info = new FileInfo(i.FilePath);
                        string tempPath = tempDirectory + "\\" + count.ToString() + info.Extension;
                        string newPath = info.DirectoryName + "\\" + i.NewName;

                        File.Move(tempPath, newPath);
                        i.FilePath = newPath;
                        i.OldName = i.NewName;
                        count++;
                    }

                    Directory.Delete(tempDirectory, true);
                }
                else
                {
                    if (_folders.Count == 0 || !CheckValidFiles(false)) return;
                    var tempDirectory = Directory.GetCurrentDirectory() + "\\" + "Temp";
                    Directory.CreateDirectory(tempDirectory);
                    int count = 0;

                    //Move vào 1 thư mục rỗng trước rồi mới move từ thư mục đó ra path ban đầu (tránh trùng tên giữa các file ở path gốc)
                    foreach (var i in _folders)
                    {
                        string tempPath = tempDirectory + "\\" + count.ToString();
                        CopyDirectory(i.FilePath, tempPath);
                        Directory.Delete(i.FilePath, true);
                        count++;
                    }

                    count = 0;
                    foreach (var i in _folders)
                    {
                        var info = new DirectoryInfo(i.FilePath);
                        string tempPath = tempDirectory + "\\" + count.ToString();
                        string newPath = info.Parent + "\\" + i.NewName;

                        CopyDirectory(tempPath, newPath);
                        Directory.Delete(tempPath, true);
                        i.FilePath = newPath;
                        i.OldName = i.NewName;
                        count++;
                    }

                    Directory.Delete(tempDirectory, true);
                }
                MessageBox.Show("Batch Successfully", "Batch Successfully Message", MessageBoxButton.OK);
                UpdateResult();
            }
            
        }

        private void CopyDirectory(string oldPath, string newPath)
        {
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            foreach (string filePath in Directory.GetFiles(oldPath))
            {
                string fileName = Path.GetFileName(filePath);
                string destPath = Path.Combine(newPath, fileName);
                File.Move(filePath, destPath);
            }

            foreach (string subDirectoryPath in Directory.GetDirectories(oldPath))
            {
                string subDirectoryName = Path.GetFileName(subDirectoryPath);
                string destPath = Path.Combine(newPath, subDirectoryName);
                CopyDirectory(subDirectoryPath, destPath);
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rules.Count == 0) return;
            foreach (var i in _rules)
                i.IsCheck = true;
            UpdateResult();
        }

        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rules.Count == 0) return;
            foreach (var i in _rules)
                i.IsCheck = false;
            UpdateResult();
        }

        private void CheckIfBatchable()
        {
            IsBatchable = true;
            if (_rules.Count == 0) IsBatchable = false;
            if (fileTabControl.SelectedIndex == 0)
            {
                if (_files.Count == 0) IsBatchable = false;
            }
            else
            {
                if (_folders.Count == 0) IsBatchable = false;
            }
        }
        private int FindLongestFileNameLength()
        {
            int longestFileNameLength = 0;
            if (fileTabControl.SelectedIndex == 0)
            {
                foreach (var i in _files)
                    if (i.NewName.Length > longestFileNameLength)
                        longestFileNameLength = i.NewName.Length;
            }
            else
            {
                foreach (var i in _folders)
                    if (i.NewName.Length > longestFileNameLength)
                        longestFileNameLength = i.NewName.Length;
            }
            return longestFileNameLength;
        }
    }

}
