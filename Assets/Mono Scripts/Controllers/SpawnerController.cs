using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {

	[SerializeField] private VariableBomb blankPrefab;

	//bombs spawned per second
	public float spawnRate = 0.6f;
	public float rngPower = 2f;
	public float avgStat;
	private float timer = 0;

	private float spawnCount = 0;
	private float valTotal = 0;

	public readonly float statMean;
	public float targerStatMean = 0.2f;

	[SerializeField] private GameObject[] bombEnhancers;

	[Header("Valid ranges: min, max, and application chance")]
	[SerializeField] private Vector3[] ranges;

	

	private void Update()
	{
		//if it's time to do stuff...
		if (TimeCheck())
		{
			SpawnBomb();
			avgStat = valTotal / spawnCount;
		}
	}

	private bool TimeCheck()
	{
		timer += Time.deltaTime;
		if(spawnRate > 0 && timer >= 1 / spawnRate)
		{
			timer = 0;
			return true;
		}
		return false;
	}

	private void SpawnBomb()
	{
		VariableBomb bomb = Instantiate(blankPrefab).GetComponent<VariableBomb>();

		//statVal determines the bomb's size, mass, and explosion
		//rngPower is used to curve the result toward the lower end
		float statVal = Mathf.Pow(Random.Range(0f, 1f), rngPower);
		bomb.SetStats(statVal);

		//determine the horizontal spawn position of the bomb
		float rng = Random.Range(-0.5f, 0.5f);
		float range = 2 * Camera.main.orthographicSize * Camera.main.aspect - 2 * bomb.AdjustedRadius;
		float xPos = rng * range;
		Vector2 spawnPosition = Vector2.up * transform.position.y + Vector2.right * xPos;
		bomb.transform.position = spawnPosition;

		spawnCount++;
		valTotal += statVal;


		//Debug.Log("Start enhancement selection ------------------------------------");

#pragma warning disable 0219
		GameObject obj;
#pragma warning restore 0219

		//determine which enhancements can be applied to the bomb given its stat value
		bool[] isValid = new bool[bombEnhancers.Length];
		for (int i = 0; i < bombEnhancers.Length; i++)
		{
			if (statVal >= ranges[i].x && statVal <= ranges[i].y)
			{
				isValid[i] = true;
			}
			else isValid[i] = false;
		}

		//Debug.Log("part1 done");

		//randomly decide if each enhancement will be added. If multiple are chosen, all but one will be removed from consideration in the next steps.
		int pickCount = 0;
		float[] pickVals = new float[isValid.Length];
		for(int i = 0; i < pickVals.Length; i++)
		{
			//if the generated number is <= the pick chance, it generates a pickVal, which is within the elligibility range (x and y) of the enhancement
			if (isValid[i] && Random.Range(0f, 1f) < ranges[i].z)
			{
				pickVals[i] = Random.Range(ranges[i].x, ranges[i].y);
				pickCount++;
				//Debug.Log("pickVals - Adding " + pickVals[i]);
			}
			else pickVals[i] = -1f;
		}

		//if nothing is selected, nothing is added. End here.
		if (pickCount == 0)
		{
			//Debug.Log("Bomb not elligible for enhancements -- Done");
			return;
		}
		//if there's only one selection, ignore the rest of the function. Add it and end.
		if(pickCount == 1)
		{
			for(int i = 0; i < isValid.Length; i++)
			{
				if (pickVals[i] >= 0)
				{
					obj = Instantiate(bombEnhancers[i], bomb.transform);
					return;
				}
			}
		}

	//	Debug.Log("part2 done");

		//get the largest pickVal
		float max = -1f;
		foreach(float val in pickVals)
		{
			if (val > max) max = val;
		}

	//	Debug.Log("part3 done");

		int chosenIDX = -1;
		//Set chosenIDX to IndexOf max
		for (int i = 0; i < pickVals.Length; i++)
		{
			if (pickVals[i] == max)
			{
				chosenIDX = i;
				break;
			}
		}

		//	Debug.Log("MAX: " + max + " Chosen IDX: " + chosenIDX);
		obj = Instantiate(bombEnhancers[chosenIDX], bomb.transform);
	}

	public void OnGameStart()
	{
		//set timer to 80% of the time needed to spawn a bomb so there 
		//isn't a long wait between game start and bomb appearance
		timer = 0.8f * (1 / spawnRate);

		spawnCount = 0;
		valTotal = 0;
	}

}
