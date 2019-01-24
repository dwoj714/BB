using UnityEngine;

public interface IRandomList
{
	GameObject GetObject();
	GameObject SpawnRandom(Vector3 position);
	bool IsDepleted
	{
		get;
	}
}
