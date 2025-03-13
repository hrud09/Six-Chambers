using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClearData : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameDevProdigy/Clear Data")]
#endif
    public static void DeleteFiles()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            try
            {
                // Delete all files in the directory
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }

                // Delete all subdirectories and their contents
                foreach (string directory in Directory.GetDirectories(path))
                {
                    Directory.Delete(directory, true);
                }

                Debug.Log("All saved data cleared successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to clear saved data: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("PersistentDataPath does not exist, nothing to delete.");
        }

        // Clear PlayerPrefs and ensure it's saved
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}
