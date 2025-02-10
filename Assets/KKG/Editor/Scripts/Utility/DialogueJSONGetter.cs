using KKG.Dialogue;
using KKG.Tool.Dialogue;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class DialogueToolTreeData
{
    public List<NodeData> nodes;
    public NodeData startingNodeData;
    public Dictionary<ConnectionTuple, Connection> connections;
    public Dictionary<ConnectionOptionTuple, Connection> connectionOptions;

    public DialogueToolTreeData()
    {
        nodes = new List<NodeData>();
        connections = new Dictionary<ConnectionTuple, Connection>();
        connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();
    }
}

public class DialogueJSONGetter
{
    [SerializeField]
    private List<NodeData> nodes;
    private Dictionary<ConnectionTuple, Connection> connections;
    private Dictionary<ConnectionOptionTuple, Connection> connectionOptions;

    [SerializeField]
    private DialogueToolTreeData treeData = new DialogueToolTreeData();

    public List<NodeData> Nodes => treeData.nodes;

    public Dictionary<ConnectionTuple, Connection> Connections => treeData.connections;

    public Dictionary<ConnectionOptionTuple, Connection> ConnectionOptions => treeData.connectionOptions;

    public DialogueToolTreeData LoadDataFromPath(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            treeData = JsonUtility.FromJson<DialogueToolTreeData>(json);

            return treeData;
        }

        return null;
    }

    public void SaveData(string path, DialogueTreeNode _start, List<DialogueTreeNode> _nodes,
            Dictionary<ConnectionTuple, Connection> _connections, Dictionary<ConnectionOptionTuple, Connection> _connectionOptions)
    {
        // Serialize to JSON and save to file
        string json = JsonUtility.ToJson(treeData, true);
        File.WriteAllText(path, json);
    }
}
