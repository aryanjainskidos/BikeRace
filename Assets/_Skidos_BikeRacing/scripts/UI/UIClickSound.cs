namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIClickSound : MonoBehaviour, IPointerClickHandler
{

    public string soundName = "Click";

    public void OnPointerClick(PointerEventData eventData)
    {

        SoundManager.Play(soundName);

    }

}

}
