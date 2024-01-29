using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	internal class ProjectLoaderScreen : Screen
	{
		private ProjectContext _projectContext;
		private PathPicker _projectCreator;
		private PathPicker _projectSelector;

		private string _projectDefaultFolder => Directory.GetCurrentDirectory() + @"\Projects";

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;
			_projectContext.TempProjectPath = _projectDefaultFolder;

			_projectCreator = new PathPicker() { Mode = PathPicker.PickerMode.Folder };
			_projectSelector = new PathPicker() { Mode = PathPicker.PickerMode.File, AllowedFiles = ["*.sln"] };
		}

		public override void Display()
		{
			uint dockspaceID = ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
			ImGui.SetNextWindowDockID(dockspaceID);

			ImGui.Begin("Project Loader");

			if (ImGui.Button("Cancel", new Vector2(100, 25)))
			{
				_projectContext.SearchingProject = false;
			}

			ImGui.Separator();

			ImGui.InputText("New Project Name", ref _projectContext.TempProjectName, 128);

			if (ImGui.Button("Select Folder", new Vector2(100, 25)))
			{
				_projectCreator.ShowModal(_projectDefaultFolder);
			}

			if (_projectCreator.Render() && !_projectCreator.Cancelled)
			{
				_projectContext.TempProjectPath = _projectCreator.SelectedFolder;
			}

			ImGui.Text("Project will be generated in " + _projectContext.AssembledTempProjectPath);

			if (_projectContext.ValidTempProjectPath)
			{
				if (ImGui.Button("Create Project", new Vector2(150, 50)))
				{
					_projectContext.CreateProject();
				}
			}
			else
			{
				ImGui.TextColored(new Vector4(1, 0, 0, 1), "Invalid path: This directory already exists or name is empty!");
			}

			ImGui.Separator();

			if (ImGui.Button("Load project", new Vector2(150, 50)))
			{
				_projectSelector.ShowModal(_projectDefaultFolder);
			}

			if (_projectSelector.Render() && !_projectSelector.Cancelled)
			{
				_projectContext.LoadProject(_projectSelector.SelectedFile);
			}

			ImGui.Separator();

			ImGui.Text("Last opened projects:");

			foreach (LastProject project in window.Settings.ProjectHistory.AsEnumerable().Reverse())
			{
				if (project.Directory != string.Empty)
				{
					if (ImGui.Button(project.Name, new Vector2(150, 25)))
					{
						if (File.Exists(project.SolutionPath))
						{
							_projectContext.LoadProject(project.SolutionPath);
						}
						else
						{
							window.Settings.ProjectHistory.Remove(project);
						}			
					}
				}
			}

			ImGui.End();
		}
	}
}
