namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;

public class PreGameLoadingBehaviour : MonoBehaviour
{

    Text progressText;
    private float lastLoadedPerc = 0;

    void Awake()
    {
        progressText = transform.Find("ProgressText").GetComponent<Text>();
    }

    void OnEnable()
    {
        progressText.text = Lang.Get("Loading |param|%").Replace("|param|", "0");
    }

    void Update()
    {

        if (LevelManager.namedObjectCount > 0)
        {

            float preObjectsLoaded = ((float)LevelManager.loadingProgressPreObjects / LevelManager.loadingTotalPreObjects);// * 100
            float objectsLoaded = ((float)LevelManager.objectsLoadedTotal / LevelManager.namedObjectCount);// * 100
            var perc = (preObjectsLoaded * 10 + objectsLoaded * 90);
            if (perc > lastLoadedPerc && perc <= 100)
            {
                lastLoadedPerc = perc;
                progressText.text = progressText.text = Lang.Get("Loading |param|%")
                    .Replace("|param|", perc.ToString("F0"));
            }
        }
        else
        {
            progressText.text = Lang.Get("Loading |param|%").Replace("|param|", "0");
        }
    }
}
}
