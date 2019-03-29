using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this is to control graphics feedback to the user regarding what their pointer is doing
public class IGController : MonoBehaviour {

	[Header("Launcher Stuff")]

	public LauncherController launcher;

	public GameObject field;
	public GameObject point;

	[Header("Ability Stuff")]
	public VFXController abilityFX;

	private SpriteRenderer fSprite, pSprite, aSprite;

	private bool armedLastFrame = false;
	private Vector3 lastPos;

	private void Awake()
	{
		fSprite = field.GetComponent<SpriteRenderer>();
		pSprite = point.GetComponent<SpriteRenderer>();
		aSprite = abilityFX.GetComponent<SpriteRenderer>();
		field.transform.localScale = (Vector3.right + Vector3.up) * launcher.maxDragLength * 2;

		ChargeFieldVisible = false;
		abilityFX.Visible = false;
	}

	private void Update ()
	{
		if (launcher.Armed())
		{
			point.transform.localPosition = launcher.Pull / (2 * launcher.maxDragLength);
			fSprite.material.SetFloat("_charge", launcher.ChargePercentage);
			fSprite.material.SetFloat("_drag", launcher.PullPercentage);
		}
	}

	public void ActivateAbilityFX()
	{
		abilityFX.ActivateFX();
	}

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
				point.transform.localPosition = launcher.Pull;
				fSprite.material.SetFloat("_charge", launcher.ChargePercentage);
				fSprite.material.SetFloat("_drag", launcher.PullPercentage);
			}
		}
	}
}
