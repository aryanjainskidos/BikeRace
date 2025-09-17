namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class ColorReceiver : MonoBehaviour
{

    public string group = ""; //-1 - no group
    SpriteRenderer spriteRenderer;
    public Color32 color;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(Color32 color)
    {

        //		print(transform.name);
        //		color. = 1f;

        this.color = color;
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }
}

}
