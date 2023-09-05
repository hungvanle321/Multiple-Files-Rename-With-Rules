using RuleBaseInterface;
using System.Text;

namespace PascalCaseRule
{
    public class PascalCaseRule : IRule
    {
        public string Name => "PascalCase";

        public bool IsCheck { get; set; }

        public string ToolTip => "Convert filename to PascalCase";

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
            var rule = new PascalCaseRule();
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
            var _isFirstLetter = true;

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] != ' ' && name[i] != '_')
                {
                    if (_isFirstLetter)
                    {
                        builder.Append(char.ToUpper(name[i]));
                        _isFirstLetter = false;
                    }
                    else
                    {
                        builder.Append(char.ToLower(name[i]));
                    }
                }
                else
                {
                    _isFirstLetter = true;
                }
            }
            builder.Append(extension);

            var result = builder.ToString();
            return result;
        }
    }
}