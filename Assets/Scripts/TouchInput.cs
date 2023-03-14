using UnityEngine;
using TMPro;

public class TouchInput : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI swipeText, joystickText, fingerIdText;

    private Vector2 joystickTouchFirstPos, joystickTouchLastPos;
    private const int NULL_TOUCH_FINGER_ID = -1;
    private int joystickFingerId = NULL_TOUCH_FINGER_ID;

    private float SWIPE_THRESHOLD = 10;
    private Vector2 swipeFingerLastPos, swipeFingerPrevPos;
    private float verticalMoveValue, horizontalMoveValue;
    private bool isSwipeFingerPressing;
    private bool upSwipe, downSwipe, leftSwipe, rightSwipe;

    private string touchResulText;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                touchResulText += $"Current finger {touch.fingerId} Pos {touch.position} Tap count {touch.tapCount}\n";

                if (touch.fingerId != joystickFingerId)
                {
                    // Double tap check
                    if (touch.tapCount > 0 && touch.tapCount % 2 == 0)
                    {
                        Debug.Log("Double tap " + touch.tapCount);
                        // NOTE: Reset tap count
                        return;
                    }

                    if (touch.phase == TouchPhase.Began)
                    {
                        if (joystickFingerId == NULL_TOUCH_FINGER_ID)
                        {
                            joystickFingerId = touch.fingerId;
                            joystickTouchFirstPos = touch.position;

                            return;
                        }
                        else
                        {
                            swipeFingerPrevPos = touch.position;
                            swipeFingerLastPos = touch.position;
                        }
                    }

                    // Swipe touch control
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (!isSwipeFingerPressing)
                        {
                            swipeFingerLastPos = touch.position;
                            DetectSwipe();
                        }
                    }

                    if (touch.phase == TouchPhase.Ended && isSwipeFingerPressing)
                    {
                        isSwipeFingerPressing = false;
                    }
                }
            }

            fingerIdText.SetText(touchResulText);
            touchResulText = "";

            if (joystickFingerId != NULL_TOUCH_FINGER_ID)
            {
                // Joystick touch control
                if (Input.GetTouch(joystickFingerId).phase == TouchPhase.Moved)
                {
                    joystickText.SetText(Input.GetTouch(joystickFingerId).position.ToString());
                    MoveJoystick(Input.GetTouch(joystickFingerId).position);
                }

                if (Input.GetTouch(joystickFingerId).phase == TouchPhase.Ended)
                {
                    joystickText.SetText("No movement");
                    joystickFingerId = NULL_TOUCH_FINGER_ID;
                }
            }
        }
    }

    void MoveJoystick(Vector2 joystickTouchNewPos)
    {
        if (joystickTouchLastPos != joystickTouchNewPos)
        {
            // NOTE: Movement direction, speed
            joystickTouchLastPos = joystickTouchNewPos;
        }
    }

    #region Swipe functions
    void DetectSwipe()
    {
        CalculateVerticalMoveValue(ref verticalMoveValue);
        CalculateHorizontalMoveValue(ref horizontalMoveValue);

        if (verticalMoveValue > SWIPE_THRESHOLD && verticalMoveValue > horizontalMoveValue)
        {
            if (swipeFingerLastPos.y - swipeFingerPrevPos.y > 0)
            {
                OnSwipeUp();
            }
            else if (swipeFingerLastPos.y - swipeFingerPrevPos.y < 0)
            {
                OnSwipeDown();
            }
            else
            {
                upSwipe = false;
                downSwipe = false;
            }

            isSwipeFingerPressing = true;
            swipeFingerPrevPos = swipeFingerLastPos;
        }
        else if (horizontalMoveValue > SWIPE_THRESHOLD && horizontalMoveValue > verticalMoveValue)
        {
            if (swipeFingerLastPos.x - swipeFingerPrevPos.x > 0)
            {
                OnSwipeRight();
            }
            else if (swipeFingerLastPos.x - swipeFingerPrevPos.x < 0)
            {
                OnSwipeLeft();
            }
            else
            {
                rightSwipe = false;
                leftSwipe = false;
            }

            isSwipeFingerPressing = true;
            swipeFingerPrevPos = swipeFingerLastPos;
        }
    }

    void CalculateVerticalMoveValue(ref float value)
    {
        value = Mathf.Abs(swipeFingerLastPos.y - swipeFingerPrevPos.y);
    }

    void CalculateHorizontalMoveValue(ref float value)
    {
        value = Mathf.Abs(swipeFingerLastPos.x - swipeFingerPrevPos.x);
    }

    void OnSwipeUp()
    {
        swipeText.SetText("up");
        upSwipe = true;
        downSwipe = false;
    }

    void OnSwipeDown()
    {
        swipeText.SetText("down");
        upSwipe = false;
        downSwipe = true;
    }

    void OnSwipeLeft()
    {
        swipeText.SetText("left");
        leftSwipe = true;
        rightSwipe = false;
    }

    void OnSwipeRight()
    {
        swipeText.SetText("right");
        leftSwipe = false;
        rightSwipe = true;
    }
    #endregion
}