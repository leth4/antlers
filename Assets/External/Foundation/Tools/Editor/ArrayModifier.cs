using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Foundation.Tools
{
    public class ArrayModifier : EditorWindow
    {
        [MenuItem("Tools/Array")]
        public static void ShowWindow() => GetWindow<ArrayModifier>("Array");

        private Vector3Int _amounts = Vector3Int.one;
        private Vector3 _distances = Vector3.zero;

        private void OnGUI()
        {
            _amounts = EditorGUILayout.Vector3IntField("Amounts", _amounts);
            _distances = EditorGUILayout.Vector3Field("Distances", _distances);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1);
            if (GUILayout.Button("Create"))
            {
                CreateArray();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length <= 2);
            if (GUILayout.Button("Auto-Align Selected"))
            {
                AutoAlignSelected();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CreateArray()
        {
            var selectedGameObject = Selection.activeGameObject;
            var gameObjects = new List<GameObject>() { selectedGameObject };

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Array Applied");
            var undoGroupIndex = Undo.GetCurrentGroup();

            for (int x = 0; x < _amounts.x; x++)
            {
                for (int y = 0; y < _amounts.y; y++)
                {
                    for (int z = 0; z < _amounts.z; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue;

                        var newGameObject = Instantiate(selectedGameObject, selectedGameObject.transform.parent);
                        newGameObject.name = selectedGameObject.name;
                        newGameObject.transform.localPosition = selectedGameObject.transform.localPosition + new Vector3(_distances.x * x, _distances.y * y, _distances.z * z);
                        gameObjects.Add(newGameObject);
                        Undo.RegisterCreatedObjectUndo(newGameObject, "");
                    }
                }
            }

            Undo.CollapseUndoOperations(undoGroupIndex);

            Selection.objects = gameObjects.ToArray();
        }

        private void AutoAlignSelected()
        {
            var transforms = GetSelectionTransforms();

            Undo.RecordObjects(transforms, "Objects Aligned");

            var minPosition = transforms[0].position;
            var maxPosition = transforms[0].position;

            for (int i = 1; i < transforms.Length; i++)
            {
                minPosition = Vector3.Min(minPosition, transforms[i].position);
                maxPosition = Vector3.Max(maxPosition, transforms[i].position);
            }

            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].position = minPosition + (maxPosition - minPosition) * i / (transforms.Length - 1);
            }
        }

        private Transform[] GetSelectionTransforms()
        {
            var transforms = new Transform[Selection.gameObjects.Length];
            for (int i = 0; i < transforms.Length; i++)
                transforms[i] = Selection.gameObjects[i].transform;
            return transforms;
        }

        private void OnEnable() => Selection.selectionChanged += Repaint;
        private void OnDisable() => Selection.selectionChanged -= Repaint;
    }
}