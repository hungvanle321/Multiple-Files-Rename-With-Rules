namespace RuleBaseInterface
{
    public interface IRule : ICloneable
    {
        string Rename(string origin);

        string ToolTip { get; }

        bool IsUnique { get; }

        string Name { get; }
        string DisplayName { get; }

        bool IsCheck { get; set; }

        IRule Parse(string data);

        //List chứa tên các tham số
        public List<string> Parameters();

        string Preset(bool IsCheck);
    }
}