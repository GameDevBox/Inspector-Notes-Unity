using UnityEngine;
using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public static class GameObjectNoteInspector
{
    private static GameObjectNotes noteData;
    private const string NOTE_DATA_PATH = "Assets/Editor/GameObjectNotes.asset";

    static GameObjectNoteInspector()
    {
        Editor.finishedDefaultHeaderGUI += DrawNoteSection;
        LoadNoteData();
    }

    private static void LoadNoteData()
    {
        noteData = AssetDatabase.LoadAssetAtPath<GameObjectNotes>(NOTE_DATA_PATH);
        if (noteData == null)
        {
            noteData = ScriptableObject.CreateInstance<GameObjectNotes>();
            AssetDatabase.CreateAsset(noteData, NOTE_DATA_PATH);
            AssetDatabase.SaveAssets();
        }
    }

    private static void DrawNoteSection(Editor editor)
    {
        if (!(editor.target is GameObject gameObject)) return;

        string objectId = gameObject.GetInstanceID().ToString();
        var existingNote = noteData.notes.FirstOrDefault(n => n.gameObjectId == objectId);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUIContent noteIcon = EditorGUIUtility.IconContent("TextAsset Icon");

        if (GUILayout.Button(noteIcon, GUILayout.Width(25), GUILayout.Height(20)))
        {
            ShowNoteWindow(gameObject, existingNote?.note);
        }

        if (existingNote != null && !string.IsNullOrEmpty(existingNote.note))
        {
            EditorGUILayout.LabelField("Edit Note", EditorStyles.miniLabel);

            if (GUILayout.Button("×", GUILayout.Width(20)))
            {
                noteData.notes.Remove(existingNote);
                SaveNoteData();
            }
        }
        else
        {
            EditorGUILayout.LabelField("Add Note", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndHorizontal();

        // ✅ Full note box under the header
        if (existingNote != null && !string.IsNullOrEmpty(existingNote.note))
        {
            EditorGUILayout.HelpBox(existingNote.note, MessageType.None);
        }
    }

    private static void ShowNoteWindow(GameObject gameObject, string currentNote = "")
    {
        NotePopupWindow.ShowWindow(gameObject, currentNote);
    }

    private static void SaveNoteData()
    {
        EditorUtility.SetDirty(noteData);
        AssetDatabase.SaveAssets();
    }
}


public class NotePopupWindow : EditorWindow
{
    private GameObject targetObject;
    private string noteText;
    private Vector2 scrollPos;

    public static void ShowWindow(GameObject gameObject, string currentNote = "")
    {
        NotePopupWindow window = CreateInstance<NotePopupWindow>();
        window.targetObject = gameObject;
        window.noteText = currentNote ?? "";
        window.titleContent = new GUIContent("GameObject Note");
        window.minSize = new Vector2(350, 250);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(targetObject.name, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        noteText = EditorGUILayout.TextArea(noteText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
            SaveNote();

        if (GUILayout.Button("Close"))
            Close();
        EditorGUILayout.EndHorizontal();
    }

    private void SaveNote()
    {
        var noteData = AssetDatabase.LoadAssetAtPath<GameObjectNotes>("Assets/Editor/GameObjectNotes.asset");
        if (noteData == null) return;

        string objectId = targetObject.GetInstanceID().ToString();
        var existingNote = noteData.notes.FirstOrDefault(n => n.gameObjectId == objectId);

        if (string.IsNullOrEmpty(noteText))
        {
            if (existingNote != null) noteData.notes.Remove(existingNote);
        }
        else
        {
            if (existingNote != null) existingNote.note = noteText;
            else noteData.notes.Add(new GameObjectNotes.NoteEntry
            {
                gameObjectId = objectId,
                note = noteText
            });
        }

        EditorUtility.SetDirty(noteData);
        AssetDatabase.SaveAssets();
        Close();
    }
}
