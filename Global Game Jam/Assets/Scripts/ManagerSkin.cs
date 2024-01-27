using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ManagerSkin : MonoBehaviour
{
    public Image sr;
    public List<Sprite> skins = new List<Sprite>();
    private int selectedskin = 0;
    public GameObject playerskin;


    public void NextOption()
    {
        selectedskin = selectedskin + 1;
        if(selectedskin == skins.Count)
        {
            selectedskin = 0;
        }
        sr.sprite = skins[selectedskin];
    }

    public void BackOption()
    {
        selectedskin = selectedskin - 1;
        if (selectedskin < 0 )
        {
            selectedskin = skins.Count-1;
        }
        sr.sprite = skins[selectedskin];
    }
}
