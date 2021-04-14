using System.Collections;
using System;
using UnityEngine;

public class RotorController : MonoBehaviour, IInputReciever
{
	[SerializeField] private float spread = 12;
	[SerializeField] private float sensitivity = 1;
	[SerializeField] private float minY = 0;
	[SerializeField] private float maxY = 0;
	[SerializeField] private float drag = 1;
	[SerializeField] private bool autoCenter = false;
	[SerializeField] private float autoCenterStrength = 1;
	public float swapTime = 0.4f;

	private bool inputActive = false;

	private Coroutine active;

	private float rotation = 0;
	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
			SetRotation(value);
		}
	}

	public Rotor[] rotors = new Rotor[3];

	private int lastMedian;

	public int MedianIdx
	{
		get;
		private set;
	}

	public delegate void RotationFinishedEventHandler(object source, EventArgs args);
	public event RotationFinishedEventHandler RotationFinished;


	private void Start()
	{
		for(int i = 0; i < rotors.Length; i++)
		{
			rotors[i].Rotation = ((rotors.Length / 2) - i) * spread;
			rotors[i].index = i;
			rotors[i].rotorController = this;
		}
		Rotation = 0;

		rotors[MedianIdx].mid = true;
	}

	private void Update()
	{
		Rotation += velocity * Time.deltaTime;
		if (velocity != 0)
		{
			velocity -= (drag + drag * Mathf.Abs(velocity)) * Time.deltaTime * (velocity > 0 ? 1 : -1);
		}

		//alter velocity to rotate towards the middle index
		if (autoCenter && !inputActive)
		{
			velocity += DegreesOffCenter * autoCenterStrength;
		}

		if (Mathf.Abs(velocity) < 0.1f) velocity = 0;
	}

	private void SetRotation(float degrees)
	{
		//range is the range of rotation values each rotor can have
		float range = spread * rotors.Length;

		for(int i = 0; i < rotors.Length; i++)
		{
			int mid = rotors.Length / 2;
			float newZ = degrees - (i - mid) * spread;
			while(newZ >= range / 2)
			{
				newZ -= range;
			}
			while(newZ < -range / 2)
			{
				newZ += range;
			}
			rotors[i].Rotation = newZ;
		}

		CalculateMedian();

		if(lastMedian != MedianIdx)
		{
			rotors[lastMedian].OnExitMiddle();
			rotors[MedianIdx].OnEnterMiddle();
		}

		lastMedian = MedianIdx;
	}

	public void IncrementRotation(bool left)
	{
		if (active != null)
		{
			StopCoroutine(active);
			active = null;
		}

		active = StartCoroutine(IncrementRotationCO(left));
	}

	private IEnumerator IncrementRotationCO(bool left)
	{
		//new position is one spread offset from current position
		float target = rotation + spread * (left ? -1 : 1);
		int targetNotches = Mathf.RoundToInt(target / spread);

		//the "real" target is the closest "notch" to the left/right
		target = targetNotches * spread;

		//the rotation to be added to the current position
		float delta = target - rotation;

		//apply the rotation over time specified by swapTime
		float timer = 0;
		float startRotation = Rotation;
		while(timer < swapTime)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, swapTime);
			Rotation = startRotation + delta * (timer / swapTime);

			yield return null;
		}

		RotationFinished?.Invoke(this, EventArgs.Empty);

	}

	public float DegreesOffCenter
	{
		get
		{
			//return the angle between the median index and 0 rotation
			return Mathf.DeltaAngle(rotors[MedianIdx].Rotation, 0);
		}
	}

	public float PercentOffCenter
	{
		get
		{
			return Mathf.Abs(DegreesOffCenter) / (spread / 2);
		}
	}

	//returns if angle measures are between each other, lower bound inclusive, upper bound exclusive
	public static bool InRange(float lower, float upper, float value)
	{
		//simplify all values
		while (lower >= 360)
			lower -= 360;
		while (lower < 0)
			lower += 360;

		while (upper >= 360)
			upper -= 360;
		while (lower < 0)
			upper += 360;

		while (value >= 360)
			value -= 360;
		while (value < 0)
			value += 360;

		if (lower > upper)
		{
			//Debug.Log(value + " between " + lower + " and " + upper + "? " + (value >= lower || value < upper));
			return value >= lower || value < upper;
		}
		else
		{
			//Debug.Log(value + " between " + lower + " and " + upper + "? " + (value >= lower && value < upper));
			return value >= lower && value < upper;
		}

	}

	private int CalculateMedian()
	{
		for (int i = 0; i < rotors.Length; i++)
		{
			float z = rotors[i].Rotation;

			//Debug.Log("abs(z = " + "rotors["+ i +"] = " + + z + ") < " + spread / 2 + " ? " + (Mathf.Abs(z) < spread / 2));
			if (InRange(-spread / 2, spread / 2, z))
			{
				MedianIdx = i;
				return i;
			}
		}

		Debug.LogError("Failed to calculate rotorController MedianIDX on " + name);
		return -1;
	}


//Input handling

	//Input Vars
	private Vector2 lastPos;
	private float velocity = 0;

	//holds the last few position deltas
	private Vector2[] history = new Vector2[12];

	public void OnInputStart(Vector2 position)
	{
		//stop the active rotation coroutine if one exists
		if (active != null)
		{
			StopCoroutine(active);
			active = null;
		}

		//store starting values
		if (position.y <= maxY && position.y >= minY)
		{
			lastPos = position;
			inputActive = true;
			velocity = 0;

			//initialize the history list
			for(int i = 0; i < history.Length; i++ )
			{
				history[i] = Vector2.zero;
			}

		}
		else inputActive = false;
	}

	public void OnInputHeld(Vector2 position)
	{
		if (inputActive)
		{
			Vector2 delta = lastPos - position;
			Rotation = rotation + delta.x * sensitivity;

			//enqueue the next position delta
			//this loop "bubbles down" the current topmost entry
			for(int i = history.Length - 1; i > 1; i--)
			{
				history[i] = history[i - 1];
			}
			//Set the topmost entry to the new delta
			history[0] = delta;

			lastPos = position;
		}
	}

	public void OnInputReleased(Vector2 position)
	{
		//apply angular velocity based on the history array
		if (inputActive)
		{
			//stores the amount of zero vectors in the deltas, so they can be disregarded in the average calculation
			int zCount = 0;
			Vector2 sum = Vector2.zero;
			for(int i = 0; i < history.Length; i++)
			{
				sum += history[i];

				if(history[i] == Vector2.zero)
				{
					zCount++;
				}
			}

			//set velocity based on the average of the nonzero entries in the history list
			if (zCount != history.Length)
			{
				//convert the sum to an average
				sum /= history.Length - zCount;

				//add to the velocity the average * sensitivity * inverse delta time
				velocity = sum.x * sensitivity * 1 / Time.fixedDeltaTime;
			}
		}

		inputActive = false;
	}

	public void OnInputCancel()
	{
		inputActive = false;
	}

}
