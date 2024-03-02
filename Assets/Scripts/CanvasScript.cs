using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasScript : MonoBehaviour
{
    [SerializeField] GameObject joystick;
    [SerializeField] Box box;
    [SerializeField] GameObject comboText;
    [SerializeField] Slider comboSlider;
    [SerializeField] GameObject canvasInformation;

    int combo;
    float comboTime, comboMult = 1, totalComboTime;
    bool shaking;

    private void Update()
    {
        comboText.GetComponent<TMPro.TextMeshProUGUI>().text = "combo: " + combo;
        comboTime -= Time.deltaTime;
        comboSlider.value = comboTime / totalComboTime;
        if(comboTime <= 0)
        {
            combo = 0;
            comboMult = 1;
            box.knockbackMult = 1;
        }

        if (shaking)
        {
            if (comboText.transform.localScale.x < 1.5)
            {
                comboText.transform.localScale *= 1.01f;
            }
        }
    }
    public void InfoActive()
    {
        canvasInformation.SetActive(!canvasInformation.activeSelf);
    }

    public void JoystickActive()
    {
        joystick.SetActive(!joystick.activeSelf);
    }

    public IEnumerator BoxHit()
    {
        combo++; comboMult += 0.2f;
        comboTime = 2.4f * comboMult;
        if(comboTime > 3.5f)
        {
            comboTime = 3.5f;
        }
        totalComboTime = comboTime;
        shaking = true;
        yield return new WaitForSeconds(0.4f);
        shaking = false;
        comboText.transform.localScale = new Vector3(1, 1, 1);
    }
}
