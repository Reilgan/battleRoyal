using battleRoyalServer.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersPool : MonoBehaviour
{
    public static PlayersPool _instance;
    public static PlayersPool Instanse
    {
        get { return _instance; }
    }
    public Dictionary<string, object> Players { get; private set; }
    public Player LocalPlayer { get; private set; }
    public Prefabs prefab;
    private void CreateLocalPlayer(Dictionary<string, object> objectTemplate) 
    {
        Dictionary<string, object> playerTemplate = (Dictionary<string, object>)objectTemplate;
        string name = playerTemplate.ElementAt(0).Key;
        if (name == null)
        {
            Debug.Log("Design error: Attempt to create a player without a name");
            return;
        }
        if (playerTemplate[name] == null)
        {
            prefab = GameObject.Find("Prefabs").GetComponent<Prefabs>();
            GameObject obj = Instantiate(prefab.prefab[0], new Vector3(0, 0, 0), Quaternion.identity);
            //Instantiate(prefab[0], new Vector3(0, 0, 0), Quaternion.identity);
            obj.name = "LocalObject";
            Player player = obj.AddComponent<Player>();
            player.CharactedName = name;
            LocalPlayer = player;
            return;
        }
        //TODO Распарсить пришедший словарь и собрать из него объект
    }

    private void CreatePlayer(Dictionary<string, object> objectTemplate)
    {
        Dictionary<string, object> playerTemplate = (Dictionary<string, object>)objectTemplate;
        string name = playerTemplate.ElementAt(0).Key;
        if (name == null)
        {
            Debug.Log("Design error: Attempt to create a player without a name");
            return;
        }
        if (playerTemplate[name] == null)
        {

            //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab = GameObject.Find("Prefabs").GetComponent<Prefabs>();
            GameObject obj = Instantiate(prefab.prefab[0], new Vector3(0, 0, 0), Quaternion.identity);
            Player player = obj.AddComponent<Player>();
            player.CharactedName = name;
            Players.Add(name, player);
            return;
        }
        //TODO Распарсить пришедший словарь и собрать из него объект
    }

    // Start is called before the first frame update

    void AWake() 
    {
        if (Instanse != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        _instance = this;
    }

    void Start()
    {
        PhotonClient.Instanse.OnReceivePlayerTemplate += OnReceivePlayerTemplate;
        PhotonClient.Instanse.RequestLocalPlayerTemplate();
        PhotonClient.Instanse.GetPlayersTemplate();
        Players = new Dictionary<string, object>();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnReceivePlayerTemplate(object sender, PlayerTemlateEventArgs e)
    {
        Dictionary<string, object> palyerTemplate = (Dictionary<string, object>)e.PlayerTemplate;
        string name = palyerTemplate.ElementAt(0).Key;
        Debug.Log("Connect player:" + name);
        if (name == PhotonClient.Instanse.CharactedName)
        {
            CreateLocalPlayer(palyerTemplate);
        }
        else 
        {
            CreatePlayer(palyerTemplate);
        }
    }
    ~PlayersPool() 
    {
        PhotonClient.Instanse.OnReceivePlayerTemplate -= OnReceivePlayerTemplate;
    }
}
