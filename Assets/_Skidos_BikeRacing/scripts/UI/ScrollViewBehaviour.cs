namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollViewBehaviour : MonoBehaviour
{
    ScrollRect scrollRect;

    // Use this for initialization
    void Awake()
    {
        //scrollRect = transform.GetComponent<ScrollRect>();
        //        scrollRect.OnDrag.
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(BaseEventData ped)
    {
        print("---" + ((PointerEventData)ped).delta);
    }

    public void OnValueChanged(Vector2 vec2)
    {
        print("+++" + vec2);
    }
}
}
