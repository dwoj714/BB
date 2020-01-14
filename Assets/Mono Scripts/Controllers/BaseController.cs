using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : PhysCircle
{

	[Header("Explosion Charges")]
	public int totalCharges = 3;
	public int Charges
	{
		get
		{
			return charges;
		}
		set
		{
			charges = value;
			StartCoroutine(CrackTransition(totalCharges - charges));
		}
	}

	private int charges = 0;

	public bool trigger = false;

	SpriteRenderer spr;
	Detonator detonator;

	[Header("Visuals")]
	public float transitionTime = 0.5f;

	[SerializeField] private float[] crackLevels = { 0, 0.777f, 1, 1 };
	[SerializeField] private float[] reachLevels = { 0.005f, 0.0033f, 0.002f, 0.002f };
	[SerializeField] private Color[] colorLevels;

	private int crackID = Shader.PropertyToID("_level");
	private int reachID = Shader.PropertyToID("_reach");
	private int colorID = Shader.PropertyToID("_color");

	// Use this for initialization
	void Start()
	{
		detonator = GetComponent<Detonator>();
		rb.isKinematic = true;
		spr = GetComponent<SpriteRenderer>();
		Charges = totalCharges;
	}

	void Update()
	{
		if (trigger)
		{
			trigger = false;
			spr.material.SetFloat("_startTime", Time.time);
		}
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		Detonator enemyDetonator = collision.collider.GetComponent<Detonator>();

		//condition added to prevent sparked bombs from using a charge
		if (Charges > 0 && !detonator.sparked && collision.collider.tag == "Enemy" && !enemyDetonator.sparkedLastFrame)
		{
			detonator.sparked = true;
			spr.material.SetFloat("_startTime", Time.time);
			spr.material.SetFloat("_angle", Vector2.Angle(collision.collider.transform.position, Vector2.right) * Mathf.Deg2Rad);
		}
	}

	private void OnExplosion()
	{
		Charges--;
		if(Charges <= 0)
		{
			GameManager.main.EndGame();
		}
	}

	public void Restart()
	{
		Charges = totalCharges;
	}

	public IEnumerator CrackTransition(int level)
	{
		level = Mathf.Clamp(level, 0, 3);

		//Values for visuals pre-transition
		float startCrack = spr.material.GetFloat(crackID);
		float startReach = spr.material.GetFloat(reachID);
		Color startColor = spr.material.GetColor(colorID);

		//Difference between post and pre-transition values
		float crackDelta = crackLevels[level] - startCrack;
		float reachDelta = reachLevels[level] - startReach;
		Color colorDelta = colorLevels[level] - startColor;

		float timer = 0;
		float progression = 0;

		while (timer <= transitionTime)
		{
			timer += Time.deltaTime;
			progression = Mathf.Clamp01(timer / transitionTime);

			spr.material.SetFloat(crackID, startCrack + crackDelta * progression);
			spr.material.SetFloat(reachID, startReach + reachDelta * progression);
			spr.material.SetColor(colorID, startColor + colorDelta * progression);

			yield return null;
		}

		//make sure reach and crack are exactly set to proper values
		spr.material.SetFloat(crackID, crackLevels[level]);
		spr.material.SetFloat(reachID, reachLevels[level]);
	}

}