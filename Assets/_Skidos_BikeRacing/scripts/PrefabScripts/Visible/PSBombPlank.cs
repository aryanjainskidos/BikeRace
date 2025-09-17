namespace vasundharabikeracing
{
    using UnityEngine;
    using System.Collections;
    using SimpleJSON;

    public class PSBombPlank : MonoBehaviour, ISaveable
    {

        public float force = 100;
        float scaleX = 1;
        float scaleY = 1;
        public BombGroup group;

        BombPlankBehaviour sb;
        Transform bp;

        void OnEnable()
        {

            Init();

            if (transform.localScale != Vector3.one)
            {
                transform.localScale = Vector3.one;
            }

        }

        void Init()
        {

            bp = transform.Find("BombPlank");
            sb = transform.Find("BombPlank").GetComponent<BombPlankBehaviour>();

            UpdateChildren();

        }

        void Start()
        {

            if (transform.localScale != Vector3.one)
            {
                transform.localScale = Vector3.one;
            }

            bp.localScale = new Vector3(scaleX * bp.localScale.x, scaleY * bp.localScale.y, bp.localScale.z);

            //print ("Start");
        }

        public void Load(JSONNode node)
        {
            Debug.LogError("Json Bomb data = " + node.ToString());
            force = node["force"].AsFloat;
            scaleX = node["scaleX"].AsFloat;
            scaleY = node["scaleY"].AsFloat;
            group = (BombGroup)node["group"].AsInt;


            Init();

            //print ("Load");

        }

        public JSONClass Save()
        {

            var J = new JSONClass();

            J["force"].AsFloat = force;
            J["scaleX"].AsFloat = transform.localScale.x * scaleX;
            J["scaleY"].AsFloat = transform.localScale.y * scaleY;
            J["group"].AsInt = (int)group;

            return J;
        }

        void UpdateChildren()
        {

            sb.force = force;
            sb.group = group;

        }

        //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        void OnValidate()
        {

            //		print ("onValidate");
            if (sb == null)
            {
                bp = transform.Find("BombPlank");
                sb = transform.Find("BombPlank").GetComponent<BombPlankBehaviour>();
            }

            if (sb != null)
            {
                UpdateChildren();
            }

        }

    }

}
