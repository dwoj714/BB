using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconManager : MonoBehaviour
{


    public static WeaponIconManager main;

    public Sprite[] icons = new Sprite[7];
    public Color[] colors = new Color[7];

    // Start is called before the first frame update
    void Start()
    {
		if (!main)
		{
            main = this;
		}
		else
		{
            Debug.LogWarning("Multiple WeaponIconManagers Set! (Should only be one)");
        }
    }

    public static void Assign(Image image, int weaponIndex)
	{
        image.sprite = main.icons[weaponIndex];
        image.color = main.colors[weaponIndex];
	}
}
