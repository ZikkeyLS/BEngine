using BEngine;
using BEngineCore;
using ImGuiNET;

namespace BEngineEditor
{
	internal class ModelResolver : TypeResolver
	{
		public override void Resolve(ResolverData data)
		{
			AssetReader _reader = data.ProjectContext.CurrentProject.AssetsReader;

			string input = string.Empty;
			object? fieldResult = data.Properties.GetScriptValue(data.Field, data.Script);

			if (fieldResult != null)
			{
				BEngine.Model? result = (BEngine.Model?)fieldResult;
				if (result != null)
					input = result.GUID;
			}

			if (input == null)
				input = string.Empty;

			string textOutput = string.Empty;
			string basePath = "None";
			AssetMetaData? initialAsset = _reader.ModelContext.Assets.Find((asset) => asset.GUID == input);
			if (initialAsset == null)
			{
				textOutput = $"(!Missing) Current model id:\n{input}";
			}
			else
			{
				basePath = Path.GetFileName(initialAsset.GetAssetPath());
				textOutput = $"{basePath}";
			}

			ImGui.TextWrapped(textOutput);
			ImGui.Button("Select Another Model");
			if (ImGui.BeginPopupContextItem("Select Another Model", ImGuiPopupFlags.MouseButtonLeft))
			{
				if (ImGui.BeginListBox("Select Model"))
				{
					foreach (AssetMetaData asset in _reader.ModelContext.Assets)
					{
						basePath = Path.GetFileName(asset.GetAssetPath());
						if (ImGui.Selectable(basePath))
						{
							object final = new BEngine.Model() { GUID = asset.GUID };

							if (final != null)
								data.Properties.UpdateField(data.Field, data.Script, data.SceneScript, final);

							ImGui.CloseCurrentPopup();
						}
					}
				}
				ImGui.EndListBox();
				ImGui.EndPopup();
			}
		}
	}
}
