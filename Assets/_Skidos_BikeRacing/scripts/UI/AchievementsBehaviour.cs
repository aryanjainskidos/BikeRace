namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class AchievementsBehaviour : MonoBehaviour
    {

        bool initialized = false;
        Transform container;
        Scrollbar scrollbar;
        List<GameObject> achievements = new List<GameObject>();

        PlayerLevelInfoBehaviour levelInfoPanel;

        GameObject pointer;

        void Awake()
        {
            //        print("AchievementsBehaviour::Awake");
            container = transform.Find("AchievementPanel/ScrollView/Content");
            scrollbar = transform.Find("AchievementPanel/Scrollbar").GetComponent<Scrollbar>();
            levelInfoPanel = transform.Find("LevelInfoPanel").GetComponent<PlayerLevelInfoBehaviour>();
            pointer = container.Find("Pointer").gameObject;
        }

        public void Init()
        {
            PopulateAchievementList();
            if (levelInfoPanel != null)
            {
                levelInfoPanel.Init();
            }

            initialized = true;
        }

        void PopulateAchievementList()
        {

            GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("Achievement") as GameObject;
            Debug.Log("<color=yellow>Prefab Loaded Name </color>= " + prefab);
            //GameObject prefab = Resources.Load("Prefabs/UI/Achievement") as GameObject;
            GameObject achievement;

            if (BikeDataManager.Achievements != null)
            {

                foreach (var item in BikeDataManager.Achievements)
                {

                    if (item.Value.Claimed)
                    {
                        continue; //do not show claimed achievements
                    }
                    achievement = Instantiate(prefab) as GameObject;
                    //achievement.layer = LayerMask.NameToLayer("UI");
                    achievement.layer = 5;
                    achievement.transform.SetParent(container);
                    achievement.transform.localScale = Vector3.one;
                    achievement.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    AchievementBehaviour apb = achievement.GetComponent<AchievementBehaviour>();
                    apb.key = item.Key;
                    apb.claimDelegate = OnRewardClaim;
                    apb.setData(item.Value);

                    achievements.Add(achievement);
                }
            }
            else
            {
                Debug.LogWarning("DataManager.Achievements == null");
            }
        }

        // Update is called once per frame
        void OnEnable()
        {
            if (Startup.Initialized)
            {
                Actualize();

                if (BikeDataManager.FirstClaim && BikeDataManager.CountUnclaimedAchievements() >= 4)
                {
                    pointer.SetActive(true);
                }
                else
                {
                    if (pointer.activeSelf)
                    {
                        pointer.SetActive(false);
                    }
                }

                //delete claimed achievements
                AchievementBehaviour apb;
                for (int i = 0; i < achievements.Count; i++)
                {
                    if (achievements[i] == null)
                    {
                        continue;
                    }
                    apb = achievements[i].GetComponent<AchievementBehaviour>();
                    if (apb.Record.Claimed)
                    {
                        Destroy(achievements[i]);
                    }
                }

                //scroll everything to top
                var pointerEventData = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(transform.Find("AchievementPanel/ScrollView").gameObject, pointerEventData, ExecuteEvents.scrollHandler);

                NewsListManager.EmptyCategory(NewsListItemType.achievement); //izdzéś visas achi zińas (lai aizejot atpakaĺ uz trases chúsku nerádítu zinjas par achi, kas jau ir kleimoti)
            }
        }

        bool firstUpdate = true;

        void Update()
        {
            if (firstUpdate)
            {
                //print("Update");
                Actualize();

                scrollbar.value = 1;//quite irritating that it's not possible to put this in OnEnable or Start because apparently it gets overwritten, since unity puts off the actual init till the first update
                firstUpdate = false;
            }

        }

        void Actualize()
        {
            if (initialized)
            {
                //print("akchualized");

                //OrderedList => List 
                List<AchievementRecord> achi = new List<AchievementRecord>();
                foreach (var a in BikeDataManager.Achievements)
                {
                    achi.Add(a.Value);
                }
                //sort list
                List<AchievementRecord> achiSorted = achi.OrderBy(go => go.Percentage).ToList();


                string key;
                AchievementBehaviour apb;
                for (int i = 0; i < achievements.Count; i++)
                {
                    if (achievements[i] == null)
                    { //skip removed (claimed) achievements
                        continue;
                    }

                    apb = achievements[i].GetComponent<AchievementBehaviour>();
                    key = apb.key;

                    apb.setData(BikeDataManager.Achievements[key]);

                    int position = achiSorted.IndexOf(apb.Record);
                    achievements[i].transform.SetSiblingIndex(position); // rearrange panels acording to achi %
                }

                //            levelInfoPanel.Actualize(); //no need to manually actualize





            }


        }


        void OnRewardClaim()
        {
            Actualize();

            if (!BikeDataManager.FirstClaim && pointer.activeSelf)
            {
                pointer.SetActive(false);
            }

        }
    }

}
