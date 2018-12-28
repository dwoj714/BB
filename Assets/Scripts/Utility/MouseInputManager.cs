using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : MonoBehaviour {

	public string controlledObject;
	public LauncherController launcher;
	private Vector3 clickPos;

	void Update ()
	{
		//Probably replace this string with an int later
		if(controlledObject == "Launcher")
		{
			ManageLauncher();
		}
	}

	void ManageLauncher()
	{
		//If the mouse is being held down...
		if (Input.GetMouseButton(0))
		{
			launcher.mouseDrag = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickPos);
			//Clamp the magnitude of mouseDrag to maxDragLength
			launcher.mouseDrag = Vector2.ClampMagnitude(launcher.mouseDrag, launcher.maxDragLength * launcher.ChargePercentage());

			if (launcher.Armed())
			{
				//Debug.DrawRay(launcher.transform.position, -launcher.mouseDrag.normalized * 20, Color.black);
				Debug.DrawRay(launcher.transform.position, -launcher.mouseDrag.normalized * launcher.maxDragLength, Color.red);
				Debug.DrawRay(launcher.transform.position, -launcher.mouseDrag, Color.green);
			}
		}

		//When the mouse is clicked...
		if (Input.GetMouseButtonDown(0))
		{
			launcher.ReadyShot();
			clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		//When the mouse is released...
		if (Input.GetMouseButtonUp(0))
		{
			launcher.LaunchShot();
			launcher.mouseDrag = Vector2.zero;
		}
	}
}
