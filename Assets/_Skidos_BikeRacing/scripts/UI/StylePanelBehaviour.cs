namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;

    public class StylePanelBehaviour : MonoBehaviour
    {

        public string key;

        Transform container;

        List<Toggle> styleToggles;
        List<StyleEntryBehaviour> styleEntryBehaviours;

        ScrollRect scrollRect;
        RectTransform scrollRectTransform;
        RectTransform contentRectTransform;
        RectTransform maskRectTransform;

        bool initialized = false;

        public int activeStyleIndex;

        Toggle activeStyleToggle;

        public delegate void StyleDelegate(int index);
        public StyleDelegate panelDelegate;

        InfiniteScrollBehaviour isb;

        Transform pointer;

        // Use this for initialization
        void Awake()
        {
            container = transform.Find("ScrollView/Content");

            scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
            scrollRectTransform = scrollRect.transform as RectTransform;
            contentRectTransform = scrollRect.content;
            maskRectTransform = transform.Find("ScrollView").GetComponent<Mask>().rectTransform;


            pointer = transform.Find("Pointer");
        }

        int frameDelay = 0;
        bool update = true;
        void OnEnable()
        {
            update = true;
            CenterOnActiveToggle(); // for some reason unity resets the normalized position for one of the panels...

            ShowPointer();

            //        if (DataManager.ShowGiftStyle)
            //        {
            //            DataManager.ShowGiftStyle = false;
            //            OnButtonClick(DataManager.GiftStyleIndex);
            //        }
            frameDelay = 1;
        }

        void ShowPointer()
        {
            if (pointer != null)
            {
                //            print("pointer != null");
                if (BikeDataManager.FirstStyle)
                {
                    //                print("DataManager.FirstStyle");
                    pointer.gameObject.SetActive(true);
                    BikeDataManager.FirstStyle = false;
                }
                else
                {
                    //                print("!DataManager.FirstStyle");
                    pointer.gameObject.SetActive(false);
                }
            }
        }

        void LateUpdate()
        {
            if (update && frameDelay <= 0)
            {
                update = false;
                CenterOnActiveToggle(); //overwrite any changes automatic scripts have made

                if (BikeDataManager.ShowGiftStyle)
                {
                    BikeDataManager.ShowGiftStyle = false;
                    OnButtonClick(BikeDataManager.GiftStyleIndex);
                }
            }

            if (update && frameDelay > 0)
            {
                frameDelay--;
            }
        }

        public void Init()
        {
            //  print("PresetPanelBehaviour::Init");

            if (container != null)
            {
                styleToggles = new List<Toggle>();
                styleEntryBehaviours = new List<StyleEntryBehaviour>();

                foreach (Transform item in container)
                {
                    Destroy(item.gameObject);
                }

                GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("StyleToggle") as GameObject;
                Debug.Log("<color=yellow>Prefab Loaded Name = </color>" + prefab);
                //GameObject prefab = Resources.Load("Prefabs/UI/StyleToggle") as GameObject;

                Toggle tmpT;
                GameObject tmpGO;

                ToggleGroup toggleGroup = container.GetComponent<ToggleGroup>();

                isb = scrollRect.GetComponent<InfiniteScrollBehaviour>();

                StyleEntryBehaviour seb;

                for (int i = 0; i < BikeDataManager.Styles.Count; i++)
                {

                    tmpGO = Instantiate(prefab) as GameObject;
                    tmpGO.layer = 5;
                    //tmpGO.layer = LayerMask.NameToLayer("UI");
                    tmpGO.transform.SetParent(container);
                    tmpGO.transform.localScale = Vector3.one;
                    tmpGO.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    //                tmpGO.transform.FindChild("ColorPreset").GetComponent<Image>().color = DataManager.Presets[i].Colors[0];
                    //                tmpGO.transform.FindChild("ColorPreset/Image").GetComponent<Image>().color = DataManager.Presets[i].Colors[1];
                    //TODO set image
                    seb = tmpGO.GetComponent<StyleEntryBehaviour>();
                    seb.StyleID = i;
                    seb.clickIndexDelegate.indexDelegate = OnButtonClick;//if unlocked
                    styleEntryBehaviours.Add(seb);

                    tmpT = tmpGO.GetComponent<Toggle>();
                    tmpT.isOn = false;
                    tmpT.group = toggleGroup;
                    styleToggles.Add(tmpT);
                }

                //reorder the styles
                for (int i = 0; i < BikeDataManager.Styles.Count; i++)
                {
                    styleToggles[i].transform.SetSiblingIndex(BikeDataManager.Styles[i].Order);
                }

                initialized = true;


            }
        }

        public bool SetToggleByIndex(int index)
        {
            //        print(key + " SetCheckboxByIndex(int paletteIndex) " + presetIndex);
            bool success = false;

            if (initialized && gameObject.activeSelf)
            {

                if (index < styleToggles.Count)
                {

                    activeStyleIndex = index;
                    activeStyleToggle = styleToggles[index];

                    //reset all
                    foreach (var item in styleToggles)
                    {
                        item.isOn = false;
                    }

                    activeStyleToggle.isOn = true; //if this is called before Start then the checkbox values don't get saved for some reason and get reset after Start, therefore the update below

                    //                CenterOnActiveToggle(); //this makes the flicker, done in late update
                    update = true;
                    success = true;

                    if (isb != null)
                    {
                        isb.Init();
                    }
                }

            }
            return success;
        }

        void OnButtonClick(int index)
        {

            if (!BikeDataManager.Styles[index].Locked)
                SoundManager.Play("Color");

            //pass palette index and group to garage

            if (panelDelegate != null)
            {
                panelDelegate(index);
            }
            else
            {
                print("panelDelegate is null");
            }

            if (pointer != null && pointer.gameObject.activeSelf)
            {
                pointer.gameObject.SetActive(false);
            }

            SetToggleByIndex(index);
        }

        void CenterOnActiveToggle()
        {
            //        print("CenterOnActiveToggle" + (activePresetToggle != null));
            if (activeStyleToggle != null)
            {
                CenterOnItem(activeStyleToggle.gameObject.GetComponent<RectTransform>());

                if (isb != null)
                {
                    isb.Distribute();
                }
            }

        }

        public void UnlockCurrentToggle()
        {
            styleEntryBehaviours[activeStyleIndex].UpdateLockState();
        }

        public void UnlockAllToggles()
        {
            foreach (var styleEntryBehaviour in styleEntryBehaviours)
            {
                styleEntryBehaviour.UpdateLockState();
            }
        }

        #region scroll rect horizontal item snapping methods
        public void CenterOnItem(RectTransform target)
        {
            // Item is here
            Vector3 itemCenterPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(target));
            // But must be here
            Vector3 targetPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(maskRectTransform));
            // So it has to move this distance
            float differenceX = targetPositionInScroll.x - itemCenterPositionInScroll.x;

            float normalizedDifferenceX = differenceX / (contentRectTransform.rect.size.x - scrollRectTransform.rect.size.x);
            float newNormalizedXPosition = scrollRect.horizontalNormalizedPosition - normalizedDifferenceX;

            if (scrollRect.movementType != ScrollRect.MovementType.Unrestricted)
            {
                newNormalizedXPosition = Mathf.Clamp01(newNormalizedXPosition);
            }

            scrollRect.horizontalNormalizedPosition = newNormalizedXPosition;
        }

        Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            //pivot position + item size has to be included
            var pivotOffset = new Vector3(
                (0.5f - target.pivot.x) * target.rect.size.x,
                (0.5f - target.pivot.y) * target.rect.size.y,
                0f);
            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }

        Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
        {
            return target.InverseTransformPoint(worldPoint);
        }
        #endregion
    }

}
