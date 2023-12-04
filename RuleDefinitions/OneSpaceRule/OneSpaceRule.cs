using RuleBaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneSpaceRule
{
    public class OneSpaceRule : IRule
    {
        public string Name => "OneSpace";
        public string DisplayName => "One Space Only";
        public bool IsCheck { get; set; }

        public string ToolTip => "Replace multiple spaces with a single space";

        public bool IsUnique => true;

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
            IsCheck = (tokens[1] == "1");
            var rule = new OneSpaceRule();
            rule.IsCheck = (tokens[1] == "1");

            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " " + IsCheck.GetHashCode().ToString();
        }

        public string Rename(string origin)
        {
            string name = Path.GetFileNameWithoutExtension(origin);
            string extension = Path.GetExtension(origin);

            var builder = new StringBuilder();
            builder.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                if (name[i] == ' ')
                {
                    if (name[i - 1] != ' ')
                    {
                        builder.Append(name[i]);
                    }
                }
                else
                {
                    builder.Append(name[i]);
                }
            }
            builder.Append(extension);

            var result = builder.ToString();
            return result;
        }
    }
}