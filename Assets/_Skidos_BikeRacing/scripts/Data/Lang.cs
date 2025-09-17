namespace vasundharabikeracing {

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if UNITY_EDITOR_OSX
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
#endif
#pragma warning disable 0162 //stfu

/**
 * short for "LanguageManager"
 * 
 * ir 2 veidu tulkojumi: 
 * a) UI -- scéná esośu <Text> komponentu tulkośana, to dara pirms UI menedźeris piestartéts
 * b) kodá -- Lang.Get("iztulko_śo");
 * 
 * jaunu terminu mekléśana un pierakstíśana notiek tikai redaktorá un tikia uz MAC, (WIN kaut ko lauź ar aizverośajám pédinjám, fuck those guys)
 */
public class Lang : MonoBehaviour
{

    private static LoadAddressable_Vasundhara loadAddressable_Vasundhara;

    public static string PreferedLanguage; //no ieríces iegútá lokále vai valoda (pagaidám lieto tikai ḱíniéśu val. identificéśanai) 
    public static string language = "en"; //izvélétá valoda tulkojumiem

    private static Dictionary<string, string> translations = new Dictionary<string, string>();
#if UNITY_EDITOR_OSX
    private static bool newTerms = false;
    private static string path = Application.dataPath + "/Resources/Data/" + language + ".txt";
#endif

    public static void Init()
    {

        loadAddressable_Vasundhara = GameObject.Find("Vasundhara_LoadAddressable").GetComponent<LoadAddressable_Vasundhara>();



        //ISO 639-1
        //@todo -- waitlistét visas izmantotás valodas http://docs.unity3d.com/ScriptReference/SystemLanguage.html
        string sysLang = Application.systemLanguage.ToString();
        switch (sysLang)
        {
            case "English":
                language = "en";
                break;
            case "French":
                language = "fr";
                break;
            case "German":
                language = "de";
                break;
            case "Italian":
                language = "it";
                break;
            case "Dutch":
                language = "nl";
                break;
            case "Portuguese":
                language = "pt";
                break;
            case "Spanish":
                language = "es";
                break;
            case "Korean":
                language = "ko";
                break;
            case "Japanese":
                language = "ja";
                break;
            case "Russian":
                language = "ru";
                break;

            case "Swedish":
                language = "sv";
                break;
            case "Turkish":
                language = "tk";
                break;
            case "Indonesian":
                language = "id";
                break;
            //	case "Arabic":
            //		language = "ar";
            //		break;

            case "Hindi": // unity tádu nepazíst, iegúsim caur PreferedLanguage
                language = "hi";
                break;
            case "Malay": // unity tádu nepazíst, iegúsim caur PreferedLanguage
                language = "ms";
                break;

            case "Danish":
                language = "dk";
                break;

            case "Norwegian":
                language = "no";
                break;

            case "Chinese":
                language = "zhs";
                break;
            case "ChineseSimplified": // nestrádá < 5.1.1, iegúsim caur PreferedLanguage
                language = "zhs";
                break;
            case "ChineseTraditional": // nestrádá < 5.1.1, iegúsim caur PreferedLanguage
                language = "zht";
                break;

            default:
                language = "en";
                break;

        }


        //unity ir neuzticams, ḱínieśu valodás nepieńemami neuzticams - tiem méǵinás prasít no OSa
#if UNITY_IOS
        PreferedLanguage = PlayerPrefs.GetString("languageFromXcode"); /// haxxx - xkodá AppController.mm failá iemests koda gabalińś pieraksta pleyerprefos atrasto lokáli
#elif UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject localeDefault = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault");
		PreferedLanguage = localeDefault.Call<string>("toString");
#else
		PreferedLanguage = "";
#endif

        //print("locale from Xcode:" + prefferedLang);
        //zh-Hans  - simplified
        //zh-Hant - tradional
        //zh-HK - traditional

        //iOSá
        if (PreferedLanguage.Length >= 7 && PreferedLanguage.Substring(0, 7) == "zh-Hans")
        {
            language = "zhs";
        }
        else if (PreferedLanguage.Length >= 7 && PreferedLanguage.Substring(0, 7) == "zh-Hant")
        {
            language = "zht";
        }
        else if (PreferedLanguage.Length >= 5 && PreferedLanguage.Substring(0, 5) == "zh-HK")
        {
            language = "zht";
        }
        else if (PreferedLanguage.Length >= 2 && PreferedLanguage.Substring(0, 2) == "hi")
        {
            language = "hi";
        }
        else if (PreferedLanguage.Length >= 2 && PreferedLanguage.Substring(0, 2) == "ms")
        {
            language = "ms";
        }

        //Androídá
        if (PreferedLanguage == "zh_HK")
        {
            language = "zht";
        }
        else if (PreferedLanguage == "zh_TW")
        {
            language = "zht";
        }
        else if (PreferedLanguage == "zh_CN")
        {
            language = "zhs";
        }
        else if (PreferedLanguage == "hi_IN")
        {
            language = "hi";
        }
        else if (PreferedLanguage == "ms_MY")
        {
            language = "ms";
        }

        //	language = "no";


        string csv = "";
        string p = language;
        //string p = "Data/" + language;
        Debug.Log("<color=yellow>Prefab is loading from = </color>" + p);
        TextAsset textAss = (TextAsset)loadAddressable_Vasundhara.GetTextAsset_Resources(p);
        Debug.Log("<color=yellow>Prefab Loaded = </color>" + textAss);
        //TextAsset textAss = (TextAsset)Resources.Load(p, typeof(TextAsset)); 
        if (textAss != null)
        {
            csv = textAss.text;
        }

        if (csv == null && csv.Length == 0)
        {
            Debug.LogError("nav valodinjas! Fallback to EN ?");
            language = "en"; //méǵinás vélreiz - angliski

            p = language;
            //p = "Data/" + language;
            textAss = (TextAsset)LoadAddressable_Vasundhara.Instance.GetTextAsset_Resources(p);
            //textAss = (TextAsset)Resources.Load(p, typeof(TextAsset));
            if (textAss != null)
            {
                csv = textAss.text;
            }

        }

        string[] lines = csv.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length < 5)
            {
                continue;//skipojam tukśas rindas
            }

            try
            {
                lines[i] = lines[i].Replace(" = ", "~"); //repleiso [" = "] pret [~]
                string[] words = lines[i].Split('~'); //sadala terminu/tulkojumu
                string term = words[0].Substring(1, words[0].Length - 2);
                string transl = words[1].Substring(1, words[1].Length - 3);


                //at-eskeipo
                transl = transl.Replace("\\n", "\n");  //burtiskus 2 simbolus [\n] uz patiesu njúlainu
                transl = transl.Replace("\\\"", "\""); //burtiskus 2 simbolus [\"] uz patiesu dubultpédinju

                //print(i + "  |" + term +  "|    |" + transl + "|");

                //translations.Add(term,transl);
                translations[term] = transl;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Kreiss Tulkojums " + language + "failaa, noskipoju, bet, luudzu, salabo! (" + (i + 1) + ") " + e.Message);
                continue;
            }

        }



        /**
         * lai atrastu visus terminus: monodevelopá meklét "Lang.Get" un nokopét visus rezultátus, attírít lieko tekstu un izpildít śajá metodé
         * parasti tas nav jádara - ja termins tiks pieprasíts tulkośanai [kods ar Lang.Get() ) izpildíts] Redaktorá tad termins tiks ievietots tulkojumu failá
         */

        if (Debug.isDebugBuild) { print("Lang: PreferedLanguage=" + PreferedLanguage + " loaded language=" + language); }

    }





    //iztulko padoto ekránu
    public static void TranslateScreen(Transform screen)
    {
        Text[] textCompos = screen.GetComponentsInChildren<Text>(true);
        //print("Screen:"  + screen.name + " =>  texts: " + textCompos.Length);
        for (int i = 0; i < textCompos.Length; i++)
        {
            string term = textCompos[i].text;
            /**
                 * Tikai terminus, kas sákas ar UI:  -- tátad waitlistétit tulkośanai
                 */
            if (term.Contains("UI:"))
            {
                textCompos[i].text = Get(term);
            }
            //print("term: " + term);
        }
    }


    public static string Get(string term)
    {

        term = term.Replace("\n", "\\n");//eskeipo njúlainu - terminá un .trings failá vienmér ir eskeipoti njúlaini

        string translation;
        if (translations.TryGetValue(term, out translation))
        {

        }
        else
        {
            //nebija tulkojums
#if UNITY_EDITOR_OSX
            Debug.LogWarning("Atrasts jauns termins: " + term);
            translations.Add(term, term); //pievienos ká jaunu terminu
            newTerms = true;
            Camera.main.GetComponent<MonoBehaviour>().StartCoroutine(Save()); //driiz saglabaas
#endif
            translation = term; //atgriezís netulkotu terminu
        }
        //    if (language == "ar")
        //    {
        //        translation = ArabicSupport.ArabicFixer.Fix(translation, false, false);
        //    }

        return translation;
    }

