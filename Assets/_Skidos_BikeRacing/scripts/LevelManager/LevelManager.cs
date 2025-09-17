namespace vasundharabikeracing
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.Collections;
    using System.Collections.Generic;
    using SimpleJSON;
    //using System.Security.Cryptography;
    using System.Text;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;
    using System.IO;
    using System.Text.RegularExpressions;



    /**
     * śí ir statiska klase (nelieto static keyword, jo tad nevar extendot monobehaviour; vnk visas metodes ir statiskas)
     * nepieder nevienam geimobjektam
     */
    public class LevelManager : MonoBehaviour
    {

        public static string CurrentlyOpenLevelPath = "";  //tikai redaktoram, lai zin kur sevot, to izmaina gan no redaktora, gan spéles (sevojot vai ieládéjot límeni))
        public static string CurrentLevelName = ""; //atvértá límeńa faila nosaukums (bez ceĺa/paplaśinájuma) lietojams, lai piekĺútu DataManager límeńa lietám
        public static string FoliageType = ""; //PolyMesh.FoliageTypes.grass.ToString(); // atvértá límeńa foliage tips (pirmais atrastais)
                                               //dikts: spraita_atlasa_nosaukums => spraiti (spraiti arí ir dikti: spraita_nosaukums_atlasá => spraita_objekts)
        private static Dictionary<string, Dictionary<string, Sprite>> spriteAtlasCache = new Dictionary<string, Dictionary<string, Sprite>>();

        //loading vars
        public static int loadingProgressPreObjects = 0;
        public static int loadingTotalPreObjects = 3;
        public static int namedObjectCount;
        public static int objectsLoadedTotal;
        static int objectsLoadPerFrame;
        static int loadedSinceLastFrame = 0;

        public static bool loadedLevel;


       
        public static void ClearLevel()
        {

            //izveidos neeksistéjośos konteinerus
            GameObject LevelContainerGO = GameObject.Find("LevelContainer");
            if (LevelContainerGO == null)
            {
                LevelContainerGO = new GameObject();
                LevelContainerGO.name = "LevelContainer";
            }

            //iztírís límeńkonteineri
            while (LevelContainerGO.transform.childCount > 0)
            { //jádara vairákkárt :\
                foreach (Transform child in LevelContainerGO.transform)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            //límeńkonteinerim obligáti piederośie skripti
            if (LevelContainerGO.GetComponent<Timer>() == null)
            {
                LevelContainerGO.AddComponent<Timer>();
            }
            if (LevelContainerGO.GetComponent<LevelInfo>() == null)
            {
                LevelContainerGO.AddComponent<LevelInfo>();
            }




            GameObject GeoContainerGO = GameObject.Find("GeoContainer");
            if (GeoContainerGO == null)
            {
                GeoContainerGO = new GameObject();
                GeoContainerGO.name = "GeoContainer";
                GeoContainerGO.transform.parent = LevelContainerGO.transform;
            }

            GameObject SkyContainerGO = GameObject.Find("SkyContainer");
            if (SkyContainerGO == null)
            {
                SkyContainerGO = new GameObject();
                SkyContainerGO.name = "SkyContainer";
                SkyContainerGO.transform.parent = LevelContainerGO.transform;
            }

            GameObject BikeContainerGO = GameObject.Find("BikeContainer");
            if (BikeContainerGO == null)
            {
                BikeContainerGO = new GameObject();
                BikeContainerGO.name = "BikeContainer";
                BikeContainerGO.transform.parent = LevelContainerGO.transform;
            }

            BikeGameManager.initialized = false;
        }

       public static void SerializeScene(string LevelPath, bool thisIsAutosave = false)
{
#if UNITY_EDITOR
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    JSONClass J = new JSONClass();

    GameObject GeoContainerGO = GameObject.Find("GeoContainer");
    GameObject SkyContainerGO = GameObject.Find("SkyContainer");

    if (GeoContainerGO == null || SkyContainerGO == null)
    {
        Debug.LogError("Missing containers: Geo or Sky.");
        return;
    }

    LevelInfo levelinfo = GeoContainerGO.transform.parent.gameObject.GetComponent<LevelInfo>();
    if (levelinfo != null)
    {
        J["level"]["levelinfo"] = levelinfo.Save();
    }

    JSONClass geo = SerializeGameObjectAndAllHisChildren(GeoContainerGO.transform, 0);
    JSONClass sky = SerializeGameObjectAndAllHisChildren(SkyContainerGO.transform, 0);

    if (geo != null) J["level"]["geo"] = geo;
    if (sky != null) J["level"]["sky"] = sky;

    string checksum = Sha1.Sha1Sum(J["level"] + "SP9HHDA");
    J["c1"] = Random.Range(11111, 99999).ToString() + Random.Range(11111, 99999).ToString();
    J["c2"] = checksum;
    J["c3"] = Sha1.Sha1Sum(J["c1"] + "x");

    string JSONText = J.ToString();

    string savePath = Application.dataPath + "/_Skidos_BikeRacing/Resources_moved/Levels/" + levelinfo.LevelName + ".json";
    File.WriteAllText(savePath, JSONText);

    sw.Stop();
    Debug.Log($"✅ [LevelManager] Saved level to JSON in {sw.ElapsedMilliseconds} ms: {savePath}");
#endif
}

        /** 
         * serializé padotá objekta bérneĺus (un tad rekursívi katra objekta bérneĺus)
         * serializé tikai waitlistétos komponentus  (SerializeComponent<T> funkçá)
         * ja objekts ir prefabs, tad to serializé ípaśi (lai deserializéjot tas atkal bútu prefabs)
         */
        private static JSONClass SerializeGameObjectAndAllHisChildren(Transform objectContainer, int level)
        {
            JSONClass J = new JSONClass();

            //prefabu tipa noteikśana ir editor-only lieta
#if UNITY_EDITOR

            int maxLevel = 5;
            if (level++ >= maxLevel)
            { //apskata tikai maxLevel skaitu ar límeńiem
                return null;
            }


            int n = 1;
            foreach (Transform o in objectContainer)
            {

                /* @does-not-compute -- seivo visu
                if(o.CompareTag("do-not-save")){ //neseivo objektus, kas notagi neseivošanai
                    continue; 
                } */

                JSONClass JArray = new JSONClass();
                //galvenais info par geimobjektu			
                JArray["name"] = o.name;
                JArray["a"].AsBool = o.gameObject.activeSelf;
                JArray["p"] = JSONHelper.Vector3ToJSONString(o.transform.position);
                JArray["r"] = JSONHelper.QuaternionToJSONString(o.transform.rotation);
                JArray["s"] = JSONHelper.Vector3ToJSONString(o.transform.localScale);

                if (o.tag != "Untagged")
                {
                    JArray["t"] = o.tag;
                }




                if (PrefabUtility.GetPrefabType(o) != UnityEditor.PrefabType.None)
                { //ir prefabs, tam neserializé komponentus un bérneĺus

                    var go = PrefabUtility.GetPrefabParent(o);
                    if (!AssetDatabase.Contains(go))
                    {
                        Debug.LogError("Ev, kļūda seivojot: nevaru saglabāt šo prefabu: " + PrefabUtility.GetPrefabParent(o).name);
                    }
                    JArray["prefabName"] = getResourcePathAndName(go);


                    /** 
                     * prefabiem jáserializé skripti, kas implementé ISaveable interfeisu
                     */
                    MonoBehaviour[] scripts = o.GetComponentsInChildren<MonoBehaviour>(); //visi skripti [,kas extendo MonoBehaviour]
                    if (scripts.Length > 0)
                    {
                        for (int i = 0; i < scripts.Length; i++)
                        {

                            ISaveable s = scripts[i] as ISaveable;
                            if (s != null)
                            { //ja skripts implementé seivośanas interfeisu
                                JArray["prefabScripts"][i] = s.Save();
                                JArray["prefabScripts"][i]["componentName"] = scripts[i].GetType().ToString();
                            }
                        }
                    }

                }


                if (PrefabUtility.GetPrefabType(o) == UnityEditor.PrefabType.None)
                {

                    //serializéju visus gemobjekta komponentus (ja nav komponents nav waitlistéts, tad tiks ignoréts)
                    int compNum = 0;
                    foreach (var component in o.GetComponents<Component>())
                    {
                        JSONClass c = SerializeComponent(component, o.gameObject);
                        if (c != null)
                        {
                            c["componentName"] = component.GetType().ToString(); //komponentam iedodu vél vienu parametru - tás nosaukumu
                            JArray["components"][compNum++] = c;
                        }
                    }

                    //serializéśu bérneĺus
                    JSONClass children = SerializeGameObjectAndAllHisChildren(o, level);
                    if (children != null)
                    { //ja IR bērneļi, tikai tad pievienot Džeisonam, lai neveidotu tukšu elementu
                        JArray["children"] = children;
                    }

                }


                J["gameobjects"][n] = JArray;
                n++;

            }


            if (n == 1)
            { //nav neviens objekts te atrasts un serializēts
                return null;
            }

#endif
            return J;
        }


        /**
         * serializéju padoto geimobjekta komponentu 
         * (joka péc arí padod paśu geimobjektu, ja vajag papildus lietas no citíem komponentiem)
         */
        public static JSONClass SerializeComponent(Component component, GameObject o)
        {
#if UNITY_EDITOR
            string componentName = component.GetType().ToString();
            JSONClass J = new JSONClass();

            if (componentName == "PolyMesh")
            {//polimeśa skripts
                PolyMesh script = component as PolyMesh;
                return script.Save(); //skripts pats sevi serializé
            }


            if (componentName == "UnityEngine.MeshFilter")
            {
                if (o.GetComponent<PolyMesh>() != null)
                { // objektam ir polimeśa skripts, nevajag seivot objekta ǵeometriju, to daudz efektívák ir izveidot no polymesh skripta
                    return null;
                }
                print(o.name);
                MeshFilter mfilter = component as MeshFilter;
                for (int i = 0; i < mfilter.sharedMesh.vertices.Length; i++)
                {
                    J["verts"][i] = JSONHelper.Vector3ToJSONStringPrec(mfilter.sharedMesh.vertices[i]); // precízais vektora serializétájs - dinamiski ǵenerétajiem objektiem vajag precízi
                }

                JSONClass colors32J = new JSONClass();
                bool colorsFound = false;
                for (int i = 0; i < mfilter.sharedMesh.colors32.Length; i++)
                {
                    Color32 col = mfilter.sharedMesh.colors32[i];
                    colors32J["c"][i] = JSONHelper.Color32ToJSONString(col);
                    if (col.r != 0 || col.g != 0 || col.b != 0 || col.a != 0)
                    { //defaultá krása
                        colorsFound = true; // kaut viena krása ir atasta, tad jáseivo visas
                    }
                }
                if (colorsFound)
                {
                    J["colors32"] = colors32J["c"];
                }


                PolyMesh script = o.GetComponent<PolyMesh>();//meśa filtra komponentam jáiedod advancétie UV parametri no polymesh skripta (ja ir polymesh)
                if (script != null)
                {
                    J["uvPosition"] = JSONHelper.Vector2ToJSONString(script.uvPosition);
                    J["uvScale"].AsFloat = script.uvScale;
                    J["uvRotation"].AsFloat = script.uvRotation;
                }

                return J;
            }

            if (componentName == "UnityEngine.MeshRenderer")
            {
                MeshRenderer r = component as MeshRenderer;
                //@note -- varbūt būs vairāki materiāli
                J["material"] = getResourcePathAndName(r.sharedMaterial);
                J["shader"] = r.sharedMaterial.shader.name;
                return J;
            }


            if (componentName == "UnityEngine.SpriteRenderer")
            {
                SpriteRenderer s = component as SpriteRenderer;
                if (s.sprite == null)
                {
                    return null;
                }
                J["texture"] = getResourcePathAndName(s.sprite.texture);
                J["sprite"] = s.sprite.name;
                J["color"] = JSONHelper.ColorToJSONString(s.color);
                J["layer"].AsInt = s.sortingLayerID;
                J["order"].AsInt = s.sortingOrder;


                return J;
            }


            if (componentName == "UnityEngine.EdgeCollider2D")
            {

                EdgeCollider2D c = component as EdgeCollider2D;
                PhysicsMaterial2D m = c.sharedMaterial;
                if (m != null)
                { //ja ir fizikas materiáls
                    J["material"] = c.sharedMaterial.name;
                }
                if (c.points.Length < 2)
                {
                    return null;
                }
                for (int i = 0; i < c.points.Length; i++)
                {
                    J["verts"][i] = JSONHelper.Vector2ToJSONString(c.points[i]);
                }

                return J;
            }


            if (componentName == "UnityEngine.PolygonCollider2D")
            {

                PolygonCollider2D c = component as PolygonCollider2D;
                PhysicsMaterial2D m = c.sharedMaterial;
                if (m != null)
                { //ja ir fizikas materiáls
                    J["material"] = c.sharedMaterial.name;
                }
                if (c.points.Length < 2)
                {
                    return null;
                }
                for (int i = 0; i < c.points.Length; i++)
                {
                    J["verts"][i] = JSONHelper.Vector2ToJSONString(c.points[i]);
                }

                return J;
            }


            //--------------------------ignoréjam-------------------------------

            if (componentName == "UnityEngine.BoxCollider2D")
            {
                Debug.Log("skipojam páréjos kolaideru tipus?");
                return null;
            }

            if (componentName == "UnityEngine.Rigidbody2D")
            {
                Debug.Log("skipojam arí rigidbodijus, rait?");
                return null;
            }

            if (componentName == "UnityEngine.Animator")
            {
                //Debug.Log("Taisi prefabu, ja gribi animáciju!");
                return null;
            }

            if (componentName == "UnityEngine.Transform")
            { //to neserializē šeit, jau serializēts ārpusē
                return null;
            }


            Debug.Log(componentName + " == unrecognized component");

#endif
            return null;
        }

        /**
         * LevelPath vai LevelNameOnly -- kuru límeni ieládét
         * 
         * singlePlayer (vai multiPlayer) ietekmé, ká tiks seivots raidfailińś
         * 
         * ja ir multiPlayer, tad ńemti vérá śie dati:
         * 		bikes,rideData  -- masívi, kur katrá ir ieraksts par attélojamo moci
         * 		pirmais vienmér ir spélétája kontroléts, páréjie ir AI (ja pirmais elements ir null, tad nav spélétája, tikai AI)
         * ja ir singlePlayer, tad tie tiek ignoréti
         * 
         */
        public static void LoadLevel(string LevelPath, string LevelNameOnly, bool singlePlayer = true, string[] bikes = null, string[] ghostDataFileNames = null, bool bulk = false, System.Action finishedCallback = null)
        {

            CleanUp();
            Time.timeScale = 0; //apturu laiku ieládes laiká - lai nesák animét un de-sinhronizét límenjobjektu animáciju
            Camera.main.GetComponent<MonoBehaviour>().StartCoroutine(_LoadLevel(LevelPath, LevelNameOnly, singlePlayer, bikes, ghostDataFileNames, bulk, delegate ()
            {

                //callback, kad ko-rutína izpildíjusies

                if (finishedCallback != null)
                {
                    finishedCallback();
                }

                if (!bulk)
                { //bulk ir load-all-levels >:)
                    SoundManager.ChangeAmbienceForLevel(LevelNameOnly);
                }

            }));

        }

        private static IEnumerator _LoadLevel(string LevelPath, string LevelNameOnly, bool singlePlayer, string[] bikes, string[] ghostDataFileNames, bool bulk = false, System.Action finishedCallback = null)
        {
            loadingProgressPreObjects = 0;
            objectsLoadedTotal = 0;
            objectsLoadPerFrame = 0;
            loadedSinceLastFrame = 0;

            loadedLevel = false;

            BikeGameManager.initialized = false;

            if (!bulk)
            { //for load-all functionality
                UIManager.ToggleScreen(GameScreenType.PopupPreGameLoading);
            }


            loadingProgressPreObjects++;
            yield return null; //yield 1
            loadingProgressPreObjects++;
            //Debug.Log("A" + Time.frameCount);

            //Debug.Log("LoadLevel: " + LevelNameOnly);
#if UNITY_EDITOR
            LevelAutosaver.AutosaveStop();
#endif

            ClearLevel();


            yield return null; //yield 2
            loadingProgressPreObjects++;
            //Debug.Log("B" + Time.frameCount);
            objectsLoadedTotal = 0;
            yield return Camera.main.GetComponent<Startup>().StartCoroutine(DeserializeScene(LevelPath, LevelNameOnly)); ////yield 3

            //        yield return null; //yield x + 3
            //Debug.Log("C" + Time.frameCount);

            if (!bulk)
            {
                if (singlePlayer)
                {

                    //spélétája kontrolétais baiks
                    BikeManager.LoadBike(true, null, BikeDataManager.SingleplayerPlayerBikeRecordName);

                    /*
                     * AI baiks - SP ghostińś
                     */
                    string SPIputData = null;
                    yield return null; //yield x + 4
                                       //            Debug.Log("D" + Time.frameCount);
                    BikeDataManager.LoadLevelGhost(CurrentLevelName);
                    BikeManager.LoadBike(false, SPIputData, "SPGhost");

                }
                else
                {//MP baiki

                    if (bikes != null)
                    {

                        if (bikes.Length >= 1)
                        { //spélétája kontrolétais baiks
                            if (bikes[0] != null)
                            {
                                BikeManager.LoadBike(true, null, BikeDataManager.MultiplayerPlayerBikeRecordName);
                            }
                        }
                        if (bikes.Length >= 2)
                        { //viens vai vairáki ghostińi
                            for (int i = 1; i < bikes.Length; i++)
                            {
                                if (BikeDataManager.LoadLevelGhost(ghostDataFileNames[i], "MPGhost" + i))
                                {
                                    BikeManager.LoadBike(false, null, "MPGhost" + i);
                                }
                            }
                        }


                    }
                    else
                    {
                        Debug.LogError("MP bez daliibniekiem ?");
                    }
                }
                yield return null;
                BikeGameManager.Init(singlePlayer);//call after ai bike was loaded
            }


            yield return null; //yield x + 5 - last

            if (!bulk)
            {
                BikeSounds bikeSounds = Camera.main.GetComponentInChildren<BikeSounds>();
                if (bikeSounds != null)
                {
                    Camera.main.GetComponentInChildren<BikeSounds>().Init();// zem kameras esośs objekts, kas pieskata mocha skanjas
                }
                else
                {
                    Debug.LogError("nav atrasts mocha skanju objekts!");
                }


                UIManager.ToggleScreen(GameScreenType.PopupPreGameLoading);
            }

            if (finishedCallback != null)
            {
                finishedCallback();
            }

            loadedLevel = true;

        }

        /**
         * jápadod vai nu pilnais ceĺs ar paplaśinájumu un faila nosaukumu
         * vai tikai faila nosaukums, bez paplasínájuma 
         * 
         * @note -- darbojas tikai límeńu dirá, bez subdirám
         */
        private static IEnumerator DeserializeScene(string LevelPath, string LevelNameOnly)
        {
            Debug.Log($"[LevelManager] Loading level: {LevelNameOnly}");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // Load the level from Resources folder
            TextAsset levelAsset = Resources.Load<TextAsset>("BikeRaceLevels/" + LevelNameOnly);
            if (levelAsset == null)
            {
                Debug.LogError($"❌ Could not load level from Resources: BikeRaceLevels/{LevelNameOnly}.json");
                Debug.LogError($"❌ Make sure the level file exists in Resources/BikeRaceLevels/ folder");
                yield break;
            }

            string jsonText = levelAsset.text;

            JSONNode N = JSON.Parse(jsonText);
            string checksum = Sha1.Sha1Sum(N["level"] + "SP9HHDA");
            if (!string.Equals(N["c2"], checksum))
            {
                Debug.LogWarning("⚠️ Checksum mismatch — level file might be corrupted.");
                yield break;
            }

            CurrentlyOpenLevelPath = LevelNameOnly;
            CurrentLevelName = LevelNameOnly;

            GameObject GeoContainerGO = GameObject.Find("GeoContainer");
            GameObject SkyContainerGO = GameObject.Find("SkyContainer");
            deserializedObjectCount = 0;
            yield return Camera.main.GetComponent<Startup>().StartCoroutine(DeserializeGameObjectAndAllHisChildren(GeoContainerGO.transform, N["level"]["geo"], 0));
            yield return Camera.main.GetComponent<Startup>().StartCoroutine(DeserializeGameObjectAndAllHisChildren(SkyContainerGO.transform, N["level"]["sky"], 0));

            // Try to find LevelInfo component on LevelContainer
            GameObject LevelContainerGO = GameObject.Find("LevelContainer");
            LevelInfo levelinfo = null;
            
            if (LevelContainerGO != null)
            {
                levelinfo = LevelContainerGO.GetComponent<LevelInfo>();
                Debug.Log($"[LevelManager] Found LevelContainer: {LevelContainerGO.name}");
            }
            else
            {
                Debug.LogError("[LevelManager] LevelContainer not found!");
            }
            
            Debug.Log($"[LevelManager] GeoContainerGO: {GeoContainerGO != null}");
            Debug.Log($"[LevelManager] GeoContainerGO.parent: {GeoContainerGO.transform.parent != null}");
            Debug.Log($"[LevelManager] LevelInfo component: {levelinfo != null}");
            Debug.Log($"[LevelManager] LevelInfo data in JSON: {N["level"]["levelinfo"] != null}");
            
            if (levelinfo != null && N["level"]["levelinfo"] != null)
            {
                levelinfo.LevelName = LevelNameOnly;
                Debug.Log($"[LevelManager] Set LevelName to: {levelinfo.LevelName}");
                levelinfo.Load(N["level"]["levelinfo"]);
                Debug.Log($"[LevelManager] After Load, LevelName is: {levelinfo.LevelName}");
            }
            else
            {
                Debug.LogError($"[LevelManager] Failed to find LevelInfo component or levelinfo data. LevelInfo: {levelinfo != null}, LevelInfoData: {N["level"]["levelinfo"] != null}");
                
                // Try to set LevelName even if Load fails
                if (levelinfo != null)
                {
                    levelinfo.LevelName = LevelNameOnly;
                    Debug.Log($"[LevelManager] Set LevelName as fallback: {levelinfo.LevelName}");
                }
            }

            sw.Stop();
            Debug.Log($"✅ [LevelManager] Loaded level in {sw.ElapsedMilliseconds} ms");
        }
        /**
                 * deserializé padoto Dźeisonu padotajá geimobjektá
                 * rekursívi izveido arí savus bérneĺus (ja Dźeisoná rakstits, ka ir bérni)
                 */

        private static int deserializedObjectCount = 0;
        private const int MAX_OBJECTS_PER_FRAME = 50; // Tune this value for performance
        private static IEnumerator DeserializeGameObjectAndAllHisChildren(Transform objectContainer, JSONNode N, int level)
        {
            level++;

            deserializedObjectCount++;

            // Inject yield every N objects to keep UI responsive on device
            if (deserializedObjectCount % MAX_OBJECTS_PER_FRAME == 0)
            {
                yield return null;
            }
            //apskata "gameobjects" elementus
            for (int i = 0; i < N["gameobjects"].Count; i++)
            {

                //---debug::detalizéti objektu ieládes laiki
                /*
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                //*/


                if (!BikeDataManager.SettingsHD && N["gameobjects"][i]["name"].ToString() == "\"shadingBelt\"")
                { //no HD, no shading belt
                    continue;
                }



                if (!N["gameobjects"][i]["a"].AsBool)
                {
#if UNITY_EDITOR
                    //Debug.LogWarning("Liimenii atrasts neaktiivs objekts: " + N["gameobjects"][i]["name"]);
                    continue;
#else
				continue; //uz ierícém neládé bezjédzígus izslégtos objektus
#endif
                }


                GameObject o;

                if (N["gameobjects"][i]["prefabName"].ToString().Length > 0)
                { //prefabs

                    string prefabname = N["gameobjects"][i]["prefabName"];


                    if (!BikeDataManager.SettingsHD && prefabname == "Prefabs/InteractiveObject/Coin")
                    { //no HD, no rotating prefabs for you
                        prefabname = "CoinNonHD";
                        //prefabname = "Prefabs/InteractiveObject/CoinNonHD";
                    }

                    string result = null;
                    string data = prefabname;
                    int lastSlashIndex = data.LastIndexOf('/');
                    if (lastSlashIndex != -1) // Check if '/' was found
                    {
                        result = data.Substring(lastSlashIndex + 1);
                        Debug.Log(result); // This will print "StuntZone" in the console
                    }
                    else
                    {
                        result = prefabname;
                        Debug.LogError("Separator character not found in the string.");
                    }
                    GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources(result) as GameObject;
                    //Debug.Log("<color=yellow>Prefab is loading from = </color>" + prefab);
                    //GameObject prefab = Resources.Load(prefabname) as GameObject;

                    if (prefab == null)
                    {
                        Debug.LogError("Nevar atrast prefabu \"" + prefabname + "\"!");
                        continue;
                    }
                    //Debug.LogError("ifff calling1");
                    //@todo -- daźreiz nav laba prefaba adrese (prefabs nav resursu dirá), un ir err: NullReferenceException: Object reference not set to an instance of an object
//#if UNITY_EDITOR
                    //o = PrefabUtility.InstantiatePrefab(prefab) as GameObject; //redaktorá (nav PLAY nospiests) instancéjot ar parasto metodi, zúd prefaba konekcija, tápéc jálieto śis veids
//#else
				o = Instantiate(prefab) as GameObject;
                Debug.LogError("Else calling1");
//#endif
                    PleaseCheckIfPrefabDoesntNeedHisChilPolymeshBuilt(o);
                    //Debug.LogError("endif calling1");

                    /**
                     * Skatás visus prefaba skriptus, kas implementé ISerializible interfeisu, 
                     * vińi paśi ir sev serializéjuśi savus datus JSONá, 
                     * piemeklés vińu JSONdatus pécvińu nosaukuma un dos vińiem deserializét 
                     */
                    MonoBehaviour[] scripts = o.GetComponentsInChildren<MonoBehaviour>(); //visi skripti [,kas extendo MonoBehaviour]

                    if (scripts.Length > 0)
                    {
                        for (int j = 0; j < scripts.Length; j++)
                        {
                            //Debug.LogError("--------------------------------------------------------------->" + scripts[j]);
                            ISaveable s = scripts[j] as ISaveable;


                            //Debug.LogError("<<<<<<<<<<<<<<<< " + s);
                            if (s != null)
                            {
                                //Debug.Log("Savable found");
                                //ja skripts implementé seivośanas interfeisu
                                //Debug.Log("skatamies prefabskriptu " + scripts[i].GetType().ToString());
                                for (int k = 0; k < N["gameobjects"][i]["prefabScripts"].Count; k++)
                                { // ikviens serializétais skripts JSONá

                                    string jScriptName = N["gameobjects"][i]["prefabScripts"][k]["componentName"].ToString().Replace("\"", "");


                                    if (jScriptName.Equals(RemoveVasundhara_fun(scripts[j].GetType().ToString())))
                                    {
                                        //Debug.LogError("Data is passing");
                                        s.Load(N["gameobjects"][i]["prefabScripts"][k]);
                                    }
                                    //if (jScriptName.Equals(scripts[j].GetType().ToString()))
                                    //{
                                    //    Debug.LogError("Data is passing");
                                    //    s.Load(N["gameobjects"][i]["prefabScripts"][k]);
                                    //}
                                }
                            }
                        }
                    }


                }
                else
                {
                    o = new GameObject();
                    if (N["gameobjects"][i]["t"].ToString().Length > 0)
                    {
                        o.tag = N["gameobjects"][i]["t"]; //tagus atjaunos tikai ne-prefabiem	
                    }
                }



                //galvenie geimobjekta parametri
                o.name = N["gameobjects"][i]["name"];
                o.SetActive(N["gameobjects"][i]["a"].AsBool);
                o.transform.position = JSONHelper.Vector3FromJSONString(N["gameobjects"][i]["p"]);
                o.transform.rotation = JSONHelper.QuaternionFromJSONString(N["gameobjects"][i]["r"]);
                o.transform.localScale = JSONHelper.Vector3FromJSONString(N["gameobjects"][i]["s"]);

                //geimobjektam pievienos komponentus
                for (int j = 0; j < N["gameobjects"][i]["components"].Count; j++)
                {
                    DeserializeComponent(ref o, N["gameobjects"][i]["components"][j]);
                }

                //---debug::detalizéti objektu ieládes laiki
                /*
                sw.Stop();
                Debug.Log("objekts: " + o.name + "  Elapsed=" + sw.Elapsed);
                //*/

                o.transform.parent = objectContainer; //novieto geimobjektu pareizajá vetá scéná

                objectsLoadedTotal++;
                loadedSinceLastFrame++;

                //if (loadedSinceLastFrame >= objectsLoadPerFrame)
                //{
                //    //Debug.Log("OL: " + objectsLoadedTotal);
                //    loadedSinceLastFrame = 0;

                //    yield return null;
                //}

                if (N["gameobjects"][i]["children"].Count > 0)
                {
                    deserializedObjectCount = 0;
                    yield return Camera.main.GetComponent<Startup>().StartCoroutine(DeserializeGameObjectAndAllHisChildren(o.transform, N["gameobjects"][i]["children"], level)); //deserializé katru bérnu - jápadod Dźeisona elements, kurá ir "gameobjects" elementi
                }

            }
        }

        static string RemoveVasundhara_fun(string _type)
        {
            if (_type.Contains("vasundharabikeracing."))
            {
                _type = _type.Replace("vasundharabikeracing.", "");
                //Debug.LogError($"After Remove Vasundhara.PepiHospital2_FluClininc. real name :: {_name} Prefab");
            }
            return _type;
        }

        /**
         * deserializé komponenti, kas padota Dźeisona elementá
         * ja izdodas, tad to pievienota padotajam geimobjektam
         * @note -- jádeserializé visas komponentes, ko prot serializét śís funkças evil twin: SerializeComponent()
         */
        public static void DeserializeComponent(ref GameObject o, JSONNode N)
        {

            //Debug.Log("C: " + N["componentName"] );
            string componentName = N["componentName"];

            if (componentName == "PolyMesh")
            {
                PolyMesh script = o.AddComponent<PolyMesh>();
                script.Load(N);//skripts pats sevi deserializé

                script.BuildMesh(false); //@does-not-compute
                /*
                #if UNITY_EDITOR
                script.BuildMesh(true); //redaktorá párbúvés automátiskos apakśobjektus - kolaideri, foliage utt
                #else
                script.BuildMesh(false); //spélé nepárbúvés - tikai ieládés
                #endif */
                return;
            }

            if (componentName == "UnityEngine.MeshFilter")
            {

                Vector3[] vertices = new Vector3[N["verts"].Count];
                for (int i = 0; i < N["verts"].Count; i++)
                {
                    vertices[i] = JSONHelper.Vector3FromJSONString(N["verts"][i]);
                }

                Vector3 uvPosition = Vector3.zero;
                float uvRotation = 0;
                float uvScale = 1;

                if (N["uvPosition"].ToString().Length > 4)
                { //ja serializeá vektora garums ir lieláks, par 4, tátad tur ir serializéts vekttors
                    uvPosition = JSONHelper.Vector2FromJSONString(N["uvPosition"]);
                    uvRotation = N["uvRotation"].AsFloat;
                    uvScale = N["uvScale"].AsFloat;
                }

                var scale = uvScale != 0 ? (1 / uvScale) : 0;
                var matrix = Matrix4x4.TRS(-uvPosition, Quaternion.Euler(0, 0, uvRotation), new Vector3(scale, scale, 1));
                var uv = new Vector2[vertices.Length];
                for (int i = 0; i < uv.Length; i++)
                {
                    var p = matrix.MultiplyPoint(vertices[i]);
                    uv[i] = new Vector2(p.x, p.y);
                }


                Color32[] colors32 = new Color32[N["colors32"].Count];
                for (int i = 0; i < N["colors32"].Count; i++)
                {
                    colors32[i] = JSONHelper.Color32FromJSONString(N["colors32"][i]);
                }

                MeshFilter mf = o.AddComponent<MeshFilter>();
                mf.sharedMesh = new Mesh();
                Mesh mesh = mf.sharedMesh;

                //Update the mesh
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.colors32 = colors32;
                mesh.triangles = Triangulate.Points(vertices);
                mesh.RecalculateNormals();
                ;

                //Debug.Log(o.name + "  vertices: " + mf.sharedMesh.vertices.Length);

                return;
            }

            string result = null;
            if (componentName == "UnityEngine.MeshRenderer")
            {
                MeshRenderer mr = o.AddComponent<MeshRenderer>();

                string data = N["material"];
                int lastSlashIndex = data.LastIndexOf('/');
                if (lastSlashIndex != -1) // Check if '/' was found
                {
                    result = data.Substring(lastSlashIndex + 1);
                    Debug.Log(result); // This will print "StuntZone" in the console
                }
                else
                {
                    result = N["material"];
                    // Handle the case where '/' was not found
                    Debug.LogError("Separator character not found in the string.");
                }

                Debug.Log(result + "<color=yellow>Material is loading from = </color>" + N["material"]);
                Material mat = (Material)LoadAddressable_Vasundhara.Instance.GetMaterial_Resources(result);
                Debug.Log("<color=yellow>Material Loaded Name = </color>" + mat);
                //Material mat = (Material)Resources.Load(N["material"], typeof(Material));
                if (mat != null)
                {
                    mr.sharedMaterial = mat;
                    mr.sharedMaterial.shader = LoadAddressable_Vasundhara.Instance.GetMotoShader(N["shader"]);
                    //mr.sharedMaterial.shader = Shader.Find(N["shader"]);
                }
                else
                {
                    Debug.LogError("Neatrodu materiaalu: " + N["material"]);
                }
                return;

            }

            if (componentName == "UnityEngine.SpriteRenderer")
            {

                Sprite s = GetSprite(N["texture"], N["sprite"]);

                if (s != null)
                {
                    SpriteRenderer sr = o.AddComponent<SpriteRenderer>();
                    sr.sprite = s;
                    sr.color = JSONHelper.ColorFromJSONString(N["color"]);
                    sr.sortingLayerID = N["layer"].AsInt;
                    sr.sortingOrder = N["order"].AsInt;
                }
                return;
            }

            if (componentName == "UnityEngine.EdgeCollider2D")
            {
                EdgeCollider2D c = o.AddComponent<EdgeCollider2D>();
                Vector2[] vertices = new Vector2[N["verts"].Count];
                for (int i = 0; i < N["verts"].Count; i++)
                {
                    vertices[i] = JSONHelper.Vector2FromJSONString(N["verts"][i]);
                }
                c.points = vertices;

                PhysicsMaterial2D mat;
                if (N["material"].ToString().Length > 0)
                { //ja tiek norádíts materiáls, tad meklés to
                    mat = (PhysicsMaterial2D)LoadAddressable_Vasundhara.Instance.GetPhysicsMaterial_Resources(N["material"]);
                    Debug.Log("<color=yellow>Prefab loaded Name = </color>" + mat);
                    //mat = (PhysicsMaterial2D)Resources.Load("PhysicsMaterials/" + N["material"], typeof(PhysicsMaterial2D));
                    if (mat == null)
                    {
                        Debug.LogError("nav atrasts fizikas materiaals: " + N["material"]);
                    }
                    else
                    {
                        c.sharedMaterial = mat; //bija norádíts un ir atrasts
                    }
                }

                return;
            }

            if (componentName == "UnityEngine.PolygonCollider2D")
            {
                PolygonCollider2D c = o.AddComponent<PolygonCollider2D>();
                Vector2[] vertices = new Vector2[N["verts"].Count];
                for (int i = 0; i < N["verts"].Count; i++)
                {
                    vertices[i] = JSONHelper.Vector2FromJSONString(N["verts"][i]);
                }
                c.points = vertices;
                c.isTrigger = true;

                PhysicsMaterial2D mat;
                if (N["material"].ToString().Length > 0)
                { //ja tiek norádíts materiáls, tad meklés to
                    Debug.Log("<color=yellow>Prefab is loading from = </color>" + N["material"]);
                    mat = (PhysicsMaterial2D)LoadAddressable_Vasundhara.Instance.GetPhysicsMaterial_Resources(N["material"]);
                    Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + mat);
                    //mat = (PhysicsMaterial2D)Resources.Load("PhysicsMaterials/" + N["material"], typeof(PhysicsMaterial2D));
                    if (mat == null)
                    {
                        Debug.LogError("nav atrasts fizikas materiaals: " + N["material"]);
                    }
                    else
                    {
                        c.sharedMaterial = mat; //bija norádíts un ir atrasts
                    }
                }

                return;
            }




            Debug.Log(componentName + " == unrecognized component :: Deserializing");

        }

        /**
         * atgriezís pierpasíto spraitu no pieprasítá atlasa
         * piekeśos ieládétos atlasus (lídz atmińas tíríśanai)
         */

        public static string CheckDuplicateName(string atlasName, string spriteName)
        {
            string Name = null;

            if (atlasName == "visuals/Sprites/Surface_Jungle")
            {
                Name = spriteName + "Jungle";
            }
            else if (atlasName == "visuals/Sprites/Surface_Desert")
            {
                Name = spriteName + "Desert";
            }
            else if (atlasName == "visuals/Sprites/Surface_Moon")
            {
                Name = spriteName + "Moon";
            }
            else
            {
                Name = atlasName;
                Debug.LogError("Sprite not found check Atlas Name = " + atlasName);
                Debug.LogError("Sprite Name = " + spriteName);
            }

            return Name;
        }
        public static Sprite GetSprite(string atlasName, string spriteName)
        {
            Debug.Log("AtlasName = " + atlasName + "   SpriteName = " + spriteName);

            if (spriteName == "surface_3" || spriteName == "surface_2" || spriteName == "surface_1")
            {
                spriteName = CheckDuplicateName(atlasName, spriteName);
            }
            Dictionary<string, Sprite> atlas;
            if (spriteAtlasCache.TryGetValue(spriteName, out atlas))
            { //vai śáds atlass ir atvérts
                Sprite s;
                if (atlas.TryGetValue(spriteName, out s))
                { //vai atlasá ir śáds spraits
                    Debug.Log("<color=yellow> Sprite Available Name = " + spriteName + "</color>");

                    return s;
                }
                else
                {
                    Debug.LogError("Ev, tiek prasiits neeksisteejoshs spraits \"" + spriteName + "\" no eksisteejosha atlasa \"" + atlasName + "\" !");
                    return null;
                }
            }
            else
            { //neatvérts atlass

                //ieládéju visus atlasa spraitus
                Debug.Log("Else caleed");
                Sprite sprite = LoadAddressable_Vasundhara.Instance.GetSprite_Resources(spriteName);
                
                if (sprite != null)
                {
                    Debug.Log("<color=yellow>" + sprite.name + "</color>");
                }
                else
                {
                    Debug.LogError($"Failed to load sprite: {spriteName}");
                }

                if (sprite != null)
                {
                    atlas = new Dictionary<string, Sprite>();

                    atlas[sprite.name] = sprite;

                    spriteAtlasCache[spriteName] = atlas;
                }
                else
                {
                    Debug.LogError("Ev, tiek prasiits spraits \"" + spriteName + "\" no neeksisteejosha atlasa \"" + atlasName + "\" !");
                    return null;
                }
                //Debug.Log("rekursijas brídinájums " + atlasName + "-"+spriteName);
                return GetSprite(atlasName, spriteName); //atliek tikai izvilkt no keśa
            }
            //Dictionary<string, Sprite> atlas;
            //if (spriteAtlasCache.TryGetValue(atlasName, out atlas))
            //{ //vai śáds atlass ir atvérts
            //    Sprite s;
            //    if (atlas.TryGetValue(spriteName, out s))
            //    { //vai atlasá ir śáds spraits			
            //        return s;
            //    }
            //    else
            //    {
            //        Debug.LogError("Ev, tiek prasiits neeksisteejoshs spraits \"" + spriteName + "\" no eksisteejosha atlasa \"" + atlasName + "\" !");
            //        return null;
            //    }
            //}
            //else
            //{ //neatvérts atlass

            //    //ieládéju visus atlasa spraitus
            //    Sprite[] sprites = Resources.LoadAll<Sprite>(atlasName);
            //    if (sprites.Length > 0)
            //    {
            //        atlas = new Dictionary<string, Sprite>();
            //        for (int i = 0; i < sprites.Length; i++)
            //        {
            //            atlas[sprites[i].name] = sprites[i]; //atlasa diktá salieku spraitus pec to nosaukumiem
            //        }
            //        spriteAtlasCache[atlasName] = atlas;
            //    }
            //    else
            //    {
            //        Debug.LogError("Ev, tiek prasiits spraits \"" + spriteName + "\" no neeksisteejosha atlasa \"" + atlasName + "\" !");
            //        return null;
            //    }
            //    //Debug.Log("rekursijas brídinájums " + atlasName + "-"+spriteName);
            //    return GetSprite(atlasName, spriteName); //atliek tikai izvilkt no keśa
            //}

        }


        /*
         * jáiztíra keśs, jápabiksta GC
         */
        public static void CleanUp()
        {
            spriteAtlasCache = new Dictionary<string, Dictionary<string, Sprite>>();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        /**
         * padotajam assetam (jebkádam objektam) atrod vińa direktoriju resursu dirá + vińa nosaukumu bez paplaśinájuma
         */
        public static string getResourcePathAndName(Object asset)
        {

#if UNITY_EDITOR

            string path = AssetDatabase.GetAssetPath(asset); // Assets/Resources/Visuals/Sprites/Seenes.png

            int dotPosition = path.LastIndexOf(".");
            if (dotPosition > 0)
            {
                path = path.Substring(0, dotPosition); // Assets/Resources/Visuals/Sprites/Seenes
            }

            path = path.Replace("Assets/Resources/", ""); // Visuals/Sprites/Seenes

#else
		string path = "";
#endif


            return path;
        }

        /**
         * hakińś prefabobjektiem, kas sák savu dzívi ar drag-n-drop: 
         * tiem automátiski netiek uzbúvéti meśi,
         * te ieslédzu meśu búvéśanu ikvienam prefaba objektam (kam ir polymesh skripts)
         */
        private static void PleaseCheckIfPrefabDoesntNeedHisChilPolymeshBuilt(GameObject o)
        {
            //Debug.Log("rebuilding " + o.name  );
            PolyMesh[] scripts = o.GetComponentsInChildren<PolyMesh>();
            foreach (PolyMesh script in scripts)
            {
                //Debug.Log("rebuilding " + o.name + " CCC" );
                script.BuildMesh(true);
            }

        }


        // Compresses the supplied memory stream, naming it as zipEntryName, into a zip,
        // which is returned as a memory stream or a byte array.
        //
        public static MemoryStream CreateToMemoryStream(MemoryStream memStreamIn, string zipEntryName)
        {

            Debug.Log("ZipOutputStream CreateToMemoryStream ");
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
            zipStream.SetLevel(9);//lieotjam 9, jo tam ir nenozímíga ietekme uz dekompresijas átrumu, toties ietaupam megabaitus
            ZipEntry newEntry = new ZipEntry(zipEntryName);
            zipStream.PutNextEntry(newEntry);
            StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;
            zipStream.Close();
            outputMemStream.Position = 0;
            return outputMemStream;
        }


        public static void DeleteDayOldSavegameBackups(string sourceDirectoryPath)
        {
 Debug.Log("ZipOutputStream CreateToMemoryStream ");
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Level-autosaves/");
            System.IO.DirectoryInfo source = new System.IO.DirectoryInfo(sourceDirectoryPath);
            foreach (System.IO.FileInfo fi in source.GetFiles())
            {
                var creationTime = fi.CreationTime;

                if (creationTime < (System.DateTime.Now - new System.TimeSpan(1, 0, 0, 0)))
                {
                    fi.Delete();
                    //print("rm " + fi.Name);
                }
            }

        }


        /**
         * iztíra límeni, pártrauc to renderét 
         * 
         * 1) @todo -- jáizsauc atverot galveno MP logu (gan líga, gan draugi) - péc MP spéles
         * 2) tiek izsaukts atverot trases chúsku
         */
        public static void KillLevel()
        {
            //print("Killing LevelContainer!");
            GameObject LevelContainerGO = GameObject.Find("LevelContainer");
            if (LevelContainerGO != null)
            {
                Destroy(LevelContainerGO);
                CleanUp();
            }

            Time.timeScale = 1;
            BikeGameManager.initialized = false;

            //izdzéś tutoriáĺa prefabu (kas tika ieládéts tikai 1. un 2. límení)
            if (PreGameBehaviour.TutorialGo != null)
            {
                Destroy(PreGameBehaviour.TutorialGo);
            }

            AdManager.JustFinishedRace = true;
            BikeDataManager.WatchedAdToGetAnExtraReplay = false; // required in mp finish and pause

        }





    }





}
