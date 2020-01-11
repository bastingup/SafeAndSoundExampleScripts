using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    // Character movement
    public CharacterController cc;
    public NavMeshAgent navAgent;
    public bool isGrounded;
    public float movementSpeed,
                 runSpeed,
                 walkSpeed,
                 maximumDistanceToGround,
                 gravity,
                 moveHorizontal,
                 moveVertical,
                 jumpForce;
    public Vector3 upwardsJump;

    protected virtual void Start()
    {
        GetRequiredComponents();
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, cc.bounds.extents.y + maximumDistanceToGround);
    }

    void GetRequiredComponents()
    {
        if (GetComponent<CharacterController>() != null)
            cc = GetComponent<CharacterController>();
        navAgent = GetComponent<NavMeshAgent>();
    }
}
