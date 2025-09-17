namespace vasundharabikeracing
{
    using UnityEngine;

    public class AutoSwitchToLevel : MonoBehaviour
    {
        [SerializeField] private float delay = 5f; // seconds before switching
        private bool hasSwitched = false;

        void OnEnable()
        {
            hasSwitched = false;
            Invoke(nameof(SwitchToLevel), delay);
        }

        void OnDisable()
        {
            CancelInvoke(nameof(SwitchToLevel));
        }

        private void SwitchToLevel()
        {
            if (hasSwitched) return;

            if (UIManager.currentScreenType == GameScreenType.Menu) // check only if still on Menu
            {
                Debug.Log("AutoSwitchToLevel: Switching to Levels after idle delay.");
                UIManager.SwitchScreen(GameScreenType.Levels);
                hasSwitched = true;
            }
        }
    }
}
