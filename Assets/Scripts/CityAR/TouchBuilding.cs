
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class TouchBuilding : MonoBehaviour, IMixedRealityTouchHandler
{
    private GameObject display;
    public void OnTouchStarted(HandTrackingInputEventData eventData){
        display = GameObject.Find("Display");
        var text=display.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        text.GetComponent<TextMeshPro>().text="Hallo";
        gameObject.GetComponent<MeshRenderer>().material.color= new Color(Random.value, Random.value, Random.value);
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData){

    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData){

    }
    

}
