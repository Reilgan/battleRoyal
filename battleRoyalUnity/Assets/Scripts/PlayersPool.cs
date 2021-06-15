using gameServer.Common;
using System;
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
    public Dictionary<int, Player> Players { get; private set; }
    public Player LocalPlayer { get; private set; }

    public Player getById(int id) 
    {
        return Players[id];
    }

    private void CreateLocalPlayer(PlayerTemlateEventArgs localPlayerTemplate) 
    {
        int id = localPlayerTemplate.Id;
        string name = localPlayerTemplate.CharactedName;
        Dictionary<byte, object> playerInfo = localPlayerTemplate.PlayerInfo;
        if (name == null)
        {
            Debug.Log("Design error: Attempt to create a player without a name");
            //TODO нужен ответ на неудачное создание локального игрока
            return;
        }
        //TODO Распарсить пришедший шаблон и собрать из него объект
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "LocalObject";
        Player player = obj.AddComponent<Player>();
        player.Id = id;
        player.Name = name;
        player.PlayerInfo = playerInfo;
        LocalPlayer = player;
        return;
    }

    private void CreatePlayer(PlayerTemlateEventArgs playerTemplate)
    {
        int id = playerTemplate.Id;
        string name = playerTemplate.CharactedName;
        Dictionary<byte, object> playerInfo = playerTemplate.PlayerInfo;
        if (name == null)
        {
            Debug.Log("Design error: Attempt to create a player without a name");
            //TODO нужен ответ на неудачное создание игрока
            return;
        }
        //TODO Распарсить пришедший шаблон и собрать из него объект
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Player player = obj.AddComponent<Player>();
        player.Id = id;
        player.Name = name;
        player.PlayerInfo = playerInfo;
        Players.Add(id, player);
        return;
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
        GameClient.Instanse.OnReceivePlayerTemplate += OnReceivePlayerTemplate;
        GameClient.Instanse.GetPlayersTemplate();
        Players = new Dictionary<int, Player>();
        //TODO Дать запрос на получение шаблона игрового объекта перенести loadStartScene в соответствующий handler
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnReceivePlayerTemplate(object sender, PlayerTemlateEventArgs player)
    {
        if (player.Id == SinglePlayerStruct.Instanse.Id)
        {
            CreateLocalPlayer(player);
        }
        else 
        {
            Debug.Log("Connect player: " + player.CharactedName);
            CreatePlayer(player);
        }
    }
    ~PlayersPool() 
    {
        GameClient.Instanse.OnReceivePlayerTemplate -= OnReceivePlayerTemplate;
    }

    internal void Exit(int id)
    {
        Player player = getById(id);
        Destroy(player.gameObject);
        Players.Remove(id);
    }
}
