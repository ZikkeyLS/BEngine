using BEngine;
using ImGuiNET;

namespace BEngineEditor
{
	internal class Vector2Resolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			string x = "0";
			string y = "0";
			Vector2 initial = Vector2.zero;

			object? resultField = data.Properties.GetScriptValue(data.Field, data.Script);

			if (resultField != null)
			{
				initial = (Vector2)resultField;
				x = Math.Round(initial.x, EditorGlobals.NumberVisualPrecision).ToString();
				y = Math.Round(initial.y, EditorGlobals.NumberVisualPrecision).ToString();
			}

			ImGui.PushItemWidth(ImGui.GetWindowSize().X / EditorGlobals.SizeOffset);
			ImGui.Text("x");
			ImGui.SameLine();
			if (ImGui.InputText("##x", ref x, 128))
			{
				x = x.Replace(".", ",");

				object? final = null;

				if (float.TryParse(x, out float result))
				{
					final = new Vector2(result, initial.y);
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
			ImGui.SameLine(0, 5);
			ImGui.Text("y");
			ImGui.SameLine();
			if (ImGui.InputText("##y", ref y, 128))
			{
				y = y.Replace(".", ",");

				object? final = null;

				if (float.TryParse(y, out float result))
				{
					final = new Vector2(initial.x, result);
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
			ImGui.PopItemWidth();
		}
	}
}
