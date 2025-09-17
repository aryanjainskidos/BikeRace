namespace vasundharabikeracing
{
    using UnityEngine;
    using System.Collections;
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /**
     * pseidostatiska klase, kas pieskatís límení esośos baikus
     * ieládés un uzsetos baika inputu (manuálo vai MP) 
     */
    public class BikeManager : MonoBehaviour
    {


        public static GameObject PlayerBike;
        public static GameObject AIBike;
        static int aiCount = 0;


        /**
         * tiek izsaukt ieládéjot límeni
         */
        public static void LoadBike(bool playerControlled, string intputData = null, string bikeRecord = "")
        {//TODO intputData will be removed
            Debug.Log("BikeManager: LoadBike called with bikeRecord: " + bikeRecord);

            if (BikeDataManager.Bikes == null)
            {
                Debug.LogError("BikeManager: BikeDataManager.Bikes is null!");
                return;
            }
            
            if (!BikeDataManager.Bikes.ContainsKey(bikeRecord))
            {
                Debug.LogError("BikeManager: Bike record not found: " + bikeRecord + ". Available records: " + string.Join(", ", BikeDataManager.Bikes.Keys));
                return;
            }

            print("BikeManager " + bikeRecord + " " + BikeDataManager.Bikes[bikeRecord].PrefabName);

            GameObject b;
            string bikePrefabName = BikeDataManager.Bikes[bikeRecord].PrefabName;
            Debug.Log("BikeManager: Looking for bike prefab: " + bikePrefabName);
            Debug.Log("BikeManager: BikeRecord details - StyleID: " + BikeDataManager.Bikes[bikeRecord].StyleID + ", PrefabName: " + bikePrefabName);
            
            if (LoadAddressable_Vasundhara.Instance == null)
            {
                Debug.LogError("BikeManager: LoadAddressable_Vasundhara.Instance is null!");
                return;
            }
            
            Debug.Log("BikeManager: LoadAddressable_Vasundhara.Instance found, calling GetPrefab_Resources");
            GameObject bikePrefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(bikePrefabName) as GameObject;

            Debug.Log("<color=yellow>Prefab is loading from = </color>" + bikePrefab);
            //GameObject bikePrefab = Resources.Load("Prefabs/Bikes/" + bikePrefabName) as GameObject;
            if (bikePrefab == null)
            {
                Debug.LogError("Nevar atrast baika prefabu \"" + bikePrefabName + "\"!");
                return;
            }
            else
            {
                Debug.Log("BikeManager: Successfully loaded bike prefab: " + bikePrefab.name);

                //#if UNITY_EDITOR
                //b = PrefabUtility.InstantiatePrefab(bikePrefab) as GameObject; //redaktorá (nav PLAY nospiests) instancéjot ar parasto metodi, zúd prefaba konekcija, tápéc jálieto śis veids
                //#else
                b = Instantiate(bikePrefab) as GameObject;
                //#endif
                Debug.Log("BikeManager: Successfully instantiated bike: " + b.name);
                //pozícija nav svaríga - tá tiks resetot sákot spéli
                Transform bikeContainer = GameObject.Find("BikeContainer").transform;
                if (bikeContainer.childCount == 0)
                { //reset variables
                    PlayerBike = null;
                    aiCount = 0;
                }
                //            print("childCount: " + bikeContainer.childCount);

                b.transform.parent = bikeContainer;
            }


            //TODO replace the rider
            Debug.Log("BikeManager: Looking for Player_parts at path: " + bikePrefabName + "Bike/Player_parts");
            Debug.Log("BikeManager: Bike instantiated name: " + b.name);
            Debug.Log("BikeManager: Bike children count: " + b.transform.childCount);
            for (int i = 0; i < b.transform.childCount; i++)
            {
                Debug.Log("BikeManager: Child " + i + ": " + b.transform.GetChild(i).name);
            }
            
            Transform tmpPlayerParts = b.transform.Find(bikePrefabName + "Bike/Player_parts");
            if (tmpPlayerParts == null)
            {
                Debug.LogError("BikeManager: Player_parts not found in bike prefab: " + bikePrefabName + "Bike/Player_parts");
                Debug.LogError("BikeManager: Available children: " + string.Join(", ", Enumerable.Range(0, b.transform.childCount).Select(i => b.transform.GetChild(i).name)));
                return;
            }
            tmpPlayerParts.gameObject.SetActive(false);


            int bikeRiderID = BikeDataManager.Bikes[bikeRecord].StyleID;
            if (bikeRecord.Contains("SPGhost"))
                bikeRiderID = BikeDataManager.Bikes[BikeDataManager.SingleplayerPlayerBikeRecordName].StyleID;

            Debug.Log("BikeManager: bikeRiderID = " + bikeRiderID);
            Debug.Log("BikeManager: BikeDataManager.Styles count = " + (BikeDataManager.Styles != null ? BikeDataManager.Styles.Count : 0));
            
            if (BikeDataManager.Styles == null)
            {
                Debug.LogError("BikeManager: BikeDataManager.Styles is null!");
                return;
            }
            
            if (!BikeDataManager.Styles.ContainsKey(bikeRiderID))
            {
                Debug.LogError("BikeManager: Style ID " + bikeRiderID + " not found in BikeDataManager.Styles!");
                return;
            }

            string bikeRiderPrefabName = BikeDataManager.Styles[bikeRiderID].PrefabName;
            Debug.Log("<color=yellow>Prefab Loading From = </color>" + bikeRiderPrefabName);
            GameObject bikeRider = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(bikeRiderPrefabName)) as GameObject;

            Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + bikeRider);
            //GameObject bikeRider = Instantiate(Resources.Load("Prefabs/Riders/" + bikeRiderPrefabName)) as GameObject;
            bikeRider.name = "Player_parts";
            bikeRider.transform.localScale = Vector3.one;
            bikeRider.transform.SetParent(tmpPlayerParts.parent);
            bikeRider.transform.localPosition = new Vector3(-0.012f, 0.055f, -0.1f);

            tmpPlayerParts.gameObject.name = "_";
            DestroyImmediate(tmpPlayerParts.gameObject); //TODO make it work with Destroy

            //switch decal sprite
            if (bikeRecord == BikeDataManager.SingleplayerPlayerBikeRecordName || bikeRecord.Contains("SPGhost"))
            {
                SpriteRenderer sr = b.transform.Find(bikePrefabName + "Bike/Bike_decal").GetComponent<SpriteRenderer>();
                if (BikeDataManager.Styles[bikeRiderID].DecalSprite != null)
                {
                    sr.sprite = BikeDataManager.Styles[bikeRiderID].DecalSprite;
                }
                else
                {
                    sr.enabled = false;
                }

                //switch decal sprite
                Transform frontTireTransform = b.transform.Find(bikePrefabName + "Bike/wheel_front/Tire_decal");
                Transform backTireTransform = b.transform.Find(bikePrefabName + "Bike/wheel_back/Tire_decal");
                
                if (frontTireTransform != null && backTireTransform != null)
                {
                    SpriteRenderer srf = frontTireTransform.GetComponent<SpriteRenderer>();
                    SpriteRenderer srr = backTireTransform.GetComponent<SpriteRenderer>();
                    
                    if (srf != null && srr != null)
                    {
                        Sprite tireDecalSprite = BikeDataManager.Styles[bikeRiderID].TireDecalSprite;
                        if (tireDecalSprite != null)
                        {
                            srf.sprite = tireDecalSprite;
                            srr.sprite = tireDecalSprite;
                            Debug.Log("BikeManager: Applied tire decal sprite: " + tireDecalSprite.name);
                        }
                        else
                        {
                            Debug.LogWarning("BikeManager: TireDecalSprite is null for style ID " + bikeRiderID + ", disabling tire decals");
                            srf.enabled = false;
                            srr.enabled = false;
                        }
                    }
                }

                Transform numberDecalTransform = b.transform.Find(bikePrefabName + "Bike/Number_decal");
                if (numberDecalTransform != null)
                {
                    SpriteRenderer srn = numberDecalTransform.GetComponent<SpriteRenderer>();
                    if (srn != null)
                    {
                        if (BikeDataManager.Styles[bikeRiderID].NumberDecalSprite != null)
                        {
                            srn.sprite = BikeDataManager.Styles[bikeRiderID].NumberDecalSprite;
                        }
                        else
                        {
                            srn.enabled = false;
                        }
                    }
                }
            }

            if (bikeRecord == BikeDataManager.MultiplayerPlayerBikeRecordName || bikeRecord.Contains("MPGhost")) // || MPGhosts
            {
                //            print("bikeRecord == DataManager.MultiplayerPlayerBikeRecordName");
                //assemble the mp bike
                int upgradeIndex;
                Transform tmpBike = b.transform.Find(bikePrefabName + "Bike");

                upgradeIndex = (int)UpgradeType.MaxSpeed;
                LoadUpgradeParts(upgradeIndex, bikeRecord, tmpBike.gameObject);

                upgradeIndex = (int)UpgradeType.Acceleration;
                LoadUpgradeParts(upgradeIndex, bikeRecord, tmpBike.gameObject);

                upgradeIndex = (int)UpgradeType.AccelerationStart;
                LoadUpgradeParts(upgradeIndex, bikeRecord, tmpBike.gameObject);

                upgradeIndex = (int)UpgradeType.BreakSpeed;
                LoadUpgradeParts(upgradeIndex, bikeRecord, tmpBike.gameObject);

                //            foreach (var item in DataManager.Bikes[bikeRecord].Upgrades.Keys) {
                //                upgradeIndex = item;
                //                if(UpgradeLineupManager.UpgradeLevelParts3D.ContainsKey(upgradeIndex)) {
                //                    LoadUpgradeParts(upgradeIndex, bikeRecord, b);
                //                }
                //            }
            }
            ////////////////

            if (playerControlled)
            { //spélétája mocim:

                //new ragdoll, ai won't need this, it'll delete it anyway
                Transform tmpPlayerRagdoll = b.transform.Find(bikePrefabName + "Ragdoll");

                string bikeRagdollPrefabName = BikeDataManager.Styles[bikeRiderID].RagdollPrefabName;
                Debug.Log("<color=yellow>Prefab is loading from = </color>" + bikeRagdollPrefabName);
                GameObject bikeRagdoll = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(bikeRagdollPrefabName)) as GameObject;

                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + bikeRagdoll);
                //GameObject bikeRagdoll = Instantiate(Resources.Load("Prefabs/Riders/" + bikeRagdollPrefabName)) as GameObject;
                bikeRagdoll.name = bikePrefabName + "Ragdoll";
                bikeRagdoll.transform.localScale = Vector3.one;
                bikeRagdoll.transform.SetParent(tmpPlayerRagdoll.parent);
                bikeRagdoll.transform.localPosition = Vector3.zero;

                GameObject ragdollEntityTrigger = Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("entity_trigger")) as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + ragdollEntityTrigger);
                //GameObject ragdollEntityTrigger = Instantiate(Resources.Load("Prefabs/Riders/entity_trigger")) as GameObject;
                ragdollEntityTrigger.name = "entity_trigger";
                ragdollEntityTrigger.transform.SetParent(bikeRagdoll.transform.Find("Core"));
                ragdollEntityTrigger.transform.localPosition = Vector3.zero;

                DestroyImmediate(tmpPlayerRagdoll.gameObject); //TODO make it work with Destroy
                ///

                PlayerBike = b;

                Transform tmpBike = PlayerBike.transform.Find(bikePrefabName + "Bike");
                Transform tmpRagdoll = PlayerBike.transform.Find(bikePrefabName + "Ragdoll");

                if (tmpBike != null)
                {//for first pref
                    tmpBike.GetComponent<ColorMePretty>().selectedRecord = bikeRecord;
                    tmpBike.GetComponent<ColorMePretty>().Run();
                    tmpRagdoll.GetComponent<ColorMePretty>().selectedRecord = bikeRecord;
                    tmpRagdoll.GetComponent<ColorMePretty>().Run();
                    tmpBike.gameObject.AddComponent<BikeInputDevice>();
                    Destroy(tmpBike.GetComponent<BikeInput>());//TODO remove component altogether
                }
                //A) piesaista galveno kameru

                GameObject[] cameraArrayWithOnlyOneCameraInIt = GameObject.FindGameObjectsWithTag("MainCamera"); //atrodu kameru
                if (cameraArrayWithOnlyOneCameraInIt[0] != null)
                {
                    BikeCamera camScript = cameraArrayWithOnlyOneCameraInIt[0].GetComponent<BikeCamera>();
                    if (camScript != null)
                    {
                        camScript.SetTarget(PlayerBike);
                    }
                    else
                    {
                        Debug.LogError("Galvenajai kameria nav \"BikeCamera\" skripta!");
                    }

                }
                else
                {
                    Debug.LogError("Nav kameras, ko nu ?");
                }





            }
            else
            { //nekontrolétajam mocim:

                AIBike = b;

                //nomaina input metodi
                Transform tmpBike = AIBike.transform.Find(bikePrefabName + "Bike");
                tmpBike.tag = "AI";

                BikeInputFile script = tmpBike.gameObject.AddComponent<BikeInputFile>();
                //            script.SetData(intputData);
                script.SetData(BikeDataManager.GetGhostRecordInputJSON(bikeRecord));
                Destroy(tmpBike.GetComponent<BikeInput>());

                //párkráso
                tmpBike.GetComponent<ColorMePretty>().selectedRecord = bikeRecord;
                tmpBike.GetComponent<ColorMePretty>().Run();

                //noliek moci fizikas slání
                //AIBike.layer = LayerMask.NameToLayer("Nothing");
                AIBike.layer = 9;
                Strip(AIBike.transform);
                aiCount++;
            }


        }

        static void LoadUpgradeParts(int index, string bikeRecordName, GameObject bike)
        {

            if (UpgradeLineupManager.UpgradeLevelParts2D.ContainsKey(index))
            {

                int upgradeIndex = index;//(int)UpgradeType.MaxSpeed;
                int upgradeLevel = BikeDataManager.Bikes[bikeRecordName].Upgrades[upgradeIndex]; // load the upgrade parts in accordance with the total upgrade value

                if (upgradeLevel >= 0 && upgradeLevel < 11)
                {

                    string placeholderName = "";

                    //for each placeholder
                    foreach (var placeholder in UpgradeLineupManager.UpgradeLevelParts2D[upgradeIndex])
                    {
                        placeholderName = placeholder.Key;

                        //                    print(bike.name +"/"+ placeholderName);
                        Transform partPlaceholder = (placeholderName != "") ? bike.transform.Find(placeholderName) : bike.transform;

                        string partName = UpgradeLineupManager.UpgradeLevelParts2D[upgradeIndex][placeholderName][upgradeLevel][0];

                        for (int i = 0; i < UpgradeLineupManager.UpgradeLevelParts2D[upgradeIndex][placeholderName][upgradeLevel].Count; i++)
                        {

                            partName = UpgradeLineupManager.UpgradeLevelParts2D[upgradeIndex][placeholderName][upgradeLevel][i];

                            GameObject partObject = (GameObject)Instantiate(LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(partName));
                            //GameObject partObject = (GameObject)Instantiate(Resources.Load("Prefabs/Bikes/MPBikeParts/" + partName));
                            Debug.LogError("Loaded Prefab Name = " + partObject);
                            partObject.name = partName;
                            var tmpScale = partObject.transform.localScale;
                            var tmpPos = partObject.transform.localPosition;
                            partObject.transform.SetParent(partPlaceholder);

                            partObject.transform.localPosition = tmpPos;//Vector3.zero;
                            partObject.transform.localRotation = Quaternion.identity;
                            partObject.transform.localScale = tmpScale;
                        }

                    }

                }
            }
        }

        static void Strip(Transform t)
        {


            if (t.childCount > 0)
            {

                foreach (Transform child in t)
                {
                    Strip(child);
                }

            }

            if (t.gameObject.tag == "PlayerRagdoll")
            {
                Destroy(t.gameObject);
            }

            if (t.gameObject.name == "ExhaustSmoke")
            {
                Destroy(t.gameObject);
            }

            if (t.gameObject.name == "magnet_trigger")
            {
                Destroy(t.gameObject);
            }

            if (PlayerBike != null || (PlayerBike == null && aiCount != 0) || t.GetComponent<Rigidbody2D>() == null || t.GetComponent<Collider2D>() == null || !(t.GetComponent<Collider2D>() is CircleCollider2D))
                //t.gameObject.layer = LayerMask.NameToLayer("Nothing");
                t.gameObject.layer = 9;

            MonoBehaviour[] mbs = t.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in mbs)
            {

                if (!(mb is ColorReceiver) &&
                    !(mb is ColorMePretty) &&
                    !(mb is BikeControl) &&
                    !(mb is BikeInput) &&
                    !(mb is BikeInputDevice) &&
                    !(mb is BikeInputFile) &&
                    !(mb is BikeStateData) &&
                    !(mb is RiderAnimationControl) &&
                    !(mb is RagdollEntityTrigger)
                    )
                {

                    Destroy(mb);
                }

            }

            if (t.GetComponent<Collider2D>() != null)
            {
                if (PlayerBike != null || (PlayerBike == null && aiCount != 0) || t.GetComponent<Rigidbody2D>() == null || !(t.GetComponent<Collider2D>() is CircleCollider2D))
                    Destroy(t.GetComponent<Collider2D>());
            }


            if (t.GetComponent<Rigidbody2D>() != null)
            {
                //Destroy(t.rigidbody2D);
                if (PlayerBike != null || (PlayerBike == null && aiCount != 0) || t.GetComponent<Collider2D>() == null || !(t.GetComponent<Collider2D>() is CircleCollider2D))
                    t.GetComponent<Rigidbody2D>().isKinematic = true;
            }

            if (t.GetComponent<Renderer>() != null)
            {
                if (PlayerBike != null || aiCount > 0)
                    t.GetComponent<Renderer>().sortingOrder -= 25; //move to back

                if (!t.GetComponent<Renderer>().enabled)
                    Destroy(t.GetComponent<Renderer>());
            }

        }


    }

}
