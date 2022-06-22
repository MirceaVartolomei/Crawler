using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootScript : MonoBehaviour
{
    public Vector3 stepNormal;
    public Transform rayCastPoint;
    public Transform target;
    public Vector3 restingPosition;
    public LayerMask mask;
    public Vector3 newPos;
    public Transform steppingPoint;
    public bool legGrounded;
    public GameObject player;
    public float offset;
    public float movedDistance;
    public static int currentMoveValue = 1;
    public int moveValue;
    public float speed = 10f;
    public FootScript otherLeg;
    public bool hasMoved;
    public bool moving;
    public bool movingDown;
    public bool walkingBack = false;

    public float moveDirection
    {
        get
        {
            return Controller.instance.movY;
        }
    }

    private void Start()
    {
        restingPosition = target.position;
        steppingPoint.position = new Vector3(restingPosition.x + offset, restingPosition.y, restingPosition.z);
    }

    private void Update()
    {
        if(moveDirection < 0 && walkingBack != true)
        {
            walkingBack = true;
            offset = -0.5f;
            steppingPoint.localPosition = new Vector3(steppingPoint.localPosition.x + offset, steppingPoint.localPosition.y, steppingPoint.localPosition.z);
        }
        else if(moveDirection > 0 && walkingBack != false)
        {
            walkingBack = false;
            offset = 0.5f;
            steppingPoint.localPosition = new Vector3(steppingPoint.localPosition.x + offset, steppingPoint.localPosition.y, steppingPoint.localPosition.z);
        }

        newPos = CalculatePoint(steppingPoint.position);

        if (Vector3.Distance(restingPosition, newPos) > movedDistance || moving && legGrounded)
        {
            Step(newPos);
        }
        UpdateIK();
    }

    public Vector3 CalculatePoint(Vector3 position)
    {
        Vector3 dir = position - rayCastPoint.position;
        RaycastHit hit;
        if(Physics.SphereCast(rayCastPoint.position, 1f, dir, out hit, 5f, mask))
        {
            stepNormal = hit.normal;
            position = hit.point;
            legGrounded = true;
        }
        else
        {
            stepNormal = Vector3.zero;
            position = restingPosition;
            legGrounded = false;
        }

        return position;
    }

    public void Step(Vector3 position)
    {
        if(currentMoveValue == moveValue)
        {
            legGrounded = false;
            hasMoved = false;
            moving = true;

            target.position = Vector3.MoveTowards(target.position, position + Vector3.up, speed * Time.deltaTime);
            restingPosition = Vector3.MoveTowards(target.position, position + Vector3.up, speed * Time.deltaTime);

            if(target.position == position + Vector3.up)
            {
                movingDown = true; //after lifting the feet we want to put it on ground so thats what movingDown is for
            }

            if(movingDown == true)
            {
                target.position = Vector3.MoveTowards(target.position, position, speed * Time.deltaTime);
                restingPosition = Vector3.MoveTowards(target.position, position, speed * Time.deltaTime);
            }

            if(target.position == position)
            {
                legGrounded = true;
                hasMoved = true;
                moving = false;
                movingDown = false;
                if(currentMoveValue == moveValue && otherLeg.hasMoved == true)
                {
                    currentMoveValue = currentMoveValue * -1 + 3;
                }
            }
        }
    }

    public void UpdateIK()
    {
        target.position = restingPosition;
    }
}
