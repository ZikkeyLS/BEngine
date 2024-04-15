using BEngine;
using BEngineCore;
using BEngineScripting;
using ImGuiNET;
using System.Reflection;

namespace BEngineEditor
{
	public class ScriptFieldData
	{
		public Dictionary<string, FieldData> Data = new();
	}

	public struct ScriptDragData
	{
		public SceneEntity entity;
		public SceneScript script;
	}

	public class PropertiesScreen : Screen
	{
		private bool _dragging = false;

		private string? _lastName = string.Empty;
		private ScriptFieldData? _copyScriptData = null;

		private CommonResolver _commonResolver = new();
		private BoolResolver _boolResolver = new();
		private Vector2Resolver _vector2Resolver = new();
		private Vector3Resolver _vector3Resolver = new();
		private Vector4Resolver _vector4Resolver = new();
		private QuaternionResolver _quaternionResolver = new();
		private ModelResolver _modelResolver = new();
		private KeyResolver _keyResolver = new();
		private MouseButtonResolver _mouseButtonResolver = new();
		private ColorResolver _colorResolver = new();

		private ProjectContext _projectContext => window.ProjectContext;
		private Scripting _scripting => _projectContext.CurrentProject.Scripting;
		private Scene _scene => _projectContext.CurrentProject.LoadedScene;
		private AssetReader _reader => _projectContext.CurrentProject.AssetsReader;
		private SelectedElement? _selectedElement => _projectContext.CurrentProject.SelectedElement;

		private const float RelevantOffset = 20f;
		private const float PaddingX = 10f;
		private const float PaddingY = 10f;
		private const string ScriptOrderPayload = "ScriptOrderPayload";

		public override unsafe void Display()
		{
			if (_selectedElement == null)
				return;

			ImGui.Begin("Properties", ImGuiWindowFlags.HorizontalScrollbar);

			if (_selectedElement.Type == ItemTypeSelected.Entity)
			{
				DisplayEntity(_selectedElement.Element);
			}

			ImGui.End();
		}

		public object? GetScriptValue(MemberInfo field, Script script)
		{
			if (field.MemberType == MemberTypes.Field)
				return ((FieldInfo)field).GetValue(script);
			else if (field.MemberType == MemberTypes.Property)
				return ((PropertyInfo)field).GetValue(script);

			return null;
		}

		public void UpdateField(MemberInfo field, Script script, SceneScript sceneScript, object final)
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

