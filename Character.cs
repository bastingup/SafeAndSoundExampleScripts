using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    military, player, civilian
}

public enum HidingStatus
{
    inStealth, inCombat
}

public class Character : Creature
{
    // Public members of each individual
    public bool isActiveCharacter, skillAvailable, canRun, isRunning, canMove, aiming, firing, reloading, crouching;
    public int healthPoints, maxHealthPoints;
    public float surveilanceLevel;
    public string information;

    // Private members for internal checking
    private int _surveilanceRef;

    //  [HideInInspector]
    public List<GameObject> observedBy;

    public Faction faction;

    private int RestoreHealth(int healthPoints)
    {
        return healthPoints++;
    }

    public bool CheckIfHidden()
    {
        if (observedBy.Count > 0) { return false; }
        else { return true; }
    }

    public void ChangeHealth(int z)
    {
        healthPoints += z;
        if (healthPoints > maxHealthPoints) { healthPoints = maxHealthPoints; }
        else if (healthPoints < 0) { healthPoints = 0; }
    }

    public void ChangeHealth()
    {
        if (healthPoints > maxHealthPoints) { healthPoints = maxHealthPoints; }
        else if (healthPoints < 0) { healthPoints = 0; }
    }

    public void ChangeSurveilanceLevel(int s)
    {
        surveilanceLevel += s;
        if (surveilanceLevel > 100) { surveilanceLevel = 100; }
        else if (surveilanceLevel < 0) { surveilanceLevel = 0; }
    }

    public void CheckSurveilanceChanged()
    {
        if (surveilanceLevel != _surveilanceRef)
        {
            _surveilanceRef = (int)Mathf.Round(surveilanceLevel);
           // ChangeContrastMaterial(); 
        }
    }

    public void ChangeContrastMaterial()
    {
        Color _colSurveiled = Color.red;
        Color _colUnsurveiled = new Color(1.0f, 0.64f, 0.0f);

        if (surveilanceLevel >= SurveilanceMaster.Instance.surveilanceLimit)
        {
            //  TODO change special parts of clothing indicating surveillance level        
        }
        else
        {
            //  TODO change special parts of clothing indicating surveillance level        
        }

    }

    public void Punish()
    {
        if (observedBy.Count > 0)
            surveilanceLevel += SurveilanceMaster.Instance.ReturnPunishment();
    }
}
