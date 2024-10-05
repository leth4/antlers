using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PinWindow : EditorWindow
{
    [MenuItem("Window/Pins")]
    public static void ShowWindow() => GetWindow<PinWindow>("Pins");

    private Vector2 _scrollPosition;
    private List<Object> _pins;

    private void OnEnable()
    {
        FillPins();

        wantsMouseMove = true;
        Selection.selectionChanged += Repaint;
        EditorSceneManager.activeSceneChanged += HandleSceneChanged;
        EditorSceneManager.activeSceneChangedInEditMode += HandleSceneChanged;
        EditorApplication.playModeStateChanged += HandlePlayModeChanged;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        EditorSceneManager.activeSceneChanged -= HandleSceneChanged;
        EditorSceneManager.activeSceneChangedInEditMode -= HandleSceneChanged;
        EditorApplication.playModeStateChanged -= HandlePlayModeChanged;
    }

    private void HandlePlayModeChanged(PlayModeStateChange state) => FillPins();
    private void HandleSceneChanged(Scene scene1, Scene scene2) => FillPins();

    private void FillPins()
    {
        _pins = new();
        if (PinnedObjects.instance == null || PinnedObjects.instance.PinsGUIDs == null) return;
        foreach (var GUID in PinnedObjects.instance.PinsGUIDs)
        {
            if (!GlobalObjectId.TryParse(GUID, out GlobalObjectId guidObject)) continue;
            var gameObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(guidObject);
            if (gameObject == null || gameObject is not Object) continue;
            _pins.Add(gameObject as Object);
        }
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        CreateDropArea(new Rect(0, 0, position.width, position.height));

        for (int i = 0; i < _pins.Count; i++)
        {
            var pin = _pins[i];

            if (_pins[i] == null) continue;

            var lineHeight = 16;

            var rect = new Rect(0, 4 + i * lineHeight, position.width, lineHeight);
            var iconRect = new Rect(rect.x + 4, rect.y, lineHeight, lineHeight);
            var labelRect = new Rect(rect.x + 4 + lineHeight, rect.y - 1, rect.width, lineHeight);

            if (Selection.activeObject == pin)
            {
                EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f) : new Color(.68f, .68f, .68f));
            }

            GUI.DrawTexture(iconRect, GetIcon(pin));
            EditorGUI.LabelField(labelRect, pin.name);

            GUILayout.Label(GUIContent.none, GUIStyle.none, GUILayout.Height(lineHeight));

            var evt = Event.current;

            if (rect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? new Color(.27f, .27f, .27f) : new Color(.6f, .6f, .6f));
                GUI.DrawTexture(iconRect, GetIcon(pin));
                EditorGUI.LabelField(labelRect, pin.name);

                if (Event.current.type is EventType.MouseUp)
                {
                    if (Event.current.button == 0) Selection.activeObject = pin;
                    if (Event.current.button == 1) ShowPinContextMenu(pin);
                }

                if (Event.current.type is EventType.MouseDrag)
                {
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.StartDrag("Dragging");
                    DragAndDrop.objectReferences = new Object[] { pin };
                }

                Repaint();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private Texture2D GetIcon(Object obj) => AssetPreview.GetMiniThumbnail(obj);

    private void ShowPinContextMenu(Object pin)
    {
        var menu = new GenericMenu();
        menu.AddItem(new("Open"), false, () =>
        {
            AssetDatabase.OpenAsset(pin);
        });
        menu.AddItem(new("Properties"), false, () =>
        {
            EditorUtility.OpenPropertyEditor(pin);
        });
        menu.AddSeparator("");
        menu.AddItem(new("Remove"), false, () =>
        {
            _pins.Remove(pin);
            PinnedObjects.instance.SaveChanges(_pins);
        });
        menu.ShowAsContext();
    }

    private void CreateDropArea(Rect rect)
    {
        Event evt = Event.current;
        if (evt.type is not EventType.DragPerform and not EventType.DragUpdated) return;
        if (!rect.Contains(evt.mousePosition)) return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (evt.type is not EventType.DragPerform) return;

        DragAndDrop.AcceptDrag();

        foreach (var draggedObject in DragAndDrop.objectReferences)
        {
            if (_pins.Contains(draggedObject)) continue;

            _pins.Add(draggedObject);
            PinnedObjects.instance.SaveChanges(_pins);
        }
    }
}