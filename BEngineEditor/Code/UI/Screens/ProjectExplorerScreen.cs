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

		private string _currentDirectoryOpened = string.Empty;

		private bool _cutFile = false;
		private string _cutPath = string.Empty;

		private bool _copyFile = false;
		private string _copyPath = string.Empty;

		private string _activePath = string.Empty;
		private bool _activeUsing = false;
		private string _lastSavePath = string.Empty;

		private Project _currentProject => _projectContext.CurrentProject;

		private string _rootAssetsDirectory => _currentProject.AssetsDirectory;
		private Logger _logger => _currentProject.Logger;
		private AssetWorker _assetWorker => _currentProject.AssetWorker;

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
			ImGui.SetNextWindowSize(new Vector2(ImGui.GetWindowSize().X, ImGui.GetWindowSize().Y / 5), ImGuiCond.FirstUseEver);

			ImGui.Begin("Project");

			int leftOffset = (int)(ImGui.GetWindowSize().X / 100 * LeftExplorerPercentage);

			ImGui.Columns(2);
			ImGui.SetColumnWidth(0, leftOffset);

			ImGui.BeginChild("LeftExplorer", Vector2.Zero, 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			ShowFoldersRecursively(_rootAssetsDirectory, _rootAssetsDirectory, true);

			ImGui.EndChild();

			ImGui.NextColumn();

			ShowDirectory(leftOffset);

			ImGui.End();
		}

		private void ShowDirectory(int leftOffset)
		{
			int itemSide = (int)(ImGui.GetWindowSize().X / 100 * ItemSidePercentage);

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

			DirectoryInfo assetsFolder = new DirectoryInfo(Path.Combine(_rootAssetsDirectory, _currentDirectoryOpened));

			int maxElementsInLine = ((int)ImGui.GetWindowSize().X - BothSideSpacing - leftOffset) / (itemSide + Spacing);
			int currentElementsInLine = 0;

			DirectoryInfo[] directories = Array.Empty<DirectoryInfo>();

			try
			{
				directories = assetsFolder.GetDirectories();
			}
			catch
			{
				return;
			}

			foreach (DirectoryInfo directory in directories)
			{
				if (directory.Name == "obj" || directory.Name == "bin")
					continue;

				CreateExplorerItem(directory.Name, directory.FullName, false, itemSide);

				ImGui.SameLine(0, Spacing); 

				currentElementsInLine += 1;

				if (currentElementsInLine == maxElementsInLine)
				{
					ImGui.NewLine();
					currentElementsInLine = 0;
				}
			}

			foreach (FileInfo file in assetsFolder.GetFiles())
			{
				if (file.Name.Contains(".csproj") || file.Extension == ".meta")
					continue;

				CreateExplorerItem(file.Name, file.FullName, true, itemSide);

				ImGui.SameLine(0, Spacing);

				currentElementsInLine += 1;

				if (currentElementsInLine == maxElementsInLine)
				{
					ImGui.NewLine();
					currentElementsInLine = 0;
				}
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

							string fileCopyPath = _rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + Path.GetFileName(_copyPath);

							if (_copyPath != fileCopyPath)
							{
								_assetWorker.RenameAsset(_copyPath, fileCopyPath);
								File.Copy(_copyPath, fileCopyPath, true);
							}
								
						}
						else
						{
							if (Directory.Exists(_copyPath) == false)
								_copyPath = string.Empty;

							DirectoryInfo copyDirectory = new DirectoryInfo(_copyPath);
							string resultPath = _rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + copyDirectory.Name;

							if (_copyPath != resultPath)
								Utils.CopyDirectory(_copyPath, resultPath, true);
						}
					}

				if (_cutPath != string.Empty)
					if (ImGui.Selectable("Paste (Cutted)"))
					{
						if (_cutFile)
						{
							string fileCutPath = _rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + Path.GetFileName(_cutPath);

							if (_copyPath != fileCutPath)
							{
								_assetWorker.RenameAsset(_copyPath, fileCutPath);
								File.Move(_cutPath, fileCutPath, true);
							}
						}
						else
						{
							DirectoryInfo cutDirectory = new DirectoryInfo(_cutPath);
							if (_copyPath != _rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + cutDirectory.Name &&
								((_rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + cutDirectory.Name).Contains(_cutPath) == false ||
								(_rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + cutDirectory.Name).Length < _cutPath.Length))
								Utils.MoveDirectory(_cutPath, _rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + cutDirectory.Name, true);
						}

						_cutPath = string.Empty;
					}



				if (ImGui.Selectable("Folder"))
				{
					Utils.CreateDirectory(_rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + "New Folder");
				}

				if (ImGui.Selectable("Script"))
				{
					Utils.CreateFile(_rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + "Empty Script.cs");
				}

				if (ImGui.Selectable("Scene"))
				{
					string scenePath = _rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + "New Scene.scene";
					string resultPath = Utils.CreateFile(scenePath);
					AssetData.CreateTemplate<Scene>(resultPath, [Path.GetFileNameWithoutExtension(resultPath)]);
				}

				if (ImGui.Selectable("Text File"))
				{
					Utils.CreateFile(_rootAssetsDirectory + @"\" + _currentDirectoryOpened + @"\" + "New Text.txt");
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
					_currentDirectoryOpened = entryPath.Replace(_rootAssetsDirectory + @"\", string.Empty);
				}
				else 
				{
					if (Path.GetExtension(entryName) == ".cs")
					{
						Utils.OpenWithDefaultProgram(_currentProject.SolutionPath);
					}
					else if (Path.GetExtension(entryName) == ".scene")
					{
						_currentProject.TryLoadScene(_assetWorker.GetMetaID(entryPath));
					} 
					else
					{
						Utils.OpenWithDefaultProgram(entryPath);
					}

					_assetWorker.CreateAsset(entryPath);
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
						{
							_assetWorker.RenameAsset(entryPath, _lastSavePath);
							new FileInfo(entryPath).Rename(_lastSavePath);
						}
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
					try
					{
						if (isFile)
							File.Delete(entryPath);
						else
							Directory.Delete(entryPath, true);
					}
					catch
					{

					}
				}
				ImGui.EndPopup();
			}

			ImGui.PopID();

			DragAndDrop(entryPath, entryName, isFile);
		}

		private Vector2 _startPos;
		private Vector2 _endPos;

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
						{

							string fileCopyPath = entryPath + @"\" + entry.EntryName;

							_assetWorker.RenameAsset(entry.EntryPath, fileCopyPath);
							File.Move(entry.EntryPath, fileCopyPath, true);
						}
	
					}
					else
					{
						if (isFile == false && entry.EntryPath != entryPath + @"\" + entry.EntryName
							&& (entryPath.Contains(entry.EntryPath) == false || entry.EntryPath.Length > entryPath.Length))
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
			string[] directories = Array.Empty<string>();

			try
			{
				directories = Directory.GetDirectories(directory);
			}
			catch
			{
				return;
			}

			if (directories.Length == 0 && directory != root)
				flags |= ImGuiTreeNodeFlags.Leaf;

			bool open = ImGui.TreeNodeEx(directory.Substring(directory.LastIndexOf(@"\") + 1), flags);

			if (ImGui.IsItemClicked())
			{
				if (directory == root)
				{
					_currentDirectoryOpened = string.Empty;
				}
				else
				{
					_currentDirectoryOpened = directory.Replace(root + @"\", string.Empty);
				}
			}

			if (directory != _rootAssetsDirectory && directory != _rootAssetsDirectory)
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
