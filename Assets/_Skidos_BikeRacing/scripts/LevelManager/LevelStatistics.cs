namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data_MainProject;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Text.RegularExpressions;
using SimpleJSON;

#pragma warning disable 0219
#pragma warning disable 0414
#if UNITY_EDITOR
//funkcijas UnityEditora Menu: LevelManager -> save, analyze
public class LevelStatistics : MonoBehaviour
{

    private static Dictionary<string, GameObject> LevelContainers = new Dictionary<string, GameObject>();



    public static void LoadAll()
    {
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();		
        //sw.Start();

        CloseAll();
        string csvLevelList = System.IO.File.ReadAllText(Application.dataPath + "/Resources/Levels/superlevellist.csv");


        string[] names = csvLevelList.Split('\n');
        LoadOneLevel(names, 0);


        //sw.Stop();
        //Debug.Log(LevelContainers.Count+ " levels loaded   Elapsed=" + sw.Elapsed );


        //padara aktívus
        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {
            lvl.Value.SetActive(true);
        }

    }

    //rekursívi ieládé pa vienam límenim, sagaida, kad gatavs, izpilda sevi vélreiz
    private static void LoadOneLevel(string[] names, int id)
    {
        /*
		if(id >= 2){
			Debug.Log("enuff");
			return;
		} //*/

        string levelname = names[id];
        if (levelname.Length > 0)
        {
            Debug.Log("bliezham!");
            LevelManager.LoadLevel("", levelname, true, null, null, true, delegate ()
            {

                //śis kólbeks tiek izsaukts, kad límenis ir ieládéts:

                GameObject container = GameObject.Find("LevelContainer");
                container.name = "LevelContainer_" + levelname; //jápárdévé konteiners unikálá várdá
                container.SetActive(false); // un jáizslédz, lai netiktu izdzésts, ieládéjot nákamo
                LevelContainers[levelname] = container;
                LoadOneLevel(names, ++id);
                Debug.Log("LoadAll: " + levelname + " loaded");

            });// ieládé límeni, izveidojot LevelContainer

        }
    }


    public static void Analyse()
    {

        OrderedList_BikeRace<string, OrderedList_BikeRace<string, int>> AllSprites = new OrderedList_BikeRace<string, OrderedList_BikeRace<string, int>>();  //sprite_name => (level_name => number_of_this_sprite_in_this_level)


        Debug.Log("izanalizeets");


        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {

            if (lvl.Value == null)
            {
                continue;
            }
            string levelID = lvl.Key;

            SpriteRenderer[] sprites = lvl.Value.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++)
            {

                if (sprites[i] == null || sprites[i].name == null || sprites[i].sprite == null)
                {
                    continue;
                }

                string spriteAtlas = "-nuttn-";
                if (sprites[i].sprite.texture != null)
                {
                    spriteAtlas = LevelManager.getResourcePathAndName(sprites[i].sprite.texture);
                }

                string spriteName = sprites[i].sprite.name;

                string spriteID = spriteAtlas + ":" + spriteName;


                if (!AllSprites.ContainsKey(spriteID))
                { //jauns spraits - jaizviedo vińam jauns Dicts, ar visiem límeńiem
                    AllSprites[spriteID] = new OrderedList_BikeRace<string, int>(); //límenis => cik śádu spraitu ir śajá límení
                    foreach (KeyValuePair<string, GameObject> zeroingLevel in LevelContainers)
                    {
                        AllSprites[spriteID][zeroingLevel.Key] = 0;
                    }
                }

                AllSprites[spriteID][levelID]++;
            }
            //Debug.Log(lvl.Value.name + " has " + sprites.Length  + " sprites ");

        }


        //CSV
        //1. rinda: kolonu virsraksti - límeńu nosaukumi
        string csv = "Fails:Spraits"; //1. kolona, tukśa
        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {
            csv += "," + lvl.Key;
        }



        //rindas - spraiti
        foreach (KeyValuePair<string, OrderedList_BikeRace<string, int>> spr in AllSprites)
        {
            csv += "\n";

            //1. kolona: spraita nosaukums
            csv += spr.Key;

            foreach (KeyValuePair<string, int> occurences in spr.Value)
            {
                //páréjás kolonas: spraita sastopamíba śajá límení
                csv += "," + occurences.Value;
            }


        }


