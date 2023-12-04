using RuleBaseInterface;
using System.Text;

namespace Lowercase_NoSpace_Rule
{
    public class Lowercase_NoSpace_Rule : IRule
    {
        public string Name => "Lowercase_NoSpaces";
        public string DisplayName => "Lowercase & No Spaces";
        public string ToolTip => "Convert all characters to lowercase, remove all spaces";

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
            IsCheck = (tokens[1] == "1");
            var rule = new Lowercase_NoSpace_Rule();
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
            //builder.Append(name[0]);

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] != ' ')
                {
                    builder.Append(char.ToLower(name[i]));
                }
            }
            builder.Append(extension);

            var result = builder.ToString();
            return result;
        }
    }
}