using BEngine;
using BEngineCore;
using ImGuiNET;

namespace BEngineEditor
{
	internal class BoolResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			bool? input = (bool?)data.Properties.GetScriptValue(data.Field, data.Script);
			bool result = false;
			if (input != null)
				result = input.Value;

			if (ImGui.Checkbox(data.FieldName, ref result))
			{
				data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, result);
			}
		}
	}
}
