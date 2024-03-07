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
		private Scene _scene => _projectContext.CurrentProject.LoadedScene;
		private AssetReader _reader => _projectContext.CurrentProject.AssetsReader;
#pragma warning disable CS8603 // Possible null reference return.
		private SelectedElement _selectedElement => _projectContext.CurrentProject.SelectedElement;
#pragma warning restore CS8603 // Possible null reference return.

		private string? _lastName = string.Empty;

		private const int precision = 5;

		public override void Display()
		{
			ImGui.Begin("Properties", ImGuiWindowFlags.HorizontalScrollbar);

			if (_selectedElement.Type == ItemTypeSelected.Entity)
			{
				SceneEntity selectedEntity = (SceneEntity)_selectedElement.Element;

				if (_projectContext.CurrentProject.LoadedScene.Entities.Contains(selectedEntity) == false)
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

					selectedEntity.SetName(newName);
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
						List<MemberInfo> fields = script.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance).
							ToList().FindAll((member) => member.MemberType == MemberTypes.Field | member.MemberType == MemberTypes.Property);

						bool searching = true;
						while (searching)
						{
							bool success = true;
							for (int j = 0; j < fields.Count; j++)
							{
								MemberInfo field = fields[j];

								if (field.IsDefined(typeof(EditorShowAt)))
								{
									foreach (object attribute in field.GetCustomAttributes(true))
									{
										if (attribute == null)
											continue;

										EditorShowAt? forceShow = attribute as EditorShowAt;

										if (forceShow == null)
											continue;

										if (j != forceShow.Placement)
										{
											fields.Remove(field);
											fields.Insert(forceShow.Placement, field);
											success = false;
										}
									}
								}
							}

							if (success == false)
								break;

							searching = false;
						}

						for (int j = 0; j < fields.Count; j++)
						{
							MemberInfo field = fields[j];
							SceneScriptField? sceneScriptField = selectedEntity.Scripts[i].Fields?.Find((scripField) => scripField.Name == field.Name);
							SceneScriptValue? sceneScriptValue = sceneScriptField?.Value;
							Type fieldType;

							if (field.MemberType == MemberTypes.Field)
							{
								fieldType = ((FieldInfo)field).FieldType;
							}
							else
							{
								fieldType = ((PropertyInfo)field).PropertyType;
							}

							if (field.IsDefined(typeof(EditorIgnore)))
								continue;

							string fieldName = field.Name;

							if (field.IsDefined(typeof(EditorName)))
							{
								foreach (object attribute in field.GetCustomAttributes(true))
								{
									if (attribute == null)
										continue;

									EditorName? editorName = attribute as EditorName;

									if (editorName == null)
										continue;

									fieldName = editorName.Name;
								}
							}

							ImGui.PushID(field.Name);

							if (IsInClassList(fieldType, typeof(string), typeof(int),
								typeof(float), typeof(double), typeof(uint), typeof(byte), typeof(sbyte),
								typeof(short), typeof(ushort)))
							{
								string input = string.Empty;

								object? fieldResult = GetScriptValue(field, script);
								if (fieldResult != null)
									input = fieldResult.ToString();

								if (input == null)
									input = string.Empty;

								if (IsInClassList(fieldType, typeof(string)))
								{
									if (input != string.Empty)
										input = input.Substring(1, input.Length - 2);
								}
								else
									input = input.Replace(".", ",");

								if (ImGui.InputText(fieldName, ref input, 128))
								{
									if (input == null)
										continue;

									if (input == string.Empty && IsInClassList(fieldType, typeof(string)) == false)
										continue;

									object? final = null;
									if (fieldType != typeof(string))
									{
										if (double.TryParse(input, out double result))
										{
											final = Convert.ChangeType(result, fieldType);
										}
									}
									else
										final = input;

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
							}
							else if (IsInClassList(fieldType, typeof(bool)))
							{
								bool input = (bool)GetScriptValue(field, script);
								bool result = input;

								if (ImGui.Checkbox(fieldName, ref result))
								{
									UpdateField(field, script, sceneScript, result);
								}
							}
							else if (IsInClassList(fieldType, typeof(Vector3)))
							{
								string x = "0";
								string y = "0";
								string z = "0";
								Vector3 initial = Vector3.zero;

								object? resultField = GetScriptValue(field, script);
								if (resultField != null)
								{
									initial = (Vector3)resultField;
									x = Math.Round(initial.x, precision).ToString();
									y = Math.Round(initial.y, precision).ToString();
									z = Math.Round(initial.z, precision).ToString();
								}

								ImGui.Text(fieldName);
								ImGui.PushItemWidth(ImGui.GetWindowSize().X / 6);
								ImGui.Text("x");
								ImGui.SameLine();
								if (ImGui.InputText("##x", ref x, 128))
								{
									x = x.Replace(".", ",");

									object? final = null;

									if (float.TryParse(x, out float result))
									{
										final = new Vector3(result, initial.y, initial.z);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.SameLine(0, 5);
								ImGui.Text("y");
								ImGui.SameLine();
								if (ImGui.InputText("##y", ref y, 128))
								{
									y = y.Replace(".", ",");

									object? final = null;

									if (float.TryParse(y, out float result))
									{
										final = new Vector3(initial.x, result, initial.z);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.SameLine(0, 5);
								ImGui.Text("z");
								ImGui.SameLine();
								if (ImGui.InputText("##z", ref z, 128))
								{
									z = z.Replace(".", ",");

									object? final = null;

									if (float.TryParse(z, out float result))
									{
										final = new Vector3(initial.x, initial.y, result);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.PopItemWidth();
							}
							else if (IsInClassList(fieldType, typeof(Quaternion)))
							{
								string x = "0";
								string y = "0";
								string z = "0";
								string w = "0";

								Quaternion initial = Quaternion.identity;

								object? fieldResult = GetScriptValue(field, script);
								if (fieldResult != null)
								{
									initial = (Quaternion)fieldResult;
									x = Math.Round(initial.x, precision).ToString();
									y = Math.Round(initial.y, precision).ToString();
									z = Math.Round(initial.z, precision).ToString();
									w = Math.Round(initial.w, precision).ToString();
								}

								ImGui.Text(fieldName);
								ImGui.PushItemWidth(ImGui.GetWindowSize().X / 6);
								ImGui.Text("x");
								ImGui.SameLine();
								if (ImGui.InputText("##x", ref x, 128))
								{
									x = x.Replace(".", ",");

									object? final = null;

									if (float.TryParse(x, out float result))
									{
										final = new Quaternion(result, initial.y, initial.z, initial.w);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.SameLine(0, 5);
								ImGui.Text("y");
								ImGui.SameLine();
								if (ImGui.InputText("##y", ref y, 128))
								{
									y = y.Replace(".", ",");
									object? final = null;

									if (float.TryParse(y, out float result))
									{
										final = new Quaternion(initial.x, result, initial.z, initial.w);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.SameLine(0, 5);
								ImGui.Text("z");
								ImGui.SameLine();
								if (ImGui.InputText("##z", ref z, 128))
								{
									z = z.Replace(".", ",");

									object? final = null;

									if (float.TryParse(z, out float result))
									{
										final = new Quaternion(initial.x, initial.y, result, initial.w);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.SameLine(0, 5);
								ImGui.Text("w");
								ImGui.SameLine();
								if (ImGui.InputText("##w", ref w, 128))
								{
									w = w.Replace(".", ",");

									object? final = null;

									if (float.TryParse(w, out float result))
									{
										final = new Quaternion(initial.x, initial.y, initial.z, result);
									}

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
								ImGui.PopItemWidth();
							}
							else if (IsInClassList(fieldType, typeof(BEngine.Model)))
							{
								string input = string.Empty;

								if (fields[j] != null)
								{
									BEngine.Model? result = (BEngine.Model?)GetScriptValue(field, script);
									if (result != null)
										input = result.GUID;
								}

								if (input == null)
									input = string.Empty;

								ImGui.TextWrapped($"Current model id:\n{input}");
								ImGui.Button("Select Another Model");
								if (ImGui.BeginPopupContextItem("Select Another Model", ImGuiPopupFlags.MouseButtonLeft))
								{
									if (ImGui.BeginListBox("Select Model"))
									{
										foreach (string modelID in _reader.ModelContext.GUIDs)
										{
											if (ImGui.Selectable(modelID))
											{
												object final = new BEngine.Model() { GUID = modelID };

												if (final != null)
													UpdateField(field, script, sceneScript, final);

												ImGui.CloseCurrentPopup();
											}
										}
									}
									ImGui.EndListBox();
									ImGui.EndPopup();
								}
							}

							ImGui.PopID();
						}
					}
					else
					{
						if (ImGui.Button("Remove"))
						{
							selectedEntity.RemoveScript(sceneScript);
						}
						ImGui.SameLine(0, 15);
						ImGui.Button("Change Instance");
						if (ImGui.BeginPopupContextItem("Change Instance", ImGuiPopupFlags.MouseButtonLeft))
						{
							if (ImGui.BeginListBox("Select Script"))
							{
								for (int j = 0; j < _scripting.Scripts.Count; j++)
								{
									Scripting.CachedScript currentScript = _scripting.Scripts[j];

									if (selectedEntity.Scripts.Find((script) => script.Namespace == currentScript.Namespace
										&& script.Name == currentScript.Name) != null)
									{
										continue;
									}

									if (ImGui.Selectable(currentScript.Fullname))
									{
										if (selectedEntity.RenameScript(sceneScript, currentScript))
										{
											ImGui.CloseCurrentPopup();
										}
									}
								}
							}
							ImGui.EndListBox();
							ImGui.EndPopup();
						}
						ImGui.Button("Change All Instances");
						if (ImGui.BeginPopupContextItem("Change All Instances", ImGuiPopupFlags.MouseButtonLeft))
						{
							if (ImGui.BeginListBox("Select Script"))
							{
								for (int j = 0; j < _scripting.Scripts.Count; j++)
								{
									Scripting.CachedScript currentScript = _scripting.Scripts[j];

									if (selectedEntity.Scripts.Find((script) => script.Namespace == currentScript.Namespace
										&& script.Name == currentScript.Name) != null)
									{
										continue;
									}

									if (ImGui.Selectable(currentScript.Fullname))
									{
										bool closed = false;
										string initialName = sceneScript.Name;
										string initialNamespace = sceneScript.Namespace;

										foreach (SceneEntity entity in _scene.Entities)
										{
											SceneScript? found = entity.Scripts.Find((allScript) =>
												allScript.Name == initialName && allScript.Namespace == initialNamespace);

											if (found == null)
												continue;

											if (entity.Scripts.Find((script) => script.Namespace == currentScript.Namespace
												&& script.Name == currentScript.Name) != null)
											{
												_projectContext.CurrentProject.Logger.LogWarning($"Can't rename " +
													$"{found.Namespace}.{found.Name} " +
													$"because it already contains {currentScript.Namespace}.{currentScript.Name} " +
													$"in entity: {entity.Name} ({entity.GUID})");
												continue;
											}

											if (entity.RenameScript(found, currentScript)
												&& closed == false)
											{
												ImGui.CloseCurrentPopup();
												closed = true;
											}
										}
									}
								}
							}
							ImGui.EndListBox();
							ImGui.EndPopup();
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

				ImGui.Button("Add Script", new System.Numerics.Vector2(100, 60));
				if (ImGui.BeginPopupContextItem("Add Script", ImGuiPopupFlags.MouseButtonLeft))
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
								ImGui.CloseCurrentPopup();
							}
						}
					}
					ImGui.EndListBox();
					ImGui.EndPopup();
				}
			}

			ImGui.End();
		}

		private object? GetScriptValue(MemberInfo field, Script script)
		{
			if (field.MemberType == MemberTypes.Field)
				return ((FieldInfo)field).GetValue(script);
			else if (field.MemberType == MemberTypes.Property)
				return ((PropertyInfo)field).GetValue(script);

			return null;
		}

		private void UpdateField(MemberInfo field, Script script, SceneScript sceneScript, object final)
		{
			try
			{
				if (field.MemberType == MemberTypes.Field)
					((FieldInfo)field).SetValue(script, final);
				else if (field.MemberType == MemberTypes.Property)
					((PropertyInfo)field).SetValue(script, final);

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
