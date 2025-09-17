namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BirdBehaviour : MonoBehaviour
{

    public BirdGroup group;
    public Animator anim;
    public Animator animWings;

    bool invoke = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        animWings = transform.Find("Birdy_body").GetComponent<Animator>();

        anim.speed = 0;
        animWings.speed = 0;
    }

    public void Activate()
    {
        //
        //		if(lifeAfterExplosion > 0) {
        float randomTimeOffset = Random.value * 0.2f;
        Invoke("Animate", randomTimeOffset);
        invoke = true;
        //		}
        //        Animate();
    }

    void Animate()
    {
        //		
        if (invoke && gameObject.transform.parent != null)
        {
            //			
            //			if (gameObject.collider2D != null) {
            //				gameObject.collider2D.enabled = false;
            //			}
            //
            //			if (gameObject.renderer != null) {
            //				gameObject.renderer.enabled = false;
            //			}
            //
            //			GameManager.ShowChildren(bomb, false);
            //
            //			GameManager.ShowChildren(blast, true);
            //			anim.enabled = true;
            anim.speed = 1;
            animWings.speed = 1;

            //            Quaternion newRot = Quaternion.identity;
            //            newRot.eulerAngles = Vector3.forward * Random.Range(-30.0f, 30.0f);
            transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-30.0f, 30.0f));
            //            anim.Play("Bird", -1, 0);
        }

    }
    //
    public void Reset()
    {
        //
        //		if (gameObject.collider2D != null) {
        //			gameObject.collider2D.enabled = true;
        //		}
        //		
        //		if (gameObject.renderer != null) {
        //			gameObject.renderer.enabled = true;
        //		}
        //
        //		GameManager.ShowChildren(bomb, true);
        //
        //		if(anim != null) {
        //			
        //			GameManager.ShowChildren(blast, false);
        //			anim.enabled = false;
        //		}
        invoke = false;
        //
        if (anim != null)
        {
            //            print("BirdBehaviour reset");
            anim.Play("Birdy_flight", -1, 0);//rewind
            anim.speed = 0;
        }

        if (animWings != null)
        {
            animWings.Play("Birdy", -1, 0);//rewind
            animWings.speed = 0;
        }

        transform.localRotation = Quaternion.identity;
    }

}

}
