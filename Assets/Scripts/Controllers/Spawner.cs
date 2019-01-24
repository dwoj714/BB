using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public float frequency;

	public bool useChain;
	[SerializeField]
	private DropChain chain;
	[SerializeField]
	private DropPool pool;

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

	void FixedUpdate () {

		clock -= Time.deltaTime;

		if(clock < 0)
		{
			SpawnObj();
			clock = frequency;
		}
	}

	void SpawnObj()
	{
		Vector3 pos = transform.position;
		pos.x = transform.position.x + (Random.value -0.5f) * transform.localScale.x;

		if (useChain)
		{
			chain.SpawnRandom(pos);
		}
		else
		{
			pool.SpawnRandom(pos);
		}

	}
}
