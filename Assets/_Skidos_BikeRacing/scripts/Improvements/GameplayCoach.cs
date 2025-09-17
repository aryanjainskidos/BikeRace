using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameplayCoach : MonoBehaviour
{
    [Header("Coach Settings")]
    [SerializeField] private GameObject handCoachPrefab;
    
    [Header("Target Settings")]
    [SerializeField] private string pregameGameObjectName = "PreGame";
    [SerializeField] private string targetButtonName = "AccelerateButton";
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private int targetStage = 1;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    private GameObject handCoachInstance;
    private Button targetButton;
    private GameObject pregameGameObject;
    private bool isCoachActive = false;
    private bool hasShownCoach = false;
    
    private const string GAMEPLAY_COACH_KEY = "GameplayCoachShown";
    private const string CURRENT_STAGE_KEY = "CurrentStage";
    
    void Start()
    {
        Debug.Log("=== GameplayCoach Start() ===");
        
        // Check if coach has already been shown
        if (PlayerPrefs.GetInt(GAMEPLAY_COACH_KEY, 0) == 1)
        {
            Debug.Log("GameplayCoach: Already shown, skipping");
            hasShownCoach = true;
            return;
        }
        
        // Check if we're on the correct stage
        int currentStage = PlayerPrefs.GetInt(CURRENT_STAGE_KEY, 1);
        Debug.Log($"GameplayCoach: Current stage: {currentStage}, Target stage: {targetStage}");
        
        if (currentStage != targetStage)
        {
            Debug.Log($"GameplayCoach: Not on target stage {targetStage}, current: {currentStage}");
            hasShownCoach = true;
            return;
        }
        
        Debug.Log("GameplayCoach: Starting coroutine...");
        StartCoroutine(WaitForPregameAndTargetButton());
    }
    
    IEnumerator WaitForPregameAndTargetButton()
    {
        Debug.Log("GameplayCoach: Waiting for PreGame GameObject...");
        
        // Wait for the pregame GameObject to load (same as PlayButtonCoach)
        while (pregameGameObject == null && !hasShownCoach)
        {
            // Try multiple search methods like PlayButtonCoach
            pregameGameObject = GameObject.Find(pregameGameObjectName);
            
            // If not found by exact name, try with "(Clone)" suffix
            if (pregameGameObject == null)
            {
                pregameGameObject = GameObject.Find(pregameGameObjectName + "(Clone)");
            }
            
            // If still not found, search for any GameObject with "PreGame" in the name
            if (pregameGameObject == null)
            {
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.Contains("PreGame"))
                    {
                        pregameGameObject = obj;
                        Debug.Log($"GameplayCoach: Found PreGame by name search: {obj.name}");
                        break;
                    }
                }
            }
            
            if (pregameGameObject != null)
            {
                Debug.Log($"GameplayCoach: Found PreGame GameObject: {pregameGameObject.name}!");
                break;
            }
            
            Debug.Log($"GameplayCoach: PreGame GameObject '{pregameGameObjectName}' not found yet...");
            yield return new WaitForSeconds(checkInterval);
        }
        
        if (pregameGameObject == null)
        {
            Debug.LogError($"GameplayCoach: PreGame GameObject '{pregameGameObjectName}' not found!");
            yield break;
        }
        
        Debug.Log($"GameplayCoach: Waiting for {targetButtonName} inside pregame...");
        
        // Wait for the target button to be available inside the pregame
        while (targetButton == null && !hasShownCoach)
        {
            // Search for target button inside the pregame by name
            Button[] buttons = pregameGameObject.GetComponentsInChildren<Button>();
            Debug.Log($"GameplayCoach: Found {buttons.Length} buttons in PreGame");
            
            foreach (Button button in buttons)
            {
                Debug.Log($"GameplayCoach: Checking button: '{button.name}'");
                if (button.name == targetButtonName)
                {
                    targetButton = button;
                    Debug.Log($"GameplayCoach: Found {targetButtonName} inside pregame!");
                    break;
                }
            }
            
            // Alternative: Direct search by name
            if (targetButton == null)
            {
                GameObject targetButtonObj = GameObject.Find(targetButtonName);
                if (targetButtonObj != null)
                {
                    targetButton = targetButtonObj.GetComponent<Button>();
                    if (targetButton != null)
                    {
                        Debug.Log($"GameplayCoach: Found {targetButtonName} by name!");
                    }
                }
            }
            
            if (targetButton == null)
            {
                Debug.Log($"GameplayCoach: {targetButtonName} not found yet...");
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
        
        if (targetButton != null && !hasShownCoach)
        {
            Debug.Log("GameplayCoach: About to show coach...");
            ShowCoach();
        }
        else
        {
            Debug.LogError("GameplayCoach: Target button not found or coach already shown!");
        }
    }
    
    void ShowCoach()
    {
        Debug.Log("=== GameplayCoach ShowCoach() ===");
        
        if (handCoachPrefab == null) 
        {
            Debug.LogError("GameplayCoach: HandCoachPrefab is not assigned!");
            return;
        }
        
        if (pregameGameObject == null)
        {
            Debug.LogError("GameplayCoach: PreGame GameObject is null!");
            return;
        }
        
        Debug.Log("GameplayCoach: Creating hand coach instance...");
        
        // Create hand coach instance as CHILD of the pregame GameObject
        handCoachInstance = Instantiate(handCoachPrefab, pregameGameObject.transform);
        
        if (handCoachInstance == null)
        {
            Debug.LogError("GameplayCoach: Failed to instantiate HandCoach!");
            return;
        }
        
        Debug.Log($"GameplayCoach: HandCoach instantiated successfully: {handCoachInstance.name}");
        
        // Position it relative to the target button
        if (targetButton != null)
        {
            // Get the target button's position relative to the pregame
            Vector3 targetButtonLocalPos = targetButton.transform.localPosition;
            Debug.Log($"GameplayCoach: Target button local position: {targetButtonLocalPos}");
            
            // Set the hand coach position relative to the pregame with offset
            handCoachInstance.transform.localPosition = targetButtonLocalPos + offset;
            
            Debug.Log($"GameplayCoach: Hand coach positioned at {handCoachInstance.transform.localPosition}");
        }
        
        // Set animation type based on target
        HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
        if (animator != null)
        {
            if (targetButtonName == "StartButton")
            {
                animator.SetAnimationType(HandCoachAnimator.AnimationType.Point);
                Debug.Log("GameplayCoach: Set animation type to Point");
            }
            else
            {
                animator.SetAnimationType(HandCoachAnimator.AnimationType.Tap);
                Debug.Log("GameplayCoach: Set animation type to Tap");
            }
        }
        else
        {
            Debug.LogWarning("GameplayCoach: HandCoachAnimator component not found on prefab!");
        }
        
        // Listen for target button click
        targetButton.onClick.AddListener(OnTargetButtonClicked);
        Debug.Log("GameplayCoach: Added button listener");
        
        isCoachActive = true;
        Debug.Log("GameplayCoach: Coach setup completed successfully!");
    }
    
    void OnTargetButtonClicked()
    {
        if (isCoachActive)
        {
            Debug.Log($"GameplayCoach: {targetButtonName} clicked, dismissing coach");
            DismissCoach();
        }
    }
    
    void DismissCoach()
    {
        Debug.Log("GameplayCoach: Dismissing coach...");
        isCoachActive = false;
        
        // Remove button listener
        if (targetButton != null)
        {
            targetButton.onClick.RemoveListener(OnTargetButtonClicked);
        }
        
        // Stop animation and destroy coach
        if (handCoachInstance != null)
        {
            HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
            if (animator != null)
            {
                animator.StopAnimation();
            }
            else
            {
                Destroy(handCoachInstance);
            }
        }
        
        // Save PlayerPrefs
        PlayerPrefs.SetInt(GAMEPLAY_COACH_KEY, 1);
        PlayerPrefs.Save();
        
        Debug.Log("GameplayCoach: Coach dismissed and saved to PlayerPrefs");
        
        hasShownCoach = true;
    }
    
    void OnDestroy()
    {
        if (targetButton != null)
        {
            targetButton.onClick.RemoveListener(OnTargetButtonClicked);
        }
    }
    
    // Public method to reset coach (for testing)
    [ContextMenu("Reset Coach")]
    public void ResetCoach()
    {
        PlayerPrefs.DeleteKey(GAMEPLAY_COACH_KEY);
        PlayerPrefs.Save();
        hasShownCoach = false;
        Debug.Log("GameplayCoach: Reset - coach will show again");
    }
}