using System;
using System.Collections.Generic;
using UnityEngine;

public class DotSight : MonoBehaviour
{
	private LauncherController launcher;
	[SerializeField] private Sprite sprite;
	[SerializeField] private SpriteRenderer[] rendererList;
	private float[] distances;
	public float maxLength, minLength = 5;
	private float offset = 0;
	[Range(0, 1)]public float transparencyMultiplier = 0.5f;

	public float GetDistance(float power)
	{
		power = Mathf.Clamp01(power);
		return (maxLength - minLength) * power + minLength;
	}

	private float GetSpacing(float power)
	{
		return GetDistance(power) / rendererList.Length;
	}

	private void Start()
	{
		rendererList = new SpriteRenderer[transform.childCount];
		
		for(int i = 0; i < rendererList.Length; i++)
		{
			rendererList[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
		}

		distances = new float[rendererList.Length];

	}

	public LauncherController Launcher
	{
		get
		{
			return launcher;
		}
		set
		{
			launcher = value;

			SpriteRenderer ammoSpr = value.Ammo.GetComponent<SpriteRenderer>();
			foreach(SpriteRenderer spr in rendererList)
			{
				spr.sprite = ammoSpr.sprite;
				spr.color = ammoSpr.color;

				Color temp = spr.color;
				temp.a *= transparencyMultiplier;
				spr.color = temp;

				spr.transform.localScale = ammoSpr.transform.localScale;
			}
		}
	}

	private bool armedLastFrame = false;

	private void Update()
	{
		if (!armedLastFrame && launcher.Armed)
		{
			SetVisible(true);
			float minSpeed = ((ProjectileController)launcher.Ammo).minSpeed;
		}

		if (launcher.Armed)
		{
			Aim(-launcher.Pull.normalized, launcher.PullPercentage);
		}

		if (armedLastFrame && !launcher.Armed)
		{
			SetVisible(false);
		}
		armedLastFrame = launcher.Armed;

	}

	public void Aim(Vector2 direction, float power)
	{
		float speed;
		float spacing = GetSpacing(power);

		try
		{
			speed = ((ProjectileController)launcher.Ammo).SpeedAtPower(power);
			maxLength = ((ProjectileController)launcher.Ammo).maxSpeed;
			minLength = ((ProjectileController)launcher.Ammo).minSpeed;	
		}
		catch (InvalidCastException) { return; }

		transform.position = launcher.Shot.transform.position;

		offset += speed * Time.deltaTime;
		if(offset > spacing)
		{
			offset -= spacing;
		}

		for (int i = 0; i < rendererList.Length; i++)
		{
			float step = spacing * (1 + i);
			rendererList[i].transform.localPosition = direction * (step + offset);
		}
	}

	public void SetVisible(bool status)
	{
		foreach(SpriteRenderer spr in rendererList)
		{
			spr.enabled = status;
		}
	} 

}
