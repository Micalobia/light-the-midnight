using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainHandler : MonoBehaviour
{
    [Header("Platforms")]
    [SerializeField] public SpriteRenderer TrainPlatform;
    [SerializeField] public uint PlatformCount;
    [Header("Train")]
    [SerializeField] public SpriteRenderer Train;
    [SerializeField] public float TrainSpawnTime;
    [SerializeField] public float TrainDeathTime;
    [SerializeField] public float TrainSpeed;
    [SerializeField] public float TrainRideHeight;
    private const float TimeDif = 0.25f;
    private SpriteRenderer[] Platforms;
    private float timeSinceTrainSpawned;
    private SpriteRenderer train;

    private void Reset()
    {
        TrainSpawnTime = 10f;
        TrainSpeed = 10f;
    }

    private void OnValidate()
    {
        PlatformCount = PlatformCount == 0 ? 1 : PlatformCount;
        TrainDeathTime = Mathf.Clamp(TrainDeathTime, TrainDeathTime, TrainSpawnTime - TimeDif);
    }

    private void Start()
    {
        Platforms = new SpriteRenderer[PlatformCount];
        for (int i = 0; i < PlatformCount; i++)
        {
            Platforms[i] = Instantiate(TrainPlatform);
            Platforms[i].transform.SetParentClean(transform);
            float t = (float)i / PlatformCount;
            float d = Platforms[i].bounds.size.x * PlatformCount / 2f;
            Platforms[i].transform.position = new Vector3(Mathf.LerpUnclamped(d, -d, t), Platforms[i].transform.position.y);
        }
        timeSinceTrainSpawned = 0f;
    }

    private void Update()
    {
        timeSinceTrainSpawned += Time.deltaTime;
        if (timeSinceTrainSpawned > TrainDeathTime && timeSinceTrainSpawned < TrainSpawnTime && train != null)
        {
            Destroy(train.gameObject);
        }
        if (timeSinceTrainSpawned > TrainSpawnTime)
        {
            timeSinceTrainSpawned -= TrainSpawnTime;
            train = Instantiate(Train);
            train.transform.SetParentClean(transform);
            train.transform.position = Platforms[0].transform.position;
            Vector3 vec = train.transform.position;
            vec.y += (Platforms[0].bounds.size.y + train.bounds.size.y * TrainRideHeight) / 2f; ;
            train.transform.position = vec;
        }
        if (train != null)
        {
            train.transform.position += Vector3.left * (TrainSpeed * Time.deltaTime * Platforms[0].bounds.size.x);
        }
    }
}
