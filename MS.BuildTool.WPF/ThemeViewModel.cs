
namespace MS.BuildTool.WPF
{
    public class ThemeViewModel
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
