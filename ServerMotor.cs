using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoverShooter;

public class ServerMotor : MonoBehaviour
{
    public List<CharacterMotor> knownCharacters;

    private void Start()
    {
        GetComponent<SphereCollider>();
    }

    #region Public Methods
    public void RegisterCharacter(CharacterMotor _cm)
    {
        knownCharacters.Add(_cm);
        StartCoroutine(ForgetCharacter(_cm, SurveillanceController.Instance.getCameraForgetTime * 3));
    }
    #endregion

    #region Private Methods
    IEnumerator ForgetCharacter(CharacterMotor _cm, float wait)
    {
        yield return new WaitForSeconds(wait);
        knownCharacters.Remove(_cm);
    }
    #endregion
}
