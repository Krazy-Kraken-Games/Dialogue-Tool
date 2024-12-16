using UnityEngine;

namespace KKG.FileHandling
{
    public class CSVReader : MonoBehaviour
    {
        public string filePath;

        private void Start()
        {
            string csvText = LoadCSV(filePath);

            if (!string.IsNullOrEmpty(csvText))
            {
                ParseCSV(csvText);
            }
        }

        private string LoadCSV(string fileName)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(fileName);

            if(csvFile != null)
            {
                return csvFile.text;
            }
            else
            {
                Debug.LogError($"CSV file not found at: {fileName}");
                return null;
            }
        }

        private void ParseCSV(string csvText)
        {
            string[] rows = csvText.Split('\n');

            foreach(string row in rows)
            {
                if(string.IsNullOrEmpty(row)) continue;

                string[] fields = row.Split(',');

                foreach(string field in fields)
                {
                    Debug.Log(field.Trim());
                }
            }
        }
    }
}
