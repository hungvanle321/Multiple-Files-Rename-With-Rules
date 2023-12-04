using RuleBaseInterface;
using System.Text;

namespace ReplaceRule
{
    public class ReplaceRule : IRule
    {
        public string Current { get; set; }

        public string Replace { get; set; }

        public string Name => "Replace";
        public string DisplayName => "Replace characters";

        public string ToolTip => "Replace certain characters into one character";

        public bool IsUnique => false;

        public bool IsCheck { get; set; }

        public ReplaceRule()
        {
            Current = "";
            Replace = "";
        }

        public string Rename(string origin)
        {
            string name = Path.GetFileNameWithoutExtension(origin);
            string extension = Path.GetExtension(origin);

            var builder = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (Current.Contains(name[i]))
                {
                    builder.Append(Replace);
                }
                else
                {
                    builder.Append(name[i]);
                }
            }
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
            var param = data.Split(new string[] { "," }, StringSplitOptions.None);
            var param1 = param[0].Split(new string[] { "=" }, StringSplitOptions.None);
            var param2 = param[1].Split(new string[] { "=" }, StringSplitOptions.None);

            var rule = new ReplaceRule();
            rule.Current = param1[1];
            rule.Replace = param2[1];
            rule.IsCheck = (tokens[2] == "1");
            return rule;
        }

        public string Preset(bool IsCheck)
        {
            return Name + " Current=" + Current + " Replace=" + Replace + " " + IsCheck.GetHashCode().ToString();
        }

        List<string> IRule.Parameters()
        {
            return new List<string>() { "Current", "Replace" };
        }
    }
}