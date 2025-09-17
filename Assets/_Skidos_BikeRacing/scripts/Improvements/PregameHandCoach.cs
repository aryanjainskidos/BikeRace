using System.Collections;
using UnityEngine;

public class PregameHandCoach : MonoBehaviour
{
    [Header("Coach Settings")]
    [SerializeField] private GameObject handCoachPrefab;

    [Header("Target Settings")]
    [SerializeField] private string targetObjectName = "StartButton"; // ✅ The name of the target object
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private Vector3 offset = new Vector3(50, 0, 0); // X offset of 50
    [SerializeField] private float showDelay = 1.0f; // Delay before showing coach

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private GameObject handCoachInstance;
    private GameObject targetObject;
    private bool hasShownCoach = false;

    private const string PREGAME_HAND_COACH_KEY = "PregameHandCoachShown";

    void Start()
    {
        // Check if coach has already been shown
        if (PlayerPrefs.GetInt(PREGAME_HAND_COACH_KEY, 0) == 1)
        {
            if (debugMode) Debug.Log("PregameHandCoach: Already shown, skipping");
            hasShownCoach = true;
            return;
        }

        StartCoroutine(WaitForTargetAndShowCoach());
    }

    IEnumerator WaitForTargetAndShowCoach()
    {
        if (debugMode) Debug.Log("PregameHandCoach: Waiting for target object...");

        // Wait for the target object to load
        while (targetObject == null && !hasShownCoach)
        {
            targetObject = GameObject.Find(targetObjectName); // ✅ Search by name instead of tag
            if (targetObject != null)
            {
                if (debugMode) Debug.Log("PregameHandCoach: Found target object!");
                break;
            }

            yield return new WaitForSeconds(checkInterval);
        }

        if (targetObject == null)
        {
            if (debugMode) Debug.LogWarning("PregameHandCoach: Target object not found!");
            yield break;
        }

        // Wait before showing coach
        yield return new WaitForSeconds(showDelay);

        if (!hasShownCoach)
        {
            ShowCoach();
        }
    }

    void ShowCoach()
    {
        if (handCoachPrefab == null)
        {
            Debug.LogError("PregameHandCoach: HandCoachPrefab is not assigned!");
            return;
        }

        if (debugMode) Debug.Log("PregameHandCoach: Showing coach");

        // Create hand coach instance as CHILD of this GameObject
        handCoachInstance = Instantiate(handCoachPrefab, transform);

        // Position it relative to the target object
        if (targetObject != null)
        {
            Vector3 targetLocalPos = targetObject.transform.localPosition;
            handCoachInstance.transform.localPosition = targetLocalPos + offset;

            if (debugMode) Debug.Log($"PregameHandCoach: Hand coach positioned at {handCoachInstance.transform.localPosition}");
        }

        // Optional animation
        HandCoachAnimator animator = handCoachInstance.GetComponent<HandCoachAnimator>();
        if (animator != null)
        {
            animator.SetAnimationType(HandCoachAnimator.AnimationType.Point);
        }

        hasShownCoach = true;
    }

    void OnDestroy()
    {
        // Save PlayerPrefs when this object is destroyed
        if (hasShownCoach)
        {
            PlayerPrefs.SetInt(PREGAME_HAND_COACH_KEY, 1);
            PlayerPrefs.Save();
            if (debugMode) Debug.Log("PregameHandCoach: Saved PlayerPrefs on destroy");
        }
    }

    // Public method to reset coach (for testing)
    [ContextMenu("Reset Coach")]
    public void ResetCoach()
    {
        PlayerPrefs.DeleteKey(PREGAME_HAND_COACH_KEY);
        PlayerPrefs.Save();
        hasShownCoach = false;
        if (debugMode) Debug.Log("PregameHandCoach: Reset - coach will show again");
    }
}
