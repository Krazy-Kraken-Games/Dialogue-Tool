using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KKG.FileHandling
{
    public class CSVReader
    {
        public string filePath;


        [ContextMenu("Read File")]
        public void ReadFile()
        {
            string csvText = LoadByFilePath();

            if (!string.IsNullOrEmpty(csvText))
            {
                List<string[]> parsedData = ParseCSV(csvText);

                foreach (var row in parsedData)
                {
                    Debug.Log(string.Join(" |", row));
                }
            }
        }

        public List<string[]> LoadFileViaPath(string _filePath)
        {
            filePath = _filePath;

            List<string[]> result = new List<string[]>();

            string csvText = LoadByFilePath();

            if (!string.IsNullOrEmpty(csvText))
            {
                result = ParseCSV(csvText);
            }

            return result;
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

        private string LoadByFilePath()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    return fileContent;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load CSV file: {e.Message}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"File not found at: {filePath}");
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
