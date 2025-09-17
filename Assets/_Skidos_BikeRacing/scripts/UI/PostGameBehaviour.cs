namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum PostGameStates
{
    Idle,
    WaitForStars,
    WaitForDrop,
    WaitForReveal,
    WaitForText,
}

public class PostGameBehaviour : MonoBehaviour
{

    GameObject stars;
    GameObject star1;
    GameObject star2;
    GameObject star3;

    List<Animator> starAnimators;

    //    GameObject time;
    //	Text lastTimeText;
    //	Text bestTimeText;
    public PostGameStates state;

    //    GameObject levelCoinPanel;
    CoinDisplayBehaviour coinDisplayBehaviour;

    Text coinText;
    //    Text coinDisplayText;
    Text titleText;

    //for animation coin view
    //    int actualCoins;
    //    float currentCoins;
    //    float deltaCoins = 5;

    //    Transform grid;
    GameObject prefab;
    List<GameObject> crateButtons;

    Text infoText;

    Button crateButton;

    GameObject cratePointerImage;

    TierPanelBehaviour tierPanelBehaviour1;
    TierPanelBehaviour tierPanelBehaviour2;
    TierPanelBehaviour tierPanelBehaviour3;

    //    public Animator starAnim;
    public Animator dropAnim;
    public Animator revealAnim;
    public Animator calloutAnim;

    //    AnimatorStateInfo starAnimState;
    AnimatorStateInfo dropAnimState;
    AnimatorStateInfo revealAnimState;

    Image boostImage;
    Text countText;
    Text calloutText;

    string[] callouts = new string[] { "Finish:Praise:almost...", "Finish:Praise:good", "Finish:Praise:great!", "Finish:Praise:awesome!" };

    int starsEarned;
    int starsShown;

    int cratesEarned;

    bool initialized = false;

    void Awake()
    {

        coinDisplayBehaviour = transform.Find("CoinPanel").GetComponent<CoinDisplayBehaviour>();
        coinDisplayBehaviour.auto = false;

        dropAnim = transform.Find("ItemBox_Drop").GetComponent<Animator>();
        revealAnim = transform.Find("ItemBox_Reveal").GetComponent<Animator>();
        calloutAnim = transform.Find("FinishCallout").GetComponent<Animator>();
        boostImage = transform.Find("ItemBox_Reveal/item").GetComponent<Image>();
        countText = transform.Find("ItemBox_Reveal/Text").GetComponent<Text>();
        calloutText = transform.Find("FinishCallout/Text").GetComponent<Text>();

        dropAnimState = dropAnim.GetCurrentAnimatorStateInfo(0);

        //titleText = transform.FindChild ("TitleText").GetComponent<Text>();

        coinText = transform.Find("LevelCoinPanel/CoinText").GetComponent<Text>();

        tierPanelBehaviour1 = transform.Find("TierPanel1").GetComponent<TierPanelBehaviour>();
        tierPanelBehaviour2 = transform.Find("TierPanel2").GetComponent<TierPanelBehaviour>();
        tierPanelBehaviour3 = transform.Find("TierPanel3").GetComponent<TierPanelBehaviour>();

        stars = transform.Find("Stars").gameObject;
        star1 = transform.Find("Stars/Star1").gameObject;
        star2 = transform.Find("Stars/Star2").gameObject;
        star3 = transform.Find("Stars/Star3").gameObject;
        star1.GetComponent<StarAnimationEvents>().finishDelegate = OnStarFinish;
        star2.GetComponent<StarAnimationEvents>().finishDelegate = OnStarFinish;
        star3.GetComponent<StarAnimationEvents>().finishDelegate = OnStarFinish;

        starAnimators = new List<Animator>() { star1.GetComponent<Animator>(), star2.GetComponent<Animator>(), star3.GetComponent<Animator>() };

        // infoText = transform.FindChild ("InfoPanel/InfoText").GetComponent<Text>();

        crateButton = transform.Find("CrateButton").GetComponent<Button>();
        crateButton.onClick.AddListener(() => OnCrateClick());

        crateButton.gameObject.SetActive(false);
        dropAnim.gameObject.SetActive(false);
        calloutAnim.gameObject.SetActive(false);

        state = PostGameStates.Idle;

        initialized = true;
    }

