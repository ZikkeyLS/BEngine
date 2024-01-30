using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	internal class ProjectExplorerScreen : Screen
	{
		private ProjectContext _projectContext;

		private const int BothSideSpacing = 10;
		private const int Spacing = 5;
		private const float LeftExplorerPercentage = 20;
		private const float ItemSidePercentage = 5.75f;

		private string _activeTargetDirectory = string.Empty;
		private string _currentDirectoryOpened = string.Empty;

		private bool _cutFile = false;
		private string _cutPath = string.Empty;

		private bool _copyFile = false;
		private string _copyPath = string.Empty;

		private string _activePath = string.Empty;
		private bool _activeUsing = false;
		private string _lastSavePath = string.Empty;

		private string _rootAssetsDirectory => _projectContext.CurrentProject.AssetsDirectory;
		private string _rootAssemblyDirectory => _projectContext.CurrentProject.ProjectAssemblyDirectory;

		private Logger _logger => _projectContext.CurrentProject.Logger;

		private const string _moveEntryExplorer = "MoveEntryExplorer";

		private struct Entry
		{
			public string EntryPath;
			public string EntryName;
			public bool IsFile;
		}


		protected override void Setup()
		{
			_projectContext = window.ProjectContext;
		}

		public override void Display()
		{
			if (_activeTargetDirectory == string.Empty)
				_activeTargetDirectory = _rootAssetsDirectory;

			ImGui.SetNextWindowSize(new Vector2(ImGui.GetWindowSize().X, ImGui.GetWindowSize().Y / 5), ImGuiCond.FirstUseEver);

			ImGui.Begin("Project");

			int leftOffset = (int)(ImGui.GetWindowSize().X / 100 * LeftExplorerPercentage);

			ImGui.Columns(2);
			ImGui.SetColumnWidth(0, leftOffset);

			ShowFoldersRecursively(_rootAssetsDirectory, _rootAssetsDirectory);
			ShowFoldersRecursively(_rootAssemblyDirectory, _rootAssemblyDirectory, true);

			ImGui.NextColumn();

			ShowDirectory(leftOffset);

			ImGui.End();
		}

		private void ShowDirectory(int leftOffset)
		{
			int itemSide = (int)(ImGui.GetWindowSize().X / 100 * ItemSidePercentage);

			bool assetsConfiguration = _activeTargetDirectory == _rootAssetsDirectory;

			ImGui.PushID(0);
			if (ImGui.Button(assetsConfiguration ? "Assets" : "Assembly"))
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

					string combineDirectory = string.Empty;

					for (int x = 0; x <= i; x++)
					{
						combineDirectory += directoryParts[x];
						if (x != i)
							combineDirectory += @"\";
					}

					ImGui.PushID(combineDirectory);
					if (ImGui.Button(directoryParts[i]))
					{
						if (i != directoryParts.Length - 1)
						{
							_currentDirectoryOpened = combineDirectory;
						}
					}
					ImGui.PopID();
				}
			}

			DirectoryInfo assetsFolder = new DirectoryInfo(Path.Combine(_activeTargetDirectory, _currentDirectoryOpened));

			int maxElementsInLine = ((int)ImGui.GetWindowSize().X - BothSideSpacing - leftOffset) / (itemSide + Spacing);
			int currentElementsInLine = 0;


			foreach (DirectoryInfo directory in assetsFolder.GetDirectories())
			{
				if (assetsConfiguration == false && (directory.Name == "obj" || directory.Name == "bin"))
					continue;

				CreateExplorerItem(directory.Name, directory.FullName, false, itemSide);

				ImGui.SameLine(0, Spacing);

				currentElementsInLine += 1;

				if (currentElementsInLine == maxElementsInLine)
					ImGui.NewLine();
			}

			foreach (FileInfo file in assetsFolder.GetFiles())
			{
				if (assetsConfiguration == false && file.Name.Contains(".csproj"))
					continue;

				CreateExplorerItem(file.Name, file.FullName, true, itemSide);

				ImGui.SameLine(0, Spacing);

				currentElementsInLine += 1;

				if (currentElementsInLine == maxElementsInLine)
					ImGui.NewLine();
			}

			if (ImGui.BeginPopupContextWindow("Explorer Menu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverExistingPopup))
			{
				if (_copyPath != string.Empty)
					if (ImGui.Selectable("Paste (Copied)"))
					{
						if (_copyFile)
						{
							if (File.Exists(_copyPath) == false)
								_copyPath = string.Empty;

							if (_copyPath != _activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + Path.GetFileName(_copyPath))
								File.Copy(_copyPath, _activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + Path.GetFileName(_copyPath), true);
						}
						else
						{
							if (Directory.Exists(_copyPath) == false)
								_copyPath = string.Empty;

							DirectoryInfo copyDirectory = new DirectoryInfo(_copyPath);

							if (_copyPath != _activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + copyDirectory.Name)
								Utils.CopyDirectory(_copyPath, _activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + copyDirectory.Name, true);
						}
					}

				if (_cutPath != string.Empty)
					if (ImGui.Selectable("Paste (Cutted)"))
					{
						if (_cutFile)
						{
							File.Move(_cutPath, _activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + Path.GetFileName(_cutPath), true);
						}
						else
						{
							DirectoryInfo cutDirectory = new DirectoryInfo(_cutPath);
							File.Move(_cutPath, _activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + cutDirectory.Name, true);
						}

						_cutPath = string.Empty;
					}

				if (ImGui.Selectable("Create Folder"))
				{
					DirectoryInfo directory = new DirectoryInfo(_activeTargetDirectory + @"\" + _currentDirectoryOpened + @"\" + "New Folder");
					if (directory.Exists == false)
						directory.Create();
				}

				if (_activeTargetDirectory.Contains(_rootAssemblyDirectory) && ImGui.Selectable("Create Script"))
				{
					File.Create(_rootAssemblyDirectory + @"\" + _currentDirectoryOpened + @"\" + "Empty Script.cs");
				}

				ImGui.EndPopup();
			}
		}

		private void CreateExplorerItem(string entryName, string entryPath, bool isFile, int itemSide)
		{
			ImGui.PushID(entryPath);
			ImGui.BeginGroup();

			if (ImGui.Button(isFile ? "File" : "Directory", new Vector2(itemSide, itemSide)))
			{
				if (isFile == false)
				{
					_currentDirectoryOpened = entryPath.Replace(_activeTargetDirectory + @"\", string.Empty);
				}
				else if (entryName.Contains(".cs"))
				{
					Utils.OpenWithDefaultProgram(_projectContext.CurrentProject.SolutionPath);
				}
				else
				{
					// show in inspector
				}
			}

			string resultName = entryName;
			ImGui.SetNextItemWidth(itemSide);

			if (ImGui.InputText(string.Empty, ref resultName, 1024))
			{
				_activePath = entryPath;
				_activeUsing = true;
				_lastSavePath = resultName;
			}
			else if (_activePath == entryPath && _activeUsing)
			{
				_activePath = "";
				_activeUsing = false;

				if (_lastSavePath != string.Empty && _lastSavePath != " ")
				{
					if (isFile)
					{
						if (_lastSavePath.Contains('.'))
							new FileInfo(entryPath).Rename(_lastSavePath);
					}
					else if (_lastSavePath.EndsWith('.') == false)
					{
						new DirectoryInfo(entryPath).Rename(_lastSavePath);
					}
				}
			}

			ImGui.EndGroup();

			if (ImGui.BeginPopupContextItem(entryPath, ImGuiPopupFlags.MouseButtonRight))
			{
				if (ImGui.Selectable("Copy"))
				{
					_copyFile = isFile;
					_copyPath = entryPath;
				}
				if (ImGui.Selectable("Cut"))
				{
					_cutFile = isFile;
					_cutPath = entryPath;
				}
				if (ImGui.Selectable("Delete"))
				{
					if (isFile)
						File.Delete(entryPath);
					else
						Directory.Delete(entryPath, true);
				}
				ImGui.EndPopup();
			}

			ImGui.PopID();

			DragAndDrop(entryPath, entryName, isFile);
		}

		private unsafe void DragAndDrop(string entryPath, string entryName, bool isFile)
		{
			if (ImGui.BeginDragDropSource())
			{
				Entry entry = new Entry() { EntryPath = entryPath, EntryName = entryName, IsFile = isFile };
				ImGui.SetDragDropPayload(_moveEntryExplorer, (IntPtr)(&entry), (uint)sizeof(Entry));
				ImGui.Text(isFile ? "File" : "Directory");
				ImGui.Text(entryName);
				ImGui.EndDragDropSource();
			}

			Drop(entryPath, isFile);
		}

		private unsafe void Drop(string entryPath, bool isFile)
		{
			if (ImGui.BeginDragDropTarget())
			{
				var payload = ImGui.AcceptDragDropPayload(_moveEntryExplorer);

				if (payload.NativePtr != null)
				{
					var entryPointer = (Entry*)payload.Data;
					Entry entry = entryPointer[0];

					if (entry.IsFile)
					{
						if (isFile == false && entry.EntryPath != entryPath + @"\" + entry.EntryName)
							File.Move(entry.EntryPath, entryPath + @"\" + entry.EntryName, true);
					}
					else
					{
						if (isFile == false && entry.EntryPath != entryPath + @"\" + entry.EntryName)
						{
							Utils.MoveDirectory(entry.EntryPath, entryPath + @"\" + entry.EntryName, true);
						}
					}
				}

				ImGui.EndDragDropTarget();
			}
		}

		private void ShowFoldersRecursively(string directory, string root, bool ignoreAssemblyData = false)
		{
			ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow;
			string[] directories = Directory.GetDirectories(directory);

			if (directories.Length == 0)
				flags |= ImGuiTreeNodeFlags.Leaf;

			bool open = ImGui.TreeNodeEx(directory.Substring(directory.LastIndexOf(@"\") + 1), flags);

			if (ImGui.IsItemClicked())
			{
				_activeTargetDirectory = root == _rootAssetsDirectory ? _rootAssetsDirectory : _rootAssemblyDirectory;

				if (directory == root)
				{
					_currentDirectoryOpened = string.Empty;
				}
				else
				{
					_currentDirectoryOpened = directory.Replace(root + @"\", string.Empty);
				}
			}

			if (directory != _rootAssetsDirectory && directory != _rootAssemblyDirectory)
				DragAndDrop(directory, new DirectoryInfo(directory).Name, false);
			else
				Drop(directory, false);

			if (open)
			{
				for (int i = 0; i < directories.Length; i++)
				{
					string leftPath = directories[i].Replace(root + @"\", string.Empty);
					if (leftPath.Contains("bin") || leftPath.Contains("obj"))
						continue;
					ShowFoldersRecursively(directories[i], root, ignoreAssemblyData);
				}

				ImGui.TreePop();
			}
		}
	}
}
