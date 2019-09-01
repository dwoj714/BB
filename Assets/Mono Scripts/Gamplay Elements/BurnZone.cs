using System.Collections;
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

	public void SetUpgrades(int[] upgradeLevels)
	{
		transform.localScale = (Vector2.up + Vector2.right) * (transform.localScale.x * (1 + .25f * upgradeLevels[4]));
	}

}
