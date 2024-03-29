using BEngine;
using BEngineCore;
using ImGuiNET;

namespace BEngineEditor
{
	internal class KeyResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			Key initialKey = Key.Unknown;
			object? fieldResult = data.Properties.GetScriptValue(data.Field, data.Script);

			if (fieldResult != null)
			{
				initialKey = (Key)fieldResult;
			}

			if (ImGui.BeginCombo($"##{initialKey}", initialKey.ToString(), ImGuiComboFlags.HeightLargest))
			{
				foreach (Key key in Enum.GetValues<Key>())
				{
					if (ImGui.Selectable(key.ToString()))
					{
						object final = key;

						if (final != null)
							data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
					}
				}
				ImGui.EndCombo();
			}
		}
	}
}
