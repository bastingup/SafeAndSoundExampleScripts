using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInformation : MonoBehaviour
{
    public Text healthNumber, surveillanceNumber, skillNumber;
    public Image healthBar, surveillanceBar;

    void Update()
    {
        this.transform.LookAt(CameraController.Instance.mainCamera.transform.position);
        this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);

        PlayerController _pc = this.GetComponentInParent<PlayerController>();

        healthNumber.text = _pc.healthPoints.ToString();
        healthBar.fillAmount = _pc.healthPoints * 0.01f;

        surveillanceNumber.text = ((int)Mathf.Round(_pc.surveilanceLevel)).ToString();
        surveillanceBar.fillAmount = _pc.surveilanceLevel * 0.01f;

        skillNumber.text = _pc.skillPoints.ToString();
    }
}
