using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillSurveillance activeSurveillanceSkill;
    public SkillPhysical activePhysicalSkill;
    public SkillCombat activeCombatSkill;

    #region Singleton and Distribution
    public static SkillManager Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;

        RefreshValueDistribution();
    }
    #endregion

    public void RefreshValueDistribution()
    {
        if (activeSurveillanceSkill != null)
            DistributeSurveillanceValues();
        if (activePhysicalSkill != null)
            DistributePhysicalValues();
        if (activeCombatSkill != null)
            DistributeCombatValues();
    }

    private void DistributeSurveillanceValues()
    {
        // Distribute Camera Range
        SurveilanceDevice[] _surveillanceDevices = GameObject.FindObjectsOfType<SurveilanceDevice>();
        foreach (SurveilanceDevice _sd in _surveillanceDevices)
        {
            SphereCollider _fov = _sd.gameObject.GetComponent<SphereCollider>();
            float _range = _fov.radius;
            _fov.radius = _range * activeSurveillanceSkill.cameraRangeMultiplier;
            Vector3 _newFov = new Vector3(_fov.center.x, _fov.center.y, _fov.center.z);
            _newFov.x = _fov.center.x * activeSurveillanceSkill.cameraRangeMultiplier;
            _fov.center = _newFov;
        }

        // Distribute Surveillance Punishment
        SurveilanceMaster.Instance.surveillanceMultiplier = activeSurveillanceSkill.punishmentMultiplier;
    }

    private void DistributePhysicalValues()
    {
        foreach (GameObject _g in CharacterManagement.Instance._playableCharacters)
        {
            // HEALTH
            float _tempHealth = _g.GetComponent<Character>().maxHealthPoints * activePhysicalSkill.healthMultiplier;
            Mathf.Round(_tempHealth);
            Character _localC = _g.GetComponent<Character>();
            _localC.maxHealthPoints = (int)_tempHealth;
            _localC.ChangeHealth();

            // SPEED
            _g.GetComponent<PlayerController>().runSpeed *= activePhysicalSkill.speedMultiplier;
        }
    }

    private void DistributeCombatValues()
    {
        // TODO Test this
        foreach (GameObject _g in CharacterManagement.Instance._playableCharacters)
        {
            // DAMAGE
            PlayerController _localPC = _g.GetComponent<PlayerController>();

            float _tempDamage = _localPC.firearm.damage * activeCombatSkill.baseDamageMultiplier;
            float _tempStealthDamage = _localPC.firearm.damageStealth * activeCombatSkill.stealthCombatMultiplier;

            _localPC.firearm.damage = (int)Mathf.Round(_tempDamage);
            _localPC.firearm.damageStealth = (int)Mathf.Round(_tempStealthDamage);
        }
    }
}
