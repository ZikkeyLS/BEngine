using BEngine;
using BEngineCore;
using BEngineScripting;
using ImGuiNET;

namespace BEngineEditor
{
	internal class ColorResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			Color? input = (Color?)data.Properties.GetScriptValue(data.Field, data.Script);
			System.Numerics.Vector4 result = new System.Numerics.Vector4();
			if (input != null)
				result = input.Value;
	
			if (ImGui.ColorEdit4(data.FieldName, ref result))
			{
				data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, (Color)result);
			}
		}
	}
}
