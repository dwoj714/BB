﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BurnZone : MonoBehaviour, IUpgradeable
{
	public float dpsMin = 10;
	public float dpsMax = 30;

	public CircleCollider2D parentCircle;

	CircleCollider2D col;

	private HashSet<BombController> bombs;

	[Header("Upgrade Scaling")]
	[SerializeField] private float damageScale = 0.15f;
	[SerializeField] private float radiusScale = 0.2f;

	[Header("Upgrade colors")]
	[SerializeField] private Color[] outerColors = new Color[3];
	[SerializeField] private Color[] innerColors = new Color[3];

	private int outerColorID = Shader.PropertyToID("_color1");
	private int innerColorID = Shader.PropertyToID("_color2");

	public int[] UpgradeLevels
	{
		set
		{
			transform.localScale = (Vector2.up + Vector2.right) * (transform.localScale.x * (1 + radiusScale * value[11]));

			dpsMin *= 1 + (value[12] * damageScale);
			dpsMax *= 1 + (value[12] * damageScale);

			SpriteRenderer spr = GetComponent<SpriteRenderer>();
			spr.material.SetColor(outerColorID, outerColors[value[12]]);
			spr.material.SetColor(innerColorID, innerColors[value[12]]);
		}
	}

	// Use this for initialization
	void Start()
	{
		col = GetComponent<CircleCollider2D>();
		bombs = new HashSet<BombController>();
	}

	private void Update()
	{
		foreach(BombController bomb in bombs)
		{
			if (!bomb)
			{
				bombs.Remove(bomb);
			}
			else
			{
				float outer = CircleExtensions.PhysicalRadius(col);
				//Debug.Log(col);
				float damagePercent = CircleExtensions.CircleCloseness(parentCircle, outer, bomb.col);
				float Dps = (dpsMax - dpsMin) * damagePercent + dpsMin;
				//float Dps = DpsMax;

				//Debug.Log(damagePercent * 100 + "% damage: " + Dps);

				bomb.hb.TakeDamage(Dps * Time.deltaTime);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		BombController bomb = collision.GetComponent<BombController>();
		if(bomb != null)
		{
			bombs.Add(bomb);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		BombController bomb = collision.GetComponent<BombController>();
		if (bomb)
		{
			bombs.Remove(bomb);
		}
	}
}
