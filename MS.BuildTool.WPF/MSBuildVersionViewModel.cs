
namespace MS.BuildTool.WPF
{
    public class MSBuildVersionViewModel
    {
        public string Path { get; set; }
        public string VisualStudioVersion { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
