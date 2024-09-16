using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] private string titulo;
    [SerializeField][Multiline] private string texto;

    public void Start()
    {

        Button_UI ui = transform.GetComponent<Button_UI>();

        if(ui == null) transform.AddComponent<Button_UI>();

        ui.MouseOverOnceFunc = () =>
        {
            string myTooltip = "<b><color=#00ff00>" + titulo + "</b></color>\n<color=#F00>"
            + texto;

            TooltipScreenSpaceUI.ShowTooltip_Static(myTooltip);
        };

        ui.MouseOutOnceFunc = () =>
        {
            TooltipScreenSpaceUI.HideTooltip_Static();
        };
        
    }
}
