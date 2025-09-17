namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class HandCoachManager : MonoBehaviour
    {
        [Header("Hand Coach Settings")]
        [Tooltip("Main hand coach prefab array (assign your 3 prefabs here)")]
        public GameObject[] handCoachPrefabs;

        [Tooltip("Which prefab index to use from the array")]
        public int prefabIndex = 0;

        [Header("Spawn Settings")]
        [Tooltip("Where to spawn the hand coach prefab (usually your UI canvas)")]
        public Transform parentTransform;

        [Tooltip("Offset position for the spawned prefab")]
        public Vector3 offsetPosition = new Vector3(0, 50, 0);

        [Header("Animation Timing")]
        [Tooltip("How long to show the hand coach animation")]
        public float animationDuration = 10f;

        [Tooltip("Fade in duration")]
        public float fadeInDuration = 0.5f;

        [Tooltip("Fade out duration")]
        public float fadeOutDuration = 0.5f;

        [Header("First Time Settings")]
        [Tooltip("PlayerPrefs key to track if hand coach has been shown")]
        public string firstTimeKey = "BikeRacing_HandCoachShown";

        [Tooltip("Show hand coach on start if first time")]
        public bool showOnStart = true;

        [Header("Animation Settings")]
        [Tooltip("Animation speed for the hand coach")]
        public float animationSpeed = 0.5f;

        [Tooltip("Scale animation curve")]
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.8f, 1, 1.2f);

        [Tooltip("Alpha animation curve for pulsing effect")]
        public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);

        private GameObject handCoachInstance;
        private bool isShowing = false;
        private Coroutine animationCoroutine;

        void Start()
        {
            if (showOnStart && ShouldShowHandCoach())
            {
                ShowHandCoach();
            }
        }

        public bool ShouldShowHandCoach()
        {
            return !PlayerPrefs.HasKey(firstTimeKey);
        }

        public void ShowHandCoach()
        {
            if (isShowing)
            {
                Debug.LogWarning("HandCoachManager: Already showing hand coach");
                return;
            }

            if (handCoachPrefabs == null || handCoachPrefabs.Length == 0)
            {
                Debug.LogWarning("HandCoachManager: No prefabs assigned!");
                return;
            }

            GameObject prefabToUse = handCoachPrefabs[Mathf.Clamp(prefabIndex, 0, handCoachPrefabs.Length - 1)];
            if (prefabToUse == null)
            {
                Debug.LogWarning("HandCoachManager: Selected prefab is null");
                return;
            }

            isShowing = true;

            Transform spawnParent = parentTransform != null ? parentTransform : this.transform;
            handCoachInstance = Instantiate(prefabToUse, spawnParent);
            handCoachInstance.transform.localPosition = offsetPosition;

            CanvasGroup canvasGroup = handCoachInstance.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = handCoachInstance.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            HandCoachPrefabBehaviour prefabBehaviour = handCoachInstance.GetComponent<HandCoachPrefabBehaviour>();
            if (prefabBehaviour == null) prefabBehaviour = handCoachInstance.AddComponent<HandCoachPrefabBehaviour>();

            prefabBehaviour.animationSpeed = animationSpeed;
            prefabBehaviour.scaleCurve = scaleCurve;
            prefabBehaviour.alphaCurve = alphaCurve;

            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateHandCoach());

            PlayerPrefs.SetInt(firstTimeKey, 1);
            PlayerPrefs.Save();

            Debug.Log("HandCoachManager: Hand coach shown for first time");
        }

        public void HideHandCoach()
        {
            if (handCoachInstance != null)
            {
                if (animationCoroutine != null) StopCoroutine(animationCoroutine);
                animationCoroutine = StartCoroutine(FadeOutAndDestroy());
            }
        }

        public void ForceShowHandCoach()
        {
            HideHandCoach();
            ShowHandCoach();
        }

        public void ResetFirstTimeFlag()
        {
            PlayerPrefs.DeleteKey(firstTimeKey);
            PlayerPrefs.Save();
            Debug.Log("HandCoachManager: First time flag reset - will show on next start");
        }

        IEnumerator AnimateHandCoach()
        {
            CanvasGroup canvasGroup = handCoachInstance.GetComponent<CanvasGroup>();

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            HandCoachPrefabBehaviour prefabBehaviour = handCoachInstance.GetComponent<HandCoachPrefabBehaviour>();
            if (prefabBehaviour != null) prefabBehaviour.StartHandAnimation();

            yield return new WaitForSeconds(animationDuration);

            yield return StartCoroutine(FadeOutAndDestroy());
        }

        IEnumerator FadeOutAndDestroy()
        {
            if (handCoachInstance == null) yield break;

            HandCoachPrefabBehaviour prefabBehaviour = handCoachInstance.GetComponent<HandCoachPrefabBehaviour>();
            if (prefabBehaviour != null) prefabBehaviour.StopHandAnimation();

            CanvasGroup canvasGroup = handCoachInstance.GetComponent<CanvasGroup>();
            float elapsed = 0f;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            if (handCoachInstance != null) Destroy(handCoachInstance);

            isShowing = false;
            animationCoroutine = null;
        }

        void OnDestroy()
        {
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        }

#if UNITY_EDITOR
        [ContextMenu("Test Show Hand Coach")]
        public void TestShowHandCoach() => ForceShowHandCoach();

        [ContextMenu("Test Reset First Time")]
        public void TestResetFirstTime() => ResetFirstTimeFlag();
#endif
    }
}
