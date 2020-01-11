using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Private Members
    [SerializeField]
    private float _rotationSpeed, _cameraCloseDistance, _cameraFarDistance, _cameraNormalHeight, _cameraCrouchHeight;
    private Quaternion _originalMainCameraRotation;

    // Camera management
    public GameObject mainCameraAnchor, uiCameraAnchor, mainCameraTarget;
    public Camera mainCamera, uiCamera;

    #region Singleton
    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
    #endregion

    private void Start()
    {
        _originalMainCameraRotation = mainCamera.transform.rotation;
    }

    void Update()
    {
        SetCameraPosition();
        RotateMainCamera();
        RoateUICamera();
        SetUICameraRenderLayer();
    }

    private void SetUICameraRenderLayer()
    {
        int layerOfChar = CharacterManagement.Instance.activeCharacter.layer;
        uiCameraAnchor.transform.GetChild(0).GetComponent<Camera>().cullingMask = 1 << layerOfChar;
    }

    void SetCameraPosition()
    {
        float _distance = Vector3.Distance(mainCameraAnchor.transform.position, CharacterManagement.Instance.activeCharacter.transform.position);
        if (_distance >= 0.005f)
            mainCameraAnchor.transform.position = Vector3.Lerp(mainCameraAnchor.transform.position, CharacterManagement.Instance.activeCharacter.transform.position, 10.0f * Time.deltaTime);
        else
            mainCameraAnchor.transform.position = CharacterManagement.Instance.activeCharacter.transform.position;

        uiCameraAnchor.transform.position = CharacterManagement.Instance.activeCharacter.transform.position;
    }

    void RotateMainCamera()
    {
        float _x = Input.GetAxis("Mouse X");
        float _y = -Input.GetAxis("Mouse Y");
        Vector3 _rotation = new Vector3(_y, _x, 0);
        mainCameraAnchor.transform.Rotate(_rotation * _rotationSpeed);

        // Lock on Z
        mainCameraAnchor.transform.eulerAngles = new Vector3(mainCameraAnchor.transform.eulerAngles.x,
                                                             mainCameraAnchor.transform.eulerAngles.y,
                                                             0);
    }

    void RoateUICamera()
    {
        Vector3 rotation = CharacterManagement.Instance.activeCharacter.transform.eulerAngles;
        uiCameraAnchor.transform.eulerAngles = new Vector3(0, rotation.y + 30, 0);
    }

    public IEnumerator AdjustCameraOnActiveChar(GameObject activeChar)
    {
        float _distance = Vector3.Distance(mainCameraAnchor.transform.position, activeChar.transform.position);
        float _startTime = Time.time;

        while (_distance != 0)
        {
            float _partialDistance = (Time.time - _startTime) * 2.0f;
            float _remainingDistance = _partialDistance / _distance;

            mainCameraAnchor.transform.position = Vector3.Lerp(mainCameraAnchor.transform.position, activeChar.transform.position, _remainingDistance * 0.4f);

            if (_remainingDistance <= 0.1)
            {
                _remainingDistance = 0;
                mainCameraAnchor.transform.position = activeChar.transform.position;
            }
        }
        return null;
    }

    public void SetDistanceToPlayer(bool closeUp, bool _crouch)
    {
        Vector3 _currentPosition = mainCamera.transform.localPosition;
        float y = _cameraNormalHeight, z = _cameraFarDistance;

        if (closeUp)
            z = _cameraCloseDistance;
        if (_crouch)
            y = _cameraCrouchHeight;

        Vector3 _newPosition = new Vector3(_currentPosition.x, y, -z);
        float _distance = Vector3.Distance(_currentPosition, _newPosition);

        if (_distance < 0.02f)
            mainCamera.transform.localPosition = _newPosition;
        else
            mainCamera.transform.localPosition = Vector3.Lerp(_currentPosition, _newPosition, _distance * 0.2f);
    }
}
