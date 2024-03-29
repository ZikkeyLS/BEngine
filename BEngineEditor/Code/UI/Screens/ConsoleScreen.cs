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

			ImGui.Begin("Console");

			if (ImGui.Button("Clear Logs"))
			{
				_projectContext.CurrentProject.Logger.ClearLogs();
			}

			ImGui.SameLine(0, 5);

			ImGui.PushStyleColor(ImGuiCol.Button, _showWarnings ? ColorConstants.Green : ColorConstants.Red);
			ImGui.PushStyleColor(ImGuiCol.Text, ColorConstants.Black);
			if (ImGui.Button("Warnings"))
			{
				_showWarnings = !_showWarnings;
			}
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();

			ImGui.SameLine(0, 5);

			ImGui.PushStyleColor(ImGuiCol.Button, _showMessages ? ColorConstants.Green : ColorConstants.Red);
			ImGui.PushStyleColor(ImGuiCol.Text, ColorConstants.Black);
			if (ImGui.Button("Messages"))
			{
				_showMessages = !_showMessages;
			}
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();

			ImGui.SameLine(0, 5);

			ImGui.PushStyleColor(ImGuiCol.Button, _stackLogs ? ColorConstants.Green : ColorConstants.Red);
			ImGui.PushStyleColor(ImGuiCol.Text, ColorConstants.Black);
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
					_compiler.BuildStartTime).TotalSeconds, 1)} sec)", ColorConstants.White);
				return;
			}

			if (_compiler.AssemblyLoaded == false)
			{
				GenerateLog(ref logID, $"Building assembly... (Build for {Math.Round((DateTime.Now -
					_compiler.AssemblyBuildStartTime).TotalSeconds, 1)} sec)", ColorConstants.White);
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
				GenerateLog(ref logID, error, ColorConstants.Red);
			}

			foreach (string error in _compileErrors)
			{
				GenerateLog(ref logID, error, ColorConstants.Red);
			}

			foreach (LogData error in _logErrors)
			{
				GenerateLog(ref logID, error.ToString(), ColorConstants.Red);
			}
		}

		private void DisplayWarnings(ref int logID)
		{
			foreach (string warning in _compileWarnings)
			{
				GenerateLog(ref logID, warning, ColorConstants.Yellow);
			}

			DisplayMessageTree(ref logID, _logWarnings, ColorConstants.Yellow);
		}

		private void DisplayMessages(ref int logID)
		{
			DisplayMessageTree(ref logID, _logMessages, ColorConstants.White);
		}

		private void DisplayMessageTree(ref int logID, IEnumerable<LogData> messages, Vector4 color)
		{
			if (_stackLogs)
			{
				_compactMessageData = new();

				foreach (LogData message in messages)
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
				{
					GenerateLog(ref logID, message.Value.Data.ToString(), color, message.Value.Count);
				}
			}
			else
			{
				foreach (LogData message in messages)
				{
					GenerateLog(ref logID, message.ToString(), color);
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

			ImGui.PopStyleColor(2);

			ImGui.PopID();
			logID += 1;
		}
	}
}
