using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KKG.FileHandling
{
    public class CSVReader : MonoBehaviour
    {
        public string filePath;


        [ContextMenu("Read File")]
        public void ReadFile()
        {
            string csvText = LoadCSV(filePath);

            if (!string.IsNullOrEmpty(csvText))
            {
                List<string[]> parsedData = ParseCSV(csvText);

                foreach (var row in parsedData)
                {
                    Debug.Log(string.Join(" |", row));
                }
            }
        }

        /// <summary>
        /// Loads the csv file into the system and reads it
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Contents of the file</returns>
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

        /// <summary>
        /// Parses the data from a csv file
        /// </summary>
        /// <param name="csvText"></param>
        /// <returns></returns>
        private List<string[]> ParseCSV(string csvText)
        {
            List<string[]> rows = new List<string[]>();

            StringReader reader = new StringReader(csvText);

            string line;
            while((line = reader.ReadLine()) != null)
            {
                rows.Add(ParseCSVLine(line));
            }

            return rows;
        }

        /// <summary>
        /// Parse each line of the file
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string[] ParseCSVLine(string line)
        {
            List<string> fields = new List<string>();
            bool insideQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    //Toggle the inside Quotes flag
                    if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField += '"';
                        i++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (c == ',' && !insideQuotes)
                {
                    fields.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            fields.Add(currentField);

            return fields.ToArray();
        }
    }
}
