using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : RaycastController
{
    AttachPoint pointAttachedTo;
    float yAttachModifier;

    public bool grabbed = false;
    public float jumpHeight = 1;
    public float distanceToGrab = 2f;
    public LayerMask playerMask;
    public LayerMask grabbablesMask;
    public LayerMask excludeGrabbed;
    public CollisionInfo collisions;
    public float gravity;
    SpriteRenderer sprite;
    public bool shaking = false;
    public IngredientType type;

    public override void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        base.Awake();
    }

    private void LateUpdate()
    {
        if (grabbed && pointAttachedTo && !shaking)
        {
            Vector2 attachPos = pointAttachedTo.transform.position;
            attachPos.y += yAttachModifier;
            transform.position = attachPos;
        }
    }

    public void BeCarried()
    {
        transform.position = pointAttachedTo.transform.position;
    }

    public void Grabbed()
    {
        if (!grabbed)
        {
            RaycastHit2D hit;
            if (hit = Physics2D.CircleCast(transform.position, distanceToGrab, Vector2.left, 0, playerMask))
            {
                AttachPoint attachTo;
                if (attachTo = (nearestAttachPoint(hit.transform.GetComponent<Player>()).Attach(this)))
                {
                    grabbed = true;
                    gameObject.layer += 1;
                    StartCoroutine(AttachToPlayer(attachTo));
                }
                else
                    StartCoroutine(FailGrab());
            }
            else
            {
                grabbed = true;
                StartCoroutine(FailGrab());
            }
        }

        else
        {
            if (pointAttachedTo)
            {
                Drop();
            }
        }
    }

    public AttachPoint nearestAttachPoint(Player player)
    {
        AttachPoint[] attachPoints = player.attachPoints;
        AttachPoint nearestAP = attachPoints[0];
        float distToNearestAP = Vector2.Distance(transform.position, nearestAP.transform.position);

        for (int i = 1; i < attachPoints.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, attachPoints[i].transform.position);
            if (dist < distToNearestAP)
            {
                nearestAP = attachPoints[i];
                distToNearestAP = dist;
            }
        }
        return nearestAP;
    }

    public void Drop()
    {
        bool canDrop = true;
        Collider2D[] hit = new Collider2D[2];
        if (Physics2D.OverlapBoxNonAlloc(collider.bounds.center, collider.bounds.size - new Vector3(0.1f, 0.1f, 0.1f), 0, hit, excludeGrabbed) > 0)
        {

            foreach (Collider2D h in hit)
            {
                if (!canDrop)
                    break;
                if (h)
                    if (h.gameObject != gameObject)
                        canDrop = false;
            }

        }

        if (canDrop)
        {
            grabbed = false;
            pointAttachedTo.Detach(this);
            pointAttachedTo = null;
            sprite.sortingOrder++;
            gameObject.layer -= 1;
        }
        else
        {
            StartCoroutine(FailDrop());
        }

    }

    public void ForceDrop()
    {
        grabbed = false;       
        pointAttachedTo = null;
        sprite.sortingOrder++;
        gameObject.layer -= 1;
        FailDrop();
    }

    public IEnumerator AttachToPlayer(AttachPoint attachPoint)
    {
        float lerpTime = 0.15f;
        float timer = 0;
        float perc;
        Vector2 endPos = attachPoint.FreeAttachPoint(this);
        yAttachModifier = endPos.y - attachPoint.transform.position.y;
        Vector2 startPos = transform.position;
        sprite.sortingOrder--;


        while (timer <= lerpTime)
        {
            timer += Time.deltaTime;
            perc = timer / lerpTime;
            transform.position = Vector2.Lerp(startPos, endPos, perc);
            yield return null;
        }
        pointAttachedTo = attachPoint;
        yield return null;
    }

    public IEnumerator FailGrab()
    {
        float lerpTime = 0.15f;
        float timer = 0;
        float perc;
        Vector2 endPos;
        Vector2 startPos = endPos = transform.position;
        endPos.y += jumpHeight;

        while (timer <= ((lerpTime / 3) * 2))
        {
            timer += Time.deltaTime;
            perc = timer / ((lerpTime / 3) * 2);
            transform.position = Vector2.Lerp(startPos, endPos, perc);
            yield return null;
        }
        endPos = transform.position;
        lerpTime -= timer;
        timer = 0;
        while (timer <= lerpTime)
        {
            timer += Time.deltaTime;
            perc = timer / lerpTime;
            transform.position = Vector2.Lerp(endPos, startPos, perc);
            yield return null;

        }
        grabbed = false;
        yield return null;
    }

    public IEnumerator FailDrop()
    {
        float lerpTime = 0.15f;
        float timer = 0;
        float perc;
        Vector2 endPos;
        Vector2 startPos = endPos = transform.position;
        endPos.x += jumpHeight;
        shaking = true;

        while (timer <= ((lerpTime / 3) * 2))
        {
            timer += Time.deltaTime;
            perc = timer / ((lerpTime / 3) * 2);
            transform.position = Vector2.Lerp(startPos, endPos, perc);
            yield return null;
        }
        endPos = transform.position;
        lerpTime -= timer;
        timer = 0;
        while (timer <= lerpTime)
        {
            timer += Time.deltaTime;
            perc = timer / lerpTime;
            transform.position = Vector2.Lerp(endPos, startPos, perc);
            yield return null;

        }
        shaking = false;
        yield return null;
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        //if (velocity.y < 0)
        //{
        //    DescendSlope(ref velocity);
        //}

        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);

    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        //check if moving up or down
        float directionY = Mathf.Sign(velocity.y);
        float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

        //using rays to check collisions
        for (int i = 0; i < verticalRayCount; i++)
        {
            //finding out where to start rays
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //checking vertical collision at next position
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D[] hit = new RaycastHit2D[2];

            //drawing rays for test
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght, Color.red);

            if (Physics2D.RaycastNonAlloc(rayOrigin, Vector2.up * directionY, hit, rayLenght, collisionMask) > 0)
            {
                //moves to the point where the ray hit something
                foreach (var h in hit)
                {
                    if (h)
                        if (h.transform.gameObject != gameObject)
                        {
                            rayLenght = h.distance;

                            if (collisions.climbingSlope)
                            {
                                //if it collides with something above it while climbing a slope
                                velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                            }

                            collisions.below = directionY == -1;
                            collisions.above = directionY == 1;

                            velocity.y = (h.distance - skinWidth) * directionY;
                        }

                }
            }
        }

        if (collisions.climbingSlope)
        {
            //fixes stuter when climbing some curved slopes
            float directionX = Mathf.Sign(velocity.x);
            rayLenght = Mathf.Abs(velocity.x + skinWidth);
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }

    }

    //stores the directions character is colliding in
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
