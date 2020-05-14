using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutMenuSequencer : MonoBehaviour
{
	public static LoadoutMenuSequencer main;

	[Header("Values")]
	[SerializeField] private float transitionTime = 0.8f;

	[Header("References")]
	[SerializeField] private Transform[] icons;
	[SerializeField] private SpriteRenderer shadow;
	[SerializeField] private RotorController weaponWheel;

	private float WheelRotation
	{
		//return z-axis rotation
		get
		{
			return weaponWheel.transform.rotation.eulerAngles.z;
		}

		//sets rotation z axis to value
		set
		{
			weaponWheel.transform.rotation = Quaternion.Euler(Vector3.forward * value);
		}
	}

	private static bool fromLeft = true;

	public static float RotationOffset
	{
		get
		{
			return 140 * (fromLeft ? 1 : -1);
		}
	}

	// Start is called before the first frame update
	void Awake()
    {
		if (!main)
		{
			main = this;
		}
    }

	public void PlayEntry()
	{
		StartCoroutine(EntrySequence());
	}

	IEnumerator EntrySequence()
	{
		WheelRotation = RotationOffset;

		float timer = 0;
		while(timer < transitionTime)
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, transitionTime);
			float progress = timer / transitionTime;

			WheelRotation = Mathf.Pow(1 - progress, 2) * RotationOffset;

			yield return null;
		}

		GameManager.main.InputMan.reciever = weaponWheel;

	}

}
