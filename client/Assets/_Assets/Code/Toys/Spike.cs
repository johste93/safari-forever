using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spike : MonoBehaviour
{
    public Vector2Int postion {
		get{
			return (Vector2Int)(transform.position - (Vector3)Globals.gameConstants.tileOffset).CeilVector3ToInt();
		}
	}
}
