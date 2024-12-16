using UnityEngine;

namespace KKG.FileHandling
{
    public class KrakenFileReader : MonoBehaviour
    {
        [SerializeField]
        private FileLocator fileLocator;

        [SerializeField]
        private CSVReader fileReader;

        [ContextMenu("File Read and Load")]
        public void ReadFile()
        {
            string filePath = fileLocator.SearchForFilePath();

            if (!string.IsNullOrEmpty(filePath))
            {
                fileReader.LoadFileViaPath(filePath);
            }
        }
    }
}
