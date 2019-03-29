using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	[SerializeField]
	private string controlledObject;
	private bool inputPause = false;
	public LauncherController launcher;
	private PointAbility ability;
	private Vector2 dragStart;

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
			if(Ability.GetType() == typeof(RadialStatus))
			{
				indicator.abilityFX.SetRadius(((RadialStatus)Ability).radius);
			}

			indicator.abilityFX.transform.position = MouseWorldPosition;
			indicator.abilityFX.Visible = true;
		}
	}

	void ManageLauncher()
	{
		//When the mouse is clicked...
		if (Input.GetMouseButtonDown(0))
		{
			dragStart = MouseWorldPosition;
			indicator.field.transform.position = dragStart;
		}

		//If the mouse is being held down...
		if (Input.GetMouseButton(0))
		{
			launcher.Drag = (MouseWorldPosition - dragStart);

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

		if (Input.touchCount > 0)
		switch (Input.GetTouch(0).phase)
		{
			case TouchPhase.Began:

				dragStart = TouchWorldPosition;
				indicator.field.transform.position = dragStart;

				break;

			case TouchPhase.Moved:

				launcher.Drag = (TouchWorldPosition - dragStart);

				if (!launcher.Armed() && launcher.Drag.magnitude >= launcher.minDragLength)
				{
					indicator.ChargeFieldVisible = true;
					launcher.ReadyShot();
				}

				break;

			case TouchPhase.Ended:

				launcher.LaunchShot();
				launcher.Drag = Vector2.zero;
				indicator.ChargeFieldVisible = false;

				break;

		}

	}

	public static Vector2 MouseWorldPosition
	{
		get
		{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	private Vector2 TouchWorldPosition
	{
		get
		{
			return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
		}
	}

}
