using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//just used to toggle the active state of a gameObject using unity's buttons.
public class ToggleActive : MonoBehaviour
{
    public void Toggle()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