    //    void Start() {
    //        if (!initialized)
    //        {
    //            Awake();
    ////            OnEnable();
    //        }
    //    }

    void OnStarFinish()
    {
        //if last star in a row, drop the box, else animate next star
        starsShown++;

        if (starsEarned == starsShown)
        { //will skip showing stars if there are no stars to show
            if (cratesEarned == 0)
            {
                state = PostGameStates.WaitForText;//go straight to text
            }
            else
            {
                state = PostGameStates.WaitForDrop;//drop the box
            }
            //            dropAnim.gameObject.SetActive(true);
            //            SoundManager.Play("FinishCrateDrop");
        }
        else if (starsShown < starsEarned)
        {
            starAnimators[starsShown].speed = 1;
        }
    }

    void Update()
    {

        switch (state)
        {
            case PostGameStates.WaitForDrop:
                if (!dropAnim.gameObject.activeSelf)
                {
                    dropAnim.gameObject.SetActive(true);
                    SoundManager.Play("FinishCrateDrop");
                }
                UpdateDrop();
                break;
            case PostGameStates.WaitForReveal:
                UpdateReveal();
                break;
            case PostGameStates.WaitForText:
                if (!calloutAnim.gameObject.activeSelf)
                {
                    calloutAnim.gameObject.SetActive(true);
                    SoundManager.Play("FinishTextShow");
                }
                state = PostGameStates.Idle;
                break;
            default:
                break;
        }

    }

    void UpdateDrop()
    {

        dropAnimState = dropAnim.GetCurrentAnimatorStateInfo(0);

        if (dropAnimState.normalizedTime >= 1)
        { // if the last star finished animating animate the crate
            state = PostGameStates.Idle;
            dropAnim.gameObject.SetActive(false);
            crateButton.gameObject.SetActive(true);
        }

    }

    void UpdateReveal()
    {

        revealAnimState = revealAnim.GetCurrentAnimatorStateInfo(0);

        if (revealAnimState.normalizedTime >= 1)
        { // if the last star finished animating animate the crate
            revealAnim.gameObject.SetActive(false);
            //slide the text in
            state = PostGameStates.WaitForText;
            //            calloutAnim.gameObject.SetActive(true);
            //            SoundManager.Play("FinishTextShow");
        }

    }


