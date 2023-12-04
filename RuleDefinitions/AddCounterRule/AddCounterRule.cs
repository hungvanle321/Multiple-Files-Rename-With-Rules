using RuleBaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddCounterRule
{
    public class AddCounterRule : IRule
    {
        private int _current = 0;
        private int _start = 0;

        public int Start
        {
            get => _start; set
            {
                _start = value;

                _current = value;
            }
        }
        public int Step { get; set; }

        public int Digits { get; set; }

        public int CurrentStart { get; set; }

        public bool IsCheck { get; set; }

        public string Name => "AddCounter";

        public string DisplayName => "Add Counter";

        public string ToolTip => "Add counter to the end of the file name";

        public bool IsUnique => false;

        public AddCounterRule()
        {
            Start = CurrentStart = 1;
            Step = 1;
        }
        public string Rename(string origin)
        {
            string name = Path.GetFileNameWithoutExtension(origin);
            string extension = Path.GetExtension(origin);

            var builder = new StringBuilder();
            builder.Append(name);

            var _padding = Digits - _current.ToString().Length;
            for (int i = 0; i < _padding; i++)
            {
                builder.Append('0');
            }

            builder.Append(_current);
            builder.Append(extension);

            _current += Step;

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
            var attributes = data.Split(new string[] { "," },
                StringSplitOptions.None);
            var pairs0 = attributes[0].Split(new string[] { "=" },
                StringSplitOptions.None);
            var pairs1 = attributes[1].Split(new string[] { "=" },
                StringSplitOptions.None);
            var pairs2 = attributes[2].Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new AddCounterRule();
            if (pairs0[1] == "")
            {
                rule.Start = rule.Step = rule.CurrentStart = 1;
            }
            else if (pairs1[1] == "")
            {
                rule.CurrentStart = rule.Start = int.Parse(pairs0[1]);
                rule.Step = 1;
            }
            else if (pairs2[1] == "")
            {
                rule.CurrentStart = rule.Start = int.Parse(pairs0[1]);
                rule.Step = int.Parse(pairs1[1]);
                rule.Digits = 0;
            }
            else
            {
                rule.CurrentStart = rule.Start = int.Parse(pairs0[1]);
                rule.Step = int.Parse(pairs1[1]);
                rule.Digits = int.Parse(pairs2[1]);
            }

            rule.IsCheck = (tokens[2] == "1");
            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " Start=" + Start.ToString() + ",Step=" + Step.ToString() + ",Digits=" + Digits.ToString() + " " + IsCheck.GetHashCode().ToString();
        }

        List<string> IRule.Parameters()
        {
            return new List<string>() { "Start", "Step", "Digits" };
        }
    }
}