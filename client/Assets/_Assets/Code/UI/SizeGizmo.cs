using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SizeGizmo : MonoBehaviour
{
    public TextMeshPro textMesh;
    public Direction4 side;

    public void SetColor(Color c)
    {
        textMesh.color = c;
    }

    public void UpdateSize(int size)
    {
        textMesh.text = size.ToString();
    }
}
