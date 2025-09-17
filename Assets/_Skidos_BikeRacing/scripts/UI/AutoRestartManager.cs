using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace vasundharabikeracing
{
    public class AutoRestartManager : MonoBehaviour
    {
        [Header("Auto Restart Settings")]
        [SerializeField] private float autoRestartDelay = 3f;
        [SerializeField] private bool enableAutoRestart = true;
        [SerializeField] private bool showDebugLogs = true;
        
        [Header("Button Reference")]
        [SerializeField] private UIButtonSwitchScreen restartButtonSwitch;
        
        private Coroutine autoRestartCoroutine;
        private bool autoRestartStarted = false;
        private bool playerPressedRestart = false;
        
        void Awake()
        {
            // Find the restart button switch component if not assigned
            if (restartButtonSwitch == null)
            {
                restartButtonSwitch = transform.Find("StartButton")?.GetComponent<UIButtonSwitchScreen>();
            }
        }
        
        void OnEnable()
        {
            // Reset state when crash screen is enabled
            ResetAutoRestart();
            
            // Start auto restart when crash screen is shown
            if (enableAutoRestart && !autoRestartStarted)
            {
                StartAutoRestart();
            }
        }
        
        void OnDisable()
        {
            // Stop auto restart when crash screen is disabled
            StopAutoRestart();
        }
        
        public void StartAutoRestart()
        {
            if (enableAutoRestart && !autoRestartStarted && !playerPressedRestart)
            {
                autoRestartStarted = true;
                autoRestartCoroutine = StartCoroutine(AutoRestartCountdown());
                
                if (showDebugLogs)
                {
                    Debug.Log("AutoRestartManager: Starting auto restart countdown");
                }
            }
        }
        
        void Update()
        {
            // Check if player manually pressed restart by detecting game command
            if (autoRestartStarted && BikeGameManager.lastCommand == GameCommand.Reset)
            {
                OnRestartButtonClicked();
            }
        }
        
        public void OnRestartButtonClicked()
        {
            if (autoRestartStarted)
            {
                if (showDebugLogs)
                {
                    Debug.Log("AutoRestartManager: Player manually pressed restart - canceling auto restart");
                }
                
                playerPressedRestart = true;
                StopAutoRestart();
            }
        }
        
        IEnumerator AutoRestartCountdown()
        {
            if (showDebugLogs)
            {
                Debug.Log($"AutoRestartManager: Waiting {autoRestartDelay} seconds before auto restart");
            }
            
            yield return new WaitForSeconds(autoRestartDelay);
            
            if (showDebugLogs)
            {
                Debug.Log("AutoRestartManager: Auto restart countdown finished");
            }
            
            // Check if we should still auto restart (player hasn't clicked manually)
            if (!playerPressedRestart)
            {
                if (showDebugLogs)
                {
                    Debug.Log("AutoRestartManager: Executing auto restart - switching to PreGame");
                }
                
                // Execute the same logic as the restart button
                ExecuteRestartLogic();
            }
        }
        
        void ExecuteRestartLogic()
        {
            if (showDebugLogs)
            {
                Debug.Log("AutoRestartManager: Executing restart logic - GameCommand.Reset + Screen Switch");
            }
            
            // Step 1: Execute GameCommand.Reset (same as UIButtonGameCommand)
            BikeGameManager.ExecuteCommand(GameCommand.Reset);
            
            // Step 2: Switch to PreGame screen (same as UIButtonSwitchScreen)
            // This replicates the logic from CrashBehaviour.cs
            if (BikeGameManager.singlePlayerRestarts == 0)
            {
                UIManager.SwitchScreen(GameScreenType.PostGameLong);
            }
            else
            {
                UIManager.SwitchScreen(GameScreenType.PreGame);
            }
        }
        
        void StopAutoRestart()
        {
            if (autoRestartCoroutine != null)
            {
                StopCoroutine(autoRestartCoroutine);
                autoRestartCoroutine = null;
            }
            autoRestartStarted = false;
        }
        
        void ResetAutoRestart()
        {
            StopAutoRestart();
            playerPressedRestart = false;
        }
        
        // Public methods for external control
        public void SetAutoRestartEnabled(bool enabled)
        {
            enableAutoRestart = enabled;
            if (!enabled)
            {
                StopAutoRestart();
            }
        }
        
        public void SetAutoRestartDelay(float delay)
        {
            autoRestartDelay = delay;
        }
        
        public bool IsAutoRestartActive()
        {
            return autoRestartStarted;
        }
    }
}