    //aka finish screen
    public void OnEnable()
    {

        if (BikeGameManager.initialized && initialized)
        {

            int visualMultiplier = 1;
            if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
            { //ja ir dubult-koinu wíkends, tad te vizuáli palielina koinu apjomu - gan nopelníto, gan nepiecieśamo: "par"
                visualMultiplier = 2;
            }

            crateButton.gameObject.SetActive(false);
            revealAnim.gameObject.SetActive(false);
            dropAnim.gameObject.SetActive(false);
            calloutAnim.gameObject.SetActive(false);

            transform.Find("RestartButton").transform.localScale = Vector3.one;
            transform.Find("LevelButton").transform.localScale = Vector3.one;
            transform.Find("Background").transform.localScale = Vector3.one;

            transform.Find("RestartButton").GetComponent<iTweenEvent>().Play();
            transform.Find("LevelButton").GetComponent<iTweenEvent>().Play();
            transform.Find("Background").GetComponent<iTweenEvent>().Play();

            //            if (GameManager.levelInfo.type == "long" && GameManager.singlePlayerRestarts == 0) {
            //titleText.text = "Level \"" + LevelManager.CurrentLevelName + "\" failed" ;
            //infoText.text = "Level \"" + LevelManager.CurrentLevelName + "\"\n failed";
            //            } else {
            //titleText.text = "Level \"" + LevelManager.CurrentLevelName + "\" cleared" ;
            //infoText.text = "Level \"" + LevelManager.CurrentLevelName + "\"\n cleared";
            //            }

            if (BikeGameManager.levelInfo.type != "long")
            {

                if (!stars.activeSelf)
                    stars.SetActive(true);

                //                if(!time.activeSelf)
                //                    time.SetActive(true);

                star1.SetActive(false);
                star2.SetActive(false);
                star3.SetActive(false);

                //print("GameManager.playerState.starsEarned " + GameManager.playerState.starsEarned);

                if (BikeGameManager.playerState.starsEarned > 0)
                {
                    star1.SetActive(true);
                    if (BikeGameManager.playerState.starsEarned > 1)
                    {
                        star2.SetActive(true);
                        if (BikeGameManager.playerState.starsEarned > 2)
                        {
                            star3.SetActive(true);
                        }
                    }
                }

                foreach (var item in starAnimators)
                {
                    item.GetComponent<Animator>().speed = 0;
                }

                starsEarned = BikeGameManager.playerState.starsEarned;
                starsShown = -1;

                cratesEarned = PickupManager.CollectedCrateCount();

                state = PostGameStates.WaitForStars;

                OnStarFinish();

                calloutText.text = Lang.Get(callouts[BikeGameManager.playerState.starsEarned]);

                //                lastTimeText.text = "Your time: " + GameManager.TimeElapsed.ToString ("F2");
                //                bestTimeText.text = "Best time: " + DataManager.Levels[LevelManager.CurrentLevelName].BestTime.ToString ("F2");              



                tierPanelBehaviour1.SetData(BikeGameManager.TimeElapsed, BikeGameManager.levelInfo.TimePar, PickupManager.CoinsCollected * visualMultiplier, BikeGameManager.levelInfo.CoinPar * visualMultiplier);
                tierPanelBehaviour2.SetData(BikeGameManager.TimeElapsed, BikeGameManager.levelInfo.TimePar2, PickupManager.CoinsCollected * visualMultiplier, BikeGameManager.levelInfo.CoinPar2 * visualMultiplier);
                tierPanelBehaviour3.SetData(BikeGameManager.TimeElapsed, BikeGameManager.levelInfo.TimePar3, PickupManager.CoinsCollected * visualMultiplier, BikeGameManager.levelInfo.CoinPar3 * visualMultiplier);

            }
            else
            {
                stars.SetActive(false);
            }

            if (BikeGameManager.levelInfo.type != "long" || BikeGameManager.singlePlayerRestarts != 0)
            { //either not long or single player restarts == 0

                //                state = PostGameStates.WaitForStars;
                //                if (PickupManager.CollectedCrateCount() > 0){
                //                    print("somecrate");
                //                }
                //
                //                if (PickupManager.CollectedCrateCount() > 0){//PickupManager.coinCratesCollected.Count > 0 || 
                ////                    if(starsEarned > 0)
                //                        state = PostGameStates.WaitForStars;
                ////                    else
                ////                        state = PostGameStates.WaitForDrop;
                //                } else {
                //                    state = PostGameStates.Idle;
                //                }

                coinDisplayBehaviour.InitData(BikeDataManager.Coins - BikeDataManager.CoinsInLastLevel);

                //int totalCoinsInLevel = (GameManager.levelInfo.CoinsInLevel + GameManager.levelInfo.Coins2InLevel * PickupManager.CoinX2Value);
                coinText.text = (PickupManager.CoinsCollected * visualMultiplier).ToString();// + " / " + totalCoinsInLevel;

            }
            else
            {
                coinDisplayBehaviour.InitData(BikeDataManager.Coins);
            }

            //-----------news--------------		
            /*if(LevelManager.CurrentLevelName == "a___008" && PlayerPrefs.GetInt("promoNews8SP", 0) == 0){
				NewsListManager.Push(Lang.Get("News:Promo:BuyABike"), NewsListItemType.promo, GameScreenType.Garage); 
				PlayerPrefs.SetInt("promoNews8SP", 1);
			}
			if(LevelManager.CurrentLevelName == "a___016" && PlayerPrefs.GetInt("promoNews16SP", 0) == 0){
				NewsListManager.Push(Lang.Get("News:Promo:UpgradeBike"), NewsListItemType.promo, GameScreenType.Garage, "Upgrade", "Career");
				PlayerPrefs.SetInt("promoNews10SP", 1);
			}
			if(LevelManager.CurrentLevelName == "a___003" && PlayerPrefs.GetInt("promoNews3SP", 0) == 0){
				NewsListManager.Push(Lang.Get("News:Promo:BuyAStyle"), NewsListItemType.promo, GameScreenType.Garage); 
				PlayerPrefs.SetInt("promoNews3SP", 1);
				@todo - ielikt atpakalj
			} */
            /*if(LevelManager.CurrentLevelName == "a___012" && PlayerPrefs.GetInt("promoNews12SP", 0) == 0){
				NewsListManager.Push(Lang.Get("News:Promo:BuyAStyle"), NewsListItemType.promo, GameScreenType.Garage);
				PlayerPrefs.SetInt("promoNews12SP", 1);
				@todo - ielikt atpakalj
			}*/


            /**
			 * -----------promo--------------
			 * 
			 * @note -- ikviens promo caur metodi "SchedulePromo" tiek rádíts max 1 reizi (pieraksta iekś PlayerPrefs)
			 */
            if (LevelManager.CurrentLevelName == "a___003")
            {
                BikeDataManager.FirstStyle = true;
                PopupPromoBehaviour.SchedulePromo(PromoSubPopups.Styles);
            }
            //			if(LevelManager.CurrentLevelName == "a___008"){
            //				PopupPromoBehaviour.SchedulePromo(PromoSubPopups.SaleStyles);
            //			}
            //			if(LevelManager.CurrentLevelName == "a___012"){
            //				PopupPromoBehaviour.SchedulePromo(PromoSubPopups.Sale50);
            //			}
            //			if(LevelManager.CurrentLevelName == "a___021"){
            //				PopupPromoBehaviour.SchedulePromo(PromoSubPopups.Sale70);
            //			}

            PlayerPrefs.Save();

        }

    }

