using System.Collections.Generic;
using MyLibrary;
using UnityEngine;

public static class Visuals
{
    public static void Spawn(Vector3 point, string bucketName, int numVisuals = 1)
    {
        var bucket = GameConfig.ResourceBuckets[bucketName];
        IReadOnlyList<string> visuals;

        if (numVisuals > bucket.Count)
            visuals = bucket;
        else
            visuals = bucket.PickRandom(numVisuals);

        for (var i = 0; i != numVisuals; ++i)
        {
            var visual = visuals[i % visuals.Count];
            var prefab = Resources.Load<GameObject>(visual);

            var spawnPoint = point;
            if (i != 0)
                spawnPoint +=
                    (Vector3)UnityEngine.Random.insideUnitCircle * 2;

            new Async(Player.I)
                .Wait(0.1f * i)
                .Then(() => GameObject.Instantiate(prefab, spawnPoint, Quaternion.identity));
        }
    }
}
