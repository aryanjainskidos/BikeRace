namespace vasundharabikeracing
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ButtonShopBehaviour : MonoBehaviour
    {
        [SerializeField] private int _answerAmount;
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleBuyClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleBuyClicked);
        }

        private void HandleBuyClicked()
        {
            Vasundhara_BikeRacingShop.instance.BuyOnClick(_answerAmount);
        }

    }
}
