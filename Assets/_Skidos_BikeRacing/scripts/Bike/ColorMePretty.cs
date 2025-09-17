namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColorMePretty : MonoBehaviour
{

    public string selectedRecord = "";

    // Use this for initialization
    //	void Awake () {
    //        print("ColorMePretty::Awake" + transform.name + "_" + transform.parent.name);
    ////		print ("colormeprettyAwake" + transform.tag);
    ////		ColorTransformAndChildren (transform);
    //		
    //	}

    //    void Start () {
    //        print("ColorMePretty::Start");
    //		//		print ("colormeprettyStart" + transform.tag);
    ////		ColorTransformAndChildren (transform);
    //
    //	}

    public void Run()
    {
        //        print("ColorMePretty::Run");
        ColorTransformAndChildren(transform);

    }

    // Update is called once per frame
    public void ColorTransformAndChildren(Transform t)
    {

        //        print("-" + selectedRecord + "- " + t.name);
        Queue<Transform> queue = new Queue<Transform>();

        queue.Enqueue(t);
        int iterations = 0;

        Transform tmp;

        Color32 oColor;
        Color sColor;
        Color bColor = new Color(0, 0, 0, 0);

        string[] groupSplit;
        string groupName;
        int colorIndex;
        int presetIndex;

        while (queue.Count > 0 && iterations < 1000)
        {

            iterations++;

            tmp = queue.Dequeue();

            if (tmp.childCount > 0)
            {

                foreach (Transform child in tmp)
                {
                    queue.Enqueue(child);
                }

            }

            SpriteRenderer spriteRenderer = tmp.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {

                ColorReceiver colorReceiver = tmp.GetComponent<ColorReceiver>();

                groupName = "";
                colorIndex = -1;
                presetIndex = -1;

                if (colorReceiver != null && selectedRecord != "SPGhost")
                { //ignore groups if ghost or has no color receiver
                    groupSplit = colorReceiver.group.Split('-');

                    if (groupSplit.Length > 0)
                        groupName = groupSplit[0];

                    if (groupSplit.Length > 1)
                        colorIndex = Int32.Parse(groupSplit[1], System.Globalization.CultureInfo.InvariantCulture);

                    if (BikeDataManager.Bikes.ContainsKey(selectedRecord) &&
                       BikeDataManager.Bikes[selectedRecord].GroupPresetIDs.ContainsKey(groupName))
                    {
                        presetIndex = BikeDataManager.Bikes[selectedRecord].GroupPresetIDs[groupName];
                    }
                }

                //                if (paletteIndex >= 0 && paletteIndex < DataManager.Presets.Count && 
                if (BikeDataManager.Presets.ContainsKey(presetIndex) &&
                    colorIndex >= 0 && colorIndex < BikeDataManager.Presets[presetIndex].Colors.Length
                    )
                {
                    oColor = BikeDataManager.Presets[presetIndex].Colors[colorIndex];
                    sColor = ((Color)oColor == bColor) ? Color.white : (Color)oColor;
                }
                else
                {
                    if (groupName != "" && BikeDataManager.Presets[0].Colors.Length > colorIndex)
                    { // && selectedRecord != "SPGhost"
                        oColor = sColor = BikeDataManager.Presets[0].Colors[colorIndex];
                    }
                    else
                    {
                        oColor = sColor = tmp.GetComponent<SpriteRenderer>().color;
                    }
                }

                if (selectedRecord == "SPGhost")
                { //single player ghost doesn't have a color scheme defined - just gray him out a bit
                    oColor = sColor = Color.white;
                    sColor = sColor * 0.75f;
                }

                sColor.a = 1;


                spriteRenderer.color = sColor;

                if (colorReceiver != null)
                    colorReceiver.color = oColor;

            }
        }

        //		

    }
}

}
