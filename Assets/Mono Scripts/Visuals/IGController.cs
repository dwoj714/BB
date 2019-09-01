﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this is to control graphics feedback to the user regarding what their pointer is doing
public class IGController : MonoBehaviour {

	[Header("Launcher Stuff")]

	public LauncherController launcher;

	public SpriteRenderer fSprite, pSprite;
	public LineRenderer line;
	private Vector3 lastPos;

	private void Start()
	{
		ConfigSize();
		ChargeFieldVisible = false;
		line = fSprite.GetComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.SetPosition(0, Vector3.zero);
	}

	private void Update ()
	{
		if (launcher &&  launcher.Armed)
		{
			pSprite.transform.localPosition = launcher.Pull / (2 * LauncherController.maxDragLength);
			pSprite.transform.rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.up, -launcher.Pull));

			line.SetPosition(1, pSprite.transform.localPosition);

			fSprite.material.SetFloat("_charge", launcher.ChargePercentage);
			fSprite.material.SetFloat("_drag", launcher.PullPercentage);
		}
	}

	//sets if the sprites that make up the charge field are visible
	public bool ChargeFieldVisible
	{
		get
		{
			return fSprite.enabled;
		}

		set
		{
			fSprite.enabled = pSprite.enabled = line.enabled = value;

			if (value)
			{
				pSprite.transform.localPosition = launcher.Pull;
				line.SetPosition(1, pSprite.transform.localPosition);
				fSprite.material.SetFloat("_charge", launcher.ChargePercentage);
				fSprite.material.SetFloat("_drag", launcher.PullPercentage);
			}
		}
	}

	public void ConfigSize()
	{
		fSprite.transform.localScale = (Vector3.right + Vector3.up) * LauncherController.maxDragLength * 2;
	}

}
