using BEngine;
using ImGuiNET;

namespace BEngineEditor
{
	internal class MouseButtonResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			MouseButton initialButton = MouseButton.Unknown;
			object? fieldResult = data.Properties.GetScriptValue(data.Field, data.Script);

			if (fieldResult != null)
			{
				initialButton = (MouseButton)fieldResult;
			}

			if (ImGui.BeginCombo($"##{initialButton}", initialButton.ToString()))
			{
				foreach (MouseButton button in Enum.GetValues<MouseButton>())
				{
					if (ImGui.Selectable(button.ToString()))
					{
						object final = button;

						if (final != null)
							data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
					}
				}
				ImGui.EndCombo();
			}
		}
	}
}
