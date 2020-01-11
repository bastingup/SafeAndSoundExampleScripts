using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ezra : PlayerController
{
    // Skill
    // TODO replace with actual lock marker
    [SerializeField] private GameObject marker;
    [SerializeField] private LayerMask _layerMask;

    protected override void Update()
    {
        base.Update();
        if (Input.GetAxis("Skill") == 1 && isActiveCharacter)
        {
            GameObject closest = SpecialSkill();
            if (closest != null)
            {
                SetLockedMarker(closest);
                if (Input.GetButtonDown("Fire") && !aiming && skillPoints > 0)
                {
                    --skillPoints;
                    HackDevice(closest);
                    DisableMarker();
                }
            }
            else
            {
                DisableMarker();
            }
        }
        else
        {
            DisableMarker();
        }
        CheckSurveilanceChanged();
    }

    void DisableMarker()
    {
        if (marker.activeSelf)
            marker.SetActive(false);
    }

    void SetLockedMarker(GameObject selected)
    {
        if (!marker.activeSelf)
            marker.SetActive(true);
        marker.transform.position = selected.transform.position;
    }

    GameObject SpecialSkill()
    {
        Ray _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, 20, _layerMask))
            if (_hit.transform.gameObject.CompareTag("Surveilance"))
            {
                return _hit.transform.gameObject;
            }
        return null;
    }

    void HackDevice(GameObject _device)
    {
        _device.BroadcastMessage("HackDevice");
    }
}
