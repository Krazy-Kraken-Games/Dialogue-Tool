
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace KKG.FileHandling
{
    public class FileLocator
    {
        public string SearchForFilePath()
        {
            string filePath = OpenFileLocator();

            if(!string.IsNullOrEmpty(filePath))
            {
                Debug.Log("Selected file: " + filePath);
                return filePath;
            }
            else
            {
                Debug.LogWarning("No file selected.");
                return string.Empty;
            }
        }

        private string OpenFileLocator()
        {
            string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            return path;
        }
    }
}

#endif
