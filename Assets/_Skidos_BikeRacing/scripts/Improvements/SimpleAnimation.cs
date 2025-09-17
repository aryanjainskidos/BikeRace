using UnityEngine;
using System.Collections;

public class ScaleAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float animationSpeed = 1.0f;
    [SerializeField] private bool startOnAwake = true;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    private Vector3 originalScale;
    private bool isAnimating = false;
    private Coroutine animationCoroutine;
    
    void Awake()
    {
        // Store the original scale
        originalScale = transform.localScale;
        
        if (debugMode)
        {
            Debug.Log($"ScaleAnimator: Original scale set to {originalScale}");
        }
    }
    
    void Start()
    {
        if (startOnAwake)
        {
            StartAnimation();
        }
    }
    
    public void StartAnimation()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            animationCoroutine = StartCoroutine(ScaleAnimation());
            
            if (debugMode)
            {
                Debug.Log("ScaleAnimator: Animation started");
            }
        }
    }
    
    public void StopAnimation()
    {
        if (isAnimating)
        {
            isAnimating = false;
            
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            
            // Reset to original scale
            transform.localScale = originalScale;
            
            if (debugMode)
            {
                Debug.Log("ScaleAnimator: Animation stopped");
            }
        }
    }
    
    IEnumerator ScaleAnimation()
    {
        while (isAnimating)
        {
            // Scale up from min to max
            yield return StartCoroutine(ScaleTo(maxScale));
            
            // Scale down from max to min
            yield return StartCoroutine(ScaleTo(minScale));
        }
    }
    
    IEnumerator ScaleTo(float targetScale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        
        float elapsedTime = 0f;
        float duration = 1f / animationSpeed;
        
        while (elapsedTime < duration && isAnimating)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // Smooth interpolation
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            yield return null;
        }
        
        // Ensure we end at the exact target scale
        if (isAnimating)
        {
            transform.localScale = endScale;
        }
    }

    void OnEnable()
    {
        if (startOnAwake)
        {
            StartAnimation();
        }
    }

    void OnDisable()
    {
        StopAnimation();
    }



    // Public methods for external control
    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = speed;
    }
    
    public void SetScaleRange(float min, float max)
    {
        minScale = min;
        maxScale = max;
    }
    
    public bool IsAnimating()
    {
        return isAnimating;
    }
}