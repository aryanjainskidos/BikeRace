namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class HandCoachPrefabBehaviour : MonoBehaviour
    {
        [Header("Hand Coach Animation")]
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.8f, 1, 1.2f);
        public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);
        public float animationSpeed = 1f;

        public bool enableScaleAnimation = true;
        public bool enableAlphaAnimation = true;

        [Header("Components")]
        public Image handImage;
        public RectTransform handTransform;

        private Vector3 originalScale;
        private CanvasGroup canvasGroup;
        private Coroutine animationCoroutine;

        void Awake()
        {
            if (handImage == null) handImage = GetComponent<Image>();
            if (handTransform == null) handTransform = GetComponent<RectTransform>();

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (handTransform != null)
                originalScale = handTransform.localScale;
        }

        void OnEnable() => StartHandAnimation();
        void OnDisable() => StopHandAnimation();

        public void StartHandAnimation()
        {
            StopHandAnimation();
            animationCoroutine = StartCoroutine(AnimateHand());
        }

        public void StopHandAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }

        IEnumerator AnimateHand()
        {
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime * animationSpeed;
                float normalizedTime = time % 1f;

                if (enableScaleAnimation && handTransform != null)
                {
                    float scaleValue = scaleCurve.Evaluate(normalizedTime);
                    handTransform.localScale = originalScale * scaleValue;
                }

                if (enableAlphaAnimation && canvasGroup != null)
                {
                    float alphaValue = alphaCurve.Evaluate(normalizedTime);
                    canvasGroup.alpha = alphaValue;
                }

                yield return null;
            }
        }

        // Helper setters
        public void SetPosition(Vector3 position) => handTransform.localPosition = position;
        public void SetScale(Vector3 scale)
        {
            originalScale = scale;
            handTransform.localScale = scale;
        }
        public void SetRotation(Quaternion rotation) => handTransform.localRotation = rotation;
    }
}
