using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    public TMP_Text hintText;
    public TMP_Text titleText;
    public float verticalDistanceFromMask = 140f;
    public float horizontalDistanceFromMask = 260f;
    public float mainMaskRadius = 50f;
    public Animator hintAnimator;
    private TutorialNew _tutorial;
    private bool _isOpen;
    public float hintAnimationTime = 0.5f;

    
    public void Show(string text, Direction direction, TutorialNew tutorial, Vector3 maskPosition, float maskScale, int tipIndex)
    {
        if (!hintAnimator)
        {
            hintAnimator = GetComponent<Animator>();
        }
        titleText.text = "#Tip #" + (tipIndex + 1).ToString();
        _tutorial = tutorial;
        hintText.text = text;
        _isOpen = true;

        float horizontalOffset = 0;
        float verticalOffset = 0;

        if (direction == Direction.Top || direction == Direction.UpperLeft || direction == Direction.UpperRight)
        {
            verticalOffset = mainMaskRadius * maskScale + verticalDistanceFromMask;
        }else if (direction == Direction.Bottom || direction == Direction.BottomLeft ||
                  direction == Direction.BottomRight)
        {
            verticalOffset = -(mainMaskRadius * maskScale + verticalDistanceFromMask);
        }
        if (direction == Direction.Left || direction == Direction.BottomLeft || direction == Direction.UpperLeft)
        {
            horizontalOffset = -(mainMaskRadius * maskScale + horizontalDistanceFromMask);
        }else if (direction == Direction.Right || direction == Direction.BottomRight ||
                  direction == Direction.UpperRight)
        {
            horizontalOffset = mainMaskRadius * maskScale + horizontalDistanceFromMask;   
        }

        Vector2 offsetPosition = new Vector2(horizontalOffset, verticalOffset);
        Vector3 uiMaskPosition = Camera.main.WorldToScreenPoint(maskPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent, 
            uiMaskPosition,
            Camera.main,
            out Vector2 localPoint
        );

        Vector3 newHintPosition = new Vector3(localPoint.x + horizontalOffset, localPoint.y + verticalOffset, 0f);     
        ((RectTransform)transform).anchoredPosition = newHintPosition;
        hintAnimator.enabled = true;
        hintAnimator.SetTrigger("Open");
    }

    IEnumerator CloseHint()
    {
        yield return new WaitForSecondsRealtime(hintAnimationTime);
        _tutorial.NextStep();
    }
    public void Hide()
    {
        _isOpen = false;
        hintAnimator.SetTrigger("Close");
        StartCoroutine(CloseHint());
    }
    
}
