using UnityEngine;

public interface IInputReciever
{
	void OnInputStart(Vector2 position);
	void OnInputHeld(Vector2 position);
	void OnInputReleased(Vector2 position);
	void OnInputCancel();
}