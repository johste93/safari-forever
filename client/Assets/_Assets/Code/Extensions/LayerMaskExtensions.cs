using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }
    
    public static LayerMask AddLayer(this LayerMask layerMask, int layer)
    {
        return layerMask |= (1 << layer);
    }

    public static LayerMask CombineLayerMask(this LayerMask layerMask, LayerMask other)
    {
        return layerMask | other; // Or, (1 << layer1) | (1 << layer2)

        //layerMask |= (1<<layer);
    }

    public static LayerMask SubtractLayerMask(this LayerMask layerMask, LayerMask other)
    {
        return layerMask & ~other;
    }
}
