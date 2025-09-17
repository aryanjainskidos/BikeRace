// Create: Assets/_Skidos_BikeRacing/scripts/UI/CrashScreenManager.cs
using UnityEngine;
using System.Collections;

namespace vasundharabikeracing
{
    public class CrashScreenManager : MonoBehaviour
    {
        [Header("Auto Restart Settings")]
        [SerializeField] private float autoRestartDelay = 3f;
        [SerializeField] private bool enableAutoRestart = true;
        
        private Coroutine autoRestartCoroutine;
        private bool autoRestartStarted = false;
        
        void OnEnable()
        {
            if (Startup.Initialized && enableAutoRestart)
            {
                StartAutoRestart();
            }
        }
        
        void OnDisable()
        {
            StopAutoRestart();
        }
        
        public void StartAutoRestart()
        {
            if (!autoRestartStarted)
            {
                autoRestartStarted = true;
                autoRestartCoroutine = StartCoroutine(AutoRestartCountdown());
            }
        }
        
        public void StopAutoRestart()
        {
            if (autoRestartCoroutine != null)
            {
                StopCoroutine(autoRestartCoroutine);
                autoRestartCoroutine = null;
            }
            autoRestartStarted = false;
        }
        
        IEnumerator AutoRestartCountdown()
        {
            yield return new WaitForSeconds(autoRestartDelay);
            
            // Check if still on crash screen and restart is available
            if (UIManager.currentScreenType == GameScreenType.Crash && 
                BikeGameManager.singlePlayerRestarts > 0)
            {
                Debug.Log("Auto-restarting after crash...");
                BikeGameManager.ExecuteCommand(GameCommand.Reset);
            }
        }
        
        // Call this when user manually taps restart button
        public void OnManualRestart()
        {
            StopAutoRestart();
            // The existing restart button logic will handle the restart
        }
        
        // Call this when user taps anywhere on screen to cancel auto-restart
        public void OnUserInput()
        {
            StopAutoRestart();
        }
    }
}