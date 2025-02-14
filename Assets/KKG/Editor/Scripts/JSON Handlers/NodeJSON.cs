using KKG.Dialogue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace KKG.Tool
{

    [Serializable]
    public class NodePacket
    {
        public int x;
        public int y;

        public Vector2 Position;

        public Dictionary<int, int> kvp = new Dictionary<int, int>();

        public List<NodeData> Nodes;


        public NodePacket(int _x, int _y)
        {
            x = _x;
            y = _y;

            Position = new Vector2(x, y);

            kvp.Add(x, y);
        }
    }

    /// <summary>
    /// The class will be the serializable struct of node data that will be stored in the JSON format on device
    /// </summary>
    /// 


    public static class NodeJSON
    {
        public static NodePacket packet;

        public static async Task<bool> SaveDataToJSON(string path,int x, int y)
        {
            try
            {
                packet = new NodePacket(x, y);

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };


                string json = JsonConvert.SerializeObject(packet, Formatting.Indented, settings);

                await File.WriteAllTextAsync(path, json);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static NodePacket LoadData(string path)
        {
            string jsonData = File.ReadAllText(path);
            NodePacket packet = JsonConvert.DeserializeObject<NodePacket>(jsonData);

            return packet;
        }
    }
}
