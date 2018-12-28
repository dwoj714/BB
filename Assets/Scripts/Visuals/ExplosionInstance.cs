using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionInstance : MonoBehaviour
{

	public Material mat;
	public SpriteRenderer spr;
	public bool selfDestruct = true;
	public bool trigger = false;
	public float timeScale = 1;
	public float outerEdge = 0;
	private float pTime;
	private Color pColor;

	private float time = 0;

	// Use this for initialization
	void Awake()
	{
		spr = GetComponent<SpriteRenderer>();
		mat = spr.material;
		time = 0;
	}

	// Update is called once per frame
	void Update()
	{

		if (trigger)
		{
			Detonate();
			trigger = false;
			time = 0; 
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

		time += Time.deltaTime;

//		Debug.Log(time + " " + time * timeScale);

	}

	public void SetRadius(float radius)
	{
		transform.localScale = Vector2.up * radius * 2 + Vector2.right * radius * 2;
	}

	public void Detonate()
	{
		mat.SetFloat("_startTime", Time.time);
		if (selfDestruct)
		{
			Destroy(gameObject, 1 / timeScale);
		}
	}
}