		private void DisplayEntity(object element)
		{
			SceneEntity selectedEntity = (SceneEntity)element;

			if (_projectContext.CurrentProject.LoadedScene.Entities.Contains(selectedEntity) == false)
			{
				_projectContext.CurrentProject.SelectedElement = null;
				return;
			}
			string newName = selectedEntity.Name;
			if (ImGui.InputText("Name", ref newName, 150))
			{
				if (newName == "" || newName == " ")
				{
					newName = "~";
				}

				selectedEntity.SetName(newName);
			}

			Vector4 colorMultiplied = System.Numerics.Vector4.Multiply(ColorConstants.HeaderColor, 256f);
			byte[] resultColor = [(byte)colorMultiplied.x, (byte)colorMultiplied.y, (byte)colorMultiplied.z, (byte)colorMultiplied.w];

			ImGui.Dummy(new System.Numerics.Vector2(0, PaddingY));

			for (int i = 0; i < selectedEntity.Scripts.Count; i++)
			{
				SceneScript sceneScript = selectedEntity.Scripts[i];
				Script? script = selectedEntity.GetScriptInstance(sceneScript);
				string fullName = sceneScript.Namespace + "." + sceneScript.Name;
				if (_dragging)
				{
					GUIPresets.CreateDropArea(i, PaddingY);
					Drop(sceneScript, i);
				}

				ImGui.PushID(fullName);

				ImGui.GetWindowDrawList().ChannelsSplit(2);
				ImGui.GetWindowDrawList().ChannelsSetCurrent(1);

				ImGui.Dummy(new System.Numerics.Vector2(0, PaddingY));

				ImGui.Dummy(new System.Numerics.Vector2(0, 0));
				ImGui.SameLine();
				ImGui.BeginGroup();

				if (sceneScript.Namespace == "BEngine")
					ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
				if (ImGui.Button(fullName, new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X - PaddingX, 40)))
				{
					if (sceneScript.Namespace != "BEngine")
					{
						_projectContext.CurrentProject?.OpenScriptFile(
							_projectContext.CurrentProject.AssetWatcher.FindFileWithClass(sceneScript.Name, sceneScript.Namespace));
					}
				}
				Drag(selectedEntity, sceneScript);
				if (sceneScript.Namespace == "BEngine")
					ImGui.PopStyleVar();


				if (script != null)
				{
					ShowScriptData(selectedEntity, sceneScript, script);
				}
				else
				{
					ShowResolverData(selectedEntity, sceneScript);
				}
				
				ImGui.EndGroup();
				ImGui.PopID();
				
				if (ImGui.BeginPopupContextItem(fullName, ImGuiPopupFlags.MouseButtonRight))
				{
					if (ImGui.Selectable("Copy"))
					{
						_copyScriptData = new ScriptFieldData()
						{
							Data = selectedEntity.CopyScriptData(selectedEntity.Scripts[i])
						};
					}
					if (_copyScriptData != null && ImGui.Selectable("Paste"))
					{
						selectedEntity.PasteScriptData(selectedEntity.Scripts[i], _copyScriptData.Data);
					}
					if (ImGui.Selectable("Reset"))
					{
						selectedEntity.ResetScript(selectedEntity.Scripts[i]);
					}
					if (ImGui.Selectable("Delete"))
					{
						selectedEntity.RemoveScript(selectedEntity.Scripts[i]);
					}
					ImGui.EndPopup();
				}

				ImGui.GetWindowDrawList().ChannelsSetCurrent(0);

				float windowX = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;
				System.Numerics.Vector2 minWindow = ImGui.GetItemRectMin() - new System.Numerics.Vector2(PaddingX, PaddingY);
				System.Numerics.Vector2 maxWindow = ImGui.GetItemRectMax() + new System.Numerics.Vector2(0, PaddingY);
				if (windowX > maxWindow.X)
				{
					maxWindow.X = windowX;
				}
				else
				{
					maxWindow.X += PaddingX;
					ImGui.SameLine();
					ImGui.Dummy(new System.Numerics.Vector2(PaddingX, 0));
				}
			
				ImGui.GetWindowDrawList().AddRect(minWindow, maxWindow, BitConverter.ToUInt32(resultColor, 0), 5.0f, ImDrawFlags.RoundCornersAll, 3.0f);
				ImGui.GetWindowDrawList().ChannelsMerge();
				
				ImGui.Dummy(new System.Numerics.Vector2(0, PaddingY));
			}

			if (selectedEntity.Scripts.Count != 0 && _dragging)
			{
				GUIPresets.CreateDropArea(selectedEntity.Scripts.Count, PaddingY);
				Drop(selectedEntity.Scripts.Last(), selectedEntity.Scripts.Count);
			}

			ImGui.Dummy(new System.Numerics.Vector2(0, PaddingY));
			ImGui.SetCursorPosX(ImGui.GetWindowSize().X / EditorGlobals.SizeOffset + RelevantOffset);
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

		private void ShowScriptData(SceneEntity selectedEntity, SceneScript sceneScript, Script script)
		{
			List<MemberInfo> fields = script.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance).
				ToList().FindAll((member) => member.MemberType == MemberTypes.Field | member.MemberType == MemberTypes.Property);

			for (int j = 0; j < fields.Count; j++)
			{
				MemberInfo field = fields[j];

				if (field.IsDefined(typeof(EditorShowAt)) == false)
					continue;

				foreach (object attribute in field.GetCustomAttributes(true))
				{
					if (attribute == null ||
						attribute is not EditorShowAt forceShow ||
						j == forceShow.Placement)
					{
						continue;
					}

					fields.Remove(field);
					fields.Insert(forceShow.Placement, field);
					break;
				}
			}

