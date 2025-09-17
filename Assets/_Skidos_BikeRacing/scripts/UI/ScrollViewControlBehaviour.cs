namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewControlBehaviour : MonoBehaviour
{

    ScrollRect scrollRect;
    RectTransform scrollRectTransform;
    RectTransform contentRectTransform;
    RectTransform maskRectTransform;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRectTransform = scrollRect.transform as RectTransform;
        contentRectTransform = scrollRect.content;
        maskRectTransform = GetComponent<Mask>().rectTransform;
    }

    #region scroll rect vertical item snapping methods
    public float scrollRectVerticalNormalizedPosition;

    public void CenterOnItem(RectTransform target)
    {
        //        print("CenterOnItem " + target.name + " " + activePresetIndex);
        // Item is here
        Vector3 itemCenterPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(target));
        // But must be here
        Vector3 targetPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(maskRectTransform));
        // So it has to move this distance
        float differenceY = targetPositionInScroll.y - itemCenterPositionInScroll.y;

        float normalizedDifferenceY = differenceY / (contentRectTransform.rect.size.y - scrollRectTransform.rect.size.y);
        float newNormalizedYPosition = scrollRect.verticalNormalizedPosition - normalizedDifferenceY;

        if (scrollRect.movementType != ScrollRect.MovementType.Unrestricted)
        {
            newNormalizedYPosition = Mathf.Clamp01(newNormalizedYPosition);
        }

        scrollRect.verticalNormalizedPosition = newNormalizedYPosition;

        scrollRectVerticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
    }

    Vector3 GetWidgetWorldPoint(RectTransform target)
    {
        //pivot position + item size has to be included
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);
        var localPosition = target.localPosition + pivotOffset;
        return target.parent.TransformPoint(localPosition);
    }

    Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
    {
        return target.InverseTransformPoint(worldPoint);
    }
    #endregion
}

}
