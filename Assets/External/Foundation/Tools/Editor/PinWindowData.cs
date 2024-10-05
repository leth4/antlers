using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[FilePath("/UserSettings/PinnedObjects.asset", FilePathAttribute.Location.ProjectFolder)]
public class PinnedObjects : ScriptableSingleton<PinnedObjects>
{
    public List<string> PinsGUIDs;

    public void SaveChanges(List<Object> pins)
    {
        PinsGUIDs = new();
        pins.ForEach(pin => PinsGUIDs.Add(GlobalObjectId.GetGlobalObjectIdSlow(pin).ToString()));
        Save(true);
    }
}