using ImGuiNET;

namespace BEngineEditor
{
	internal class CommonResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			string? input = string.Empty;

			object? fieldResult = data.Properties.GetScriptValue(data.Field, data.Script);
			if (fieldResult != null)
				input = fieldResult.ToString();

			if (input == null)
				input = string.Empty;

			if (IsInClassList(data.FieldType, typeof(string)) == false)
				input = input.Replace(".", ",");

			if (ImGui.InputText("##common", ref input, 128))
			{
				if (input == null)
					return;

				if (input == string.Empty && IsInClassList(data.FieldType, typeof(string)) == false)
					return;

				object? final = null;
				if (data.FieldType != typeof(string))
				{
					if (double.TryParse(input, out double result))
					{
						final = Convert.ChangeType(result, data.FieldType);
					}
				}
				else
				{
					final = input;
				}

				if (final != null)
					data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);
			}
		}
	}
}
