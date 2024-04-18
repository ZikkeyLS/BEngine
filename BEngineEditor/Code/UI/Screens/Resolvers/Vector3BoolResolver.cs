using BEngine;
using ImGuiNET;

namespace BEngineEditor
{
	internal class Vector3BoolResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			bool x = false;
			bool y = false;
			bool z = false;
			Vector3Bool initial = new Vector3Bool();

			object? resultField = data.Properties.GetScriptValue(data.Field, data.Script);

			if (resultField != null)
			{
				initial = (Vector3Bool)resultField;
				x = initial.x;
				y = initial.y;
				z = initial.z;
			}

			ImGui.PushItemWidth(ImGui.GetWindowSize().X / EditorGlobals.SizeOffset);
			ImGui.Text("x");
			ImGui.SameLine();
			if (ImGui.Checkbox("##x", ref x))
			{
				if (x != initial.x)
				{
					object final = new Vector3Bool(x, initial.y, initial.z);
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
				}
			}
			ImGui.SameLine(0, 5);
			ImGui.Text("y");
			ImGui.SameLine();
			if (ImGui.Checkbox("##y", ref y))
			{
				if (y != initial.y)
				{
					object final = new Vector3Bool(initial.x, y, initial.z);
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
				}
			}
			ImGui.SameLine(0, 5);
			ImGui.Text("z");
			ImGui.SameLine();
			if (ImGui.Checkbox("##z", ref z))
			{
				if (z != initial.z)
				{
					object final = new Vector3Bool(initial.x, initial.y, z);
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
				}
			}
			ImGui.PopItemWidth();
		}
	}
}
