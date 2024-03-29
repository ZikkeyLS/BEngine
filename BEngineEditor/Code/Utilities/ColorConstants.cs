using ImGuiNET;
using System.Numerics;

namespace BEngineEditor
{
	internal static class ColorConstants
	{
		public static readonly Vector4 White = new(1, 1, 1, 1);
		public static readonly Vector4 Black = new(0, 0, 0, 1);
		public static readonly Vector4 Red = new(1, 0, 0, 1);
		public static readonly Vector4 Green = new(0, 1, 0, 1);
		public static readonly Vector4 Yellow = new(1, 1, 0, 1);

		public unsafe static Vector4 HeaderColor => *ImGui.GetStyleColorVec4(ImGuiCol.Header);
	}
}
