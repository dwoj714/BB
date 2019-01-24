using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{

	public Material mat;
	public SpriteRenderer spr;
	public float timeScale = 1;
	public float duration = 1;
	private float pTime;
	private float timer = 0;
	private Color pColor;

	// Use this for initialization
	void Awake()
	{
		spr = GetComponent<SpriteRenderer>();
		mat = spr.material;
	}

	// Update is called once per frame
	void Update()
	{

		if(timer < duration)
		{
			timer += Time.deltaTime;

			if(timer >= duration)
			{
				Visible = false;
				mat.SetFloat("_flip", 0);
			}
		}


		if (pTime != timeScale)
		{
			pTime = timeScale;
			mat.SetFloat("_timeScale", timeScale);
		}

		if (spr.color != pColor)
		{
			pColor = spr.color;
			mat.SetColor("_color", pColor);
		}
	}

	public void ActivateFX()
	{
		Visible = true;
		mat.SetFloat("_startTime", Time.time);
		mat.SetFloat("_flip", 1);
		timer = 0;
	}



	public void SetRadius(float radius)
	{
		transform.localScale = Vector2.up * radius * 2 + Vector2.right * radius * 2;
	}

	public bool Visible
	{
		get
		{
			return spr.enabled;
		}

		set
		{
			spr.enabled = value;
		}
	}

}
