namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TabSwitchBehaviour : MonoBehaviour
{

    public Toggle[] toggles;
    public string postfix = "Toggle";

    void Awake()
    {

        //int eventCount;

    }

    public void Switch(string tabName, string subTabName = "")
    {
        //        print("Switch " + tabName + " " + subTabName);

        Toggle tabToggle = GetTabToggle(tabName);
        if (tabToggle != null)
        {
            tabToggle.isOn = true;
            //            print(tabToggle.name);

            if (subTabName != "")
            {
                SwitchSubtab(tabToggle, subTabName);
            }
        }
        else print("couldn't get tabToggle");
    }

    public void SwitchSubtab(Toggle toggle, string subTabName)
    {
        //        print("SwitchSubtab " + toggle.name + " " + subTabName);

        Object tmpO;
        GameObject tmpGO;

        int eventCount = toggle.onValueChanged.GetPersistentEventCount();
        TabSwitchBehaviour tabSwitchBehaviour;

        for (int i = 0; i < eventCount; i++)
        {

            tmpO = toggle.onValueChanged.GetPersistentTarget(i);
            //            print("tab " + tmpO.name + tmpO.GetType());

            if (tmpO.GetType() == typeof(GameObject))
            {

                tmpGO = (GameObject)tmpO;
                tabSwitchBehaviour = tmpGO.GetComponent<TabSwitchBehaviour>();

                if (tabSwitchBehaviour != null)
                {
                    tabSwitchBehaviour.Switch(subTabName);
                }

            }

        }
    }

    Toggle GetTabToggle(string tabName)
    {

        Toggle tab = null;

        foreach (Toggle toggle in toggles)
        {
            if (toggle.name == tabName + postfix)
            {
                tab = toggle;
            }
        }

        return tab;
    }
}

}