        //Debug.Log(csv);
        string path = Application.dataPath + "/Resources/Levels/statistics.csv";
        System.IO.File.WriteAllText(path, csv);
        Debug.Log("Ver valjaa ar exeliiti: " + path);

    }


    public static void SaveAll()
    {
        //visus izslédz
        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {
            lvl.Value.SetActive(false);
        }


        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {
            lvl.Value.SetActive(true); //ieslédz apskatámo
            string lvlName = lvl.Value.GetComponent<LevelInfo>().LevelName; //LevelInfo skriptam uzprasa límeńa nosaukumu
            string lvlPath = Application.dataPath + "/Resources/Levels/" + lvlName + ".bytes";
            LevelManager.SerializeScene(lvlPath); //seivotájs meklés geo un sky konteinerus (redzés tikai śí LevelContainter bérnus, jo paréjie konteineri ir neaktívi)
            lvl.Value.SetActive(false);  //izslédz apskatámo
        }


        //visus ieslédz atpakaĺ
        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {
            lvl.Value.SetActive(true);
        }

        Debug.Log(LevelContainers.Count + " levels saved");

    }


    /**
	 * pagalam neefektívá veidá likvidés visus LevelContainer objektus (pat, ja tie ir pársaukti)
	 */
    public static void CloseAll()
    {


        //tikai ieslégtie 
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (gameObj.name.Length >= 14 && gameObj.name.Substring(0, 14) == "LevelContainer")
            {
                DestroyImmediate(gameObj);
            }
        }

        //zinámie ieládétie, arí izslégtie
        foreach (KeyValuePair<string, GameObject> lvl in LevelContainers)
        {
            DestroyImmediate(lvl.Value);
        }



    }



    static string OUTPUT = "";
    static Dictionary<string, Dictionary<string, string>> SpritesInLevels = new Dictionary<string, Dictionary<string, string>>(); // level = > (spritePath => spriteName)
    static Dictionary<string, string> ReplaceSprites1 = new Dictionary<string, string>();
    static Dictionary<string, string> ReplaceSprites2 = new Dictionary<string, string>();
    static Dictionary<string, string> ReplaceSprites3 = new Dictionary<string, string>();

    public static void AnalyzeJSON(bool rebuild)
    {

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();


        if (rebuild)
        {


            //individuálie
            //#pass




            //visi spraiti no diras
            //#pass


            //brute force fixit
            //#pass


        }



        string csvLevelList = System.IO.File.ReadAllText(Application.dataPath + "/Resources/Levels/superlevellist.csv");
        string[] names = csvLevelList.Split('\n');

        OUTPUT = "";
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length < 3)
            {
                continue;
            }
            if (i >= 3) break;

            string levelname = names[i];
            string path = Application.dataPath + "/Resources/Levels/" + levelname + ".bytes";

            MemoryStream zippedMs = new MemoryStream();
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                zippedMs.Write(bytes, 0, (int)file.Length);
            }


            string jsontext = "";
            byte[] zippedBytes = zippedMs.ToArray();

            using (MemoryStream mem = new MemoryStream(zippedBytes))
            using (ZipInputStream zipStream = new ZipInputStream(mem))
            {
                ZipEntry currentEntry;
                while ((currentEntry = zipStream.GetNextEntry()) != null)
                {
                    byte[] data = new byte[currentEntry.Size];
                    zipStream.Read(data, 0, data.Length);

                    jsontext = System.Text.Encoding.UTF8.GetString(data);
                }
            }

            System.IO.File.WriteAllText(path + ".before.json", jsontext); //pieseivo kaa JSON failinju blakus originaalam
            JSONNode N = JSON.Parse(jsontext);

            SpritesInLevels[levelname] = new Dictionary<string, string>();
            AnalyzeJSONCheckNodeAndItsKids(rebuild, levelname, 0, N["level"]["geo"]);

            //System.IO.File.WriteAllText(path + ".after.json", N.ToString()); 

            if (rebuild)
            {
                //sazipo memorystream un ieraksta to failá
                MemoryStream streamToZip = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(N.ToString()));
                MemoryStream zipped = LevelManager.CreateToMemoryStream(streamToZip, "lvl");

                using (FileStream file = new FileStream(path, FileMode.OpenOrCreate, System.IO.FileAccess.Write))
                {
                    byte[] bytes = new byte[zipped.Length];
                    zipped.Read(bytes, 0, (int)zipped.Length);
                    file.Write(bytes, 0, bytes.Length);
                    zipped.Close();
                }
            }



        }

        foreach (var level in SpritesInLevels)
        {
            OUTPUT += "\nL:" + level.Key;

            foreach (var sprite in level.Value)
            {
                OUTPUT += "\nS:" + sprite.Key;
            }
        }
        print(OUTPUT);

        string finalpath = Application.dataPath + "/Resources/Levels/statistics.csv";
        System.IO.File.WriteAllText(finalpath, OUTPUT);
        Debug.Log("Ver valjaa ar exeliiti: " + finalpath);

        sw.Stop();
        Debug.Log("rebuild:" + rebuild + "  chiki-chiki bumb-bum  Elapsed=" + sw.Elapsed);

    }

    private static void AnalyzeJSONCheckNodeAndItsKids(bool rebuild, string levelname, int depth, JSONNode N)
    {

        for (int i = 0; i < N["gameobjects"].Count; i++)
        {

            //apskataamais objekts ir spraits (jo satur komponenti ar nosaukumu UnityEngine.SpriteRenderer)
            if (N["gameobjects"][i]["components"] != null)
            {
                /*
                                for(int j = 0; j < N["gameobjects"][i]["components"].Count; j++){ //visi komponenti shajaa geimobjekt (spraitiem, gan parasti ir tikai 1 - SpriteRenderer 


                                    if(N["gameobjects"][i]["components"][j]["componentName"].ToString() == "\"UnityEngine.SpriteRenderer\""){


                                        string spritePath = N["gameobjects"][i]["components"][j]["texture"].ToString().Replace("\"","");// JSONa .ToString() metode pieliek nevajadziigas peedinjas
                                        string[] textureSegments = spritePath.Split('/');
                                        //string spriteFileName = textureSegments[ textureSegments.Length-1 ]; //peedeejais segments = faila nosaukums

                                        string spriteName = N["gameobjects"][i]["components"][j]["sprite"].ToString().Replace("\"","");

                                        //print(spriteFileName + "->" + spriteName);
                /*
                                        if(spritePath.Contains("Objects_Village") && spriteName.Contains("sign")){
                                            SpritesInLevels[levelname][spritePath] = spriteName;
                                            print(levelname + " =>" + spriteFileName + "->" + spriteName);
                                        }
                                        if(spritePath.Contains("Objects_Village") && spriteName.Contains("torch")){
                                            SpritesInLevels[levelname][spritePath] = spriteName;
                                            print(levelname + " =>" + spriteFileName + "->" + spriteName);
                                        }
                                        if(spritePath.Contains("Objects_Village") && spriteName.Contains("fallen")){
                                            SpritesInLevels[levelname][spritePath] = spriteName;
                                            print(levelname + " =>" + spriteFileName + "->" + spriteName);
                                        } * /				


                                        / *
                                        //ja tekstuura ir vienaada ar spraitu - taatad nav spraitshiits
                                        if(spriteName == spriteFileName){
                                            //print(N["gameobjects"][i]["components"][j]["texture"] + " -> " + N["gameobjects"][i]["components"][j]["sprite"]);
                                            //OUTPUT += "\n" + spritePath;
                                            SpritesInLevels[levelname][spritePath] = spriteName;
                                        } * /

                                        if(rebuild){
                                            /**
                                             * nomainiia textuuru: visuals/Sprites/Jungle/grass
                                             * uz visuals/Sprites/Jungle_sprite_pack
                                             * 
                                             * svariigi, lai spraita nosaukums - grass - nemainaas
                                             * /
                                            string newSpritePath = "";
                                            if(ReplaceSprites1.TryGetValue(spritePath, out newSpritePath)){
                                                N["gameobjects"][i]["components"][j]["texture"] = newSpritePath;
                                                continue;
                                            }

                                            //nomaina tekstúru, nezinot precízu iepriekśéjo tekstúru - tikai péc iepr. tekstúras direktorijas (segment 2)
                                            newSpritePath = "";
                                            if(ReplaceSprites2.TryGetValue(textureSegments[2], out newSpritePath)){
                                                N["gameobjects"][i]["components"][j]["texture"] = newSpritePath;
                                                //print(textureSegments[2]  + " :: " + N["gameobjects"][i]["components"][j]["texture"] + " -> " + );
                                            }

                                            //vnk brute-force repleiseris
                                            newSpritePath = "";
                                            if(ReplaceSprites3.TryGetValue(spritePath, out newSpritePath)){
                                                N["gameobjects"][i]["components"][j]["texture"] = newSpritePath;
                                                //print(textureSegments[2]  + " :: " + N["gameobjects"][i]["components"][j]["texture"] + " -> " + );
                                            }


                                            /* izváca road_sign spraitu no spratśíta
                                            if(spritePath == "visuals/Sprites/Objects_Village" && spriteName == "road_sign"){
                                                N["gameobjects"][i]["components"][j]["texture"] = "visuals/Sprites/road_sign";
                                            } * /


                                        }
                                    }
                                }//end spraiti */

            }

            if (N["gameobjects"][i]["prefabScripts"] != null)
            {
                for (int j = 0; j < N["gameobjects"][i]["prefabScripts"].Count; j++)
                {

                    //print(N["gameobjects"][i]["name"] + " "  + N["gameobjects"][i]["prefabScripts"].Count);

                    if (N["gameobjects"][i]["prefabScripts"][j]["componentName"].ToString() == "\"PSCoinCrate\"")
                    {
                        //print("got coincrate prefab");
                        N["gameobjects"][i]["prefabScripts"][j]["componentName"] = "PSCrate";

                        N["gameobjects"][i]["prefabScripts"][j]["count"] = N["gameobjects"][i]["prefabScripts"][j]["coinCount"].ToString().Replace("\"", ""); // "100" => 100


                    }
                }
            }





            if (N["gameobjects"][i]["children"].Count > 0)
            {
                AnalyzeJSONCheckNodeAndItsKids(rebuild, levelname, depth++, N["gameobjects"][i]["children"]);
            }

        }

    }



}
#endif
}
