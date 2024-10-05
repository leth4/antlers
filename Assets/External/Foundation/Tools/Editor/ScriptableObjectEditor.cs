using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Foundation.Tools
{
    public class ScriptableObjectEditor : EditorWindow
    {
        private List<Type> _scriptableObjectTypes;

        private Type _selectedType;
        private List<Type> _selectedSubtypes;

        private Type _newAssetType;
        private string _newAssetFolder = "Assets";
        private string _newAssetName;

        private Vector2 _scrollPosition;
        private bool _isTableView;
        private float _tableElementWidth;

        private Dictionary<string, bool> _propertyDisplayedDictionary = new();
        private Dictionary<string, bool> _objectFoldedDictionary = new();
        private Dictionary<string, bool> _propertyFoldedDictionary = new();

        [MenuItem("Tools/Scriptable Object Editor")]
        public static void ShowWindow() => GetWindow<ScriptableObjectEditor>("Scriptable Object Editor");

        private void OnEnable()
        {
            FindScriptableObjectTypes();
            if (_selectedType != null) HandleTypeSelected();
        }

        private void FindScriptableObjectTypes()
        {
            _scriptableObjectTypes = new();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(CreateAssetMenuAttribute), true).Length > 0)
                    {
                        _scriptableObjectTypes.Add(type);
                        if (type.BaseType != typeof(ScriptableObject)) _scriptableObjectTypes.Add(type.BaseType);
                    }
                }
            }

            if (_scriptableObjectTypes.Count > 0) _selectedType = _scriptableObjectTypes[0];
        }

        private void OnGUI()
        {
            CalculateTableWidth();

            EditorGUILayout.Space(5);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.BeginHorizontal();

            DrawTypeSelector();
            if (GUILayout.Button(_isTableView ? "Switch to List View" : "Switch to Table View")) _isTableView = !_isTableView;

            EditorGUILayout.EndHorizontal();

            if (_selectedType == null)
            {
                EditorGUILayout.EndScrollView();
                return;
            }

            EditorGUILayout.Space(5);

            if (_isTableView) DrawTableHeader();

            foreach (var path in GetInstancesPaths(_selectedType.ToString()))
            {
                if (_isTableView) DrawObjectInTable(path);
                else DrawObjectInList(path);
            }

            EditorGUILayout.Space(5);

            DrawNewObjectCreator();

            EditorGUILayout.EndScrollView();
        }

        private void DrawTypeSelector()
        {
            var objectTypesNames = new string[_scriptableObjectTypes.Count];
            for (int i = 0; i < _scriptableObjectTypes.Count; i++) objectTypesNames[i] = _scriptableObjectTypes[i].Name;
            EditorGUI.BeginChangeCheck();
            _selectedType = _scriptableObjectTypes[EditorGUILayout.Popup(_scriptableObjectTypes.IndexOf(_selectedType), objectTypesNames)];
            if (EditorGUI.EndChangeCheck()) HandleTypeSelected();

            MakeClickable(GUILayoutUtility.GetLastRect(), null, () => ShowSelectorContextMenu());
        }

        private void CalculateTableWidth()
        {
            var tableElements = 0;
            foreach (var property in _propertyDisplayedDictionary) if (property.Value) tableElements++;
            _tableElementWidth = Mathf.Min(200, (position.width - 60) / (tableElements + 1));
        }

        private void DrawNewObjectCreator()
        {
            EditorGUILayout.BeginHorizontal();

            if (_selectedSubtypes.Count > 1)
            {
                var subtypesNames = new string[_selectedSubtypes.Count - 1];
                for (int i = 1; i < _selectedSubtypes.Count; i++) subtypesNames[i - 1] = _selectedSubtypes[i].Name;
                EditorGUI.BeginChangeCheck();
                _newAssetType = _selectedSubtypes[EditorGUILayout.Popup(_selectedSubtypes.IndexOf(_newAssetType) - 1, subtypesNames, GUILayout.MaxWidth(150)) + 1];
                if (EditorGUI.EndChangeCheck()) _newAssetName = _newAssetType.Name;
            }

            if (GUILayout.Button(_newAssetFolder + "/", GUILayout.Width(EditorStyles.label.CalcSize(new(_newAssetFolder)).x + 20)))
            {
                var openFolder = EditorUtility.OpenFolderPanel("Save to", _newAssetFolder, "");
                if (openFolder.Contains("Assets/"))
                {
                    _newAssetFolder = openFolder.Substring(openFolder.IndexOf("Assets/"));
                }
                else
                {
                    Debug.LogWarning("Please select a path within the project folder.");
                }
            }

            _newAssetName = EditorGUILayout.TextField(_newAssetName);

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), GUILayout.MaxWidth(100)))
            {
                var newAsset = ScriptableObject.CreateInstance(_newAssetType);

                var newPath = $"{_newAssetFolder}/{_newAssetName}.asset";
                AssetDatabase.CreateAsset(newAsset, AssetDatabase.GenerateUniqueAssetPath(newPath));
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void HandleTypeSelected()
        {
            _selectedSubtypes = new() { _selectedType };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(_selectedType))
                    {
                        _selectedSubtypes.Add(type);
                    }
                }
            }

            _propertyDisplayedDictionary = new();

            foreach (var type in _selectedSubtypes)
            {
                if (type.IsAbstract) continue;
                var objectInstance = ScriptableObject.CreateInstance(type);
                var serializedObject = new SerializedObject(objectInstance);
                var iterator = serializedObject.GetIterator();
                iterator.NextVisible(true);
                while (iterator.NextVisible(false))
                {
                    _propertyDisplayedDictionary.TryAdd(iterator.name, true);
                }
            }

            _newAssetType = _selectedSubtypes.Count > 1 ? _selectedSubtypes[1] : _selectedType;
            _newAssetName = _selectedType.Name;
        }

        private void ShowSelectorContextMenu()
        {
            var menu = new GenericMenu();

            foreach (var property in new List<string>(_propertyDisplayedDictionary.Keys))
            {
                menu.AddItem(new GUIContent(property), _propertyDisplayedDictionary[property], () => _propertyDisplayedDictionary[property] = !_propertyDisplayedDictionary[property]);
            }

            menu.ShowAsContext();
        }

        private void ApplyFoldToAllProperties(bool fold)
        {
            foreach (var property in new List<string>(_objectFoldedDictionary.Keys))
            {
                _objectFoldedDictionary[property] = fold;
            }
        }

        private void DrawObjectInList(string path)
        {
            EditorGUILayout.BeginVertical("HelpBox");

            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            var guid = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();

            Undo.RecordObject(obj, "Edited Scriptable Object");

            EditorGUILayout.BeginHorizontal();

            _objectFoldedDictionary.TryAdd(guid, false);
            _objectFoldedDictionary[guid] = EditorGUILayout.Toggle(_objectFoldedDictionary[guid], "foldout", GUILayout.Width(15));
            EditorGUILayout.LabelField(obj.name, GUILayout.Width(EditorStyles.label.CalcSize(new(obj.name)).x));

            var labelRect = GUILayoutUtility.GetLastRect();
            labelRect.width = position.width - labelRect.x - 30;
            MakeClickable(labelRect, () => _objectFoldedDictionary[guid] = !_objectFoldedDictionary[guid], () => ApplyFoldToAllProperties(!_objectFoldedDictionary[guid]));

            var typeLabelStyle = new GUIStyle("miniBoldLabel");
            typeLabelStyle.normal.textColor = Color.gray;
            EditorGUILayout.LabelField(_selectedSubtypes.Count > 1 ? $"({obj.GetType().Name})" : "", typeLabelStyle);

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_scenepicking_pickable_hover"), GUILayout.Width(25))) Selection.activeObject = obj;

            EditorGUILayout.EndHorizontal();

            var serializedObject = new SerializedObject(obj);
            var iterator = serializedObject.GetIterator();

            if (!_objectFoldedDictionary[guid])
            {
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUI.BeginChangeCheck();

            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                if (iterator.name == "m_Script") continue;
                if (!_propertyDisplayedDictionary[iterator.name]) continue;
                if (iterator.isArray && iterator.propertyType != SerializedPropertyType.String)
                {
                    DrawArray(iterator);
                    continue;
                }
                EditorGUILayout.PropertyField(iterator);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTableHeader()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(30);

            EditorGUILayout.LabelField("Asset Name", EditorStyles.boldLabel, GUILayout.Width(_tableElementWidth));

            foreach (var propertyDisplayed in _propertyDisplayedDictionary)
            {
                if (propertyDisplayed.Key == "m_Script") continue;
                if (!propertyDisplayed.Value) continue;
                EditorGUILayout.LabelField(propertyDisplayed.Key, EditorStyles.boldLabel, GUILayout.Width(_tableElementWidth));
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawObjectInTable(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj == null) return;

            var serializedObject = new SerializedObject(obj);

            Undo.RecordObject(obj, "Edited Scriptable Object");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_scenepicking_pickable_hover"), GUILayout.Width(25))) Selection.activeObject = obj;
            EditorGUILayout.LabelField(obj.name, GUILayout.Width(_tableElementWidth));

            foreach (var propertyDisplayed in _propertyDisplayedDictionary)
            {
                if (!propertyDisplayed.Value) continue;
                var property = serializedObject.FindProperty(propertyDisplayed.Key);
                if (property == null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("", GUILayout.Width(_tableElementWidth));
                    EditorGUI.EndDisabledGroup();
                    continue;
                }
                if (property.isArray && property.propertyType != SerializedPropertyType.String)
                {
                    DrawArray(property);
                    continue;
                }
                EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.Width(_tableElementWidth));
            }

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawArray(SerializedProperty arrayProperty)
        {
            EditorGUIUtility.labelWidth = 15;

            if (_isTableView) EditorGUILayout.BeginVertical("HelpBox", GUILayout.Width(_tableElementWidth));
            else EditorGUILayout.BeginVertical("HelpBox");

            var propertyName = $"{GlobalObjectId.GetGlobalObjectIdSlow(arrayProperty.serializedObject.targetObject)}_{arrayProperty.name}";

            if (!_isTableView)
            {
                _propertyFoldedDictionary.TryAdd(propertyName, false);

                EditorGUILayout.BeginHorizontal();
                _propertyFoldedDictionary[propertyName] = EditorGUILayout.Toggle(_propertyFoldedDictionary[propertyName], "foldout", GUILayout.Width(15));
                EditorGUILayout.LabelField(arrayProperty.name);
                var labelRect = GUILayoutUtility.GetLastRect();
                labelRect.width = position.width;
                MakeClickable(labelRect, () => _propertyFoldedDictionary[propertyName] = !_propertyFoldedDictionary[propertyName]);
                EditorGUILayout.EndHorizontal();
            }

            if (_isTableView || _propertyFoldedDictionary[propertyName])
            {
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(arrayProperty.GetArrayElementAtIndex(i), new GUIContent($"{i}"));
                    if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(30)))
                    {
                        arrayProperty.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus")))
                {
                    arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = 0;
        }

        private void MakeClickable(Rect rect, Action onClick, Action onRightClick = null)
        {
            var evt = Event.current;

            if (rect.Contains(Event.current.mousePosition) && Event.current.type is EventType.MouseDown)
            {
                if (Event.current.button == 0) onClick?.Invoke();
                if (onRightClick != null && Event.current.button == 1) onRightClick?.Invoke();

                Repaint();
            }
        }

        private string[] GetInstancesPaths(string typeName)
        {
            var guids = AssetDatabase.FindAssets($"t:{typeName}");
            var paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            return paths;
        }
    }
}