using RuleBaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoSpaceBeginEndRule
{
    public class NoSpaceBeginEndRule : IRule
    {
        public string Name => "RemoveSpacesOfBeginEnd";
        public string DisplayName => "Remove Spaces Of Begin & End";
        public bool IsCheck { get; set; }

        public string ToolTip => "Remove all space from the beginning and the ending of the filename";

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
            var rule = new NoSpaceBeginEndRule();
            rule.IsCheck = (tokens[1] == "1");

            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " " + IsCheck.GetHashCode().ToString();
        }

        public string Rename(string origin)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(origin);
            string extension = System.IO.Path.GetExtension(origin);

            var builder = new StringBuilder();
            var _begin = 0;
            var _end = name.Length;
            
            while (name[_begin] == ' ')
            {
                _begin++;
            }

            while (name[_end - 1] == ' ')
            {
                _end--;
            }

            for (int i = _begin; i < _end; i++)
            {
                builder.Append(name[i]);
            }
            builder.Append(extension);

            var result = builder.ToString();
            return result;
        }
    }
}