#if UNITY_EDITOR_OSX

    /**
     * katrs jauns termins izveido ko-rutiinu, kas sevios visus terminu (ja taadi ir) 
     * korutiinas saakuma pagaida 1 sekundi un tad seivo - taadejaadi seivoshana notiks tikai 1 reizi, pat ja vienlaiciigi ir atrasti 100 termini
     */
    private static IEnumerator Save()
    {

        yield return new WaitForSeconds(1);

        if (!newTerms)
        {
            yield break;
        }
        newTerms = false;

        if (language != "en")
        {
            Debug.LogWarning("nav EN valoda, nesaglabaas terminus");
            yield break;
        }



        string file = "";
        var list = translations.Keys.ToList();
        list.Sort();
        foreach (var key in list)
        {
            //print(key + " -> " +  translations[key]);

            string term = key;
            string transl = translations[key];
            transl = transl.Replace("\n", "\\n"); //eskeipoju
            transl = transl.Replace("\"", "\\\""); //eskeipoju

            //izveidoju 1 rindinju tulkojumu failam
            file += "\"" + term + "\" = \"" + transl + "\"" + ";\n";

        }
        System.IO.File.WriteAllText(path, file);
        Debug.Log("Termini saglabaati");
    }
#endif


#if UNITY_EDITOR_OSX
    [PostProcessBuild(6000)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        return; // Xcode nepatík, ja maina sourci :\
#if UNITY_IOS
        ///Users/martinslatkovskis/apps/MotoTrial/wutwut-ios/Classes

        //print("path=" + path);
        string fullPath = path + "/Classes/UnityAppController.mm";
        string source = System.IO.File.ReadAllText(fullPath);
        //print(source);
        print("fullPath=" + fullPath);


        int lineToPutMyHaxxx = 0;
        string finalSource = "";

        string[] sourceLines = source.Split('\n');
        for (int i = 0; i < sourceLines.Length; i++)
        {
            if (sourceLines[i].Contains("(void)startUnity:(UIApplication*)application"))
            {
                lineToPutMyHaxxx = i + 2; //atradu metodes sákuma rindińu
            }
        }

        if (lineToPutMyHaxxx != 0)
        {

            for (int i = 0; i < sourceLines.Length; i++)
            {
                if (i < lineToPutMyHaxxx)
                {
                    finalSource += sourceLines[i] + "\n"; //ridnińas pirms mana haka
                }
                else if (i == lineToPutMyHaxxx)
                {
                    finalSource += "\n";
                    finalSource += "// *** *** *** *** *** *** dirtyhaxxx *** *** *** *** *** *** *** ***\n";
                    finalSource += "NSLog(@\"preferredLanguage: %@\", [[NSLocale preferredLanguages] objectAtIndex:0]);\n";
                    finalSource += "[[NSUserDefaults standardUserDefaults] setObject:[[NSLocale preferredLanguages] objectAtIndex:0] forKey:@\"languageFromXcode\"];\n";
                    finalSource += "[[NSUserDefaults standardUserDefaults] synchronize];\n";
                    finalSource += "// *** *** *** *** *** *** *** *** *** *** *** *** *** ***\n";
                }
                else
                {
                    finalSource += sourceLines[i] + "\n"; //ridnińas péc mana haka
                }
            }

            //save
            //print(finalSource);
            System.IO.File.WriteAllText(fullPath, finalSource);

        }
        else
        {
            print("Neizdoas nohakot Xcode projektu!");
        }






#endif
    }
#endif


}
}
