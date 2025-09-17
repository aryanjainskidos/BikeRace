namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColorMePretty3D : MonoBehaviour
{

    public string selectedRecord = "";
    //    bool firstRun = true;

    // Use this for initialization
    //	void Awake () {
    //        print("ColorMePretty::Awake");
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
        //  print("ColorMePretty3D::Run " + selectedRecord + " - " + firstRun);
        //		print ("colormeprettyStart" + transform.tag);
        ColorTransformAndChildren(transform);
        //        firstRun = false;
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

            ColorReceiver3D colorReceiver = tmp.GetComponent<ColorReceiver3D>();

            if (colorReceiver != null)
            {

                //                if (firstRun) {
                //                    //set the materials for each bike
                //                    if(DataManager.Bikes.ContainsKey (selectedRecord) &&
                //                       DataManager.Bikes[selectedRecord].Materials.ContainsKey(colorReceiver.group))
                //                        colorReceiver.ChangeMaterial(DataManager.Bikes[selectedRecord].Materials[colorReceiver.group]);
                //                }

                groupName = "";
                colorIndex = -1;
                presetIndex = -1;

                if (selectedRecord != "SPGhost")
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


                //                if (presetIndex >= 0 && presetIndex < DataManager.Presets.Count && 
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
                        Debug.Log(BikeDataManager.Presets[0].Colors.Length + " " + colorIndex);
                        oColor = sColor = BikeDataManager.Presets[0].Colors[colorIndex];
                    }
                    else
                    {
                        oColor = sColor = Color.white;//tmp.GetComponent<SpriteRenderer>().color;
                    }
                }

                if (selectedRecord == "SPGhost")
                { //single player ghost doesn't have a color scheme defined - just gray him out a bit
                    oColor = sColor = Color.white;
                    sColor = sColor * 0.75f;
                }

                sColor.a = 1;

                //                colorReceiver.color = oColor;
                colorReceiver.ChangeColor(oColor);

                //                if (DataManager.Bikes[selectedRecord].Materials.ContainsKey(colorReceiver.group)) {
                //                    DataManager.Bikes[selectedRecord].Materials[colorReceiver.group].color = oColor;
                //                }
            }
        }

        //		

    }
}

}
