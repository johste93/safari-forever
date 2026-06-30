using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailNodeManager : MonoBehaviour
{
    public delegate void RailNodeEvent();
    public static RailNodeEvent HideNodes;

    public delegate void RailNodeTypeEvent(NodeType type, LevelEntity sourceEntity);
    public static RailNodeTypeEvent RevealNodes;
}
