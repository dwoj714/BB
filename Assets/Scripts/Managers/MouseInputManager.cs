using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : MonoBehaviour {

	[SerializeField]
	private string controlledObject;
	private bool inputPause = false;
	public LauncherController launcher;
	private PointAbility ability;
	private Vector2 clickPos;

	private bool releasedLastFrame = false;

	public IGController indicator;

	public PointAbility Ability
	{
		get
		{
			return ability;
		}

		set
		{
			ability = value;
		}
	}

	public string ControlledObject
	{
		get
		{
			return controlledObject;
		}

		set
		{
			//When inputPause is set true, inputs are ignored for the first frame after doing so to avoid issues with button releases
			inputPause = true;
			controlledObject = value;
		}
	}

	void Update ()
	{
		if (!inputPause)
		{
			if (controlledObject == "Launcher")
			{
				ManageLauncher();
			}

			if (controlledObject == "Ability")
			{
				ManageAbility();
			}
		}
		else inputPause = false;

	}

	void ManageAbility()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Ability.Activate(MouseWorldPosition);
			Ability.Disarm(this);
			indicator.ActivateAbilityFX();
		}
		if (Input.GetMouseButton(0))
		{
			indicator.abilityFX.transform.position = MouseWorldPosition;
			indicator.abilityFX.Visible = true;
		}
	}

	void ManageLauncher()
	{
		//When the mouse is clicked...
		if (Input.GetMouseButtonDown(0))
		{
			clickPos = MouseWorldPosition;
			indicator.field.transform.position = clickPos;
		}

		//If the mouse is being held down...
		if (Input.GetMouseButton(0))
		{
			launcher.Drag = (MouseWorldPosition - clickPos);

			if (!launcher.Armed() && launcher.Drag.magnitude >= launcher.minDragLength)
			{
				indicator.ChargeFieldVisible = true;
				launcher.ReadyShot();
			}
		}

		//When the mouse is released...
		if (Input.GetMouseButtonUp(0))
		{
			launcher.LaunchShot();
			launcher.Drag = Vector2.zero;
			indicator.ChargeFieldVisible = false;
		}
	}

	public static Vector2 MouseWorldPosition
	{
		get
		{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

}