    void OnCrateClick()
    {

        foreach (var item in PickupManager.cratesCollected)
        {
            if (item.Value.Count > 0)
            {

                int visualMultiplier = 1;
                if (CentralizedOfferManager.IsDoubleCoinWeekendOn())
                { //ja ir dubult-koinu wíkends, tad te vizuáli palielina koinu apjomu
                    visualMultiplier = 2;
                }

                // set image + extra for coin
                if (item.Key == "coin" || item.Key == "coinOnce")
                {
                    coinDisplayBehaviour.coinsTo += item.Value[0] * visualMultiplier;
                    boostImage.sprite = UIManager.GetRideSprite(item.Key == "coinOnce" ? "coin" : item.Key);
                }
                else
                {
                    boostImage.sprite = UIManager.GetBoostSprite(item.Key);
                }

                // set count text
                countText.text = "x" + (item.Value[0] * visualMultiplier).ToString();

                item.Value.RemoveAt(0);
                break;
            }
        }

        //        if (PickupManager.cratesCollected["coin"].Count > 0) { //if (PickupManager.coinCratesCollected.Count > 0)
        //            coinDisplayBehaviour.coinsTo += PickupManager.cratesCollected["coin"][0];
        //            PickupManager.cratesCollected["coin"].RemoveAt(0);
        //        }

        if (PickupManager.CollectedCrateCount() <= 0)
        {//if (PickupManager.coinCratesCollected.Count <= 0){
            crateButton.gameObject.SetActive(false);
        }

        state = PostGameStates.WaitForReveal;

        revealAnim.gameObject.SetActive(true);
        SoundManager.Play("FinishCrateClick");
        SoundManager.Play("FinishCrateOpen");
    }

}



}
