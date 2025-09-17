namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class MapLevelCoach : MonoBehaviour
    {
        [Header("Coach Settings")]
        [SerializeField] private GameObject handCoachPrefab;
        
        [Header("Target Settings")]
        [SerializeField] private string playerPointerTag = "PlayerPointer";
        [SerializeField] private float checkInterval = 0.1f;
        [SerializeField] private Vector3 offset = new Vector3(0, 80, 0); // Y offset above PlayerPointer
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        
        private GameObject handCoachInstance;
        private GameObject playerPointerTarget;
        private bool isCoachActive = false;
        private bool hasShownCoach = false;
        
        private const string MAP_LEVEL_COACH_KEY = "MapLevelCoachShown";
        
        void Start()
        {
            Debug.Log("MapLevelCoach: Start() called");
            
            // Check if coach has already been shown
            if (PlayerPrefs.GetInt(MAP_LEVEL_COACH_KEY, 0) == 1)
            {
                Debug.Log("MapLevelCoach: Already shown, skipping");
                hasShownCoach = true;
                return;
            }
            
            Debug.Log("MapLevelCoach: Started, waiting for Levels screen to be active");
        }

        void Awake()
        {
            Debug.Log("MapLevelCoach: Start() called");
            
            // Check if coach has already been shown
            if (PlayerPrefs.GetInt(MAP_LEVEL_COACH_KEY, 0) == 1)
            {
                Debug.Log("MapLevelCoach: Already shown, skipping");
                hasShownCoach = true;
                return;
            }
            
            Debug.Log("MapLevelCoach: Started, waiting for Levels screen to be active");
        }
        
        void OnEnable()
        {
            Debug.Log("MapLevelCoach: OnEnable() called");
            
            if (!hasShownCoach && Startup.Initialized)
            {
                Debug.Log("MapLevelCoach: Starting coach search");
                StartCoroutine(WaitForPlayerPointer());
            }
            else
            {
                Debug.Log($"MapLevelCoach: Skipping - hasShownCoach: {hasShownCoach}, Startup.Initialized: {Startup.Initialized}");
            }
        }
        
        void OnDisable()
        {
            Debug.Log("MapLevelCoach: OnDisable() called");
            
            if (isCoachActive)
            {
                DismissCoach();
            }
        }
        
        IEnumerator WaitForPlayerPointer()
        {
            Debug.Log("MapLevelCoach: Searching for PlayerPointer with tag: " + playerPointerTag);
            
            int attempts = 0;
            while (playerPointerTarget == null && !hasShownCoach && attempts < 100) // Max 10 seconds
            {
                playerPointerTarget = GameObject.FindGameObjectWithTag(playerPointerTag);
                if (playerPointerTarget != null)
                {
                    Debug.Log("MapLevelCoach: Found PlayerPointer! Name: " + playerPointerTarget.name);
                    break;
                }
                
                Debug.Log($"MapLevelCoach: Attempt {attempts + 1} - PlayerPointer not found yet...");
                yield return new WaitForSeconds(checkInterval);
                attempts++;
            }
            
            if (playerPointerTarget == null)
            {
                Debug.LogError("MapLevelCoach: PlayerPointer not found after 10 seconds!");
                yield break;
            }
            
            Debug.Log("MapLevelCoach: PlayerPointer found, waiting 1 second before showing coach...");
            yield return new WaitForSeconds(1.0f);
            
            ShowCoach();
        }

        // void ShowCoach()
        // {
        //     Debug.Log("MapLevelCoach: ShowCoach() called");

        //     if (handCoachPrefab == null) 
        //     {
        //         Debug.LogError("MapLevelCoach: HandCoachPrefab is not assigned!");
        //         return;
        //     }

        //     if (playerPointerTarget == null)
        //     {
        //         Debug.LogError("MapLevelCoach: PlayerPointer target is null!");
        //         return;
        //     }

        //     Debug.Log("MapLevelCoach: HandCoachPrefab is assigned, creating instance...");

        //     // Create hand coach instance as CHILD of the found PlayerPointer GameObject
        //     handCoachInstance = Instantiate(handCoachPrefab, playerPointerTarget.transform);
        //     Debug.Log("MapLevelCoach: Hand coach instance created as child of PlayerPointer!");

        //     // Position it relative to the PlayerPointer with offset
        //     handCoachInstance.transform.localPosition = offset;
        //     Debug.Log($"MapLevelCoach: Hand coach positioned at {handCoachInstance.transform.localPosition}");

        //     // Set animation type to Point
        //     HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
        //     if (animator != null)
        //     {
        //         Debug.Log("MapLevelCoach: HandCoachAnimator found, setting animation type to Point");
        //         animator.SetAnimationType(HandCoachAnimator.AnimationType.Point);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("MapLevelCoach: HandCoachAnimator component not found on prefab!");
        //     }

        //     isCoachActive = true;
        //     Debug.Log("MapLevelCoach: Coach is now active!");
        // }
        

        void ShowCoach()
{
    Debug.Log("MapLevelCoach: ShowCoach() called");

    if (handCoachPrefab == null) 
    {
        Debug.LogError("MapLevelCoach: HandCoachPrefab is not assigned!");
        return;
    }
    
    if (playerPointerTarget == null)
    {
        Debug.LogError("MapLevelCoach: PlayerPointer target is null!");
        return;
    }

    Debug.Log("MapLevelCoach: HandCoachPrefab is assigned, creating instance...");
    
    // Create hand coach instance as CHILD of the found PlayerPointer GameObject
    handCoachInstance = Instantiate(handCoachPrefab, playerPointerTarget.transform);
    Debug.Log("MapLevelCoach: Hand coach instance created as child of PlayerPointer!");
    
    // Position it relative to the PlayerPointer with offset
    handCoachInstance.transform.localPosition = offset;
    Debug.Log($"MapLevelCoach: Hand coach positioned at {handCoachInstance.transform.localPosition}");
    
    // Set animation type to Point
    HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
    if (animator != null)
    {
        Debug.Log("MapLevelCoach: HandCoachAnimator found, setting animation type to Point");
        animator.SetAnimationType(HandCoachAnimator.AnimationType.Point);
    }
    else
    {
        Debug.LogWarning("MapLevelCoach: HandCoachAnimator component not found on prefab!");
    }
    
    isCoachActive = true;
    Debug.Log("MapLevelCoach: Coach is now active!");

    // ✅ Save immediately so it never shows again on next visit
    PlayerPrefs.SetInt(MAP_LEVEL_COACH_KEY, 1);
    PlayerPrefs.Save();
    hasShownCoach = true;
}

        
        void Update()
        {
            // Check for any input to dismiss coach
            if (isCoachActive && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
            {
                Debug.Log("MapLevelCoach: Input detected, dismissing coach");
                DismissCoach();
            }
        }
        
        void DismissCoach()
        {
            Debug.Log("MapLevelCoach: DismissCoach() called");
            isCoachActive = false;
            
            if (handCoachInstance != null)
            {
                HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
                if (animator != null)
                {
                    animator.StopAnimation();

                    PlayerPrefs.SetInt(MAP_LEVEL_COACH_KEY, 1);
                    PlayerPrefs.Save();
                    Debug.Log("MapLevelCoach: Coach dismissed and saved to PlayerPrefs");
                    hasShownCoach = true;
                    Destroy(handCoachInstance);
                }
                else
                {
                    PlayerPrefs.SetInt(MAP_LEVEL_COACH_KEY, 1);
                    PlayerPrefs.Save();
                    Debug.Log("MapLevelCoach: Coach dismissed and saved to PlayerPrefs");
                    hasShownCoach = true;
                    Destroy(handCoachInstance);
                }
            }
            
            
        }
        
        [ContextMenu("Reset Coach")]
        public void ResetCoach()
        {
            PlayerPrefs.DeleteKey(MAP_LEVEL_COACH_KEY);
            PlayerPrefs.Save();
            hasShownCoach = false;
            Debug.Log("MapLevelCoach: Reset - coach will show again");
        }
    }
}