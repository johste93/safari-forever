using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayGateExit : MonoBehaviour
{
    public OneWayGate oneWayGate;

    public void OnTriggerExit2D(Collider2D other)
	{
		if( !GameMaster.instance.IsPlaying() )
			return;

		if(other.isTrigger)
			return;

		if( oneWayGate.entity.inputNode.HasConnections() )
			return;
		
		this.Delay(0.1f / (SaveManager.currentSave.gameSpeed + 0.1f), ()=>{
			oneWayGate.Close();
		});
	}
}
