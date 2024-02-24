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
						FieldInfo[] fields = script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

						for (int j = 0; j < fields.Length; j++)
						{
							FieldInfo field = fields[j];
							SceneScriptField? sceneScriptField = selectedEntity.Scripts[i].Fields?.Find((scripField) => scripField.Name == field.Name);
							SceneScriptValue? sceneScriptValue = sceneScriptField?.Value;

							ImGui.PushID(field.Name);

							if (IsInClassList(field.FieldType, typeof(string), typeof(int),
								typeof(float), typeof(double), typeof(uint), typeof(byte), typeof(sbyte),
								typeof(short), typeof(ushort)))
							{
								string input = string.Empty;

								if (fields[j] != null)
								{
									object? result = fields[j].GetValue(script);
									if (result != null)
										input = result.ToString();
								}
								else if (sceneScriptValue != null)
								{
									input = sceneScriptValue.Value.ToString();
								}

								if (input == null)
									input = string.Empty;

								if (IsInClassList(field.FieldType, typeof(string)))
								{
									if (input != string.Empty)
										input = input.Substring(1, input.Length - 2);
								}
								else
									input = input.Replace(".", ",");

								if (ImGui.InputText(field.Name, ref input, 128))
								{
									if (input == null)
										continue;

									if (input == string.Empty && IsInClassList(field.FieldType, typeof(string)) == false)
										continue;

									object? final = null;
									if (field.FieldType != typeof(string))
									{
										if (double.TryParse(input, out double result))
										{
											final = Convert.ChangeType(result, field.FieldType);
										}
									}
									else
										final = input;

									if (final != null)
										UpdateField(field, script, sceneScript, final);
								}
							}
							else if (IsInClassList(field.FieldType, typeof(bool)))
							{
								bool input = (bool)field.GetValue(script);
								bool result = input;

								if (ImGui.Checkbox(field.Name, ref result))
								{
									UpdateField(field, script, sceneScript, result);
								}
							}
							else if (IsInClassList(field.FieldType, typeof(Vector3)))
							{
								string x = "0";
								string y = "0";
								string z = "0";
								Vector3 initial = new Vector3(0, 0, 0);

								if (fields[j] != null)
								{
									object? result = fields[j].GetValue(script);
									if (result != null)
									{
										initial = (Vector3)result;
										x = initial.x.ToString();
										y = initial.y.ToString();
										z = initial.z.ToString();
									}
								}
								else if (sceneScriptValue != null)
								{
									initial = JsonSerializer.Deserialize<Vector3>(sceneScriptValue.Value);
									x = initial.x.ToString();
									y = initial.y.ToString();
									z = initial.z.ToString();
								}

								ImGui.Text(field.Name);
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
							else if (IsInClassList(field.FieldType, typeof(Quaternion)))
							{
								string x = "0";
								string y = "0";
								string z = "0";
								string w = "0";

								Quaternion initial = new Quaternion(0, 0, 0, 0);

								if (sceneScriptValue != null)
								{
									initial = JsonSerializer.Deserialize<Quaternion>(sceneScriptValue.Value);
									x = initial.x.ToString();
									y = initial.y.ToString();
									z = initial.z.ToString();
									w = initial.w.ToString();
								}
								else
								{
									object? result = fields[j].GetValue(script);
									if (result != null)
									{
										initial = (Quaternion)result;
										x = initial.x.ToString();
										y = initial.y.ToString();
										z = initial.z.ToString();
										w = initial.w.ToString();
									}
								}


								ImGui.Text(field.Name);
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
							else if (IsInClassList(field.FieldType, typeof(BEngine.Model)))
							{
								string input = string.Empty;

								if (fields[j] != null)
								{
									BEngine.Model? result = (BEngine.Model?)fields[j].GetValue(script);
									if (result != null)
										input = result.GUID;
								}
								else if (sceneScriptValue != null)
								{
									BEngine.Model? model = JsonSerializer.Deserialize<BEngine.Model>(sceneScriptValue.Value);
									if (model != null)
									{
										input = model.GUID;
									}
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
