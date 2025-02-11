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
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            json = json.Trim(new char[] { '\uFEFF', '\u200B' });
            treeData = JsonConvert.DeserializeObject<DialogueToolTreeData>(json);

            return treeData;
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
}
