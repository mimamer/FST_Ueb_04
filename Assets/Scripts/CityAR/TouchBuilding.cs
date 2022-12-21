
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class TouchBuilding : MonoBehaviour, IMixedRealityTouchHandler
{
    public void OnTouchStarted(HandTrackingInputEventData eventData){
        gameObject.GetComponent<MeshRenderer>().material.color= new Color(Random.value, Random.value, Random.value);
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData){

    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData){

    }
    

}
