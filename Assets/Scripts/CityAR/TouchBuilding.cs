
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using DefaultNamespace;

namespace CityAR
{
    public class TouchBuilding : MonoBehaviour, IMixedRealityTouchHandler
    {
        private GameObject display;
        static GameObject previous;
        public Entry entry;
        public void OnTouchStarted(HandTrackingInputEventData eventData){
            display = GameObject.Find("Display");
            var text=display.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            text.GetComponent<TextMeshPro>().text=entry.name;
            gameObject.GetComponent<MeshRenderer>().material.color= new Color(Random.value, Random.value, Random.value);
            if(previous!=null){
                previous.GetComponent<MeshRenderer>().material.color= new Color(1f,1f,1f);
            }
            previous=gameObject;
        }

        public void OnTouchUpdated(HandTrackingInputEventData eventData){

        }

        public void OnTouchCompleted(HandTrackingInputEventData eventData){

        }
        
    }
}
