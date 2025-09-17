namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class BombPlankBehaviour : MonoBehaviour
{

    //public float radius = 10f;
    public float force = 100;

    GameObject player;
    GameObject[] bikeParts;
    GameObject[] fakeBombs;

    public float lastExplodeTime = 0;
    public float explodeCooldown = 0.1f;

    public float lifeAfterExplosion = 1;

    public BombGroup group;

    //	bool exploded = false;
    bool invoke = false;

    Transform parts;
    Animator anim;

    Rigidbody2D rigidBody;

    void Start()
    {
        parts = transform.Find("parts");
        anim = parts.GetComponent<Animator>();
        anim.speed = 0;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {

        //to avoid multiple explosions at the same time
        if ((lastExplodeTime == 0 || Time.time - lastExplodeTime >= explodeCooldown) && (coll.gameObject.tag == "bike-part" || coll.gameObject.tag == "Player"))
        {
            Explode();
            lastExplodeTime = Time.time;
        }

    }

    private void Awake()
    {
        rigidBody = transform.GetComponent<Rigidbody2D>();
    }


    public void Explode()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        bikeParts = GameObject.FindGameObjectsWithTag("bike-part");
        fakeBombs = GameObject.FindGameObjectsWithTag("BombFake");

        player.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.up * force);

        for (int i = 0; i < bikeParts.GetLength(0); i++)
        {

            if (bikeParts[i].GetComponent<Collider2D>() != null &&
               !bikeParts[i].GetComponent<Collider2D>().isTrigger &&
               bikeParts[i].GetComponent<Collider2D>().attachedRigidbody != null)
            {
                bikeParts[i].GetComponent<Rigidbody2D>().AddForce(gameObject.transform.up * force);
            }

        }

        BombFakeBehaviour tbb;
        foreach (GameObject bomb in fakeBombs)
        {

            tbb = bomb.GetComponent<BombFakeBehaviour>();

            if (tbb.group == group)
            {
                tbb.Explode();
            }

        }

        // transform.GetComponent<Rigidbody2D>().isKinematic = false;
        rigidBody.isKinematic = false;
        transform.GetComponent<Collider2D>().enabled = false;
        rigidBody.AddForce(transform.up * force);
        // transform.GetComponent<Rigidbody2D>().AddForce(transform.up * force);

        //anim.enabled = true;
        anim.speed = 1;

        if (lifeAfterExplosion > 0)
        {
            Invoke("Remove", lifeAfterExplosion);
            invoke = true;
        }

        SoundManager.Play("Explosion");

    }

    void Remove()
    {

        if (invoke && gameObject.transform.parent != null)
        {

            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }

            if (gameObject.GetComponent<Renderer>() != null)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }

            rigidBody.isKinematic = true;
            // transform.GetComponent<Rigidbody2D>().isKinematic = true;
            invoke = false;

            BikeGameManager.ShowChildren(parts, false);

        }

    }

    public void Reset()
    {

        invoke = false;

        if (rigidBody != null)// && !rigidBody.isKinematic)
        {
            rigidBody.isKinematic = true;
            rigidBody.linearVelocity = Vector2.zero;
            rigidBody.angularVelocity = 0;
        }

        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        if (gameObject.GetComponent<Renderer>() != null)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }

        // if (gameObject.GetComponent<Rigidbody2D>()!= null && !GetComponent<Rigidbody2D>().isKinematic) {
        // 	GetComponent<Rigidbody2D>().isKinematic = true;
        // }

        BikeGameManager.ShowChildren(parts, true);

        if (anim != null)
        {
            anim.Play("PlankShatter", -1, 0);
            anim.speed = 0;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

    }

}

}
