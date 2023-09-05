using RuleBaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddSuffixRule
{
    public class AddSuffixRule : IRule
    {
        public string Suffix { get; set; }

        public string Name => "AddSuffix";

        public string ToolTip => "Adding a suffix to all the file names";

        public bool IsUnique => false;

        public bool IsCheck { get; set; }

        public AddSuffixRule()
        {
            Suffix = "";
        }

        public string Rename(string origin)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(origin);
            string extension = System.IO.Path.GetExtension(origin);

            var builder = new StringBuilder();
            builder.Append(name);
            builder.Append(Suffix);
            builder.Append(extension);

            string result = builder.ToString();
            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IRule Parse(string line)
        {
            var tokens = line.Split(new string[] { " " },
                StringSplitOptions.None);
            var data = tokens[1];

            var pairs = data.Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new AddSuffixRule();
            rule.Suffix = pairs[1];
            rule.IsCheck = (tokens[2] == "1");
            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " Suffix=" + Suffix + " " + IsCheck.GetHashCode().ToString();
        }

        List<string> IRule.Parameters()
        {
            return new List<string>() { "Suffix" };
        }
    }
}