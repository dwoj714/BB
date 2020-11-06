using System;
using UnityEngine;

public class InputManager : MonoBehaviour {

	private static bool inputPause = false;
	public static bool inputHalt = false;
	public static InputManager main;

	public IInputReciever reciever;

	private bool mouseDetected = false;

	private void Awake()
	{
		Debug.Log("Inputman set");
		if (!main) main = this;
		else Debug.LogError("More than one InputManager instantiated!");

		GameManager.GameEnded += OnGameEnd;
	}

	void Update ()
	{
		if (reciever != null && !(inputPause || inputHalt) && !GameManager.frozen)
		{
			//foreach (IInputReciever reciever in recievers)
			//{
			if (Input.GetMouseButtonDown(0))
			{

				mouseDetected = true;
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

			if (!mouseDetected && Input.touchCount > 0)
			{
				switch (Input.GetTouch(0).phase)
				{
					case TouchPhase.Began:
						reciever.OnInputStart(TouchWorldPosition);
						break;

					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						reciever.OnInputHeld(TouchWorldPosition);
						break;

					case TouchPhase.Ended:
						reciever.OnInputReleased(TouchWorldPosition);
						break;

					case TouchPhase.Canceled:
						reciever.OnInputCancel();
						break;

				}
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.main.OnEscapePressed();
			}

		}
		else inputPause = false;
	}

	void OnGameEnd(object o, EventArgs e)
	{
		reciever = null;
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

	//Public call to cancel input for the reciever
	public void CancelInput()
	{
		reciever.OnInputCancel();
	}

}
