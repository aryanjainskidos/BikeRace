namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICloudMove : MonoBehaviour
{


    [Range(5, 500)]
    public float speed;


    RectTransform rectTransform;
    float x;



    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        x = rectTransform.anchoredPosition.x;
    }

    void Update()
    {

        x += speed * Time.unscaledDeltaTime;
        rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);

        if (x > 1024)
        {
            x -= 1024;
        }

    }
}

}
