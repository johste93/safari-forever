using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawer : MonoBehaviour
{
    private static List<DrawShapeEvent> items;


    public static void DrawShape(Vector2 point, Vector2 size)
    {
        if(!Application.isEditor)
            return;

        items.Add(
            new DrawShapeEvent(){ 
                center = point, 
                size = size,
                shape = Shape.Box,
                time = Time.fixedTime
                }
        );
    }

    public static void DrawShape(BoxCollider2D collider2D)
    {
        if(!Application.isEditor)
            return;

        items.Add(
            new DrawShapeEvent(){ 
                center = (collider2D.transform.position + (Vector3)collider2D.offset), 
                size = collider2D.size,
                shape = Shape.Box,
                time = Time.fixedTime
                }
        );
    }

    private void OnDrawGizmos()
    {
        if(!Application.isEditor)
            return;

        if(items == null)
            return;
        
        var cache = new List<DrawShapeEvent>(items);
        foreach(DrawShapeEvent itemToDraw in cache)
        {
            if(itemToDraw.time < Time.fixedTime - Time.fixedDeltaTime)
            {
                items.Remove(itemToDraw);
                continue;
            }
            
            switch(itemToDraw.shape)
            {
                case Shape.Box:
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(itemToDraw.center, itemToDraw.size);
                break;
            }}     
    }

    private void OnEnable()
    {
        items = new List<DrawShapeEvent>();
    }

    private void OnDestroy()
    {
        items = null;
    }

    public class DrawShapeEvent
    {
        public Vector2 center;
        public Vector2 size;
        public DebugDrawer.Shape shape;
        public float time;
    }

    public enum Shape{
        Box
    }
}

