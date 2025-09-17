using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float tapAnimationDuration = 0.5f;
    
    [Header("Animation Type")]
    [SerializeField] private AnimationType animationType = AnimationType.Pulse;
    
    [Header("Auto Start")]
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private float startDelay = 1f; // Increased delay for runtime loading
    [SerializeField] private bool waitForParentActive = true; // Wait for parent to be active
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    public enum AnimationType
    {
        Pulse,
        Tap,
        Fade,
        Bounce,
        Rotate
    }
    
    private Image image;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private bool isAnimating = false;
    private bool isTapping = false;
    private bool hasStarted = false;
    
    void Awake()
    {
        image = GetComponent<Image>();
        
        // Add CanvasGroup if it doesn't exist
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Store original scale
        originalScale = transform.localScale;
        
        // Ensure we have a valid scale
        if (originalScale == Vector3.zero)
        {
            originalScale = Vector3.one;
            transform.localScale = originalScale;
        }
        
        if (debugMode) Debug.Log($"UIAnimator Awake: {gameObject.name}, Scale: {originalScale}");
    }
    
    void Start()
    {
        if (debugMode) Debug.Log($"UIAnimator Start: {gameObject.name}");
        
        if (startOnAwake)
        {
            StartCoroutine(WaitAndStart());
        }
    }
    
    IEnumerator WaitAndStart()
    {
        if (debugMode) Debug.Log($"UIAnimator: Waiting to start animation for {gameObject.name}");
        
        // Wait for parent to be active (if enabled)
        if (waitForParentActive)
        {
            while (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
            {
                if (debugMode) Debug.Log($"UIAnimator: Waiting for parent to be active: {transform.parent.name}");
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // Wait for the specified delay
        if (startDelay > 0)
        {
            if (debugMode) Debug.Log($"UIAnimator: Waiting {startDelay} seconds before starting");
            yield return new WaitForSeconds(startDelay);
        }
        
        // Ensure the GameObject is still active
        if (gameObject.activeInHierarchy)
        {
            if (debugMode) Debug.Log($"UIAnimator: Starting animation for {gameObject.name}");
            StartAnimation();
        }
        else
        {
            if (debugMode) Debug.LogWarning($"UIAnimator: GameObject {gameObject.name} is not active, skipping animation");
        }
    }
    
    void OnEnable()
    {
        if (debugMode) Debug.Log($"UIAnimator OnEnable: {gameObject.name}");
        
        // If the GameObject was disabled and re-enabled, restart animation
        if (hasStarted && !isAnimating)
        {
            StartCoroutine(WaitAndStart());
        }
    }
    
    void OnDisable()
    {
        if (debugMode) Debug.Log($"UIAnimator OnDisable: {gameObject.name}");
        StopAnimation();
    }
    
    public void StartAnimation()
    {
        if (isAnimating) 
        {
            if (debugMode) Debug.Log($"UIAnimator: Animation already running for {gameObject.name}");
            return;
        }
        
        if (!gameObject.activeInHierarchy)
        {
            if (debugMode) Debug.LogWarning($"UIAnimator: GameObject {gameObject.name} is not active, cannot start animation");
            return;
        }
        
        if (debugMode) Debug.Log($"UIAnimator: Starting {animationType} animation for {gameObject.name}");
        
        isAnimating = true;
        hasStarted = true;
        
        StartCoroutine(FadeIn());
        
        switch (animationType)
        {
            case AnimationType.Pulse:
                StartCoroutine(PulseAnimation());
                break;
            case AnimationType.Tap:
                StartCoroutine(TapAnimation());
                break;
            case AnimationType.Fade:
                StartCoroutine(FadeAnimation());
                break;
            case AnimationType.Bounce:
                StartCoroutine(BounceAnimation());
                break;
            case AnimationType.Rotate:
                StartCoroutine(RotateAnimation());
                break;
        }
    }
    
    public void StopAnimation()
    {
        if (!isAnimating) return;
        
        if (debugMode) Debug.Log($"UIAnimator: Stopping animation for {gameObject.name}");
        
        isAnimating = false;
        isTapping = false;
        StopAllCoroutines();
    }
    
    IEnumerator PulseAnimation()
    {
        if (debugMode) Debug.Log($"UIAnimator: Starting pulse animation for {gameObject.name}");
        
        Vector3 targetScale = originalScale * pulseScale;
        
        while (isAnimating && !isTapping)
        {
            float time = 0f;
            while (time < 1f && isAnimating && !isTapping)
            {
                time += Time.deltaTime * pulseSpeed;
                float pingPongValue = Mathf.PingPong(time, 1f);
                float scale = Mathf.Lerp(originalScale.x, targetScale.x, pingPongValue);
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
        }
    }
    
    IEnumerator TapAnimation()
    {
        if (debugMode) Debug.Log($"UIAnimator: Starting tap animation for {gameObject.name}");
        
        while (isAnimating)
        {
            // Wait before starting tap animation
            yield return new WaitForSeconds(2f);
            
            if (!isAnimating) break;
            
            isTapping = true;
            
            // Tap down animation
            Vector3 tapScale = originalScale * 0.8f;
            
            float elapsed = 0f;
            while (elapsed < tapAnimationDuration / 2f && isAnimating)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (tapAnimationDuration / 2f);
                transform.localScale = Vector3.Lerp(originalScale, tapScale, progress);
                yield return null;
            }
            
            // Tap up animation
            elapsed = 0f;
            while (elapsed < tapAnimationDuration / 2f && isAnimating)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (tapAnimationDuration / 2f);
                transform.localScale = Vector3.Lerp(tapScale, originalScale, progress);
                yield return null;
            }
            
            isTapping = false;
            
            // Wait before next tap
            yield return new WaitForSeconds(1f);
        }
    }
    
    IEnumerator FadeAnimation()
    {
        if (debugMode) Debug.Log($"UIAnimator: Starting fade animation for {gameObject.name}");
        
        while (isAnimating)
        {
            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeOutDuration && isAnimating)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }
            
            if (!isAnimating) break;
            
            // Wait
            yield return new WaitForSeconds(0.5f);
            
            if (!isAnimating) break;
            
            // Fade in
            elapsed = 0f;
            while (elapsed < fadeInDuration && isAnimating)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }
            
            // Wait
            yield return new WaitForSeconds(1f);
        }
    }
    
    IEnumerator BounceAnimation()
    {
        if (debugMode) Debug.Log($"UIAnimator: Starting bounce animation for {gameObject.name}");
        
        while (isAnimating)
        {
            // Bounce up
            float elapsed = 0f;
            float bounceHeight = 0.3f;
            Vector3 bounceScale = originalScale * (1f + bounceHeight);
            
            while (elapsed < 0.3f && isAnimating)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / 0.3f;
                float bounce = Mathf.Sin(progress * Mathf.PI);
                transform.localScale = Vector3.Lerp(originalScale, bounceScale, bounce);
                yield return null;
            }
            
            // Return to normal
            elapsed = 0f;
            while (elapsed < 0.3f && isAnimating)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / 0.3f;
                transform.localScale = Vector3.Lerp(bounceScale, originalScale, progress);
                yield return null;
            }
            
            // Wait
            yield return new WaitForSeconds(1f);
        }
    }
    
    IEnumerator RotateAnimation()
    {
        if (debugMode) Debug.Log($"UIAnimator: Starting rotate animation for {gameObject.name}");
        
        while (isAnimating)
        {
            float elapsed = 0f;
            float rotationSpeed = 360f; // degrees per second
            
            while (elapsed < 2f && isAnimating)
            {
                elapsed += Time.deltaTime;
                float rotation = elapsed * rotationSpeed;
                transform.rotation = Quaternion.Euler(0, 0, rotation);
                yield return null;
            }
            
            // Reset rotation
            transform.rotation = Quaternion.identity;
            
            // Wait
            yield return new WaitForSeconds(1f);
        }
    }
    
    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration && isAnimating)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        
        if (isAnimating)
        {
            canvasGroup.alpha = 1f;
        }
    }
    
    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
    
    public void SetAnimationType(AnimationType type)
    {
        animationType = type;
        if (debugMode) Debug.Log($"UIAnimator: Animation type changed to {type} for {gameObject.name}");
    }
    
    public void SetPulseSpeed(float speed)
    {
        pulseSpeed = speed;
    }
    
    public void SetPulseScale(float scale)
    {
        pulseScale = scale;
    }
    
    // Public method to fade out and destroy
    public void FadeOutAndDestroy()
    {
        StartCoroutine(FadeOutAndDestroyCoroutine());
    }
    
    IEnumerator FadeOutAndDestroyCoroutine()
    {
        yield return StartCoroutine(FadeOut());
        Destroy(gameObject);
    }
    
    // Debug method to test animation manually
    [ContextMenu("Test Animation")]
    public void TestAnimation()
    {
        if (debugMode) Debug.Log($"UIAnimator: Testing animation for {gameObject.name}");
        StartAnimation();
    }
}