using KKG.Dialogue;
using KKG.Tool.Dialogue;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Unity.Plastic.Newtonsoft.Json;


[Serializable]
public class DialogueToolTreeData
{
    public List<NodeData> nodes;
    public NodeData startingNodeData;
    public Dictionary<ConnectionTuple, Connection> connections;
    public Dictionary<ConnectionOptionTuple, Connection> connectionOptions;

    //Turning the dictionary to list for proper working serializing
    public List<Connection> storedConnections;
    public List<Connection> storedConnectionOptions;

    public DialogueToolTreeData()
    {
        nodes = new List<NodeData>();
        connections = new Dictionary<ConnectionTuple, Connection>();
        connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();
    }

    public DialogueToolTreeData(List<NodeData> _nodes, Dictionary<ConnectionTuple, Connection> _connections, Dictionary<ConnectionOptionTuple, Connection> _connectionOptions)
    {
        nodes = new List<NodeData>();
        connections = new Dictionary<ConnectionTuple, Connection>();
        connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();

        foreach(var _node in _nodes)
        {
            nodes.Add(_node);
        }

        foreach(var _connection in _connections)
        {
            connections.Add(_connection.Key, _connection.Value);
        }

        foreach(var _option in connectionOptions)
        {
            connectionOptions.Add(_option.Key, _option.Value);
        }
    }
}

public class DialogueJSONGetter
{
    [SerializeField]
    private List<NodeData> nodes;
    private Dictionary<ConnectionTuple, Connection> connections;
    private Dictionary<ConnectionOptionTuple, Connection> connectionOptions;

    [SerializeField]
    private DialogueToolTreeData treeData;

    public List<NodeData> Nodes => treeData.nodes;

    public Dictionary<ConnectionTuple, Connection> Connections => treeData.connections;

    public Dictionary<ConnectionOptionTuple, Connection> ConnectionOptions => treeData.connectionOptions;

    public DialogueJSONGetter()
    {
        treeData = new DialogueToolTreeData();
    }

    public DialogueJSONGetter(List<NodeData> _nodes, Dictionary<ConnectionTuple, Connection> _connections, Dictionary<ConnectionOptionTuple, Connection> _connectionOptions)
    {

        treeData = new DialogueToolTreeData(_nodes, _connections,_connectionOptions);
    }

    public DialogueToolTreeData LoadDataFromPath(string path)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            Debug.Log("Loaded JSON:" + jsonText);

            JsonTextReader reader = new JsonTextReader(new StringReader(jsonText));
            using (reader)
            {
                treeData = ParseDialogueTree(reader);
                return treeData;
            }
        }


        return null;
    }

    public void SaveData(string path)
    {
        // Serialize to JSON and save to file
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };


        string json = JsonConvert.SerializeObject(treeData, Formatting.Indented, settings);
        File.WriteAllText(path, json);
    }

    public void UpdateData(List<NodeData> _nodes, Dictionary<ConnectionTuple, Connection> _connections, Dictionary<ConnectionOptionTuple, Connection> _connectionOptions)
    {
        treeData.nodes = _nodes;
        treeData.connections = _connections;
        treeData.connectionOptions = _connectionOptions;
    }



    #region CUSTOM JSON PARSER
    private static DialogueToolTreeData ParseDialogueTree(JsonTextReader reader)
    {
        DialogueToolTreeData treeData = new DialogueToolTreeData();
        treeData.nodes = new List<NodeData>();

        NodeData currentNode = null;
        DialogueNodeData currentData = new DialogueNodeData();
        string propertyName = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                propertyName = reader.Value.ToString();
            }
            else if (reader.TokenType == JsonToken.StartObject && propertyName == "nodes")
            {
                //currentNode = new NodeData();
                currentData = new DialogueNodeData();
            }
            else if (reader.TokenType == JsonToken.EndObject && currentNode != null)
            {
                currentNode.data = currentData;
                treeData.nodes.Add(currentNode);
                currentNode = null;
            }
            else if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                float value = float.Parse(reader.Value.ToString());

                if (propertyName == "x") currentNode.Position.x = value;
                if (propertyName == "y") currentNode.Position.y = value;
                if (propertyName == "Size_x") currentNode.Size.x = value;
                if (propertyName == "Size_y") currentNode.Size.y = value;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value.ToString();

                if (propertyName == "Id") currentData.Id = value;
                if (propertyName == "SpeakerName") currentData.SpeakerName = value;
                if (propertyName == "Message") currentData.Message = value;
                if (propertyName == "nextIndex") currentData.nextIndex = value;
            }
        }

        return treeData;
    }

    #endregion
}
