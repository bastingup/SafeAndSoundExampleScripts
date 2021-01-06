using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoverShooter;
using System.Linq;

public class CameraMotor : MonoBehaviour
{
    [SerializeField]
    private int _cameraSide = 0, _viewDistance, _viewAngle;

    [SerializeField]
    private Color _clearColor, _awareColor, _spottedColor;

    [SerializeField]
    private Light _cameraLight;

    [SerializeField]
    private ConeCollider _coneCollider;

    public List<CharacterMotor> detectedCharacters, rememberingCharacters;
    public List<CameraMotor> connectedCameras;
    public List<ServerMotor> connectedServers;

    #region Awake/Start/Update
    void Start()
    {
        SetLightColor(_clearColor);
        SetFovAsTrigger();
        if (CheckConnectionAvailable())
            RegisterWithConnectedCameras();
        GetUplinkTrigger();
    }
    private void OnEnable()
    {
        ConnectedCamerasToDistinct();
    }
    #endregion

    #region Get/Set
    public int GetTeam
    {
        get { return _cameraSide; }
    }
    public int GetViewDistance
    {
        get { return _viewDistance; }
    }
    #endregion

    #region Public Methods
    // Sets
    public void SetTeam(int _newTeam)
    {
        _cameraSide = _newTeam;
    }
    public void SetFov(int _newFov)
    {
        _coneCollider.m_angle = _newFov;
        _cameraLight.spotAngle = ((_newFov * 2) + 8);
    }
    public void SetDistance(int _newDistance)
    {
        _coneCollider.m_distance = _newDistance;
        _cameraLight.range = _newDistance;
    }
    public void DebugInfo()
    {
        Debug.Log("fov: " + _coneCollider.m_angle.ToString() + " distance: " + _coneCollider.m_distance.ToString());
    }

    // Character Registration
    public void CharacterEnteredFov(GameObject _character)
    {
        CharacterMotor _cm = _character.GetComponent<CharacterMotor>();

        // Only register if score is above threshold and of another side
        if (_cm.GetSurveillanceScore > SurveillanceController.Instance.GetSurveillanceLimit
            && _cm.GetComponent<Actor>().Side != _cameraSide)
        {
            detectedCharacters.Add(_cm);
            detectedCharacters = detectedCharacters.Distinct().ToList();
            SetLightColor(_spottedColor);
            if (rememberingCharacters.Contains(_cm))
                rememberingCharacters.Remove(_cm);

            RegisterCharacterOnOtherCameras(_cm);
        }
    }
    public void CharacterLeftFov(GameObject _character)
    {
        CharacterMotor _cm = _character.GetComponent<CharacterMotor>();
        if (detectedCharacters.Contains(_cm))
        {
            // Remove from currently detected characters
            detectedCharacters.Remove(_cm);

            // Add to characters the cam remembers
            if (!rememberingCharacters.Contains(_cm))
                rememberingCharacters.Add(_cm);

            // Check whether no characters are detected but at least one is remembered. Change Light.
            if (detectedCharacters.Count == 0 && rememberingCharacters.Count > 0)
                SetLightColor(_awareColor);

            StartCoroutine(ResetToClear(SurveillanceController.Instance.getCameraForgetTime, _cm));
            ClearFromOtherCameras(_cm);
        } 
    }

    public IEnumerator ResetToClear(float wait, CharacterMotor _cm)
    {
        yield return new WaitForSecondsRealtime(wait);

        if (!detectedCharacters.Contains(_cm) && rememberingCharacters.Contains(_cm) && !FindCharacterInNetworkDetected(_cm))
            rememberingCharacters.Remove(_cm);
        if (detectedCharacters.Count == 0 && rememberingCharacters.Count == 0)
            SetLightColor(_clearColor);
    }
    #endregion

    #region Private Methods
    void SetLightColor(Color _col)
    {
        _cameraLight.color = _col;
    }
    void SetFovAsTrigger()
    {
        _coneCollider.m_isTrigger = true;
    }
    void GetUplinkTrigger()
    {
        GetComponent<SphereCollider>();
    }
    void RegisterWithConnectedCameras()
    {
        foreach (CameraMotor _cm in connectedCameras)
        {
            _cm.connectedCameras.Add(this);
        }
    }
    void ConnectedCamerasToDistinct()
    {
        connectedCameras = connectedCameras.Distinct().ToList();
    }
    void RegisterCharacterOnOtherCameras(CharacterMotor _cm)
    {
        if (CheckConnectionAvailable())
        {
            foreach (CameraMotor _camera in connectedCameras)
            {
                _camera.rememberingCharacters.Add(_cm);
                _camera.SetLightColor(_awareColor);
            }
        }
    }
    void ClearFromOtherCameras(CharacterMotor _cm)
    {
        if (CheckConnectionAvailable())
        {
            foreach (CameraMotor _camera in connectedCameras)
            {
                StartCoroutine(_camera.ResetToClear(SurveillanceController.Instance.getCameraForgetTime, _cm));
                _camera.rememberingCharacters = rememberingCharacters.Distinct().ToList();
            }
        }     
    }
    bool FindCharacterInNetworkDetected(CharacterMotor _cm)
    {
        foreach (CameraMotor _camera in connectedCameras)
            if (_camera.detectedCharacters.Contains(_cm))
                return true;
        return false;
    }
    bool FindCharacterInNetworkRemembering(CharacterMotor _cm)
    {
        foreach (CameraMotor _camera in connectedCameras)
            if (_camera.rememberingCharacters.Contains(_cm))
                return true;
        return false;
    }
    bool CheckConnectionAvailable()
    {
        if (connectedCameras.Count > 0)
            return true;
        return false;
    }
    Threat DetermineThreat()
    {
        Threat _t = new Threat();

        if (detectedCharacters.Count > 0)
        {
            _t.Actor = detectedCharacters[Random.Range(0, detectedCharacters.Count - 1)].GetComponent<Actor>();
            _t.Position = _t.Actor.transform.position;
        }
        else if (rememberingCharacters.Count > 0)
        {
            _t.Actor = rememberingCharacters[Random.Range(0, rememberingCharacters.Count - 1)].GetComponent<Actor>();
            _t.Position = _t.Actor.transform.position;
        }
        return _t;
    }
    FighterState AlertAI()
    {
        return FighterState.standAndFight;
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterMotor>() != null)
        {
            FighterBrain _localFighter = other.GetComponent<FighterBrain>();
            if (other.GetComponent<Actor>().Side == _cameraSide)
            {
                if (_localFighter.Threat == null)
                {
                    Threat _t = DetermineThreat();
                    if (_t.Actor != null)
                    {
                        _localFighter.ToSetThreat(_t);
                        _localFighter.SetLastKnownThreatPosition(_t.Position);
                        _localFighter.overwriteState(AlertAI(), true, true);
                    }
                }
            } 
        }
    }
    #endregion
}
