namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

/**
 * pievienots geimobjektam, izslédz/ieslédz to atkaríbá vai Debug.isDebugBuild ir true  |  to maina Build Settingos pirms búvéśanas, Editorá vienmér = true
 */
public class ShowOnlyInDebugMode : MonoBehaviour
{

    public DebugModeRestrictionType Restriction;



    public void OnEnable()
    {

        //print( transform.name +  "  Restriction:" + Restriction + "   Debug.isDebugBuild:" + Debug.isDebugBuild);
        if (Restriction == DebugModeRestrictionType.OnlyIfDebug && !Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
        }
        if (Restriction == DebugModeRestrictionType.OnlyIfNotDebug && Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
        }

    }


}

public enum DebugModeRestrictionType
{
    Whatevs,
    OnlyIfDebug, //turn off if not Debug
    OnlyIfNotDebug,//turn off if Debug
}

}
