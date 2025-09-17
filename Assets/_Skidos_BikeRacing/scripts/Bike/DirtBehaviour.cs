namespace vasundharabikeracing
{
    using UnityEngine;
    using System.Collections;

    public class DirtBehaviour : MonoBehaviour
    {

        int dirtIndex;
        GameObject dirt;
        GameObject[] dirtPool;
        ParticleSystem dirtPS;
        ParticleSystem[] dirtPSPool;
        //    BikeTriggerCollision frontTrigger;
        //    BikeTriggerCollision backTrigger;

        public Vector3 tmp;

        public float angle;

        public Quaternion rot;

        public int contactCount;

        public Vector2 norm;
        public Vector2 perp;
        public Vector2[] perpendiculars;
        public float[] magnitudes;

        public float angularVelocity;

        public int particelCount;

        public bool colliding = false;

        public bool collisionStay = false;

        public int layerMask;

        public float dot;
        public float dotThresholdNegative = 0.9f;
        public float dotThresholdPositive = 0.99f;
        public float angleDir;

        public float maxStartSize;
        public float maxStartSpeed;

        public float sizeCoef = 1f;

        void Awake()
        {
            dirtPool = new GameObject[5];
            dirtPSPool = new ParticleSystem[5];
            perpendiculars = new Vector2[5];
            magnitudes = new float[5];

            //get dirt corresponding ro foliage
            //Object dirtResource = Resources.Load ("Prefabs/Effects/Dirt"+LevelManager.FoliageType); //load a corresponding to foliage
            Object dirtResource = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Prefabs/Effects/Dirt" + LevelManager.FoliageType); //load a corresponding to foliage
            if (dirtResource == null)
            {
                dirtResource = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Dirt"); //load default
                Debug.Log("<color=yellow>Prefab Name = </color>" + dirtResource);
                //dirtResource = Resources.Load("Prefabs/Effects/Dirt"); //load default

                Debug.LogWarning("dirt at \"Prefabs/Effects/Dirt" + LevelManager.FoliageType + "\"not found. Loading default.");
            }

            for (int i = 0; i < 5; i++)
            {
                dirt = (GameObject)Instantiate(dirtResource);

                dirt.transform.parent = transform.parent.parent;
                dirtPool[i] = dirt;

                dirtPS = dirt.transform.Find("particles").GetComponent<ParticleSystem>();
                dirtPS.enableEmission = false;
                dirtPSPool[i] = dirtPS;

                dirtIndex = i;
            }

            maxStartSize = dirtPS.startSize;
            maxStartSpeed = dirtPS.startSpeed;
            //        frontTrigger = transform.parent.FindChild("wheel_front/wheel_front_trigger").GetComponent<BikeTriggerCollision>();
            //        backTrigger = transform.FindChild("wheel_back_trigger").GetComponent<BikeTriggerCollision>();
            //        dirtPS
            layerMask = 0;
            //layerMask = LayerMask.NameToLayer("Default");
            //        norm = Vector2.up;

        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            //        if (backTrigger.colliding || frontTrigger.colliding)
            //        {
            //A workaround to fix the faulty behaviour of OnCollisionStay2D
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, -norm, 1.2f);

            Debug.DrawRay(transform.position, -norm * 1f, Color.red);
            //            print(hits.Length);
            bool hitGround = false;
            RaycastHit2D hit;
            //foreach (var hit in hits) { <-- foreach in Update() is bad, mkey
            for (int i = 0; i < hits.Length; i++)
            {
                hit = hits[i];
                if (hit.collider.name == "2dCollider")
                {

                    dot = Vector2.Dot(norm, hit.normal);
                    Debug.DrawRay(transform.position, -hit.normal * 1.2f, Color.blue);

                    angleDir = AngleDir(norm, hit.normal);

                    //                    if(dot >= dotThreshold) {// || (backTrigger.colliding && frontTrigger.colliding)
                    if ((angleDir <= 0 && dot >= dotThresholdNegative) ||
                        (angleDir > 0 && dot >= dotThresholdPositive))
                    {// || (backTrigger.colliding && frontTrigger.colliding)
                        MoveDirt(hit.point, hit.normal);
                        hitGround = true;
                    }
                }
            }

            for (int i = 0; i < dirtPSPool.Length; i++)
            {
                ParticleSystem dPS = dirtPSPool[i];

                if (!dPS.enableEmission && dPS.particleCount > 0)
                {
                    //move particle
                    dirtPool[i].transform.Translate(Vector3.right * Time.fixedDeltaTime * magnitudes[i]);
                    magnitudes[i] *= 0.97f;
                }
            }

            if (!hitGround)
            {
                dirtPS.enableEmission = false;
                perpendiculars[dirtIndex] = perp;
                magnitudes[dirtIndex] = GetComponent<Rigidbody2D>().linearVelocity.magnitude;
                PickFreeDirt();
            }
            //        } else {
            //            dirtPS.enableEmission = false;
            //        }

            collisionStay = false;
        }

        public float AngleDir(Vector2 A, Vector2 B)
        {
            return -A.x * B.y + A.y * B.x;
        }


        void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.name != "2dCollider" || coll.contacts.Length <= 0) return;
            var contacts = coll.contacts[0];
            norm = contacts.normal;
            var rigid = GetComponent<Rigidbody2D>();
            if (rigid == null) return;
            if (!(rigid.angularVelocity <= 0)) return;

            dirtPS.enableEmission = true;
            dirtPS.Play();
            particelCount = dirtPS.particleCount;

            //TODO use a pool
            colliding = true;
        }

        void PickFreeDirt()
        {

            if (dirtPS.particleCount != 0)
            {
                //            dirtPS.Stop();
                //            dirtPS.Clear();
                dirtPS.enableEmission = false;

                for (int i = 0; i < dirtPSPool.Length; i++)
                {

                    if (dirtPSPool[i].particleCount == 0)
                    {
                        dirtIndex = i;
                        dirt = dirtPool[i];
                        dirtPS = dirtPSPool[i];
                        //                    dirtPS.Clear();
                        dirtPS.Stop();
                        dirtPS.Clear();
                        if (dirtPS.GetComponent<ParticleSystem>() != null)
                        {
                            dirtPS.GetComponent<ParticleSystem>().Clear();
                        }
                        break;
                    }

                }
            }

        }

        void OnCollisionExit2D(Collision2D coll)
        {
            if (coll.gameObject.name == "2dCollider")
            {
                if (coll.contacts.Length > 0)
                    norm = coll.contacts[0].normal;
            }
        }

        void MoveDirt(Vector2 point, Vector2 normal)
        {
            if (GetComponent<Rigidbody2D>().angularVelocity <= 0)
            {

                //                Mathf.Round(rigidbody2D.velocity.sqrMagnitude)
                dirtPS.startSize = Mathf.Min(maxStartSize, GetComponent<Rigidbody2D>().linearVelocity.magnitude / 2) * sizeCoef;
                dirtPS.startSpeed = Mathf.Min(maxStartSpeed, GetComponent<Rigidbody2D>().linearVelocity.magnitude) * sizeCoef;

                tmp = point;
                tmp.z = dirt.transform.position.z;
                dirt.transform.position = tmp;

                norm = normal;
                perp = new Vector2(-norm.y, norm.x);

                Debug.DrawRay(point, normal, Color.white);
                Debug.DrawRay(point, perp, Color.green);

                angle = -1 * Mathf.Sign(perp.y) * Vector2.Angle(perp, -Vector2.right);
                rot = Quaternion.AngleAxis(angle, Vector3.forward);
                dirt.transform.localRotation = rot;

                angularVelocity = GetComponent<Rigidbody2D>().linearVelocity.magnitude;

                //if going forward show dirt
                if (!dirtPS.enableEmission)
                {
                    dirtPS.enableEmission = true;
                    dirtPS.Play();
                }
                //                    dirt.SetActive(true);

            }
            else
            {
                //if going backwards hide dirt
                if (dirtPS.enableEmission)
                {
                    dirtPS.Stop();
                    dirtPS.Clear();
                    dirtPS.enableEmission = false;
                }
                //                    dirt.SetActive(false);
            }
        }

        public void Reset()
        {
            foreach (var item in dirtPSPool)
            {
                item.Clear();
            }
        }
    }

}
