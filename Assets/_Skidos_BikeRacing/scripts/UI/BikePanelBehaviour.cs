namespace vasundharabikeracing
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class BikePanelBehaviour : MonoBehaviour
    {

        //TODO !refactor, leaks memory when creating/removing the bike

        public GameObject bike;
        GameObject rider;

        ColorMePretty3D bikeColorer;

        public int bikeRiderID;
        public string bikeRecordName;
        public string loadedPrefabName;

        Dictionary<string, List<GameObject>> partGroups = new Dictionary<string, List<GameObject>>();

        SkinnedMeshRenderer decalMeshRenderer;
        SkinnedMeshRenderer tireDecalFrontMeshRenderer;
        SkinnedMeshRenderer tireDecalRearMeshRenderer;
        SkinnedMeshRenderer numberDecalMeshRenderer;

        Dictionary<string, int> groupPresetIDs = new Dictionary<string, int>();

        BikeRotationBehaviour bikeRotationBehaviour;

        bool initialized = false;

        // Use this for initialization
        public void Init()
        {
            //print("BikePanelBehaviour::Init");

            if (bike != null)
            {
                bike.transform.parent = null;
                Destroy(bike);
            }

            //        materials = new Dictionary<string, Material>();

            //        foreach (string key in DataManager.Bikes[bikeRecordName].GroupPresetIDs.Keys)
            //        {
            //            for (int i = 0; i < DataManager.Presets[0].Colors.Length; i++) {
            //                materials[key + "-" + i] = new Material(i > 0 ? Shader.Find("_MotoTrial/Texture plus Color plus Alpha") : Shader.Find("_MotoTrial/Texture plus Color"));
            //                materials[key + "-" + i].name = key + "-" + i;
            //            }
            //        }

            foreach (var item in BikeDataManager.Bikes[bikeRecordName].GroupPresetIDs)
            {
                groupPresetIDs[item.Key] = item.Value;
            }

            bikeRotationBehaviour = GetComponent<BikeRotationBehaviour>();

            LoadBike(bikeRecordName);

            initialized = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadPresetIDs(string bikeRecordName, int riderID)
        {
            foreach (var item in BikeDataManager.Bikes[bikeRecordName].StyleGroupPresetIDs[riderID])
            {
                groupPresetIDs[item.Key] = item.Value;
            }
        }

        void OnEnable()
        {
            if (Startup.Initialized && bike == null && initialized)
            {
                //print("BikePanelBehaviour::OnEnable");
                LoadBike(bikeRecordName);
            }

        }

        void OnDisable()
        {
            if (Startup.Initialized && bike != null)
            {
                //print("BikePanelBehaviour::OnDisable");
                iTween.tweens.Clear();
                loadedParts.Clear();
                decalMeshRenderer = null;
                tireDecalFrontMeshRenderer = null;
                tireDecalRearMeshRenderer = null;
                numberDecalMeshRenderer = null;
                bikeRotationBehaviour.bike = null;
                Destroy(rider);
                rider = null;
                Destroy(bike);
                bike = null;
            }
        }

        public void LoadBike(string bikeRecordName)
        {
            //print("LoadBike: " + bikeRecordName);

            this.bikeRecordName = bikeRecordName;
            LoadPresetIDs(bikeRecordName, BikeDataManager.Bikes[bikeRecordName].StyleID);
            loadedPrefabName = BikeDataManager.Bikes[bikeRecordName].PrefabName;

            int styleID = BikeDataManager.Bikes[bikeRecordName].StyleID;
            string loadedRiderPrefabName = BikeDataManager.Styles[styleID].GaragePrefabName;

            partGroups.Clear();

            bike = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(loadedPrefabName + "3D"));
            //bike = (GameObject)Instantiate(Resources.Load("Prefabs/Bikes/" + loadedPrefabName + "3D"));
            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + bike);
            //TODO this is valid for singleplayer bike only
            //if has decal add it, else hide it
            if (bike != null)
            {
                if (bikeRecordName == BikeDataManager.SingleplayerPlayerBikeRecordName)
                {

                    Transform decal = bike.transform.Find("Garage_Bike_Generic/Bike_Decal");

                    if (decal != null)
                    {
                        decalMeshRenderer = decal.GetComponent<SkinnedMeshRenderer>();
                        if (BikeDataManager.Styles[styleID] != null)
                        {
                            decalMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].DecalTexture;
                        }
                        else
                        {
                            decalMeshRenderer.gameObject.SetActive(false);
                        }
                    }

                    //tire decals
                    Transform tireDecalFront = bike.transform.Find("Garage_Bike_Generic/Bike_Tire_Decal_Front");

                    if (tireDecalFront != null)
                    {
                        tireDecalFrontMeshRenderer = tireDecalFront.GetComponent<SkinnedMeshRenderer>();
                        if (BikeDataManager.Styles[styleID] != null && BikeDataManager.Styles[styleID].TireDecalTexture != null)
                        {
                            tireDecalFrontMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].TireDecalTexture;
                        }
                        else
                        {
                            tireDecalFrontMeshRenderer.gameObject.SetActive(false);
                        }
                    }

                    Transform tireDecalRear = bike.transform.Find("Garage_Bike_Generic/Bike_Tire_Decal_Rear");

                    if (tireDecalRear != null)
                    {
                        tireDecalRearMeshRenderer = tireDecalRear.GetComponent<SkinnedMeshRenderer>();
                        if (BikeDataManager.Styles[styleID] != null && BikeDataManager.Styles[styleID].TireDecalTexture != null)
                        {
                            tireDecalRearMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].TireDecalTexture;
                        }
                        else
                        {
                            tireDecalRearMeshRenderer.gameObject.SetActive(false);
                        }
                    }

                    //numberplate decals
                    Transform numberDecal = bike.transform.Find("Garage_Bike_Generic/Bike_NumberDecals");

                    if (numberDecal != null)
                    {
                        numberDecalMeshRenderer = numberDecal.GetComponent<SkinnedMeshRenderer>();
                        if (BikeDataManager.Styles[styleID] != null && BikeDataManager.Styles[styleID].NumberDecalTexture != null)
                        {
                            numberDecalMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].NumberDecalTexture;
                        }
                        else
                        {
                            numberDecalMeshRenderer.gameObject.SetActive(false);
                        }
                    }

                }

                if (bikeRecordName == BikeDataManager.MultiplayerPlayerBikeRecordName)
                {

                    int upgradeIndex;

                    upgradeIndex = (int)UpgradeType.MaxSpeed;
                    LoadUpgradeParts(upgradeIndex);

                    upgradeIndex = (int)UpgradeType.Acceleration;
                    LoadUpgradeParts(upgradeIndex);

                    upgradeIndex = (int)UpgradeType.AccelerationStart;
                    LoadUpgradeParts(upgradeIndex);

                    upgradeIndex = (int)UpgradeType.BreakSpeed;
                    LoadUpgradeParts(upgradeIndex);
                    //                foreach (var item in DataManager.Bikes[bikeRecordName].Upgrades.Keys) {
                    //                    upgradeIndex = item;
                    //                    LoadUpgradeParts(upgradeIndex);
                    //                }

                    //                int upgradeLevel = DataManager.Bikes[bikeRecordName].Upgrades[upgradeIndex];
                    //                if (upgradeLevel >= 0 && upgradeLevel < 11) {
                    ////                    Transform bodyPlaceholder = bike.transform.FindChild(DataManager.Upgrades[upgradeIndex].Part3DPlaceholderName);//"MpBike_Body"
                    //                    Transform bodyPlaceholder = bike.transform.FindChild(UpgradeLineupManager.UpgradePart3DPlaceholders[upgradeIndex]);//"MpBike_Body"
                    ////                    string bodyPartName = DataManager.Upgrades[upgradeIndex].Part3DPrefabNamePerLevel[upgradeLevel];
                    //                    string bodyPartName = UpgradeLineupManager.UpgradeLevelParts3D[upgradeIndex][upgradeLevel][0];
                    //                    GameObject bikeBody = (GameObject)Instantiate(Resources.Load("Prefabs/Bikes/MPBikeParts3D/" + bodyPartName));
                    //
                    //                    bikeBody.transform.SetParent(bodyPlaceholder);
                    //
                    //                    bikeBody.transform.localRotation = Quaternion.identity;//Quaternion.Euler(-90.0f, 90.0f, 0);
                    //                    bikeBody.transform.localScale = Vector3.one;
                    //                }
                }
            }

            //        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();

            Debug.Log("<color=yellow>Prefab is loading from = </color>" + loadedRiderPrefabName);
            rider = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(loadedRiderPrefabName));

            Debug.Log("<color=yellow>Prefab Loading Name = </color>" + rider);

            //rider = (GameObject)Instantiate(Resources.Load("Prefabs/Riders/" + loadedRiderPrefabName));
            rider.transform.SetParent(bike.transform);
            rider.transform.localPosition = Vector3.zero;

            if (bikeRotationBehaviour != null)
            {
                bikeRotationBehaviour.bike = bike;
            }

            RegisterParts(bike.transform);

            RunColoring(bikeRecordName);

            bike.transform.SetParent(transform);

            bike.transform.localPosition = new Vector3(-55, -213.2f, 400);
            bike.transform.localScale = new Vector3(150, 150, 150);

        }

        public void RunColoring(string bikeRecordName)
        {

            if (BikeDataManager.Bikes[bikeRecordName].PrefabName == loadedPrefabName)
            {
                bikeColorer = bike.GetComponent<ColorMePretty3D>();
                bikeColorer.selectedRecord = bikeRecordName;
                bikeColorer.Run();
            }
            else
            {
                print("using color data of an other prefab: " + BikeDataManager.Bikes[bikeRecordName].PrefabName + " should be " + loadedPrefabName);
            }

        }

        void RegisterParts(Transform t)
        {

            //move each bike part to UI layer
            //t.gameObject.layer = LayerMask.NameToLayer("UI");
            t.gameObject.layer = 5;

            RegisterPart(t.gameObject);

            if (t.childCount > 0)
            {

                foreach (Transform child in t)
                {
                    RegisterParts(child);
                }

            }

        }

        void RegisterPart(GameObject part)
        {

            //if bike part has a color receiver, then add that part to part groups if it's not already there
            ColorReceiver3D colorReceiver = part.GetComponent<ColorReceiver3D>();
            if (colorReceiver != null && colorReceiver.enabled)
            {

                //            colorReceiver.ChangeMaterial(materials[colorReceiver.group]);

                if (partGroups.ContainsKey(colorReceiver.group))
                {

                    partGroups[colorReceiver.group].Add(part);

                }
                else
                {
                    partGroups.Add(colorReceiver.group, new List<GameObject> { part });
                }
            }
        }

        public void ColorPartGroup(string groupName, int presetIndex)
        {
            //        print("GrageBehaviour::ColorPartGroup " + groupName + " " + presetIndex);

            Color32[] presetColors = BikeDataManager.Presets[presetIndex].Colors;
            groupPresetIDs[groupName] = presetIndex;

            //        string group;
            //        
            //        for (int i = 0; i < presetColors.Length; i++)
            //        {
            //            group = groupName + "-" + i;
            //            if (materials.ContainsKey(group)) {
            //                materials[group].color = presetColors[i];
            //            }
            //        }

            string tmpGroupName;

            for (int i = 0; i < presetColors.Length; i++)
            {

                tmpGroupName = groupName + "-" + i.ToString();

                if (partGroups.ContainsKey(tmpGroupName))
                {

                    foreach (GameObject part in partGroups[tmpGroupName])
                    {

                        if (part != null)
                        {
                            part.GetComponent<ColorReceiver3D>().ChangeColor(presetColors[i]);
                        }
                        else
                        {
                            //                        print("!object is missing");
                        }

                    }

                }
                else
                {
                    //                print("!group is missing " + tmpGroupName);
                }

            }

        }

        public void SetRider(int styleID)
        {
            //change the rider and color him
            //        print("SetRider: " + styleID);

            if (bike == null)
            {
                OnEnable();
            }

            if (rider != null)
            {
                Destroy(rider);
                rider = null;
            }

            //riderID = DataManager.Bikes[bikeRecordName].RiderID;
            string loadedRiderPrefabName = BikeDataManager.Styles[styleID].GaragePrefabName;

            //set decal or hide it
            if (decalMeshRenderer != null)
            {
                if (BikeDataManager.Styles[styleID] != null)
                {
                    decalMeshRenderer.gameObject.SetActive(true);
                    decalMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].DecalTexture;
                }
                else
                {
                    decalMeshRenderer.gameObject.SetActive(false);
                }
            }
            if (tireDecalFrontMeshRenderer != null)
            {
                if (BikeDataManager.Styles[styleID] != null && BikeDataManager.Styles[styleID].TireDecalTexture != null)
                {
                    tireDecalFrontMeshRenderer.gameObject.SetActive(true);
                    tireDecalFrontMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].TireDecalTexture;
                }
                else
                {
                    tireDecalFrontMeshRenderer.gameObject.SetActive(false);
                }
            }
            if (tireDecalRearMeshRenderer != null)
            {
                if (BikeDataManager.Styles[styleID] != null && BikeDataManager.Styles[styleID].TireDecalTexture != null)
                {
                    tireDecalRearMeshRenderer.gameObject.SetActive(true);
                    tireDecalRearMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].TireDecalTexture;
                }
                else
                {
                    tireDecalRearMeshRenderer.gameObject.SetActive(false);
                }
            }
            if (numberDecalMeshRenderer != null)
            {
                if (BikeDataManager.Styles[styleID] != null && BikeDataManager.Styles[styleID].NumberDecalTexture != null)
                {
                    numberDecalMeshRenderer.gameObject.SetActive(true);
                    numberDecalMeshRenderer.material.mainTexture = BikeDataManager.Styles[styleID].NumberDecalTexture;
                }
                else
                {
                    numberDecalMeshRenderer.gameObject.SetActive(false);
                }
            }

            Debug.Log("<color=yellow>Prefab is loading from = </color>" + loadedRiderPrefabName);
            rider = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(loadedRiderPrefabName));

            Debug.Log("<color=yellow>Prefab Name = </color>" + rider);
            //rider = (GameObject)Instantiate(Resources.Load("Prefabs/Riders/" + loadedRiderPrefabName));
            rider.transform.SetParent(bike.transform);

            rider.transform.localPosition = Vector3.zero;
            rider.transform.localScale = Vector3.one;
            rider.transform.localRotation = Quaternion.identity;

            partGroups.Clear();

            RegisterParts(bike.transform);

            //        RunColoring(bikeRecordName);
            LoadPresetIDs(bikeRecordName, styleID);

            foreach (var item in groupPresetIDs)
            {

                string groupName = item.Key;
                int presetIndex = item.Value;

                Color32[] presetColors = BikeDataManager.Presets[presetIndex].Colors;

                string tmpGroupName;

                for (int i = 0; i < presetColors.Length; i++)
                {

                    tmpGroupName = groupName + "-" + i.ToString();

                    if (partGroups.ContainsKey(tmpGroupName))
                    {

                        foreach (GameObject part in partGroups[tmpGroupName])
                        {

                            if (part != null)
                            {
                                part.GetComponent<ColorReceiver3D>().ChangeColor(presetColors[i]);
                            }
                            else
                            {
                                print("!object is missing");
                            }

                        }

                    }
                    else
                    {
                        //                    print("!group is missing " + tmpGroupName);
                    }

                }
            }

            bike.transform.localRotation = Quaternion.Euler(0, 30.0f, 0);
            bikeRotationBehaviour.OnPointerUp(null);
        }

        public void SwapBikes(string bikeRecordName)
        {
            if (this.bikeRecordName != bikeRecordName)
            {
                rider = null;
                var tmpBike = bike;
                Destroy(tmpBike);
                loadedParts.Clear(); //TODO could actually clear only the list, the dict keys can stay
                LoadBike(bikeRecordName);
            }

            this.bikeRecordName = bikeRecordName;
            LoadPresetIDs(bikeRecordName, BikeDataManager.Bikes[bikeRecordName].StyleID);
            SetRider(BikeDataManager.Bikes[bikeRecordName].StyleID);
            RunColoring(bikeRecordName);
        }

        //upgradeIndex, placeholderName, partNames
        Dictionary<int, Dictionary<string, List<string>>> loadedParts = new Dictionary<int, Dictionary<string, List<string>>>();

        void LoadUpgradeParts(int index, bool animate = false)
        {

            if (bikeRecordName == BikeDataManager.MultiplayerPlayerBikeRecordName &&
               UpgradeLineupManager.UpgradeLevelParts3D.ContainsKey(index)
               )
            {

                int upgradeIndex = index;//(int)UpgradeType.MaxSpeed;
                int upgradeLevel = BikeDataManager.Bikes[bikeRecordName].Upgrades[upgradeIndex]; //bike displayed based on all upgrades temp and perm
                if (upgradeLevel >= 0 && upgradeLevel < 11)
                {

                    string placeholderName = "";

                    //for each placeholder
                    foreach (var placeholder in UpgradeLineupManager.UpgradeLevelParts3D[upgradeIndex])
                    {
                        placeholderName = placeholder.Key;

                        Transform partPlaceholder = (placeholderName != "") ? bike.transform.Find(placeholderName) : bike.transform;

                        if (loadedParts.ContainsKey(upgradeIndex) && loadedParts[upgradeIndex].ContainsKey(placeholderName))
                        {
                            List<string> toRemove = new List<string>();
                            //destroy all the unnecessary parts
                            foreach (var item in loadedParts[upgradeIndex][placeholderName])
                            {
                                if (!UpgradeLineupManager.UpgradeLevelParts3D[upgradeIndex][placeholderName][upgradeLevel].Contains(item))
                                {
                                    var tmpT = partPlaceholder.Find(item);
                                    if (tmpT != null)
                                    {
                                        iTween.Stop(tmpT.gameObject, true);
                                        Destroy(tmpT.gameObject);
                                    }
                                }
                            }
                            //delayed removal
                            for (int i = 0; i < toRemove.Count; i++)
                            {
                                loadedParts[upgradeIndex][placeholderName].Remove(toRemove[i]);
                            }
                        }

                        string partName = UpgradeLineupManager.UpgradeLevelParts3D[upgradeIndex][placeholderName][upgradeLevel][0];

                        for (int i = 0; i < UpgradeLineupManager.UpgradeLevelParts3D[upgradeIndex][placeholderName][upgradeLevel].Count; i++)
                        {

                            partName = UpgradeLineupManager.UpgradeLevelParts3D[upgradeIndex][placeholderName][upgradeLevel][i];
                            //if part not loaded, load it
                            if (!loadedParts.ContainsKey(upgradeIndex) ||
                                !loadedParts[upgradeIndex].ContainsKey(placeholderName) ||
                                !loadedParts[upgradeIndex][placeholderName].Contains(partName))
                            {

                                partName = partName + "3D";
                                GameObject partObject = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(partName));
                                Debug.LogError(partObject + " = Bike 3D Part is loading Name = " + partName);
                                //GameObject partObject = (GameObject)Instantiate(Resources.Load("Prefabs/Bikes/MPBikeParts3D/" + partName));

                                partObject.name = partName;
                                partObject.transform.SetParent(partPlaceholder);

                                partObject.transform.localPosition = Vector3.zero;
                                partObject.transform.localRotation = Quaternion.identity;
                                partObject.transform.localScale = Vector3.one;


                                if (animate)
                                {
                                    AnimatePart(partObject.transform);

                                    //link spokes to tires
                                    if (partName.ToLower().Contains("tire"))
                                    {
                                        //                                    print("animate the spokes");
                                        var spokesIndex = (int)UpgradeType.BreakSpeed;
                                        foreach (var item in loadedParts[spokesIndex][placeholderName])
                                        {
                                            if (item.ToLower().Contains("spokes"))
                                            {
                                                Transform spokesPart = partPlaceholder.Find(item);
                                                if (spokesPart != null)
                                                {
                                                    iTween.Stop(spokesPart.gameObject, true);
                                                    AnimatePart(spokesPart);
                                                }
                                                //                                            else print(item + "couldn't be found in " + partPlaceholder.name);
                                            }
                                        }
                                    }
                                }


                                if (loadedParts.ContainsKey(upgradeIndex))
                                {
                                    if (!loadedParts[upgradeIndex].ContainsKey(placeholderName))
                                    {
                                        loadedParts[upgradeIndex].Add(placeholderName, new List<string> { partName });
                                    }
                                    else
                                    {
                                        loadedParts[upgradeIndex][placeholderName].Add(partName);
                                    }
                                }
                                else
                                {
                                    loadedParts.Add(upgradeIndex,
                                                    new Dictionary<string, List<string>>(){
                                                    {
                                                        placeholderName,
                                                        new List<string>{partName}
                                                    }
                                                    }
                                    );
                                }
                                //                            print(loadedParts[upgradeIndex][placeholderName]);
                            }
                        }

                    }

                }
            }
        }

        void AnimatePart(Transform partTransform)
        {
            foreach (Transform item in partTransform)
            {
                iTween.Init(item.gameObject);
                iTween.ScaleFrom(item.gameObject, iTween.Hash(
                    "name", "wiggle_" + item.name,
                    "scale", Vector3.zero,
                    "time", 1f,
                    "easetype", iTween.EaseType.spring,
                    "ignoretimescale", true
                    )
                                 );
            }
        }

        public void UpgradeBike(int index)
        {
            //        print("boost " + index + " upgraded");

            if (UpgradeLineupManager.UpgradeLevelParts3D.ContainsKey(index))
            {
                ///----------------
                LoadUpgradeParts(index, true);
                ///----------------
                RegisterParts(bike.transform);

                RunColoring(bikeRecordName);
            }
        }
    }

}
