using UnityEditor;

namespace Foundation
{
    [FilePath("/UserSettings/HierarchySettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class HierarchySettings : ScriptableSingleton<HierarchySettings>
    {
        public bool ShowComponents = false;
        public bool ShowToggles = true;

        public void SaveChanges() => Save(true);
    }
}