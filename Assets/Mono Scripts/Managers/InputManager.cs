using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	private bool inputPause = false;

	public IInputReciever reciever;

	private bool releasedLastFrame = false;

	void Update ()
	{
		if (GameManager.gameInProgress && !inputPause)
		{
			if (Input.GetMouseButtonDown(0))
			{
				reciever.OnInputStart(MouseWorldPosition);
			}
			if (Input.GetMouseButton(0))
			{
				reciever.OnInputHeld(MouseWorldPosition);
			}
			if (Input.GetMouseButtonUp(0))
			{
				reciever.OnInputReleased(MouseWorldPosition);
			}
		}
		else inputPause = false;

	}

	/*void ManageAbility()
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
	}*/

	/*void ManageLauncher()
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

	}*/

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
