using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public float frequency;

	DropPool pool;
    float clock;

	void Start()
	{
		pool = GetComponent<DropPool>();
	}

    void FixedUpdate () {

		clock -= Time.deltaTime;

		if(clock < 0)
		{
			OnClockTimeout();
			clock = frequency;
		}
	}

	void OnClockTimeout()
	{
		Vector3 pos = transform.position;
		pos.x = transform.position.x + (Random.value -0.5f) * transform.localScale.x;
		Instantiate(pool.GetObject(), pos, Quaternion.identity);
	}
}
