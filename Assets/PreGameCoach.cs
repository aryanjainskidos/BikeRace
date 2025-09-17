using UnityEngine;
using UnityEngine.UI;

public class DestroyChildImageOnce : MonoBehaviour
{
    [SerializeField] private Image childImage; 
    private string prefKey = "HasSeenImage";

    private void Start()
    {
        
        if (PlayerPrefs.GetInt(prefKey, 0) == 1 && childImage != null)
        {

            Destroy(childImage.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (childImage != null)
        {

            Destroy(childImage.gameObject);


            PlayerPrefs.SetInt(prefKey, 1);
            PlayerPrefs.Save();
        }
    }
}