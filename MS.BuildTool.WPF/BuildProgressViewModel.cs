using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MS.BuildTool.WPF
{
    public class BuildProgressViewModel : BaseViewModel
    {
        private const string MSBuildArguments = "\"{0}\" /t:{1} {2} /p:Configuration=Debug /p:Platform=\"Any CPU\" /p:SolutionRoot=. /m:4";
        private const string BuildFile = @"C:\Source\Trunk\BuildAllAndTest.targets";

        private bool _buildButtonEnabled;
        private bool _buildCompleted;
        private string _projectPath;
        private TimeSpan _totalBuildTime;
        private string _msBuildPath;
        private string _msBuildArgs;
        private bool _optionsVisible;
        private string _buildStatus;
        private string _buildLogText;
        private List<string> _projectTypes;
        private bool _codeAnalysisEnabled;
        private bool _installerEnabled;
        private Process _msBuidProcess;
        private string _buildButtonText;
        private bool _cancel;
        private bool _buildSucceeded;
        private bool _logVisible;
        private ThemeViewModel _selectedTheme;

        public BuildProgressViewModel()
        {
            BuildCommand = new DelegateCommand(BuildExecute);
            ViewLogCommand = new DelegateCommand(ViewLogExecute);
            ToggleOptionsCommand = new DelegateCommand(ToggleOptionsExecute);
            BuildArgumentsChanged = new DelegateCommand(BuildArgumentsChangedExecute);
            SaveLogCommand = new DelegateCommand(SaveLogExecute);
            Projects = new ObservableCollection<ProjectProgressViewModel>();

            BuildButtonEnabled = true;

            SetupMSBuildPath();
            SetupThemes();

            BuildButtonText = "Build";
            BuildArgumentsChangedExecute();
            _projectTypes = new List<string> { ".csproj", ".vbproj", ".sqlproj", ".wixproj" };
        }

        private void SetupThemes()
        {
            Themes = new ObservableCollection<ThemeViewModel>();
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/ExpressionDark.xaml", Name = "Expression Dark" });
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/ExpressionLight.xaml", Name = "Expression Light" });
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/BureauBlack.xaml", Name = "Bureau Black" });
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/BureauBlue.xaml", Name = "Bureau Blue" });
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/ShinyBlue.xaml", Name = "Shiny Blue" });
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/ShinyRed.xaml", Name = "Shiny Red" });
            Themes.Add(new ThemeViewModel { Path = @"Themes/Skins/WhistlerBlue.xaml", Name = "Whistler Blue" });

            SelectedTheme = Themes.First();
        }

        private void SetupMSBuildPath()
        {
            MSBuildPaths = new ObservableCollection<MSBuildVersionViewModel>();
            MSBuildPaths.Add(new MSBuildVersionViewModel { Path = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe", VisualStudioVersion = "Visual Studio Enterprise 2019" });
            MSBuildPaths.Add(new MSBuildVersionViewModel { Path = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\msbuild.exe", VisualStudioVersion = "Visual Studio Professional 2019" });
            MSBuildPaths.Add(new MSBuildVersionViewModel { Path = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe", VisualStudioVersion = "Visual Studio Community 2019" });
            MSBuildPaths.Add(new MSBuildVersionViewModel { Path = @"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe", VisualStudioVersion = "Visual Studio 2015" });

            MSBuildVersionViewModel selectedMSBuildVersionViewModel = null;

            foreach (var msBuildVersionViewModel in MSBuildPaths)
            {
                if (File.Exists(msBuildVersionViewModel.Path))
                {
                    selectedMSBuildVersionViewModel = msBuildVersionViewModel;
                    break;
                }
            }

            if (selectedMSBuildVersionViewModel != null)
            {
                MSBuildPath = selectedMSBuildVersionViewModel.Path;
            }
        }

        public ObservableCollection<ThemeViewModel> Themes { get; set; }
        public ObservableCollection<MSBuildVersionViewModel> MSBuildPaths { get; set; }
        public ObservableCollection<ProjectProgressViewModel> Projects { get; set; }

        public ICommand BuildCommand { get; set; }

        public ICommand ViewLogCommand { get; set; }

        public ICommand ToggleOptionsCommand { get; set; }

        public ICommand BuildArgumentsChanged { get; set; }

        public ICommand SaveLogCommand { get; set; }

        public int BuildOrder { get; set; }
        
        public ThemeViewModel SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                _selectedTheme = value;
                ApplyTheme(_selectedTheme);

                OnPropertyChanged(nameof(SelectedTheme));
            }
        }

        public string BuildButtonText
        {
            get
            {
                return _buildButtonText;
            }
            set
            {
                _buildButtonText = value;
                OnPropertyChanged(nameof(BuildButtonText));
            }
        }

        public TimeSpan TotalBuildTime
        {
            get
            {
                return _totalBuildTime;
            }
            set
            {
                _totalBuildTime = value;
                OnPropertyChanged(nameof(TotalBuildTime));
            }
        }

        public bool InstallerEnabled
        {
            get
            {
                return _installerEnabled;
            }
            set
            {
                _installerEnabled = value;
                OnPropertyChanged(nameof(InstallerEnabled));
            }
        }

        public bool CodeAnalysisEnabled
        {
            get
            {
                return _codeAnalysisEnabled;
            }
            set
            {
                _codeAnalysisEnabled = value;
                OnPropertyChanged(nameof(CodeAnalysisEnabled));
            }
        }

        public bool BuildButtonEnabled
        {
            get
            {
                return _buildButtonEnabled;
            }
            set
            {
                _buildButtonEnabled = value;
                OnPropertyChanged(nameof(BuildButtonEnabled));
            }
        }

        public bool BuildCompleted
        {
            get
            {
                return _buildCompleted;
            }
            set
            {
                _buildCompleted = value;
                OnPropertyChanged(nameof(BuildCompleted));
            }
        }

        public bool BuildSucceeded
        {
            get
            {
                return _buildSucceeded;
            }
            set
            {
                _buildSucceeded = value;
                OnPropertyChanged(nameof(BuildSucceeded));
            }
        }

        public bool OptionsVisible
        {
            get
            {
                return _optionsVisible;
            }
            set
            {
                _optionsVisible = value;
                OnPropertyChanged(nameof(OptionsVisible));
            }
        }

        public bool LogVisible
        {
            get
            {
                return _logVisible;
            }
            set
            {
                _logVisible = value;
                OnPropertyChanged(nameof(LogVisible));
            }
        }

        public string MSBuildPath
        {
            get
            {
                return _msBuildPath;
            }
            set
            {
                _msBuildPath = value;
                OnPropertyChanged(nameof(MSBuildPath));
            }
        }

        public string MSBuildArgs
        {
            get
            {
                return _msBuildArgs;
            }
            set
            {
                _msBuildArgs = value;
                OnPropertyChanged(nameof(MSBuildArgs));
            }
        }

        public string BuildStatus
        {
            get
            {
                return _buildStatus;
            }
            set
            {
                _buildStatus = value;
                OnPropertyChanged(nameof(BuildStatus));
            }
        }

        public string BuildLogText
        {
            get
            {
                return _buildLogText;
            }
            set
            {
                _buildLogText = value;
                OnPropertyChanged(nameof(BuildLogText));
            }
        }

        private void BuildArgumentsChangedExecute()
        {
            var targets = "Build;ClientServer;Package";

            if (InstallerEnabled)
                targets += ";Installer";

            var codeAnalysis = string.Empty;
            if (!CodeAnalysisEnabled)
                codeAnalysis = "/p:RunCodeAnalysis=false";

            MSBuildArgs = string.Format(MSBuildArguments, BuildFile, targets, codeAnalysis);
        }

        private void ToggleOptionsExecute()
        {
            OptionsVisible = !OptionsVisible;
        }

        private void ViewLogExecute()
        {
            if (_buildLogText == null)
                return;

            LogVisible = !LogVisible;
        }

        private void SaveLogExecute()
        {
            if (_buildLogText == null)
                return;

            var buildFilePathName = Path.GetTempFileName();
            WriteTextToFile(buildFilePathName, _buildLogText);

            var process = new Process { StartInfo = { FileName = "notepad.exe", Arguments = buildFilePathName } };
            process.Start();
        }

        private void BuildExecute()
        {
            LogVisible = false;

            if (BuildButtonText == "Build")
            {
                if (!IsMSBuildPathValid())
                {
                    BuildCompleted = true;
                    BuildSucceeded = false;
                    BuildStatus = $"MSBuild.exe path not found '{MSBuildPath}'";
                    return;
                }

                _cancel = false;
                BuildCompleted = false;
                BuildSucceeded = false;
                BuildButtonEnabled = false;
                BuildButtonText = "Cancel";
                _buildLogText = null;

                Projects.Clear();
                Task.Run(() => BuildExecuteThreaded());
            }
            else if (BuildButtonText == "Cancel")
            {
                BuildButtonText = "Cancelling";

                _cancel = true;
            }
        }

        public bool IsMSBuildPathValid()
        {
            return File.Exists(MSBuildPath);
        }

        private void BuildExecuteThreaded()
        {
            _msBuidProcess = new Process();
            var startTime = DateTime.Now;

            _msBuidProcess.StartInfo.FileName = MSBuildPath;
            _msBuidProcess.StartInfo.Arguments = MSBuildArgs;
            _msBuidProcess.StartInfo.UseShellExecute = false;
            _msBuidProcess.StartInfo.CreateNoWindow = true;
            _msBuidProcess.StartInfo.RedirectStandardOutput = true;
            _msBuidProcess.StartInfo.RedirectStandardError = true;
            _msBuidProcess.Start();

            string text;
            var stringBuilder = new StringBuilder();
            var buildOrder = 1;
            var anyErrors = false;

            do
            {
                text = _msBuidProcess.StandardOutput.ReadLine();
                stringBuilder.AppendLine(text);

                if (text != null)
                {
                    if (text.Contains(">Project "))
                    {
                        if (CheckProjectType(text))
                        {
                            var projectPath = GetProjectPath(text);
                            var endPosition = projectPath.Length - 7;
                            var startPos = projectPath.LastIndexOf(@"\") + 1;
                            var projectName = projectPath.Substring(startPos, endPosition - startPos);
                            var project = Projects.FirstOrDefault(x => x.ProjectPath == projectPath);

                            if (project == null)
                            {
                                var solutionPathStartIndex = text.IndexOf("\"") + 1;
                                var solutionPathEndIndex = text.IndexOf("\"", solutionPathStartIndex);
                                var solutionPath = text.Substring(solutionPathStartIndex, (solutionPathEndIndex - solutionPathStartIndex));

                                Application.Current.Dispatcher.Invoke((() => AddProject(projectPath, projectName, buildOrder++, solutionPath)), DispatcherPriority.Background);
                            }
                        }
                    }

                    if (text.Contains("Done Building Project"))
                    {
                        if (CheckProjectType(text))
                        {
                            var projectPath = GetProjectPath(text);
                            var project = Projects.FirstOrDefault(x => x.ProjectPath == projectPath);

                            if (project != null)
                            {
                                var buildError = text.EndsWith("FAILED.");
                                if (buildError)
                                    anyErrors = true;

                                project.Status = buildError ? "Failed" : "Succeeded";
                                project.BuildTime = DateTime.Now.Subtract(project.StartBuildTime);
                            }
                        }
                    }

                    if (text.Contains(": warning CS") || text.Contains(": warning : CA"))
                    {
                        if (CheckProjectType(text))
                        {
                            var projectPath = GetProjectPath2(text);
                            var project = Projects.FirstOrDefault(x => x.ProjectPath == projectPath);

                            if (project != null)
                            {
                                var startOfWarningIndex = text.IndexOf(": warning") + 2;
                                var warningText = text.Substring(startOfWarningIndex, text.Length - startOfWarningIndex);

                                if (!project.Warnings.Contains(warningText))
                                {
                                    project.Warnings.Add(warningText);
                                    project.WarningCount++;
                                }
                            }
                        }
                    }
                }


            } while ((text != null) && (!_cancel));

            string buildStatusMessage;

            if (_cancel)
            {
                _msBuidProcess.Kill();
                buildStatusMessage = "Build Cancelled";
            }
            else
            {
                var error = _msBuidProcess.StandardError.ReadToEnd();
                BuildLogText = stringBuilder.ToString();
                _msBuidProcess.WaitForExit();
                BuildSucceeded = !anyErrors;
                buildStatusMessage = anyErrors ? "Build Failed" : "Build Succeeded";

                if (anyErrors)
                {
                    LogVisible = true;
                }
            }

            TotalBuildTime = DateTime.Now.Subtract(startTime);
            BuildCompleted = true;
            BuildStatus = string.Format("{0} at {1}", buildStatusMessage, DateTime.Now);
            BuildButtonEnabled = true;
            BuildButtonText = "Build";
        }

        private bool CheckProjectType(string text)
        {
            return _projectTypes.Any(text.Contains);
        }

        private string GetProjectType(string text)
        {
            return _projectTypes.Where(text.Contains).First();
        }

        private static void WriteTextToFile(string fileName, string text)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.Write(text);
            }
        }

        private ProjectProgressViewModel AddProject(string projectPath, string projectName, int buildOrder, string solutionPath)
        {
            var projectProgressViewModel = new ProjectProgressViewModel { Name = projectName, Status = "Building", ProjectPath = projectPath, BuildOrder = buildOrder, SolutionPath = solutionPath };
            Projects.Insert(0, projectProgressViewModel);

            return projectProgressViewModel;
        }

        private string GetProjectPath(string text)
        {
            var projectType = GetProjectType(text);
            var endPosition = text.IndexOf(projectType);

            endPosition += projectType.Length;
            var temp = text.Substring(0, endPosition);
            var startPos = temp.LastIndexOf("\"", endPosition) + 1;

            return text.Substring(startPos, endPosition - startPos);
        }

        private string GetProjectPath2(string text)
        {
            var projectType = GetProjectType(text);
            var endPosition = text.IndexOf(projectType);
            endPosition += projectType.Length;

            var temp = text.Substring(0, endPosition);
            var startPos = temp.LastIndexOf("[", endPosition) + 1;

            return text.Substring(startPos, endPosition - startPos);
        }

        private void ApplyTheme(ThemeViewModel theme)
        {
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri(theme.Path, UriKind.Relative); ;

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
