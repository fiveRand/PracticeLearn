using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatUI : MonoBehaviour
{
    [SerializeField] Image hpImage;

    public void DisplayHP(int curHP,int maxHP)
    {
        if(hpImage.type != Image.Type.Filled)
        {
            hpImage.type = Image.Type.Filled;
        }
        float value = (float)curHP / (float)maxHP;
        hpImage.fillAmount = value;
    }
}
