namespace RuleBaseInterface
{
    public interface IRule : ICloneable
    {
        //Đồi tên từ tên file gốc
        string Rename(string origin);

        string ToolTip { get; }

        bool IsUnique { get; }

        string Name { get; }

        string DisplayName { get; }

        //Để khi load preset sẽ biết rule đó có được check hay không
        bool IsCheck { get; set; }

        //Chuyển đổi 1 dòng của preset thành rule với tham số tương ứng
        IRule Parse(string data);

        //List chứa tên các tham số
        public List<string> Parameters();

        //Viết vào Preset (TêncủaRule Param1=value1,Param2=value2 1/0 (tượng trưng cho rule đó đang được check hay không)
        string Preset(bool IsCheck);
    }
}