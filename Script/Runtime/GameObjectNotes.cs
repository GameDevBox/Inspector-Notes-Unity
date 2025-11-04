using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameObjectNotes", menuName = "Editor/GameObject Notes")]
public class GameObjectNotes : ScriptableObject
{
    [System.Serializable]
    public class NoteEntry
    {
        public string gameObjectId;
        public string note;
    }

    public List<NoteEntry> notes = new List<NoteEntry>();
}