namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuFriendsPanelBehaviour : MonoBehaviour
{

    int lastDataID_friends = 0;
    bool askedForFriendsAdHoc = false;
    System.DateTime lastTimeFriendsChecked = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

    GameObject loadingImageGo;
    GameObject loginButtonGo;
    GameObject inviteButtonGo;
    Transform friendsContainer;


    void Awake()
    {
        loadingImageGo = transform.Find("LoadingImage").gameObject;
        loginButtonGo = transform.Find("LogInButton").gameObject;
        inviteButtonGo = transform.Find("InviteButton").gameObject;
        friendsContainer = transform.Find("FriendGamesPanel/ScrollView/Content");

        loginButtonGo.SetActive(false);
        inviteButtonGo.SetActive(false);
    }


    void OnEnable()
    {


        if (Startup.Initialized)
        { //@btw -- onEnble tiek izsaukts arí atvero MP logu, kamér śis tabs nav aktívs
          //print("MenuFriendsPanelBehaviour::OnEnable");
          //print("MenuFriendsPanelBehaviour::Enabled");
            if (MultiplayerManager.HasFB)
            {
                //print("MenuFriendsPanelBehaviour == login cool");

                if (!MultiplayerManager.FBFriendsDownloaded)
                { //pirmajá reizé pirms jaunu draugu ieládéśanas parádís LOADING bildi
                    loadingImageGo.SetActive(true);

                    UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
                }
                MultiplayerManager.MPGetFriends(); //prasís serverim jaunákos draugus
                lastTimeFriendsChecked = System.DateTime.Now.ToUniversalTime();


            }
            else
            {//nav FB, tad ir járáda logina poga
             //print("MenuFriendsPanelBehaviour == must login");
                loadingImageGo.SetActive(false);
                loginButtonGo.SetActive(true);
            }

            StartCoroutine(LoadFriendsAgain());
        }

    }

    void OnDisable()
    {
        StopCoroutine(LoadFriendsAgain());
    }

    bool update = false;

    void Update()
    {

        if (update)
        { //delay this update by one frame, otherwise the content doesn't get filled
            friendsContainer.parent.GetComponent<ScrollViewMoveToTopBehaviour>().forceRecalculate = true;
            update = false;
        }

        //tikko ielogojies, no śí ekrána - jáieládé draudzinji
        if (Startup.Initialized && !MultiplayerManager.FBFriendsDownloaded && MultiplayerManager.HasFB && !askedForFriendsAdHoc)
        {
            //            print("Update:foo");
            loadingImageGo.SetActive(true);
            loginButtonGo.SetActive(false);
            MultiplayerManager.MPGetFriends();
            lastTimeFriendsChecked = System.DateTime.Now.ToUniversalTime();
            askedForFriendsAdHoc = true;
            UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, true);
        }

        if (MultiplayerManager.HasFB)
        {
            //            print("Update:bar:HasFB");

            //nomainíjuśies dati - jáapdeito ekráns
            if (Startup.Initialized && MultiplayerManager.DataID_friends != lastDataID_friends && MultiplayerManager.FBFriendsDownloaded)
            {
                //                print("Update:bar");

                foreach (Transform child in friendsContainer.transform)
                {//iztíra draudzińus un ieslédz loading bildi
                    Destroy(child.gameObject);
                }
                loadingImageGo.SetActive(true);

                UIManager.ToggleScreen(GameScreenType.PopupMultiplayerLoading, false);

                //print("MenuFriendsPanelBehaviour::atjaunojam info");
                lastDataID_friends = MultiplayerManager.DataID_friends;
                loadingImageGo.SetActive(false); //izslédzu loading bildi
                loginButtonGo.SetActive(false); //arí login pogu, if any

                GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("FriendGameEntry") as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prefab);
                //GameObject prefab = Resources.Load("Prefabs/UI/FriendGameEntry") as GameObject;

                for (int i = 0; i < MultiplayerManager.OpponentsStarted.Count; i++)
                {
                    GameObject entry = Instantiate(prefab) as GameObject;
                    entry.transform.SetParent(friendsContainer);
                    entry.transform.localScale = Vector3.one;
                    entry.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    entry.GetComponent<FriendGameEntryBehaviour>().SetData(MultiplayerManager.OpponentsStarted[i]); //iedod pretinieka ierakstu - poga pati sev saliks datus
                }

                if (MultiplayerManager.OpponentsStarted.Count == 0)
                {
                    //you have no friends, invite some
                    inviteButtonGo.SetActive(true);
                }
                else
                {
                    //you have friends, never mind
                    if (inviteButtonGo.activeSelf)
                        inviteButtonGo.SetActive(false);
                }

                //noskrollé uz augśu
                //			var pointer = new PointerEventData(EventSystem.current);
                //			ExecuteEvents.Execute(friendsContainer.parent.gameObject, pointer, ExecuteEvents.scrollHandler);

                //            friendsContainer.parent.GetComponent<ScrollViewMoveToTopBehaviour>().forceRecalculate = true;
                update = true; //delay this update by one frame, otherwise the content doesn't get filled

            }

        }

        if (Startup.Initialized)
        {

            if (NewsListManager.ActiveRides != NewsListManager.LastShownActiveRides)
            {
                NewsListManager.LastShownActiveRides = NewsListManager.ActiveRides;
                //				print("LastShownActiveRides reset");
            }

        }
    }


    /**
	 *  reizi n sekundés apskatás - vai draugu saraksts nav par vecu
	 *  (śí metode strádá arí, ja appa ir minimizéta)
	 */
    IEnumerator LoadFriendsAgain()
    {

        while (true)
        {
            yield return new WaitForSeconds(2);

            System.TimeSpan diff = System.DateTime.Now.ToUniversalTime() - lastTimeFriendsChecked.ToUniversalTime();
            //print("Draudzinji chekoti pirms " + diff.TotalSeconds + "sekundeem");
            if (diff.TotalSeconds > 60)
            {
                //print("njemsism draudzinjus tagad!");
                if (MultiplayerManager.HasFB)
                {//don't even try to download friends if not logged in
                    MultiplayerManager.MPGetFriends();
                }
                lastTimeFriendsChecked = System.DateTime.Now.ToUniversalTime();
            }



        }



    }

}

}
