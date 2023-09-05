using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using RuleBaseInterface;
using System.Linq;

namespace MultipleFilesRename
{
    public class Preset : ICloneable
    {
        public string Name { get; set; }
        public string[] Description { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public partial class MainWindow
    {
        ObservableCollection<Preset> _preset = new ObservableCollection<Preset>();

        private void LoadDefaultPresets()
        {
            _preset.Add(new Preset()
            {
                Name = "Change extension to .jpg, remove original name and add counter with 2 paddings",
                Description = new string[3] {
                "ChangeExtension NewExtension=jpg 1", "AddCounter Start=1,Step=1,Digits=2 1", "NoOriginalName 1"
                }
            });
        }

        private void SavePresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rules.Count == 0) return;
            var saveFiles = new CommonSaveFileDialog();
            saveFiles.DefaultFileName = "Preset";
            saveFiles.DefaultExtension = ".txt";
            saveFiles.Filters.Add(new CommonFileDialogFilter("Text documents (.txt)", "*.txt"));
            saveFiles.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFiles.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string Result = "";
                for (int i = 0; i < _rules.Count - 1; i++)
                    Result += _rules[i].RuleTypes.Preset(_rules[i].IsCheck) + "\n";
                Result += _rules[_rules.Count - 1].RuleTypes.Preset(_rules[_rules.Count - 1].IsCheck);
                File.WriteAllText(saveFiles.FileName, Result);
            }
        }

        private void LoadPresetButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new CommonOpenFileDialog();
            screen.Filters.Add(new CommonFileDialogFilter("Text documents (.txt)", "*.txt"));


            if (screen.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (_rules.Count > 0)
                    _rules.Clear();
                string presetPath = screen.FileName;
                var info = new FileInfo(screen.FileName);
                string fileName = Path.GetFileNameWithoutExtension(info.Name);

                var lines = File.ReadAllLines(presetPath);
                _preset.Add(new Preset() { Name = fileName, Description = lines });
                presetComboBox.SelectedIndex = _preset.Count - 1;
                var factory = new RuleFactory();

                foreach (var line in lines)
                {
                    IRule rule = factory.Parse(line);
                    if (rule.IsUnique && _rules.Any(rul => rul.RuleName == rule.Name))
                    {
                        continue;
                    }

                    var newRule = new RuleView() { RuleName = rule.Name, RuleTypes = rule, IsCheck = true, ToolTip = rule.ToolTip };
                    _rules.Add(newRule);
                    CheckIfBatchable();
                    UpdateResult();
                }
            }
        }

        private void PresetComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (_preset.Count == 0 || presetComboBox.SelectedIndex == -1) return;
            if (_rules.Count > 0)
                _rules.Clear();
            var i = (presetComboBox.SelectedItem as Preset).Clone() as Preset;

            var factory = new RuleFactory();

            foreach (var line in i.Description)
            {
                IRule rule = factory.Parse(line);
                if (rule.IsUnique && _rules.Any(rul => rul.RuleName == rule.Name))
                {
                    continue;
                }

                var newRule = new RuleView() { RuleName = rule.Name, RuleTypes = rule, IsCheck = true, ToolTip = rule.ToolTip };
                _rules.Add(newRule);
                CheckIfBatchable();
                UpdateResult();
            }
        }

        private void ClearPresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_preset.Count == 0) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure ?", "Remove All Presets Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                presetComboBox.SelectedIndex = -1;
                _preset.Clear();
            }
        }
    }
}
