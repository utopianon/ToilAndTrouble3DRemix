using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public CharacterController target;
    public float verticalOffset;
    public float lookAheadDistX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    Vector2 targetPosInScreenSpace;

    FocusArea focusArea;
    Camera thisCamera;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirectionX;
    float smoothLookVelocityX;
    float smoothVelocityY;
    float actualVerticalOffset;
    bool following;

    private void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
        following = true;
        thisCamera = GetComponent<Camera>();
    }

    private void Update()
    {

        if (target.collisions.descendingSlope)
        {
            actualVerticalOffset = -4;
        }
        else if (target.collisions.climbingSlope)
        {
            actualVerticalOffset = 6;
        }
        else
        {
            actualVerticalOffset = verticalOffset;
        }


    }

    private void LateUpdate()
    {
        if (following)
        {
            focusArea.UpdateFocusArea(target.collider.bounds);

            Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        //{
        //    focusArea.UpdateFocusArea(target.collider.bounds);
        //    Vector2 focusPosition = focusArea.centre + Vector2.up * actualVerticalOffset;

        //    if (focusArea.velocity.x != 0)
        //    {
        //        lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
        //    }

        //    targetLookAheadX = lookAheadDirectionX * lookAheadDistX;
        //    currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        //    focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        //    focusPosition += Vector2.right * currentLookAheadX;
        //    transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        //    targetPosInScreenSpace = thisCamera.WorldToScreenPoint(target.transform.position);
        //    Debug.Log("fOCUS aRea centre : " + focusArea.centre);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    public void DeathCamera()
    {
        StartCoroutine(StopFollowingWithDelay(.5f));
    }

    IEnumerator StopFollowingWithDelay(float delay)
    {
        float timer = 0;
        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        following = false;
    }

    Vector2 ScreenSpaceToFraction(Vector2 screenSpaceCoordinates)
    {
        return new Vector2(screenSpaceCoordinates.x / Screen.currentResolution.width, screenSpaceCoordinates.y / Screen.currentResolution.height);
    }



    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void UpdateFocusArea(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);

        }

    }
}
