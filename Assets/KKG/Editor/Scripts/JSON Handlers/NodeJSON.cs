using KKG.Dialogue;
using KKG.Tool.Dialogue;
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
        public List<NodeData> Nodes = new List<NodeData>();


        [JsonConstructor]
        public NodePacket(List<DialogueTreeNode> nodes)
        {
            //Save the nodes
            foreach (var node in nodes)
            {
                NodeData nd = new NodeData(node);
                Nodes.Add(nd);
            }
        }
    }

    /// <summary>
    /// The class will be the serializable format of node data that will be stored in the JSON format on device
    /// </summary>
    /// 

    public static class NodeJSON
    {
        public static NodePacket packet;

        public static async Task<bool> SaveDataToJSON(string path,int x, int y, List<DialogueTreeNode> nodes)
        {
            try
            {
                packet = new NodePacket(nodes);

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

            NodePacket packet = JsonUtility.FromJson<NodePacket>(jsonData);

            return packet;
        }
    }
}
