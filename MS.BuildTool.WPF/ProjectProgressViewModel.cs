using System;
using System.Collections.Generic;

namespace MS.BuildTool.WPF
{
    public class ProjectProgressViewModel : BaseViewModel
    {
        private string _name;
        private string _status;
        private string _projectPath;
        private string _solutionPath;
        private TimeSpan _buildTime;
        private int _warningCount;

        public ProjectProgressViewModel()
        {
            Warnings = new List<string>();
        }
        
        public int BuildOrder { get; set; }

        public List<string> Warnings { get; set; }

        public DateTime StartBuildTime { get; } = DateTime.Now;

        public string Name
        {
            get 
            { 
                return _name;
            }
            set 
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string ProjectPath
        {
            get
            {
                return _projectPath;
            }
            set
            {
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
            }
        }

        public string SolutionPath
        {
            get
            {
                return _solutionPath;
            }
            set
            {
                _solutionPath = value;
                OnPropertyChanged(nameof(SolutionPath));
            }
        }

        public TimeSpan BuildTime
        {
            get
            {
                return _buildTime;
            }
            set
            {
                _buildTime = value;
                OnPropertyChanged(nameof(BuildTime));
            }
        }

        public int WarningCount
        {
            get
            {
                return _warningCount;
            }
            set
            {
                _warningCount = value;
                OnPropertyChanged(nameof(WarningCount));
            }
        }
    }
}
