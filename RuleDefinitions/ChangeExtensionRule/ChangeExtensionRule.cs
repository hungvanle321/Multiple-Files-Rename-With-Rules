using RuleBaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChangeExtensionRule
{
    public class ChangeExtensionRule : IRule
    {
        public string NewExtension { get; set; }

        public string Name => "ChangeExtension";

        public bool IsCheck { get; set; }

        public bool IsUnique => true;

        public string ToolTip => "Change the extension to another extension";

        public ChangeExtensionRule()
        {
            NewExtension = "";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public List<string> Parameters()
        {
            return new List<string>() { "NewExtension" };
        }

        public IRule Parse(string line)
        {
            var tokens = line.Split(new string[] { " " },
                StringSplitOptions.None);
            var data = tokens[1];

            var pairs = data.Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new ChangeExtensionRule();
            rule.NewExtension = pairs[1];
            rule.IsCheck = (tokens[2] == "1");
            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " NewExtension=" + NewExtension + " " + IsCheck.GetHashCode().ToString();
        }

        public string Rename(string origin)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(origin);
            string extension = System.IO.Path.GetExtension(origin);

            var builder = new StringBuilder();
            builder.Append(name);
            builder.Append('.' + NewExtension);

            var result = builder.ToString();
            return result;
        }
    }
}