namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewMoveToTopBehaviour : MonoBehaviour
{

    ScrollRect scrollView;
    Scrollbar scrollbar;

    void Awake()
    {
        scrollView = transform.GetComponent<ScrollRect>();
    }

    public bool forceRecalculate = true;
    public bool auto = false;

    void Update()
    {
        if (forceRecalculate)
        {
            //print("Update");
            //            Actualize();

            //put the elements under the top, rather than in the center
            var aPos = scrollView.content.anchoredPosition;
            aPos.y = -scrollView.content.rect.height / 2;
            scrollView.content.anchoredPosition = aPos;

            if (scrollView.verticalScrollbar == null && scrollView.content.rect.height < scrollView.GetComponent<RectTransform>().rect.height)
            {
                //                print("little content container");
                scrollView.enabled = false;
            }
            else
            {
                scrollView.enabled = true;
            }

            //scrollbar messes with positioning of elements in small scroll views, so break their binding if content height is less than view height, add it back if not
            if (scrollView.verticalScrollbar != null && scrollView.content.rect.height < scrollView.GetComponent<RectTransform>().rect.height)
            {
                scrollbar = scrollView.verticalScrollbar;
                scrollView.verticalScrollbar = null;
                scrollbar.size = scrollbar.value = 1;
            }
            else if (scrollView.verticalScrollbar == null && scrollbar != null)
            {
                scrollView.verticalScrollbar = scrollbar;
            }

            forceRecalculate = false;
        }
    }

    void OnEnable()
    {
        if (auto)
        {
            forceRecalculate = true;
        }
    }
}

}
