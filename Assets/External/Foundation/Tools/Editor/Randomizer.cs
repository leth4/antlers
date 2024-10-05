using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Foundation.Tools
{
    public class Randomizer : EditorWindow
    {
        [MenuItem("Tools/Randomizer")]
        public static void ShowWindow() => GetWindow<Randomizer>();

        private bool _angleToggle = false;
        private bool _positionToggle = false;
        private bool _scaleToggle = false;

        private bool _mirrorAngles = true;
        private bool _lerpBetween = true;
        private bool _singularScale = true;
        private bool _absolutePosition = false;

        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private Vector3 _minRotation;
        private Vector3 _maxRotation;
        private Vector3 _minScale;
        private Vector3 _maxScale;
        private Color _minColor;
        private Color _maxColor;

        Vector2 _scrollPosition = Vector2.zero;
        bool _settingsFoldout = false;

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0 || !SomethingToggled()))
                if (GUILayout.Button("Randomize Selection"))
                    RandomizeSelection();

            ShowValueFields();

            GUILayout.Space(10);

            _settingsFoldout = EditorGUILayout.Foldout(_settingsFoldout, "Settings");
            if (_settingsFoldout)
            {
                _mirrorAngles = EditorGUILayout.ToggleLeft("Rotate both ways", _mirrorAngles);
                _lerpBetween = EditorGUILayout.ToggleLeft("Lerp between values", _lerpBetween);
                _singularScale = EditorGUILayout.ToggleLeft("Fixed scale proportions", _singularScale);
                _absolutePosition = EditorGUILayout.ToggleLeft("Position is absolute", _absolutePosition);
            }

            GUILayout.EndScrollView();
        }

        private void ShowValueFields()
        {
            _positionToggle = EditorGUILayout.ToggleLeft("Position", _positionToggle);
            if (_positionToggle)
            {
                CustomVector3Field(ref _minPosition);
                CustomVector3Field(ref _maxPosition);
            }

            _angleToggle = EditorGUILayout.ToggleLeft("Angle", _angleToggle);
            if (_angleToggle)
            {
                if (!_mirrorAngles)
                    CustomVector3Field(ref _minRotation);
                CustomVector3Field(ref _maxRotation);
            }

            _scaleToggle = EditorGUILayout.ToggleLeft("Scale", _scaleToggle);
            if (_scaleToggle)
            {
                if (_singularScale)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Min");
                    _minScale.x = EditorGUILayout.FloatField(_minScale.x, GUILayout.MinWidth(10));
                    GUILayout.Label("Max");
                    _maxScale.x = EditorGUILayout.FloatField(_maxScale.x, GUILayout.MinWidth(10));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    CustomVector3Field(ref _minScale);
                    CustomVector3Field(ref _maxScale);
                }
            }
        }

        private void RandomizeSelection()
        {
            Undo.RecordObjects(GetSelectionTransforms(), "Randomized Selection");
            foreach (var obj in Selection.gameObjects)
            {
                if (_positionToggle)
                {
                    var randomPosition = RandomVector3(_minPosition, _maxPosition, _lerpBetween);
                    if (_absolutePosition)
                        obj.transform.position = randomPosition;
                    else
                        obj.transform.position += randomPosition;
                }
                if (_angleToggle)
                {
                    var rotation = obj.transform.rotation;
                    if (_mirrorAngles)
                        rotation.eulerAngles += RandomVector3(-_maxRotation, _maxRotation, _lerpBetween);
                    else
                        rotation.eulerAngles += RandomVector3(_minRotation, _maxRotation, _lerpBetween);
                    obj.transform.rotation = rotation;
                }
                if (_scaleToggle)
                {
                    if (_singularScale)
                    {
                        float scale = 0;
                        if (_lerpBetween)
                            scale = Random.Range(_minScale.x, _maxScale.y);
                        else
                            scale = (Random.Range(0, 2) == 0) ? _minScale.x : _maxScale.y;
                        obj.transform.localScale = new Vector3(scale, scale, scale);
                    }
                    else
                    {
                        obj.transform.localScale = RandomVector3(_minScale, _maxScale, _lerpBetween);
                    }
                }
            }
        }

        private void CustomVector3Field(ref Vector3 vector)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("X");
            vector.x = EditorGUILayout.FloatField(vector.x, GUILayout.MinWidth(10));
            GUILayout.Label("Y");
            vector.y = EditorGUILayout.FloatField(vector.y, GUILayout.MinWidth(10));
            GUILayout.Label("Z");
            vector.z = EditorGUILayout.FloatField(vector.z, GUILayout.MinWidth(10));
            GUILayout.EndHorizontal();
        }

        private bool SomethingToggled() => (_angleToggle || _scaleToggle || _positionToggle);

        private Transform[] GetSelectionTransforms()
        {
            var transforms = new Transform[Selection.gameObjects.Length];
            for (int i = 0; i < transforms.Length; i++)
                transforms[i] = Selection.gameObjects[i].transform;
            return transforms;
        }

        private Vector3 RandomVector3(Vector3 min, Vector3 max, bool lerp)
        {
            if (lerp)
                return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            else
                return (Random.Range(0, 2) == 0) ? min : max;
        }

        private void OnDisable() => Selection.selectionChanged -= Repaint;

        private void OnEnable() => Selection.selectionChanged += Repaint;
    }
}