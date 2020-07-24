using UnityEngine.Networking;
using UnityEngine;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using SharedStatsDisplay;
using Newtonsoft.Json;

class Networking
{
    //Static references so we do not need to do tricky things with passing references.
    internal static GameObject CentralNetworkObject;
    private static GameObject _centralNetworkObjectSpawned;

    public void Init()
    {
        // Create empty GameObject to hold all networking components, then do a couple things with it.
        // 1) Add the networkidentity so that Unity knows which Object it's going to be networking all about.
        // 2) we use InstantiateClone from the PrefabAPI to make sure we have full control over our GameObject.  
        var tmpGo = new GameObject("tmpGo");

        tmpGo.AddComponent<NetworkIdentity>();
        CentralNetworkObject = tmpGo.InstantiateClone("somethingUnique");
        GameObject.Destroy(tmpGo);

        //Finally, we add a specific component that we want networked.
        CentralNetworkObject.AddComponent<StatsUpdateNetworkComponent>();
        Debug.unityLogger.logEnabled = false;
    }

    public static void ServerSendStatsUpdate(StatsUpdateList updateList)
    {
        // Before we can Invoke our NetworkMessage, we need to make sure our centralized network object is spawned.
        // For doing that, we Instantiate the CentralNetworkObject, we obviously check if we don't already have one that is already instantiated and activated in the current scene.
        // Note : Make sure you instantiate the gameobject, and not spawn it directly, it would get deleted otherwise on scene change, even with DontDestroyOnLoad.

        if (!_centralNetworkObjectSpawned)
        {
            Debug.Log("Creating centralized network object");
            _centralNetworkObjectSpawned = GameObject.Instantiate(CentralNetworkObject);
            NetworkServer.Spawn(_centralNetworkObjectSpawned);
        }

        Debug.Log("Server sending stats to all clients: " + updateList.BuildUpdateStringOneLine());

        foreach (NetworkUser user in NetworkUser.readOnlyInstancesList)
        {
            StatsUpdateNetworkComponent.Invoke(user, updateList);
        }
    }
}

//Important to note that these NetworkBehaviour classes must not be nested for UNetWeaver to find them.
internal class StatsUpdateNetworkComponent: NetworkBehaviour
{
    // We only ever have one instance of the networked behaviour here.
    private static StatsUpdateNetworkComponent _instance;
    public static StatsUpdateList updateListCache = new StatsUpdateList();
    private void Awake()
    {
        Debug.Log("StatsUpdateNetworkComponent Awake");
        _instance = this;
    }
    public static void Invoke(NetworkUser user, StatsUpdateList updateList)
    {
        string json = JsonConvert.SerializeObject(updateList, Formatting.Indented);
        _instance.TargetReceiveUpdate(user.connectionToClient, json);
    }

    // While we can't find the entirety of the Unity Script API in here, we can provide links to them.
    // This attribute is explained here: https://docs.unity3d.com/2017.3/Documentation/ScriptReference/Networking.TargetRpcAttribute.html
    [TargetRpc]
    private void TargetReceiveUpdate(NetworkConnection target, string updateJson)
    {
        if (NetworkServer.active) return;
        StatsUpdateList statsList = JsonConvert.DeserializeObject<StatsUpdateList>(updateJson);
        updateListCache.Clear();

        Debug.Log("Received StatsUpdateList from server: " + statsList.BuildUpdateStringOneLine());
        Debug.Log("Update json from server: " + updateJson);
        foreach (StatsUpdate update in statsList.statsUpdateList)
        { 
            updateListCache.Add(update);
        }
    }
}
