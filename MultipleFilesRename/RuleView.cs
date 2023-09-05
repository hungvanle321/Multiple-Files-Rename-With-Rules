using System.ComponentModel;
using RuleBaseInterface;

namespace MultipleFilesRename
{
    public class RuleView : INotifyPropertyChanged
    {
        public string RuleName { get; set; }
        public IRule RuleTypes { get; set; }
        public bool IsCheck { get; set; }

        public string ToolTip { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
