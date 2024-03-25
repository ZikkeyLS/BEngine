using BEngineCore;
using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	public class ConsoleLogData
	{
		public LogData Data;
		public BigInteger Count;
	}

	internal class ConsoleScreen : Screen
	{
		private ProjectContext _projectContext;

		private HashSet<string> _buildErrors => _compiler.BuildCompileErrors;

		private HashSet<string> _compileErrors => _compiler.AssemblyCompileErrors;
		private HashSet<string> _compileWarnings => _compiler.AssemblyCompileWarnings;
		private ProjectCompiler _compiler => _projectContext.CurrentProject.Compiler;

		private HashSet<LogData> _logErrors => _projectContext.CurrentProject.Logger.ErrorsLogs;
		private HashSet<LogData> _logWarnings => _projectContext.CurrentProject.Logger.WarningsLogs;
		private List<LogData> _logMessages => _projectContext.CurrentProject.Logger.MessageLogs;

		private Vector4 _white = new Vector4(1, 1, 1, 1);
		private Vector4 _black = new Vector4(0, 0, 0, 1);
		private Vector4 _red = new Vector4(1, 0, 0, 1);
		private Vector4 _greed = new Vector4(0, 1, 0, 1);
		private Vector4 _yellow = new Vector4(1, 1, 0, 1);

		private bool _showMessages = true;
		private bool _showWarnings = true;
		private bool _stackLogs = true;

		private Dictionary<string, ConsoleLogData> _compactWarningData = new();
		private Dictionary<string, ConsoleLogData> _compactMessageData = new();

		protected override void Setup()
		{
			_projectContext = window.ProjectContext;
		}

		public override void Display()
		{
			ImGui.SetNextWindowSize(new Vector2(ImGui.GetWindowViewport().Size.X,
				ImGui.GetWindowViewport().Size.Y / 5), ImGuiCond.FirstUseEver);

			ImGui.Begin("Assembly Status");

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

			ImGui.SameLine(0, 5);

			ImGui.PushStyleColor(ImGuiCol.Button, _stackLogs ? _greed : _red);
			ImGui.PushStyleColor(ImGuiCol.Text, _black);
			if (ImGui.Button("Stack Logs"))
			{
				_stackLogs = !_stackLogs;
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

			foreach (LogData error in _logErrors)
			{
				GenerateLog(ref logID, error.ToString(), _red);
			}
		}

		private void DisplayWarnings(ref int logID)
		{
			foreach (string warning in _compileWarnings)
			{
				GenerateLog(ref logID, warning, _yellow);
			}

			if (_stackLogs)
			{
				_compactWarningData = new();

				foreach (LogData warning in _logWarnings)
				{
					if (_compactWarningData.ContainsKey(warning.Data))
					{
						_compactWarningData[warning.Data].Count += 1;
						_compactWarningData[warning.Data].Data.Time = warning.Time;
						continue;
					}
					else
					{
						_compactWarningData.Add(warning.Data, new ConsoleLogData() { Count = 1, Data = warning });
					}
				}

				foreach (var warning in _compactWarningData)
					GenerateLog(ref logID, warning.Value.Data.ToString(), _white, warning.Value.Count);
			}
			else
			{
				foreach (LogData warning in _logWarnings)
				{
					GenerateLog(ref logID, warning.ToString(), _yellow);
				}
			}
		}

		private void DisplayMessages(ref int logID)
		{
			if (_stackLogs)
			{
				_compactMessageData = new();

				foreach (LogData message in _logMessages)
				{
					if (_compactMessageData.ContainsKey(message.Data))
					{
						_compactMessageData[message.Data].Count += 1;
						_compactMessageData[message.Data].Data.Time = message.Time;
						continue;
					}
					else
					{
						_compactMessageData.Add(message.Data, new ConsoleLogData() { Count = 1, Data = message });
					}
				}

				foreach (var message in _compactMessageData)
					GenerateLog(ref logID, message.Value.Data.ToString(), _white, message.Value.Count);
			}
			else
			{
				foreach (LogData message in _logMessages)
				{
					GenerateLog(ref logID, message.ToString(), _white);
				}
			}
		}

		private void GenerateLog(ref int logID, string data, Vector4 color) => GenerateLog(ref logID, data, color, new BigInteger(0));

		private void GenerateLog(ref int logID, string data, Vector4 color, BigInteger count)
		{
			ImGui.PushID(logID);
			ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);
			ImGui.PushStyleColor(ImGuiCol.Text, color);
			ImGui.PushItemWidth(ImGui.GetWindowSize().X * 0.9f);
			ImGui.InputText(string.Empty, ref data, 1024, ImGuiInputTextFlags.ReadOnly);
			ImGui.PopItemWidth();
			if (count > 1)
			{
				ImGui.SameLine();
				ImGui.Text(count.ToString());
			}
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();
			ImGui.PopID();
			logID += 1;
		}
	}
}
