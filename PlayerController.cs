using UnityEngine;
using System.Collections;

public enum PlayerCharacterState
{
    // TODO find all meaningful states
    free, combat
}

public class PlayerController : Character
{
    // Private Members
    [SerializeField]
    private float _rotationSpeed, _characterHeight;
    private PlayerAnimationController _pac;

    // Public Members
    public int skillPoints, maximumSkillPoints;

    // Combat
    public Firearm firearm;
    public LayerMask layerMask;
    private PlayerInventory _inventory;

    // Player AI
    public bool followPlayer;

    protected override void Start()
    {
        base.Start();

        _inventory = this.GetComponent<PlayerInventory>();
        _pac = this.GetComponent<PlayerAnimationController>();
        upwardsJump = Vector3.zero;
    }

    protected virtual void Update()
    {
        if (base.IsGrounded() && isActiveCharacter)
        {
            firing = false;

            if (Input.GetButton("Aim") && !isRunning)
            {
                aiming = true;
                CameraController.Instance.SetDistanceToPlayer(true, crouching);
            }
            else
            {
                aiming = false;
                CameraController.Instance.SetDistanceToPlayer(false, crouching);
            }

            if (Input.GetButtonDown("Fire") && !firing && aiming)
            {
                Fire();
                Punish();
            }

            if (Input.GetButtonDown("Reload") && _inventory.clips > 0 && !firearm.reloading && firearm.ammo < firearm.maxAmmo)
                Reload();

            if (Input.GetButtonDown("Shoulder"))
                SwitchShoulder();

            if (Input.GetButton("Crouch"))
            {
                crouching = true;
                walkSpeed = 1.0f;
                navAgent.height = _characterHeight * 0.6f;
                cc.height = _characterHeight * 0.6f;
                CharacterManagement.Instance.SetCrouchForFollowingCharacters(true);
            }
            else
            {
                crouching = false;
                walkSpeed = 1.6f;
                navAgent.height = _characterHeight;
                cc.height = _characterHeight;
                CharacterManagement.Instance.SetCrouchForFollowingCharacters(false);
            }

            RecenterCC();

            if (Input.GetButtonDown("Follow"))
                CharacterManagement.Instance.SetFollowActivePlayer();

            // TODO Currently jumping is discarded
            //if (Input.GetButtonDown("Jump") && !_pac.startsToJump)
            // Jump();

        }
    }

    void FixedUpdate()
    {
        if (base.IsGrounded() && canMove)
        {
            if (isActiveCharacter)
            {
                movementSpeed = MovementSpeedActiveChar();
                MovePlayer();
                RotateCharacter();
            }
            else
            {
                navAgent.speed = MovementSpeedInactiveChar();
                InactivePlayerFollowing();
            }
        }
        if (upwardsJump.y > 0 || !base.IsGrounded())
            ApplyGravityForce();
    }

    void SwitchShoulder()
    {
        // TODO make smooth shoulder switch
        Vector3 _formerPosition = CameraController.Instance.mainCamera.transform.localPosition;
        Vector3 _newPosition = new Vector3(-_formerPosition.x, _formerPosition.y, _formerPosition.z);
        CameraController.Instance.mainCamera.transform.localPosition = _newPosition;
    }

    void RecenterCC()
    {
        cc.center = new Vector3(0, cc.height / 2, 0);
    }

    void Jump()
    {
        _pac.startsToJump = true;
        StartCoroutine(JumpDelay(0.78f));
    }

    IEnumerator JumpDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        upwardsJump.y = jumpForce;
        _pac.startsToJump = false;
    }

    void Fire()
    {
        firing = true;
        Vector3 _destination = GetHitPoint(firearm.projectileSpawn.position);
        firearm.Fire(_destination, CheckIfHidden());
    }

    void Reload()
    {
        firearm.reloading = true;
        _inventory.clips--;
        StartCoroutine(firearm.Reload());
    }

    Vector3 GetHitPoint(Vector3 _gunPosition)
    {
        Vector3 _aimAt;
        RaycastHit _hit;
        Ray _centerOfScreenRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(_centerOfScreenRay, out _hit, 100f, ~layerMask))
            _aimAt = _hit.point;
        else
            _aimAt = _centerOfScreenRay.origin + _centerOfScreenRay.direction * 100f;
        Ray _fire = new Ray(_gunPosition, (_aimAt - _gunPosition).normalized);
        if (!Physics.Raycast(_fire, out _hit, 100f, ~layerMask))
            return _aimAt;
        return _hit.point;
    }

    void InactivePlayerFollowing()
    {
        GameObject active = CharacterManagement.Instance.activeCharacter;
        float distance = Vector3.Distance(this.transform.position, active.transform.position);
        if (distance >= 3.0f && followPlayer)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(active.transform.position);
        }
        else
        {
            navAgent.isStopped = true; ;
        }
    }

    void RotateCharacter()
    {
        if (cc.velocity.magnitude >= 0.1f || aiming)
        {
            Transform _cameraTarget = CameraController.Instance.mainCameraTarget.transform;
            var _lookPos = _cameraTarget.position - transform.position;
            _lookPos.y = 0;
            var _rotation = Quaternion.LookRotation(_lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * _rotationSpeed);
        }
    }

    void MovePlayer()
    {
        if (!isRunning)
            moveHorizontal = Input.GetAxis("Horizontal");
        else
            moveHorizontal = 0;
        moveVertical = Input.GetAxis("Vertical");
        Vector3 _movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        _movement = this.transform.TransformDirection(_movement);
        cc.SimpleMove(_movement * movementSpeed);
    }

    void ApplyGravityForce()
    {
        Vector3 _movement = new Vector3(0.0f, upwardsJump.y, 0.0f);
        upwardsJump.y -= gravity * Time.deltaTime;
        Debug.Log(_movement);
        cc.Move(_movement * Time.deltaTime);
    }

    float MovementSpeedInactiveChar()
    {
        if (Vector3.Distance(this.transform.position, CharacterManagement.Instance.activeCharacter.transform.position) >= 6.0f)
        {
            return 4.0f;
        }
        return 2.0f;
    }

    float MovementSpeedActiveChar()
    {
        if (Input.GetAxis("Run") == 1 && canRun)
        {
            isRunning = true;
            return runSpeed;
        }
        isRunning = false;
        return walkSpeed;
    }
}
