namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;

    public class ColorPresetPanelBehaviour : MonoBehaviour
    {

        public string key;

        Transform container;

        List<Toggle> presetToggles;
        List<Toggle> additionalPresetToggles;

        ScrollRect scrollRect;
        RectTransform scrollRectTransform;
        RectTransform contentRectTransform;
        RectTransform maskRectTransform;

        bool initialized = false;

        public int activePresetIndex;

        Toggle activePresetToggle;

        public delegate void GroupPaletteDelegate(string groupName, int paletteIndex);
        public GroupPaletteDelegate panelDelegate;

        InfiniteScrollBehaviour infiniteScrollBehaviour;

        int frameDelay = 0;

        List<int> presetToggleIndexToKey;
        Dictionary<int, int> presetToggleKeyToIndex;

        public int maxPresetCount = 6;

        // Use this for initialization
        void Awake()
        {
            container = transform.Find("ScrollView/Content");

            scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
            scrollRectTransform = scrollRect.transform as RectTransform;
            contentRectTransform = scrollRect.content;
            maskRectTransform = transform.Find("ScrollView").GetComponent<Mask>().rectTransform;
        }

        public bool update = true;
        void OnEnable()
        {
            update = true;
            //        print("OnEnable scrollRect.horizontalNormalizedPosition" + scrollRect.horizontalNormalizedPosition);
            CenterOnActiveToggle(); // for some reason unity resets the normalized position for one of the panels...
                                    //        print("scrollRect.normalizedPosition" + scrollRect.normalizedPosition);
        }

        //DONE strange flicker when switching from career to competitive, also at beginning in either direction

        void LateUpdate()
        {
            if (update && frameDelay <= 0)
            {
                update = false;
                //            print("Update scrollRect.horizontalNormalizedPosition" + scrollRect.horizontalNormalizedPosition);
                CenterOnActiveToggle(); //overwrite any changes automatic scripts have made
                                        //            print("Update scrollRect.horizontalNormalizedPosition" + scrollRect.horizontalNormalizedPosition);
            }

            if (update && frameDelay > 0)
            {
                frameDelay--;
            }
        }


        public float lastSizeX = 0;

        void Update()
        {
            //
            if (lastSizeX != contentRectTransform.rect.size.x || frameDelay > 0)
            {
                //            print("changed");
                CenterOnActiveToggle();
                lastSizeX = contentRectTransform.rect.size.x;
            }
        }

        public void Init()
        {
            //  print("PresetPanelBehaviour::Init");

            if (container != null)
            {
                presetToggles = new List<Toggle>();
                additionalPresetToggles = new List<Toggle>();

                presetToggleIndexToKey = new List<int>();
                presetToggleKeyToIndex = new Dictionary<int, int>();

                foreach (Transform item in container)
                {
                    Destroy(item.gameObject);
                }


                GameObject prefab = LoadAddressable_Vasundhara.Instance.GetPrefab_Resources("ColorPresetToggle") as GameObject;
                Debug.Log("<color=yellow>Prefab is loading from = </color>" + prefab);
                //GameObject prefab = Resources.Load("Prefabs/UI/ColorPresetToggle") as GameObject;

                Toggle tmpT;
                GameObject tmpGO;

                ToggleGroup toggleGroup = container.GetComponent<ToggleGroup>();
                ToggleGroup toggleGroupB = scrollRect.GetComponent<ToggleGroup>();
                if (toggleGroupB == null)
                {
                    toggleGroupB = scrollRect.gameObject.AddComponent<ToggleGroup>();
                    scrollRect.gameObject.AddComponent<InfiniteScrollBehaviour>();
                    //                scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
                }

                infiniteScrollBehaviour = scrollRect.GetComponent<InfiniteScrollBehaviour>();

                //            for (int i = 0; i < DataManager.Presets.Count; i++) {
                //            int i = 0;
                foreach (var presetKVP in BikeDataManager.Presets)
                {
                    presetToggleKeyToIndex.Add(presetKVP.Key, -1);//preset_id => toggle_id
                }

                //            foreach (var presetKVP in DataManager.Presets) {
                //
                //                tmpGO = Instantiate(prefab) as GameObject;
                //                tmpGO.layer = LayerMask.NameToLayer("UI");
                //                tmpGO.name += "_a_"+i;
                //                tmpGO.transform.SetParent(container);
                //                tmpGO.transform.localScale = Vector3.one;
                //                tmpGO.GetComponent<RectTransform>().localPosition = Vector3.zero;
                //                
                //                tmpGO.transform.FindChild("ColorPreset").GetComponent<Image>().color = presetKVP.Value.Colors[0];
                //                if (presetKVP.Value.Colors.Length > 1) {
                //                    tmpGO.transform.FindChild("ColorPreset/Image").GetComponent<Image>().color = presetKVP.Value.Colors[1];
                //                } else {
                //                    tmpGO.transform.FindChild("ColorPreset/Image").GetComponent<Image>().enabled = false;
                //                }
                //
                //                
                //                tmpT = tmpGO.GetComponent<Toggle>();
                //                //            tmpT.isOn = (i == 1) ? true : false;
                //                tmpT.isOn = false;
                //                tmpT.GetComponent<UIClickIndexDelegate>().index = i;
                //                tmpT.GetComponent<UIClickIndexDelegate>().indexDelegate = OnButtonClick;
                //                //            tmpT.GetComponent<UICheckbox>().radioButtonRoot = transform;
                //                tmpT.group = toggleGroup;
                //                presetToggles.Add(tmpT);
                //
                //                presetToggleIndexToKey.Add(presetKVP.Key);
                //                presetToggleKeyToIndex.Add(presetKVP.Key, i);
                //
                //                i++;
                //            }

                //create primary toggles
                for (int i = 0; i < maxPresetCount; i++)
                {

                    tmpGO = Instantiate(prefab) as GameObject;
                    tmpGO.layer = 5;
                    //tmpGO.layer = LayerMask.NameToLayer("UI");
                    tmpGO.name += "_a_" + i;
                    tmpGO.transform.SetParent(container);
                    tmpGO.transform.localScale = Vector3.one;
                    tmpGO.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    tmpT = tmpGO.GetComponent<Toggle>();
                    tmpT.isOn = false;
                    tmpT.GetComponent<UIClickIndexDelegate>().index = i;
                    tmpT.GetComponent<UIClickIndexDelegate>().indexDelegate = OnButtonClick;
                    tmpT.group = toggleGroup;
                    presetToggles.Add(tmpT);

                    presetToggleIndexToKey.Add(-1);//init toggle_id => preset_id
                }

                //create additional toggles
                for (int i = 0; i < maxPresetCount; i++)
                {

                    tmpGO = Instantiate(prefab) as GameObject;
                    tmpGO.layer = 5;
                    //tmpGO.layer = LayerMask.NameToLayer("UI");
                    tmpGO.name += "_b_" + i;
                    tmpGO.transform.SetParent(container);
                    tmpGO.transform.localScale = Vector3.one;
                    tmpGO.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    tmpT = tmpGO.GetComponent<Toggle>();
                    tmpT.isOn = false;
                    tmpT.GetComponent<UIClickIndexDelegate>().index = i;
                    tmpT.GetComponent<UIClickIndexDelegate>().indexDelegate = OnButtonClick;
                    tmpT.group = toggleGroupB;
                    additionalPresetToggles.Add(tmpT);
                }

                //            for (int i = 0; i < DataManager.Presets.Count; i++) {
                //            i = 0;
                //            foreach (var presetKVP in DataManager.Presets) {
                //                
                //                tmpGO = Instantiate(prefab) as GameObject;
                //                tmpGO.layer = LayerMask.NameToLayer("UI");
                //                tmpGO.name += "_b_"+i;
                //                tmpGO.transform.SetParent(container);
                //                tmpGO.transform.localScale = Vector3.one;
                //                tmpGO.GetComponent<RectTransform>().localPosition = Vector3.zero;
                //                
                //                tmpGO.transform.FindChild("ColorPreset").GetComponent<Image>().color = presetKVP.Value.Colors[0];
                //                if (presetKVP.Value.Colors.Length > 1) {
                //                    tmpGO.transform.FindChild("ColorPreset/Image").GetComponent<Image>().color = presetKVP.Value.Colors[1];
                //                } else {
                //                    tmpGO.transform.FindChild("ColorPreset/Image").GetComponent<Image>().enabled = false;
                //                }
                //                
                //                tmpT = tmpGO.GetComponent<Toggle>();
                //                //            tmpT.isOn = (i == 1) ? true : false;
                //                tmpT.isOn = false;
                //                tmpT.GetComponent<UIClickIndexDelegate>().index = i;
                //                tmpT.GetComponent<UIClickIndexDelegate>().indexDelegate = OnButtonClick;
                //                //            tmpT.GetComponent<UICheckbox>().radioButtonRoot = transform;
                //                tmpT.group = toggleGroupB;
                //                additionalPresetToggles.Add(tmpT);
                //                i++;
                //            }

                initialized = true;

            }
        }

        public bool SetCheckboxByIndex(int presetIndex)
        {
            //        print(key + " SetCheckboxByIndex(int paletteIndex) " + presetIndex);
            presetIndex = presetToggleKeyToIndex[presetIndex];
            bool success = false;

            if (initialized && gameObject.activeSelf && presetIndex >= 0)
            {

                if (presetIndex < presetToggles.Count)
                {

                    activePresetIndex = presetIndex;
                    activePresetToggle = presetToggles[presetIndex];

                    //reset all
                    foreach (var item in presetToggles)
                    {
                        item.isOn = false;
                    }

                    activePresetToggle.isOn = true; //if this is called before Start then the checkbox values don't get saved for some reason and get reset after Start, therefore the update below

                    //                foreach (var item in additionalPresetToggles) {
                    //                    item.isOn = false;
                    ////                    item.gameObject.SetActive(true);
                    //                }
                    foreach (var item in additionalPresetToggles)
                    {
                        item.isOn = false;
                    }
                    //
                    additionalPresetToggles[presetIndex].isOn = true;

                    //                CenterOnActiveToggle(); //this makes the flicker, done in late update
                    update = true;
                    success = true;
                }

            }// else print("SetCheckboxByIndex "+initialized +" "+ gameObject.activeSelf +" "+ presetIndex);
            return success;
        }

        void OnButtonClick(int presetIndex)
        {

            SoundManager.Play("Color"); //"change color" sound

            //pass palette index and group to garage        
            if (panelDelegate != null)
            {
                panelDelegate(key, presetToggleIndexToKey[presetIndex]);
            }
            else
            {
                print("panelDelegate is null");
            }

            SetCheckboxByIndex(presetToggleIndexToKey[presetIndex]);
            //        print("contentRectTransform.hasChanged " +contentRectTransform.hasChanged);

        }

        void CenterOnActiveToggle()
        {
            //        print(key + " CenterOnActiveToggle " + activePresetIndex);
            if (activePresetToggle != null)
            {
                CenterOnItem(activePresetToggle.gameObject.GetComponent<RectTransform>());

                if (infiniteScrollBehaviour != null)
                {
                    infiniteScrollBehaviour.Distribute();
                }
            }

        }

        public void SetVisiblePresets(int[] presets)
        {
            //        print(key + " SetVisiblePresets" + presets);

            //                presetToggleIndexToKey.Add(presetKVP.Key);
            //                presetToggleKeyToIndex.Add(presetKVP.Key, i);

            foreach (var item in presetToggles)
            {
                item.gameObject.SetActive(false);
            }

            //        int i = 0;
            //        foreach (var item in presets) {
            //            presetToggleIndexToKey[i] = item;
            //            i++;
            ////            presetToggles[presetToggleKeyToIndex[item]].gameObject.SetActive(true);
            //        }
            //
            foreach (var item in additionalPresetToggles)
            {
                item.gameObject.SetActive(false);
            }
            //
            //        if (presets.Length > 1){
            //            foreach (var item in presets) {
            //                additionalPresetToggles[presetToggleKeyToIndex[item]].gameObject.SetActive(true);
            //            }
            //        }

            PresetRecord presetRecord;
            for (int i = 0; i < presetToggles.Count; i++)
            {
                if (i < presets.Length)
                {

                    presetToggleIndexToKey[i] = presets[i];
                    presetToggleKeyToIndex[presets[i]] = i;

                    presetRecord = BikeDataManager.Presets[presets[i]];

                    //set color
                    presetToggles[i].gameObject.SetActive(true);
                    ColorPresetToggle(presetToggles[i], presetRecord);

                    if (presets.Length > 1)
                    { //show additional presets if more than one
                        additionalPresetToggles[i].gameObject.SetActive(true);
                        ColorPresetToggle(additionalPresetToggles[i], presetRecord);
                    }

                }// else {
                 //hide
                 //            }
            }

            if (infiniteScrollBehaviour != null)
            {
                infiniteScrollBehaviour.Init();
            }

            frameDelay = 1;
        }

        void ColorPresetToggle(Toggle toggle, PresetRecord presetRecord)
        {

            toggle.transform.Find("ColorPreset").GetComponent<Image>().color = presetRecord.Colors[0];
            if (presetRecord.Colors.Length > 1)
            {
                toggle.transform.Find("ColorPreset/Image").GetComponent<Image>().color = presetRecord.Colors[1];
            }
            else
            {
                toggle.transform.Find("ColorPreset/Image").GetComponent<Image>().enabled = false;
            }
        }

        #region scroll rect horizontal item snapping methods
        public float scrollRectHorizontalNormalizedPosition;
        public void CenterOnItem(RectTransform target)
        {
            //        print("CenterOnItem " + target.name + " " + activePresetIndex);
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

            scrollRectHorizontalNormalizedPosition = scrollRect.horizontalNormalizedPosition;
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
