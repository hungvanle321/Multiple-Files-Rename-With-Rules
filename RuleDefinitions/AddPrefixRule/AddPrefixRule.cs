using RuleBaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddPrefixRule
{
    public class AddPrefixRule : IRule
    {
        public string Prefix { get; set; }

        public string Name => "AddPrefix";

        public string ToolTip => "Adding a prefix to all the file names";

        public bool IsUnique => false;

        public bool IsCheck { get; set; }

        public AddPrefixRule()
        {
            Prefix = "";
        }

        public string Rename(string origin)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(origin);
            string extension = System.IO.Path.GetExtension(origin);

            var builder = new StringBuilder();
            builder.Append(Prefix);
            builder.Append(name);
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

            var rule = new AddPrefixRule();
            rule.Prefix = pairs[1];
            rule.IsCheck = (tokens[2] == "1");
            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " Prefix=" + Prefix + " " + IsCheck.GetHashCode().ToString();
        }

        List<string> IRule.Parameters()
        {
            return new List<string>() { "Prefix" };
        }
    }
}