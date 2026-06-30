using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxCollider2DExtensions
{
    struct rect
    {
        public float x;
        public float y;
        public float width;
        public float height;
    };

    public static bool Overlaps(this BoxCollider2D a, BoxCollider2D b)
    {
        rect A = new rect(){
            x = a.transform.position.x + a.offset.x - (a.size.x*0.5f),
            y = a.transform.position.y + a.offset.y - (a.size.y*0.5f),
            width = a.size.x,
            height = a.size.y
        };

        rect B = new rect(){
            x = b.transform.position.x + b.offset.x - (b.size.x*0.5f),
            y = b.transform.position.y + b.offset.y - (b.size.y*0.5f),
            width = b.size.x,
            height = b.size.y
        };

        bool xOverlap = valueInRange(A.x, B.x, B.x + B.width) ||
                        valueInRange(B.x, A.x, A.x + A.width);

        bool yOverlap = valueInRange(A.y, B.y, B.y + B.height) ||
                        valueInRange(B.y, A.y, A.y + A.height);

        return xOverlap && yOverlap;
    }
    

    private static bool valueInRange(float value, float min, float max)
    { 
        return (value > min) && (value < max);
    }
}
