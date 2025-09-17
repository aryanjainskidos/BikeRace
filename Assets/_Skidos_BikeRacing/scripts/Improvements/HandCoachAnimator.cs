using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HandCoachAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float tapAnimationDuration = 0.5f;
    
    [Header("Animation Type")]
    [SerializeField] private AnimationType animationType = AnimationType.Pulse;
    
    public enum AnimationType
    {
        Pulse,
        Tap,
        Point
    }
    
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private bool isAnimating = false;
    private bool isTapping = false;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Store the original scale BEFORE any modifications
        originalScale = transform.localScale;
        
        // Ensure we have a valid scale to start with
        if (originalScale == Vector3.zero)
        {
            originalScale = Vector3.one;
            transform.localScale = originalScale;
        }
    }
    
    void Start()
    {
        StartAnimation();
    }
    
    public void StartAnimation()
    {
        if (isAnimating) return;
        
        isAnimating = true;
        StartCoroutine(FadeIn());
        
        switch (animationType)
        {
            case AnimationType.Pulse:
                StartCoroutine(PulseAnimation());
                break;
            case AnimationType.Tap:
                StartCoroutine(TapAnimation());
                break;
            case AnimationType.Point:
                StartCoroutine(PulseAnimation());
                break;
        }
    }
    
    public void StopAnimation()
    {
        if (!isAnimating) return;
        
        isAnimating = false;
        isTapping = false;
        StartCoroutine(FadeOut());
    }
    
    IEnumerator PulseAnimation()
    {
        Vector3 targetScale = originalScale * pulseScale;
        
        while (isAnimating && !isTapping)
        {
            float time = 0f;
            while (time < 1f && isAnimating && !isTapping)
            {
                time += Time.deltaTime * pulseSpeed;
                // FIX: Use proper PingPong calculation
                float pingPongValue = Mathf.PingPong(time, 1f);
                float scale = Mathf.Lerp(originalScale.x, targetScale.x, pingPongValue);
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
        }
    }
    
    IEnumerator TapAnimation()
    {
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
        
        // Destroy after fade out
        Destroy(gameObject);
    }
    
    public void SetAnimationType(AnimationType type)
    {
        animationType = type;
    }
}