namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


public enum SoundZoneSounds
{ //nosaukumiem ir jábút identiskiem ar SoundManager.LoadClip() funkchá norádítajiem (Arí KapitalizácijA!)
    Birdie,
    Cow,
    Heli,
    Car,
    Cave
}


public class PSSound : MonoBehaviour, ISaveable
{

    public SoundZoneSounds SoundID;
    public bool Repeat;
    public float MinPauseAfter = 1;
    public float MaxPauseAfter = 2;

    private string sound;
    private bool inZone = false;



    public void Load(JSONNode node)
    {
        SoundID = (SoundZoneSounds)node["Sound"].AsInt;
        Repeat = node["Repeat"].AsBool;
        MinPauseAfter = node["MinPauseAfter"].AsFloat;
        MaxPauseAfter = node["MaxPauseAfter"].AsFloat;

        sound = SoundID.ToString();
    }

    public JSONClass Save()
    {

        var J = new JSONClass();
        J["Sound"].AsInt = (int)SoundID;
        J["Repeat"].AsBool = Repeat;
        J["MinPauseAfter"].AsFloat = MinPauseAfter;
        J["MaxPauseAfter"].AsFloat = MaxPauseAfter;
        return J;
    }


    void OnValidate()
    {
        sound = SoundID.ToString();
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "entity_trigger")
        {

            inZone = true;

            if (Repeat)
            {
                StartCoroutine(RepeatTheSound());
            }
            else
            {
                SoundManager.Play(sound);  // you know this one time ...
                inZone = false;
            }


        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.name == "entity_trigger")
        {
            inZone = false;
        }

    }


    IEnumerator RepeatTheSound()
    {

        while (true)
        {
            if (!inZone)
            {
                yield break;
            }

            SoundManager.Play(sound);

            float x = Random.Range(MinPauseAfter, MaxPauseAfter);
            yield return new WaitForSeconds(SoundManager.GetClipLength(sound) + x);

        }
    }

}
}
