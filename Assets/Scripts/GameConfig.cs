using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Config/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    const float MUSIC_VOLUME_ON = -10f;
    const float MUSIC_VOLUME_OFF = -80f;

    static StaticCache _sc = new StaticCache();

    static GameConfig _i
    {
        get
        {
            if (_sc.i == null)
                _sc.i = Resources.Load<GameConfig>("GameConfig");
            return _sc.i;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void HandleDomainReload()
    {
        _sc = new StaticCache();

        Application.targetFrameRate = 144;
    }
    
    public static int SpawnCheckLayer
    {
        get
        {
            if (_sc.spawnCheckLayer == -1)
                _sc.spawnCheckLayer = LayerMask.NameToLayer("DeathTrashSpawnCheck");
            return _sc.spawnCheckLayer;
        }
    }

    public static void FadeMusicVolume(bool on)
    {
        float initialVolume;
        GameConfig.Mixer.GetFloat("MusicVolume", out initialVolume);

        var target = on ? MUSIC_VOLUME_ON : MUSIC_VOLUME_OFF;
        var targetChange = target - initialVolume;

        new Async()
            .Lerp(
                1f,
                (step) => GameConfig.Mixer.SetFloat(
                    "MusicVolume",
                    initialVolume + targetChange * step
                ),
                TimeMode.Unscaled
            );
    }
    
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> ResourceBuckets
    {
        get
        {
            if (_sc.resourceBuckets == null)
            {
                var dict = new Dictionary<string, List<string>>();
                foreach (var resource in _i._auto.resourceBucketItems)
                {
                    if (!dict.ContainsKey(resource.type))
                        dict[resource.type] = new List<string>();
                    dict[resource.type].Add(resource.path);
                }

                var roDict = new Dictionary<string, IReadOnlyList<string>>();
                foreach (var pair in dict)
                    roDict[pair.Key] = pair.Value;

                _sc.resourceBuckets = roDict;
            }

            return _sc.resourceBuckets;
        }
    }

    public static int NewID => _sc.id++;
    public static AudioMixer Mixer => _i._masterMixer;

#pragma warning disable CS0649
    [SerializeField] AutoPopulated _auto;
    [SerializeField] AudioMixer _masterMixer;
#pragma warning restore CS0649

    class StaticCache
    {
        public GameConfig i;
        public int spawnCheckLayer = -1;
        public int id;
        public Dictionary<string, IReadOnlyList<string>> resourceBuckets;
    }

    [Serializable]
    struct AutoPopulated
    {
        public ResourceBucketItem[] resourceBucketItems;

        [Serializable]
        public struct ResourceBucketItem
        {
            public string type;
            public string path;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Game/Config: Auto Populate")]
    static void AutoPopulateConfig()
    {
        Debug.Log("Config: Auto Populate underway");

        var config = Resources.Load<GameConfig>("GameConfig");

        UpdateResourceBucketItems(config);

        EditorUtility.SetDirty(config);

        Debug.Log("Config: Auto Populate complete");
    }

    static void UpdateResourceBucketItems(GameConfig config)
    {
        var resourcePathReg = new Regex("Assets/Resources/(Content/Buckets/([^/]+)/.*)\\.prefab");

        config._auto.resourceBucketItems = 
            AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Resources/Content/Buckets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(_ =>
                {
                    var regMatch = resourcePathReg.Match(_);
                    return new AutoPopulated.ResourceBucketItem
                    {
                        type = regMatch.Groups[2].Value,
                        path = regMatch.Groups[1].Value
                    };
                })
                .ToArray();
        
        Debug.Log(string.Format(
            "Found {0} resources in Assets/Resources/Content/Buckets",
            config._auto.resourceBucketItems.Length
        ));
    }
#endif
}
