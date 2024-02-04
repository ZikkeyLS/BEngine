using BEngine;
using BEngineCore;
using ImGuiNET;
using System.Reflection;
using System.Text.Json;

namespace BEngineEditor
{
	public class PropertiesScreen : Screen
	{
		private ProjectContext _projectContext => window.ProjectContext;
		private Scripting _scripting => _projectContext.CurrentProject.Scripting;
#pragma warning disable CS8603 // Possible null reference return.
		private SelectedElement _selectedElement => _projectContext.CurrentProject.SelectedElement;
#pragma warning restore CS8603 // Possible null reference return.

		private string? _lastName = string.Empty;

		private bool _showScriptSelection = false;

		public override void Display()
		{
			ImGui.Begin("Properties", ImGuiWindowFlags.HorizontalScrollbar);

			if (_selectedElement.Type == ItemTypeSelected.Entity)
			{
				SceneEntity selectedEntity = (SceneEntity)_selectedElement.Element;

				if (_projectContext.CurrentProject.OpenedScene.Entities.Contains(selectedEntity) == false)
				{
					_projectContext.CurrentProject.SelectedElement = null;
					return;
				}

				string newName = selectedEntity.Name;

				if (_lastName != selectedEntity.GUID)
				{
					_lastName = selectedEntity.GUID;
					return;
				}

				if (ImGui.InputText("Name", ref newName, 150))
				{
					if (newName == "" || newName == " ")
					{
						newName = "~";
					}

					selectedEntity.Name = newName;
				}

				ImGui.Separator();

				for (int i = 0; i < selectedEntity.Scripts.Count; i++)
				{
					string fullName = selectedEntity.Scripts[i].Namespace + "." + selectedEntity.Scripts[i].Name;

					ImGui.PushID(fullName);
					ImGui.BeginGroup();
					ImGui.Text(fullName);

					SceneScript sceneScript = selectedEntity.Scripts[i];
					Script? script = selectedEntity.GetScriptInstance(sceneScript);

					if (script != null)
					{
						FieldInfo[] fields = script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

						for (int j = 0; j < fields.Length; j++)
						{
							FieldInfo field = fields[j];
							SceneScriptField? sceneScriptField = selectedEntity.Scripts[i].Fields?.Find((scripField) => scripField.Name == field.Name);
							SceneScriptValue? sceneScriptValue = sceneScriptField?.Value;

							if (IsInClassList(field.FieldType, typeof(string), typeof(int), 
								typeof(float), typeof(double), typeof(uint), typeof(byte), typeof(sbyte),
								typeof(short), typeof(ushort)))
							{
								string input = sceneScriptValue != null ? sceneScriptValue.Value.ToString() : fields[j]?.GetValue(script)?.ToString();

								if (IsInClassList(field.FieldType, typeof(string)))
								{
									input = input.Substring(1, input.Length - 2);
								}

								if (ImGui.InputText(field.Name, ref input, 128))
								{
									if (input == null)
										continue;

									if (input == string.Empty && IsInClassList(field.FieldType, typeof(string)) == false)
										continue;

									if (IsInClassList(field.FieldType, typeof(float), typeof(double)))
									{
										input = input.Replace(",", ".");
									}

									try
									{
										object? final = null;
										if (field.FieldType != typeof(string))
											final = JsonSerializer.Deserialize(input, field.FieldType);
										else
											final = input;

										if (final != null)
											UpdateField(field, script, sceneScript, final);

									}
									catch
									{

									}
								}
							}
							else if (IsInClassList(field.FieldType, typeof(bool)))
							{
								bool input = sceneScriptValue != null ? bool.Parse(sceneScriptValue.Value) : (bool)field.GetValue(script);
								bool result = input;

								if (ImGui.Checkbox(field.Name, ref result))
								{
									UpdateField(field, script, sceneScript, result);
								}
							}
						}
					}

					ImGui.EndGroup();
					ImGui.PopID();

					if (ImGui.BeginPopupContextItem(fullName, ImGuiPopupFlags.MouseButtonRight))
					{
						if (ImGui.Selectable("Delete"))
						{
							selectedEntity.RemoveScript(selectedEntity.Scripts[i]);
						}
						ImGui.EndPopup();
					}
				}

				ImGui.SetCursorPosX((ImGui.GetWindowWidth() - 125) / 4);
				if (ImGui.Button("Add script", new System.Numerics.Vector2(100, 60)))
				{
					_showScriptSelection = !_showScriptSelection;
				}

				if (_showScriptSelection)
				{
					if (ImGui.BeginListBox("Select Script"))
					{
						for (int i = 0; i < _scripting.Scripts.Count; i++)
						{
							Scripting.CachedScript currentScript = _scripting.Scripts[i];

							if (selectedEntity.Scripts.Find((script) => script.Namespace == currentScript.Namespace
								&& script.Name == currentScript.Name) != null)
							{
								continue;
							}

							if (ImGui.Selectable(currentScript.Fullname))
							{
								selectedEntity.AddScript(currentScript);
								_showScriptSelection = false;
							}
						}
					}
					ImGui.EndListBox();
				}
			}

			ImGui.End();
		}

		private void UpdateField(FieldInfo field, Script script, SceneScript sceneScript, object final)
		{
			try
			{
				field.SetValue(script, final);

				if (sceneScript.ContainsField(field.Name))
				{
					sceneScript.ChangeField(field.Name, final);
				}
				else
				{
					sceneScript.AddField(field.Name, final);
				}
			}
			catch
			{

			}
		}

		public bool IsInClassList(Type current, params Type[] typeList)
		{
			for (int i = 0; i < typeList.Length; i++)
			{
				if (current == typeList[i])
					return true;
			}

			return false;
		}
	}
}
