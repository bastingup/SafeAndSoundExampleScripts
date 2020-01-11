using UnityEngine;

public class Ember : PlayerController
{
    // Diegetic User Interface and Feedback
    [SerializeField]
    private GameObject[] jacketParts;

    protected override void Update()
    {
        base.Update();
        if (Input.GetButtonDown("Skill"))
            if (skillAvailable && isActiveCharacter && skillPoints > 0)
                SpecialSkill();
        CheckSurveilanceChanged();
    }

    void SpecialSkill()
    {
        foreach (SurveilanceDevice s in FindObjectsOfType<SurveilanceDevice>())
            s.SetRefreshnetworkTrue();
        foreach (RobotAI r in FindObjectsOfType<RobotAI>())
            r.DrawConnectionLine(r);
        skillPoints--;
    }
}
