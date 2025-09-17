namespace vasundharabikeracing {
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class InfiniteScrollBehaviour : MonoBehaviour, IScrollHandler, IEndDragHandler, IBeginDragHandler
{

    ScrollRect scrollRect;
    RectTransform content;
    public int childCount = 0;
    public int visibleChildCount = 0;

    //    RectTransform maskRectTransform;

    //    Vector3 targetPositionInScroll;

    RectTransform scrollRectTransform;
    public Vector2 size;
    public float width;
    public float limX;
    public float spacing;
    public float outsideBorderX;

    //TODO make private, public to be able to see them in editor
    public bool firstIsNotVisible = false;
    public bool lastIsNotVisible = false;

    public bool updateLeft = false;
    public bool updateRight = false;

    public bool dragging;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;

        //        maskRectTransform = scrollRect.GetComponent<Mask>().rectTransform;
        scrollRectTransform = scrollRect.transform as RectTransform;
        //        targetPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(maskRectTransform));
        size = scrollRectTransform.rect.size;

        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

        spacing = scrollRect.content.GetComponent<HorizontalLayoutGroup>().spacing;
    }

    public float scrollRectHorizontalNormalizedPosition;
    public Vector2 scrollRectVelocity;

    public Transform firstChild;
    public Transform lastChild;

    public bool justEndedDragging = false;
    public bool justEndedScrolling = false;

    int frameDelay = 0;

    public void Init()
    {
        //        print("init");

        if (scrollRect == null)
        {
            Awake();
        }

        childCount = scrollRect.content.childCount;
        visibleChildCount = 0;
        foreach (Transform item in content)
        {
            if (item.gameObject.activeSelf)
            {
                visibleChildCount++;
            }
        }

        spacing = scrollRect.content.GetComponent<HorizontalLayoutGroup>().spacing;
        width = (scrollRect.content.rect.size.x + spacing) / visibleChildCount;
        limX = (scrollRect.content.rect.size.x - scrollRectTransform.rect.size.x) * 0.5f;

        outsideBorderX = (size.x + width) * 0.5f + width * 2;

        if (visibleChildCount > 4)
        {
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }
        else
        {
            //            scrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

    }

    void Update()
    {

        //TODO click on mouse scrollend wheel

        //stopped scrolling (wheel, or released)
        if (scrollRectVelocity != scrollRect.velocity || justEndedDragging)
        {
            scrollRectVelocity = scrollRect.velocity;
            justEndedDragging = false;

            if (Mathf.Abs(scrollRectVelocity.x) < 50)
            {//threshold
                scrollRectVelocity.x = 0;
                scrollRect.velocity = scrollRectVelocity;
            }

            if (!dragging && scrollRectVelocity.x == 0)
            {
                //print("SELECT THE THING IN THE CENTER");

                //find the item closest to the center
                GameObject closestToCenter = null;
                //                
                float smallestDistanceToCenter = 3000;

                foreach (Transform item in content)
                {
                    if (item.gameObject.activeSelf)
                    {
                        distanceToCenterX = Mathf.Abs(CenterOnItem(item.GetComponent<RectTransform>()));//calculate the distance of the first item's center to the center of the scroll rect 
                        if (smallestDistanceToCenter > distanceToCenterX)
                        {
                            smallestDistanceToCenter = distanceToCenterX;
                            closestToCenter = item.gameObject;
                        }
                    }
                }

                //simulate a click on that element
                if (closestToCenter != null)
                {
                    ExecuteEvents.Execute(
                        closestToCenter,
                        new PointerEventData(EventSystem.current),
                        ExecuteEvents.pointerClickHandler);

                    //DONE distribute the items equally on the sides(in distribute function)
                }
            }
        }

        if (!dragging && visibleChildCount > 4)
        {

            if (scrollRectVelocity.x < 0)
            {
                //                Debug.Log("!dragging && scrollRectVelocity.x < 0");
                updateLeft = true;
            }

            if (scrollRectVelocity.x > 0)
            {
                //                Debug.Log("!dragging && scrollRectVelocity.x > 0");
                updateRight = true;
            }

            if (updateLeft && frameDelay <= 0)
            {
                //                Debug.Log("!dragging && updateLeft " + transform.parent.name);
                ScrollLeft();
                updateLeft = false;
            }

            if (updateRight && frameDelay <= 0)
            {
                //                Debug.Log("!dragging && updateRight " + transform.parent.name);
                ScrollRight();
                updateRight = false;
            }

            if ((updateLeft || updateRight) && frameDelay > 0)
            {
                frameDelay--;
                Init();
            }
        }

        scrollRectHorizontalNormalizedPosition = scrollRect.horizontalNormalizedPosition;
    }

    void ScrollLeft()
    {
        //if scrolling left and the scroll rect normalized position changed since last update
        //        if (scrollRectHorizontalNormalizedPosition != scrollRect.horizontalNormalizedPosition)//scrollRectVelocity.x != 0 || 
        {

            do
            {
                //get the first active item in the content container
                int i = 0;
                firstChild = content.GetChild(0);

                while (!firstChild.gameObject.activeSelf && i++ < childCount)
                {//push inactive elements back, not to break the sequence
                    firstChild.SetAsLastSibling();
                    firstChild = content.GetChild(0);//get the new first
                }

                distanceToCenterX = CenterOnItem(firstChild.GetComponent<RectTransform>());//calculate the distance of the first item's center to the center of the scroll rect 

                if (distanceToCenterX < -outsideBorderX) //if the distance is larger than scroll rect width /2 + item width / 2, move the first active element to the end
                {

                    firstChild.SetAsLastSibling();

                    Vector2 ap = content.anchoredPosition; // adjust content container position to account for the missing first element
                    ap.x += width;
                    content.anchoredPosition = ap;

                    firstIsNotVisible = true; //check the next element, in case more than one element is outside of the border

                }
                else
                {

                    firstIsNotVisible = false; //element is visible, abort

                }

            } while (firstIsNotVisible);

        }
    }

    void ScrollRight()
    {
        //if scrolling left and the scroll rect normalized position changed since last update
        //        if (scrollRectHorizontalNormalizedPosition != scrollRect.horizontalNormalizedPosition)//scrollRectVelocity.x != 0 || 
        {

            do
            {
                //get the first active item in the content container
                int i = 0;
                lastChild = content.GetChild(childCount - 1);

                while (!lastChild.gameObject.activeSelf && i++ < childCount)
                {//push inactive elements back, not to break the sequence
                    lastChild.SetAsFirstSibling();
                    lastChild = content.GetChild(childCount - 1);//get the new first
                }

                distanceToCenterX = CenterOnItem(lastChild.GetComponent<RectTransform>());//calculate the distance of the first item's center to the center of the scroll rect 

                if (distanceToCenterX > outsideBorderX) //if the distance is larger than scroll rect width /2 + item width / 2, move the first active element to the end
                {
                    lastChild.SetAsFirstSibling();

                    Vector2 ap = content.anchoredPosition; // adjust content container position to account for the missing first element
                    ap.x -= width;// + spacing * 0.5f;
                    content.anchoredPosition = ap;

                    lastIsNotVisible = true; //check the next element, in case more than one element is outside of the border

                }
                else
                {

                    lastIsNotVisible = false; //element is visible, abort

                }

            } while (lastIsNotVisible);

        }
    }


    public void OnScroll(PointerEventData pointerEventData)
    {
        if (pointerEventData.scrollDelta.y > 0)
        {
            updateLeft = true;
        }

        if (pointerEventData.scrollDelta.y < 0)
        {
            updateRight = true;
        }
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        dragging = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {

        if (visibleChildCount > 4)
        {
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }
        justEndedDragging = true;

        dragging = false;
    }

    public void Distribute()
    {
        //        Debug.Log("!!!!! Distribute");

        if (content.anchoredPosition.x > 0)
        {
            updateRight = true;
        }
        if (content.anchoredPosition.x < 0)
        {
            updateLeft = true;
        }

        if (frameDelay <= 0)
        {
            frameDelay = 2;
        }
    }

    public Vector3 itemCenterPositionInScroll;

    public float distanceToCenterX;

    #region scroll rect horizontal item snapping methods
    public float CenterOnItem(RectTransform target)
    {
        itemCenterPositionInScroll = GetWorldPointInWidget(scrollRectTransform, GetWidgetWorldPoint(target));
        return itemCenterPositionInScroll.x;//targetPositionInScroll.x - itemCenterPositionInScroll.x;
    }

    Vector3 GetWidgetWorldPoint(RectTransform target)
    {
        //pivot position + item size has to be included
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);
        var localPosition = target.localPosition + pivotOffset;//center
        return target.parent.TransformPoint(localPosition);
    }

    Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
    {
        return target.InverseTransformPoint(worldPoint);
    }
    #endregion

}

}
