using System;

namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelsImageLoadingBehaviour : MonoBehaviour
{

    List<Image> bgImages;
    ScrollRect scrollRect;

    //float imageSpan = 0.125f;//1/8

    float contHeight;
    float rectHeight;

    float imgindex;

    bool initialized = false;

    // Use this for initialization
    void Awake()
    {

        // imageSpan = 1.0f/7.0f;
        try
        {
            bgImages = new List<Image>();
            for (int i = 1; i < 9; i++)
            {
                bgImages.Add(transform.Find("Content/Background0" + i).GetComponent<Image>());
            }

            //scrollRect = GetComponent<ScrollRect>();
            //scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            for (int i = 0; i < 8; i++)
            {
                Sprite tmp = bgImages[i].sprite;
                bgImages[i].sprite  = LoadAddressable_Vasundhara.Instance.GetSprite_Resources("World_part_0" + (i + 1).ToString()); 

                //Resources.UnloadAsset(tmp);
            }

            initialized = true;
        }
        catch (Exception e)
        {
            Debug.Log("Exception in awake: "+e);
        }
        
    }

    void OnEnable()
    {
        try
        {
            contHeight = scrollRect.content.rect.height;
            rectHeight = scrollRect.GetComponent<RectTransform>().rect.height;

            //        print(contHeight);
            //        print(rectHeight);

            if (!initialized)
            {
                Awake();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception in enable: "+e);
        }
        

        //OnScrollValueChanged(Vector2.up);
        //OnScrollValueChanged(-Vector2.up);
    }

    [SerializeField]
    int currentBackground = 0;
    Vector2 lastValueChange;

    void OnScrollValueChanged(Vector2 arg0)
    {
        float imageIndex = ((contHeight - rectHeight) * scrollRect.verticalNormalizedPosition) / (contHeight / 8);
        currentBackground = Mathf.FloorToInt(imageIndex);

        if (bgImages.Count > currentBackground && currentBackground >= 0 && bgImages[currentBackground].sprite == null)
        {
            print("active is null");
            //bgImages[currentBackground].sprite = Resources.Load<Sprite>( "visuals/Sprites/GUI_sprites/Backgrounds/World_part_0" + (currentBackground+1).ToString() );
            bgImages[currentBackground].sprite = LoadAddressable_Vasundhara.Instance.GetSprite_Resources("World_part_0" + (currentBackground + 1).ToString());

            Debug.Log("<color=yellow>Sprite Loaded Name = </color>" + bgImages[currentBackground].sprite);
            if (currentBackground + 1 < bgImages.Count && bgImages[currentBackground + 1].sprite == null)
            {
                //bgImages[currentBackground+1].sprite = Resources.Load<Sprite>( "visuals/Sprites/GUI_sprites/Backgrounds/World_part_0" + (currentBackground+2).ToString() );
                bgImages[currentBackground + 1].sprite = LoadAddressable_Vasundhara.Instance.GetSprite_Resources("World_part_0" + (currentBackground + 2).ToString());
                Debug.Log("<color=yellow>Sprite Loaded Name = </color>" + bgImages[currentBackground + 1].sprite);
            }
            if (currentBackground - 1 >= 0 && bgImages[currentBackground - 1].sprite == null)
            {
                bgImages[currentBackground - 1].sprite = LoadAddressable_Vasundhara.Instance.GetSprite_Resources("World_part_0" + (currentBackground).ToString());
                Debug.Log("<color=yellow>Sprite Loaded Name = </color>" + bgImages[currentBackground - 1].sprite);
            }
            for (int i = 0; i < bgImages.Count; i++)
            {
                if (i < currentBackground - 1 || i > currentBackground + 1)
                {
                    Sprite tmp = bgImages[i].sprite;
                    bgImages[i].sprite = null;

                    Resources.UnloadAsset(tmp);
                }
            }
        }

        if (lastValueChange.y - arg0.y < 0)
        {//moving up
            if (currentBackground - 2 >= 0 && bgImages[currentBackground - 2].sprite != null)
            {
                //delete the lower background
                Sprite tmp = bgImages[currentBackground - 2].sprite;
                bgImages[currentBackground - 2].sprite = null;

                Resources.UnloadAsset(tmp);
            }
            if (currentBackground + 1 < bgImages.Count && bgImages[currentBackground + 1].sprite == null)
            {
                //add upper background
                //bgImages[currentBackground + 1].sprite = Resources.Load<Sprite>("visuals/Sprites/GUI_sprites/Backgrounds/World_part_0" + (currentBackground + 2).ToString());
                bgImages[currentBackground + 1].sprite = LoadAddressable_Vasundhara.Instance.GetSprite_Resources("World_part_0" + (currentBackground + 2).ToString());
                Debug.Log("<color=yellow>Sprite Loaded Name = </color>" + bgImages[currentBackground + 1].sprite);

            }
        }
        else
        {//moving down
            if (currentBackground - 1 >= 0 && bgImages[currentBackground - 1].sprite == null)
            {
                //add the lower background
                //bgImages[currentBackground - 1].sprite = Resources.Load<Sprite>("visuals/Sprites/GUI_sprites/Backgrounds/World_part_0" + (currentBackground).ToString());
                bgImages[currentBackground - 1].sprite = LoadAddressable_Vasundhara.Instance.GetSprite_Resources("World_part_0" + (currentBackground).ToString());
                Debug.Log("<color=yellow>Sprite Loaded Name = </color>" + bgImages[currentBackground - 1].sprite);

            }
            if (currentBackground + 2 < bgImages.Count && bgImages[currentBackground + 2].sprite != null)
            {
                //delete upper background
                Sprite tmp = bgImages[currentBackground + 2].sprite;
                bgImages[currentBackground + 2].sprite = null;

                Resources.UnloadAsset(tmp);
            }
        }

        lastValueChange = arg0;
    }


}
}
