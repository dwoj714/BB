using System;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController main;
    public SpawnerController spawner;

    private int waveNumber = 0;

    //per wave
    private int bombsSpawned, bombsToSpawn, bombsDetonated;

    public delegate void WaveEvent(object source, WaveEventArgs args);
    public static WaveEvent waveEvent;

    public bool WaveInProgress
	{
        get; private set;
	}

    // Start is called before the first frame update
    void Start()
    {
        if (!main) main = this;
        else Debug.LogWarning("More than one GameplayController instantiated!");

        spawner = GameObject.Find("Spawner").GetComponent<SpawnerController>();

        BombController.BombDetonated += OnBombDetonated;
        GameManager.GameStarted += OnGameStarted;
        spawner.BombSpawned += OnBombSpawned;

    }

    void OnGameStarted(object source, EventArgs args)
	{
        waveNumber = 0;
        BeginWave();
	}

	public void BeginWave()
	{
        Debug.Log("Beginning Wave...");
        waveNumber++;
        bombsDetonated = 0;
        bombsSpawned = 0;
        bombsToSpawn = 15;
        spawner.Spawning = true;
        WaveInProgress = true;

        waveEvent?.Invoke(this, new WaveEventArgs(true));

	}
    public void EndWave()
	{
        spawner.Spawning = false;
        WaveInProgress = false;
        waveEvent?.Invoke(this, new WaveEventArgs(false));
    }

    void OnBombDetonated(object source, EventArgs args)
	{
        //Increment the bomb detonation counter, end the wave if all bombs are detonated
        bombsDetonated++;
        if(bombsDetonated >= bombsToSpawn)
		{
            EndWave();
		}
	}

    void OnBombSpawned(object source, EventArgs args)
	{
        //Increment the bomb spawn counter, disable the spawner if we've hit the spawn limit for the wave
        bombsSpawned++;
        if(bombsSpawned >= bombsToSpawn)
		{
            spawner.Spawning = false;
            Debug.Log("Spawner Disabled");
		}
	}

    public float WaveProgress()
	{
        return ((float)bombsDetonated) / bombsToSpawn;
	}

	/*
     * Functions this class will need to do
     * 
     * start games
     * start waves
     * keep track of bombs spawned/detonated
     * enable loadout switching between waves
     * enable upgrades/a shop between waves
     * apply modifiers to waves
     * manage difficulty
     * 
     * */
}

public class WaveEventArgs : EventArgs
{
	public WaveEventArgs(bool waveStart)
	{
        this.waveStart = waveStart;
	}

    //true if wave is starting, false otherwise
    public bool waveStart;

}
