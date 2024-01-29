using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	internal class ProjectExplorerScreen : Screen
	{
		private ProjectContext _projectContext;

		private const int BothSideSpacing = 10;
		private const int Spacing = 5;
		private const float LeftExplorerPercentage = 15;
		private const float ItemSidePercentage = 5.75f;

		private string _currentDirectoryOpened = string.Empty;
		private string _copyBuffer = string.Empty;

		private string _rootAssetsDirectory => _projectContext.CurrentProject.AssetsDirectory;

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;
		}

		public override void Display()
		{
			ImGui.SetNextWindowSize(new Vector2(ImGui.GetWindowSize().X, ImGui.GetWindowSize().Y / 5), ImGuiCond.FirstUseEver);

			ImGui.Begin("Project");

			int leftOffset = (int)(ImGui.GetWindowSize().X / 100 * LeftExplorerPercentage);
			int itemSide = (int)(ImGui.GetWindowSize().X / 100 * ItemSidePercentage);

			ImGui.Columns(2);
			ImGui.SetColumnWidth(0, leftOffset);

			// Project folder show

			ShowFoldersRecursively(_rootAssetsDirectory);

			ImGui.NextColumn();

			ImGui.PushID(0);
			if (ImGui.Button("Assets"))
			{
				_currentDirectoryOpened = string.Empty;
			}
			ImGui.PopID();

			if (_currentDirectoryOpened != string.Empty)
			{
				string[] directoryParts = _currentDirectoryOpened.Split(@"\");
				for (int i = 0; i < directoryParts.Length; i++)
				{
					ImGui.SameLine();

					if (ImGui.Button(directoryParts[i]))
					{
						if (i != directoryParts.Length - 1)
							_currentDirectoryOpened = _currentDirectoryOpened.Remove(_currentDirectoryOpened.LastIndexOf(directoryParts[i + 1]) - 1);
					}
				}
			}

			DirectoryInfo assetsFolder = new DirectoryInfo(Path.Combine(_rootAssetsDirectory, _currentDirectoryOpened));

			int maxElementsInLine = ((int)ImGui.GetWindowSize().X - BothSideSpacing - leftOffset) / (itemSide + Spacing);
			int currentElementsInLine = 0;

			foreach (DirectoryInfo directory in assetsFolder.GetDirectories())
			{
				if (ImGui.Button(directory.Name, new Vector2(itemSide, itemSide))) 
				{
					_currentDirectoryOpened = directory.FullName.Replace(_rootAssetsDirectory + @"\", string.Empty);
				}

				if (ImGui.BeginPopupContextItem(directory.FullName, ImGuiPopupFlags.MouseButtonRight))
				{
					if (ImGui.Selectable("Copy")) { }
					if (ImGui.Selectable("Paste")) { }
					if (ImGui.Selectable("Delete")) 
					{
						Directory.Delete(directory.FullName, true);
					}
					ImGui.EndPopup();
				}

				ImGui.SameLine(0, Spacing);

				currentElementsInLine += 1;

				if (currentElementsInLine == maxElementsInLine)
					ImGui.NewLine();
			}


			foreach (FileInfo file in assetsFolder.GetFiles())
			{
				if (ImGui.Button(file.Name, new Vector2(itemSide, itemSide)))
				{

				}

				if (ImGui.BeginPopupContextItem(file.FullName, ImGuiPopupFlags.MouseButtonRight))
				{
					if (ImGui.Selectable("Copy")) { }
					if (ImGui.Selectable("Paste")) { }
					if (ImGui.Selectable("Delete")) 
					{
						File.Delete(file.FullName);
					}
					ImGui.EndPopup();
				}

				ImGui.SameLine(0, Spacing);

				currentElementsInLine += 1;

				if (currentElementsInLine == maxElementsInLine)
					ImGui.NewLine();
			}

			if (ImGui.BeginPopupContextWindow("Explorer Menu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverExistingPopup))
			{
				if (ImGui.Selectable("Test")) { }
				if (ImGui.Selectable("Test2")) { }
				ImGui.EndPopup();
			}

			ImGui.End();
		}

		private void ShowFoldersRecursively(string directory)
		{
			if (ImGui.TreeNodeEx(directory.Substring(directory.LastIndexOf(@"\") + 1), ImGuiTreeNodeFlags.DefaultOpen))
			{
				string[] directories = Directory.GetDirectories(directory);

				if (ImGui.IsItemClicked())
				{
					if (directory == _rootAssetsDirectory)
					{
						_currentDirectoryOpened = string.Empty;
					}
					else
					{
						_currentDirectoryOpened = directory.Replace(_rootAssetsDirectory + @"\", string.Empty);
					}
				}

				for (int i = 0; i < directories.Length; i++)
				{
					ShowFoldersRecursively(directories[i]);
				}

				ImGui.TreePop();
			}
		}
	}
}
