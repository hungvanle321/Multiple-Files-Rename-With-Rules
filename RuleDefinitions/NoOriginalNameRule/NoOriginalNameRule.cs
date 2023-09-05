using RuleBaseInterface;
using System.Text;

namespace NoOriginalNameRule
{
    public class NoOriginalNameRule : IRule
    {
        public string Name => "NoOriginalName";

        public string ToolTip => "Remove the original name";

        public bool IsUnique => true;

        public bool IsCheck { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public List<string> Parameters()
        {
            return new List<string>();
        }

        public IRule Parse(string data)
        {
            var tokens = data.Split(new string[] { " " },
                StringSplitOptions.None);

            var rule = new NoOriginalNameRule();
            rule.IsCheck = (tokens[1] == "1");

            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " " + IsCheck.GetHashCode().ToString();
        }

        public string Rename(string origin)
        {
            var builder = new StringBuilder();

            var result = builder.ToString();
            return result;
        }
    }
}