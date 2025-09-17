namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class ThrowCoinAtHUD : MonoBehaviour
{

    //	public Vector3 from;
    public Vector3 toPosition; // normalized
    public Vector3 toScale;
    public Color fromColor = new Color(0, 0, 0, 0);
    public Color toColor = new Color(0, 0, 0, 0);

    public bool completed = true;
    public bool playerReached = false;

    SpriteRenderer spriteRenderer;

    public Vector3 playerRelativePos;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!completed)
        {

            if (BikeDataManager.Boosts["magnet"].Active && !playerReached)
            {

                playerRelativePos = Camera.main.transform.InverseTransformPoint(BikeGameManager.player.transform.position);
                transform.localPosition = Vector3.Lerp(transform.localPosition, playerRelativePos, Time.deltaTime * 5);
                transform.localScale = Vector3.Lerp(transform.localScale, toScale * 1.5f, Time.deltaTime * 5);

                spriteRenderer.color = Color.Lerp(spriteRenderer.color, toColor + ((fromColor - toColor) * 0.5f), Time.deltaTime * 5);

                if ((transform.localPosition - playerRelativePos).sqrMagnitude < 1.3f)
                {
                    playerReached = true;
                }

            }
            else
            {

                transform.localPosition = Vector3.Lerp(transform.localPosition, toPosition * Camera.main.orthographicSize, Time.deltaTime * 5);
                transform.localScale = Vector3.Lerp(transform.localScale, toScale, Time.deltaTime * 5);

                spriteRenderer.color = Color.Lerp(spriteRenderer.color, toColor, Time.deltaTime * 5);

                if ((transform.localPosition - toPosition * Camera.main.orthographicSize).sqrMagnitude < 0.1f)
                {
                    completed = true;
                    playerReached = false;
                }
            }

            //			if(GameManager.player != null) {
            //				playerRelativePos = Camera.main.transform.InverseTransformPoint (GameManager.player.transform.position);
            //				print (playerRelativePos);
            //				print (playerRelativePos / Camera.main.orthographicSize);
            //			}
        }
    }

    public void Reset()
    {
        completed = true;
        playerReached = false;
    }

}

}
