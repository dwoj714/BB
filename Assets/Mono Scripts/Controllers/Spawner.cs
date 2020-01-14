using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public float delay;

	private int spawnCount = 0;

	public int SpawnCount
	{
		get
		{
			return spawnCount;
		}
	}

	public bool useChain;
	[SerializeField]
	private DropChain chain;
	[SerializeField]
	private DropPool pool;

	//Holds randomly chosen GameObjects that don't have a free space to spawn in
	private GameObject hold;
	private float holdRad;


    float clock;

	public DropChain Chain
	{
		get
		{
			return chain;
		}

		set
		{
			useChain = true;
			chain = value;
		}
	}
	public DropPool Pool
	{
		get
		{
			return pool;
		}

		set
		{
			useChain = false;
			pool = value;
		}
	}

	public void OnGameStart()
	{
		spawnCount = 0;
		clock = delay/4;
	}

	void Update () {

		clock -= Time.deltaTime;

		if(clock < 0)
		{
			clock = delay;
			SpawnObj();
		}
	}

	void SpawnObj()
	{
		spawnCount++;

		Vector3 pos = transform.position;
		pos.x = transform.position.x + (Random.value -0.5f) * transform.localScale.x;

		if (!hold)
		{
			if (useChain)
			{
				hold = chain.SpawnRandom(pos);
			}
			else
			{
				hold = pool.SpawnRandom(pos);
			}
		}

		Collider2D[] hits = new Collider2D[0];
		BombController bomb = hold.GetComponent<BombController>();

		if (bomb)
		{

			hits = Physics2D.OverlapCircleAll(pos, holdRad);
		}

		//If the OverlapCircle detects hits, wait delay/4 bafore attempting to spawn again
		if (hits.Length > 0)
		{
			hold.SetActive(false);
			clock = delay / 4;
		}
		else
		{
			hold.SetActive(true);
			hold = null;
		}

	}
}
