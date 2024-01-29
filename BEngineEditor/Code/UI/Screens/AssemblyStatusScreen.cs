using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	internal class AssemblyStatusScreen : Screen
	{
		private ProjectContext _projectContext;

		private HashSet<string> _buildErrors => _compiler.BuildCompileErrors;

		private HashSet<string> _compileErrors => _compiler.AssemblyCompileErrors;
		private HashSet<string> _compileWarnings => _compiler.AssemblyCompileWarnings;
		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;

		private HashSet<string> _logErrors => _projectContext.CurrentProject.Logger.ErrorsLogs;
		private HashSet<string> _logWarnings => _projectContext.CurrentProject.Logger.WarningsLogs;
		private List<string> _logMessages => _projectContext.CurrentProject.Logger.MessageLogs;

		private Vector4 _white = new Vector4(1, 1, 1, 1);
		private Vector4 _black = new Vector4(0, 0, 0, 1);
		private Vector4 _red = new Vector4(1, 0, 0, 1);
		private Vector4 _greed = new Vector4(0, 1, 0, 1);
		private Vector4 _yellow = new Vector4(1, 1, 0, 1);

		private bool _showMessages = true;
		private bool _showWarnings = true;

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;
		}

		public override void Display()
		{
			ImGui.SetNextWindowSize(new Vector2(ImGui.GetWindowSize().X, ImGui.GetWindowSize().Y / 5), ImGuiCond.FirstUseEver);

			ImGui.Begin("Assembly Status", ImGuiWindowFlags.AlwaysAutoResize);

			if (ImGui.Button("Clear Logs"))
			{
				_projectContext.CurrentProject.Logger.ClearLogs();
			}

			ImGui.SameLine(0, 5);

			ImGui.PushStyleColor(ImGuiCol.Button, _showWarnings ? _greed : _red);
			ImGui.PushStyleColor(ImGuiCol.Text, _black);
			if (ImGui.Button("Warnings"))
			{
				_showWarnings = !_showWarnings;
			}
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();

			ImGui.SameLine(0, 5);

			ImGui.PushStyleColor(ImGuiCol.Button, _showMessages ? _greed : _red);
			ImGui.PushStyleColor(ImGuiCol.Text, _black);
			if (ImGui.Button("Messages"))
			{
				_showMessages = !_showMessages;
			}
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();

			ImGui.SameLine();

			ImGui.Separator();

			int logID = 0;


			if (_compiler.BuildingGame)
			{
				GenerateLog(ref logID, $"Building game... (Build for {Math.Round((DateTime.Now -
					_compiler.BuildStartTime).TotalSeconds, 1)} sec)", _white);
				return;
			}

			if (_compiler.AssemblyLoaded == false)
			{
				GenerateLog(ref logID, $"Building assembly... (Build for {Math.Round((DateTime.Now -
					_compiler.AssemblyBuildStartTime).TotalSeconds, 1)} sec)", _white);
				return;
			}

			DisplayErrors(ref logID);
			if (_showWarnings)
				DisplayWarnings(ref logID);
			if (_showMessages)
				DisplayMessages(ref logID);

			ImGui.End();
		}

		private void DisplayErrors(ref int logID)
		{
			foreach (string error in _buildErrors)
			{
				GenerateLog(ref logID, error, _red);
			}

			foreach (string error in _compileErrors)
			{
				GenerateLog(ref logID, error, _red);
			}

			foreach (string error in _logErrors)
			{
				GenerateLog(ref logID, error, _red);
			}
		}

		private void DisplayWarnings(ref int logID)
		{
			foreach (string warning in _compileWarnings)
			{
				GenerateLog(ref logID, warning, _yellow);
			}

			foreach (string warning in _logWarnings)
			{
				GenerateLog(ref logID, warning, _yellow);
			}
		}

		private void DisplayMessages(ref int logID)
		{
			foreach (string message in _logMessages)
			{
				GenerateLog(ref logID, message, _white);
			}
		}

		private void GenerateLog(ref int logID, string data, Vector4 color)
		{
			ImGui.PushID(logID);
			ImGui.PushItemWidth(ImGui.GetWindowSize().X);
			ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);
			ImGui.PushStyleColor(ImGuiCol.Text, color);
			ImGui.InputText(string.Empty, ref data, 1024, ImGuiInputTextFlags.ReadOnly);
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();
			ImGui.PopItemWidth();
			ImGui.PopID();
			logID += 1;
		}
	}
}