			for (int j = 0; j < fields.Count; j++)
			{
				MemberInfo field = fields[j];
				SceneScriptField? sceneScriptField = sceneScript.Fields?.Find((scripField) => scripField.Name == field.Name);
				SceneScriptValue? sceneScriptValue = sceneScriptField?.Value;
				object? resultField = GetScriptValue(field, script);

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
						if (attribute == null || attribute is not EditorName editorName)
							continue;

						fieldName = editorName.Name;
					}
				}

				ImGui.PushID(field.Name);

				ImGui.Text(fieldName);
				TypeResolver? resultResolver = null;

				if (TypeResolver.IsInClassList(fieldType, typeof(string), typeof(int),
					typeof(float), typeof(double), typeof(uint), typeof(byte), typeof(sbyte),
					typeof(short), typeof(ushort)))
				{
					resultResolver = _commonResolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(bool)))
				{
					resultResolver = _boolResolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(Vector2)))
				{
					resultResolver = _vector2Resolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(Vector3)))
				{
					resultResolver = _vector3Resolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(Vector4)))
				{
					resultResolver = _vector4Resolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(Quaternion)))
				{
					resultResolver = _quaternionResolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(BEngine.Model)))
				{
					resultResolver = _modelResolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(Key)))
				{
					resultResolver = _keyResolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(MouseButton)))
				{
					resultResolver = _mouseButtonResolver;
				}
				else if (TypeResolver.IsInClassList(fieldType, typeof(Color)))
				{
					resultResolver = _colorResolver;
				}

				resultResolver?.Resolve(new TypeResolver.ResolverData()
				{
					Field = field,
					FieldName = fieldName,
					FieldType = fieldType,
					Properties = this,
					ProjectContext = _projectContext,
					SceneScript = sceneScript,
					Script = script
				});

				ImGui.PopID();
			}
		}

		private void ShowResolverData(SceneEntity selectedEntity, SceneScript sceneScript)
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

						if (ImGui.Selectable(currentScript.Fullname)
							&& selectedEntity.RenameScript(sceneScript, currentScript))
						{
							ImGui.CloseCurrentPopup();
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

						if (ImGui.Selectable(currentScript.Fullname) == false)
							continue;

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
				ImGui.EndListBox();
				ImGui.EndPopup();
			}
		}

		private unsafe void Drag(SceneEntity entity, SceneScript script)
		{
			if (ImGui.BeginDragDropSource())
			{
				_dragging = true;
				ScriptDragData data = new ScriptDragData() { entity = entity, script = script };
				ImGui.SetDragDropPayload(ScriptOrderPayload, (IntPtr)(&data), (uint)sizeof(ScriptDragData));
				ImGui.Text("Script");
				ImGui.Text($"{script.Name} ({script.Namespace}.{script.Name})");
				ImGui.EndDragDropSource();
			}

			ImGuiPayloadPtr payload = ImGui.GetDragDropPayload();
			if ((payload.NativePtr == null && _dragging) ||
				(payload.NativePtr != null && payload.IsDataType(ScriptOrderPayload) == false && _dragging))
			{
				_dragging = false;
			}
		}

		private unsafe void Drop(SceneScript script, int i)
		{
			if (ImGui.BeginDragDropTarget())
			{
				var payload = ImGui.AcceptDragDropPayload(ScriptOrderPayload);

				if (payload.NativePtr != null)
				{
					_dragging = false;
					var entryPointer = (ScriptDragData*)payload.Data;
					ScriptDragData entry = entryPointer[0];

					int currentID = entry.entity.Scripts.IndexOf(entry.script);
					if (currentID != i)
					{
						entry.entity.Scripts.Insert(i, entry.script);
						if (currentID >= i)
							currentID += 1;
						entry.entity.Scripts.RemoveAt(currentID);
					}
				}

				ImGui.EndDragDropTarget();
			}
		}
	}
}
