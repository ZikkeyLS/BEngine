using BEngine;
using ImGuiNET;
using Math = BEngine.Math;

namespace BEngineEditor
{
	internal class Vector3Resolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			string x = "0";
			string y = "0";
			string z = "0";
			Vector3 initial = Vector3.zero;

			object? resultField = data.Properties.GetScriptValue(data.Field, data.Script);

			if (resultField != null)
			{
				initial = (Vector3)resultField;
				x = Math.Round(initial.x, EditorGlobals.NumberVisualPrecision).ToString();
				y = Math.Round(initial.y, EditorGlobals.NumberVisualPrecision).ToString();
				z = Math.Round(initial.z, EditorGlobals.NumberVisualPrecision).ToString();
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
					final = new Vector3(result, initial.y, initial.z);
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
					final = new Vector3(initial.x, result, initial.z);
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
			ImGui.SameLine(0, 5);
			ImGui.Text("z");
			ImGui.SameLine();
			if (ImGui.InputText("##z", ref z, 128))
			{
				z = z.Replace(".", ",");

				object? final = null;

				if (float.TryParse(z, out float result))
				{
					final = new Vector3(initial.x, initial.y, result);
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
			ImGui.PopItemWidth();
		}
	}
}
