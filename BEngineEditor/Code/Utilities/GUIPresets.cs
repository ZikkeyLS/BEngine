using ImGuiNET;

namespace BEngineEditor
{
	internal static class GUIPresets
	{
		public static bool CreateDropArea(object ID, float y)
		{
			return ImGui.ColorButton("DropArea" + ID, ColorConstants.HeaderColor,
						ImGuiColorEditFlags.NoTooltip |
						ImGuiColorEditFlags.NoPicker |
						ImGuiColorEditFlags.NoDragDrop,
						new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, y));
		}
	}
}
