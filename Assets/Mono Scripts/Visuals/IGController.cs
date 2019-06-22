using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this is to control graphics feedback to the user regarding what their pointer is doing
public class IGController : MonoBehaviour {

	[Header("Launcher Stuff")]

	public LauncherController launcher;

	//[Header("Ability Stuff")]
	//public VFXController abilityFX;

	public SpriteRenderer fSprite, pSprite;//, aSprite;
	private bool armedLastFrame = false;
	private Vector3 lastPos;

	private void Start()
	{
		//aSprite = abilityFX.GetComponent<SpriteRenderer>();
		fSprite.transform.localScale = (Vector3.right + Vector3.up) * launcher.maxDragLength * 2;

		ChargeFieldVisible = false;
		//abilityFX.Visible = false;
	}

	private void Update ()
	{
		if (launcher &&  launcher.Armed())
		{
			pSprite.transform.localPosition = launcher.Pull / (2 * launcher.maxDragLength);
			fSprite.material.SetFloat("_charge", launcher.ChargePercentage);
			fSprite.material.SetFloat("_drag", launcher.PullPercentage);
		}
	}

	/*public void ActivateAbilityFX()
	{
		abilityFX.ActivateFX();
	}*/

	//sets if the sprites that make up the charge field are visible
	public bool ChargeFieldVisible
	{
		get
		{
			return fSprite.enabled;
		}

		set
		{
			fSprite.enabled = pSprite.enabled = value;

			if (value)
			{
				pSprite.transform.localPosition = launcher.Pull;
				fSprite.material.SetFloat("_charge", launcher.ChargePercentage);
				fSprite.material.SetFloat("_drag", launcher.PullPercentage);
			}
		}
	}
}
