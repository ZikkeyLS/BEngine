using BEngine;
using ImGuiNET;

namespace BEngineEditor
{
	internal class Vector4Resolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			string x = "0";
			string y = "0";
			string z = "0";
			string w = "0";
			Vector4 initial = Vector4.zero;

			object? resultField = data.Properties.GetScriptValue(data.Field, data.Script);

			if (resultField != null)
			{
				initial = (Vector4)resultField;
				x = Math.Round(initial.x, EditorGlobals.NumberVisualPrecision).ToString();
				y = Math.Round(initial.y, EditorGlobals.NumberVisualPrecision).ToString();
				z = Math.Round(initial.z, EditorGlobals.NumberVisualPrecision).ToString();
				w = Math.Round(initial.w, EditorGlobals.NumberVisualPrecision).ToString();
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
					final = new Vector4(result, initial.y, initial.z, initial.w);
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
					final = new Vector4(initial.x, result, initial.z, initial.w);
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
					final = new Vector4(initial.x, initial.y, result, initial.w);
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
			ImGui.SameLine(0, 5);
			ImGui.Text("w");
			ImGui.SameLine();
			if (ImGui.InputText("##w", ref w, 128))
			{
				w = w.Replace(".", ",");

				object? final = null;

				if (float.TryParse(w, out float result))
				{
					final = new Vector4(initial.x, initial.y, initial.z, result);
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
			ImGui.PopItemWidth();
		}
	}
}
