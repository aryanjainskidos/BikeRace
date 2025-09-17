using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonCoach : MonoBehaviour
{
    [Header("Coach Settings")]
    [SerializeField] private GameObject handCoachPrefab;
    
    [Header("Target Settings")]
    [SerializeField] private string menuGameObjectTag = "MainMenu";
    [SerializeField] private string playButtonTag = "PlayButton";
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private Vector3 offset = new Vector3(50, 0, 0); // X offset of 50
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    private GameObject handCoachInstance;
    private Button playButton;
    private GameObject menuGameObject;
    private bool isCoachActive = false;
    private bool hasShownCoach = false;
    
    private const string PLAY_BUTTON_COACH_KEY = "PlayButtonCoachShown";
    
    void Start()
    {
        // Check if coach has already been shown
        if (PlayerPrefs.GetInt(PLAY_BUTTON_COACH_KEY, 0) == 1)
        {
            if (debugMode) Debug.Log("PlayButtonCoach: Already shown, skipping");
            hasShownCoach = true;
            return;
        }
        
        StartCoroutine(WaitForMenuAndPlayButton());
    }
    
    IEnumerator WaitForMenuAndPlayButton()
    {
        if (debugMode) Debug.Log("PlayButtonCoach: Waiting for Menu GameObject...");
        
        // Wait for the menu GameObject to load
        while (menuGameObject == null && !hasShownCoach)
        {
            menuGameObject = GameObject.FindGameObjectWithTag(menuGameObjectTag);
            if (menuGameObject != null)
            {
                if (debugMode) Debug.Log("PlayButtonCoach: Found Menu GameObject!");
                break;
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
        
        if (menuGameObject == null)
        {
            if (debugMode) Debug.LogWarning("PlayButtonCoach: Menu GameObject not found!");
            yield break;
        }
        
        if (debugMode) Debug.Log("PlayButtonCoach: Waiting for PlayButton inside menu...");
        
        // Wait for the PlayButton to be available inside the menu
        while (playButton == null && !hasShownCoach)
        {
            // Search for PlayButton inside the menu
            Button[] buttons = menuGameObject.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.CompareTag(playButtonTag))
                {
                    playButton = button;
                    if (debugMode) Debug.Log("PlayButtonCoach: Found PlayButton inside menu!");
                    break;
                }
            }
            
            // Alternative: Direct search by tag
            if (playButton == null)
            {
                GameObject playButtonObj = GameObject.FindGameObjectWithTag(playButtonTag);
                if (playButtonObj != null)
                {
                    playButton = playButtonObj.GetComponent<Button>();
                    if (playButton != null)
                    {
                        if (debugMode) Debug.Log("PlayButtonCoach: Found PlayButton by tag!");
                    }
                }
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
        
        if (playButton != null && !hasShownCoach)
        {
            ShowCoach();
        }
    }
    
    void ShowCoach()
    {
        if (handCoachPrefab == null) 
        {
            Debug.LogError("PlayButtonCoach: HandCoachPrefab is not assigned!");
            return;
        }
        
        if (debugMode) Debug.Log("PlayButtonCoach: Showing coach");
        
        // Create hand coach instance as CHILD of the menu GameObject
        handCoachInstance = Instantiate(handCoachPrefab, menuGameObject.transform);
        
        // Position it relative to the PlayButton with X offset of 50
        if (playButton != null)
        {
            // Get the PlayButton's position relative to the menu
            Vector3 playButtonLocalPos = playButton.transform.localPosition;
            
            // Set the hand coach position relative to the menu with X offset
            handCoachInstance.transform.localPosition = playButtonLocalPos + offset;
            
            if (debugMode) Debug.Log($"PlayButtonCoach: Hand coach positioned at {handCoachInstance.transform.localPosition}");
        }
        
        // Set animation type to Point
        HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
        if (animator != null)
        {
            animator.SetAnimationType(HandCoachAnimator.AnimationType.Point);
        }
        else
        {
            Debug.LogWarning("PlayButtonCoach: HandCoachAnimator component not found on prefab!");
        }
        
        // Listen for PlayButton click
        playButton.onClick.AddListener(OnPlayButtonClicked);
        
        isCoachActive = true;
    }
    
    void OnPlayButtonClicked()
    {
        if (isCoachActive)
        {
            if (debugMode) Debug.Log("PlayButtonCoach: PlayButton clicked, dismissing coach");
            DismissCoach();
        }
    }
    
    void DismissCoach()
    {
        isCoachActive = false;
        
        // Remove button listener
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
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
        PlayerPrefs.SetInt(PLAY_BUTTON_COACH_KEY, 1);
        PlayerPrefs.Save();
        
        if (debugMode) Debug.Log("PlayButtonCoach: Coach dismissed and saved to PlayerPrefs");
        
        hasShownCoach = true;
    }
    
    void OnDestroy()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
        
        // Save PlayerPrefs when GameObject is destroyed to prevent coach from showing again
        if (!hasShownCoach)
        {
            PlayerPrefs.SetInt(PLAY_BUTTON_COACH_KEY, 1);
            PlayerPrefs.Save();
            if (debugMode) Debug.Log("PlayButtonCoach: GameObject destroyed, saving PlayerPrefs");
        }
    }
    
    // Public method to reset coach (for testing)
    [ContextMenu("Reset Coach")]
    public void ResetCoach()
    {
        PlayerPrefs.DeleteKey(PLAY_BUTTON_COACH_KEY);
        PlayerPrefs.Save();
        hasShownCoach = false;
        if (debugMode) Debug.Log("PlayButtonCoach: Reset - coach will show again");
    }
}
