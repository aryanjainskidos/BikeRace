namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class CheckpointPoleBehaviour : MonoBehaviour
{

    public CheckpointGroup group;
    public Animator anim;
    //	public Transform blast;
    //	public Transform bomb;
    //
    void Awake()
    {
        anim = GetComponent<Animator>();
        //        anim.enabled = false;
        anim.speed = 0;
    }

    public void Activate()
    {
        //
        ////		if(lifeAfterExplosion > 0) {
        ////			float randomTimeOffset = (Random.value - 0.5f) * lifeAfterExplosion;
        ////			Invoke("Remove", lifeAfterExplosion + randomTimeOffset);
        ////		}
        Animate();
    }

    void Animate()
    {
        //		
        if (gameObject.transform.parent != null)
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
            //            anim.Play("Checkpoint", -1, 0);
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
        //
        if (anim != null)
        {
            //            print("CheckpointPoleBehaviour reset");
            anim.Play("Checkpoint", -1, 0);//rewind
            anim.speed = 0;
        }
    }

}

}
