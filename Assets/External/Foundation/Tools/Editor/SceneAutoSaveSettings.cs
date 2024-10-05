using UnityEditor;

namespace Foundation
{
    [FilePath("/UserSettings/AutoSaveSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SceneAutoSaveSettings : ScriptableSingleton<SceneAutoSaveSettings>
    {
        public bool Enabled = true;
        public int Timeout = 1;

        public void SaveChanges() => Save(true);
    }
}