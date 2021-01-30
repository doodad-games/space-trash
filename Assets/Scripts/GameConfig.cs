using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Config/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    static StaticCache _sc;

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
    
    public static IReadOnlyList<string> DeathTrashResourceNames => _i._auto.deathTrashResourceNames;
    public static int NewID => _sc.id++;

#pragma warning disable CS0649
    [SerializeField] AutoPopulated _auto;
#pragma warning restore CS0649

    class StaticCache
    {
        public GameConfig i;
        public int spawnCheckLayer = -1;
        public int id;
    }

    [Serializable]
    struct AutoPopulated
    {
        public string[] deathTrashResourceNames;
    }

#if UNITY_EDITOR
    [MenuItem("Game/Config: Auto Populate")]
    static void AutoPopulateConfig()
    {
        Debug.Log("Config: Auto Populate underway");

        var config = Resources.Load<GameConfig>("GameConfig");

        UpdateDeathTrashResourceNames(config);

        EditorUtility.SetDirty(config);

        Debug.Log("Config: Auto Populate complete");
    }

    static void UpdateDeathTrashResourceNames(GameConfig config)
    {
        var resourcePathReg = new Regex("Assets/Resources/(Content/DeathTrash/.*)\\.prefab");

        config._auto.deathTrashResourceNames = 
            AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Resources/Content/DeathTrash" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(_ => resourcePathReg.Match(_).Groups[1].Value)
                .ToArray();
        
        Debug.Log(string.Format(
            "Found {0} death trash in Assets/Resources/Content/DeathTrash",
            config._auto.deathTrashResourceNames.Length
        ));
    }
#endif
}
