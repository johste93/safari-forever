using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public delegate void NodeEvent();
    public static NodeEvent HideNodes;

    public delegate void NodeTypeEvent(NodeType type, LevelEntity sourceEntity);
    public static NodeTypeEvent RevealNodes;
}
