using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grav : ProjectileController
{
	[Header("Grav Specific")]
	[SerializeField] private GameObject gravField;
	[SerializeField] private float expandDelay = 2f;
	[SerializeField] private float expandDuration = 0.5f;

	private Vector3 storedScale;

	private void Start()
	{
		//stores the size of the grav field after size upgrades are applied (if any)
		storedScale = gravField.transform.localScale;

		//shrink the field size down to invisible levels
		gravField.transform.localScale = Vector2.one;
	}

	public override void Launch(Vector2 direction, float power)
	{
		base.Launch(direction, power);

		StartCoroutine(ExpandField());
	}

	private IEnumerator ExpandField()
	{
		yield return new WaitForSeconds(expandDelay);

		float timer = 0, progress = 0;

		do
		{
			timer = Mathf.Clamp(timer + Time.deltaTime, 0, expandDuration);
			progress = timer / expandDuration;

			gravField.transform.localScale = Vector3.Lerp(Vector2.one, storedScale, Mathf.Sqrt(progress));

			yield return null;

		} while (timer < expandDuration);

	}

}
