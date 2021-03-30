using gameServer.Common;
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

    private void CreateLocalPlayer(PlayerTemlateEventArgs localPlayerTemplate) 
    {
        string name = localPlayerTemplate.CharactedName;
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
        player.CharactedName = name;
        LocalPlayer = player;
        return;
    }

    private void CreatePlayer(PlayerTemlateEventArgs playerTemplate)
    {
        string name = playerTemplate.CharactedName;
        if (name == null)
        {
            Debug.Log("Design error: Attempt to create a player without a name");
            //TODO нужен ответ на неудачное создание игрока
            return;
        }
        //TODO Распарсить пришедший шаблон и собрать из него объект
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Player player = obj.AddComponent<Player>();
        player.CharactedName = name;
        Players.Add(name, player);
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
        GameClient.Instanse.RequestLocalPlayerTemplate();
        GameClient.Instanse.GetPlayersTemplate();
        Players = new Dictionary<string, object>();
        //TODO Дать запрос на получение шаблона игрового объекта перенести loadStartScene в соответствующий handler
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnReceivePlayerTemplate(object sender, PlayerTemlateEventArgs player)
    {
        Debug.Log("Connect player:" + player.CharactedName);
        if (player.CharactedName == GameClient.Instanse.Player.Name)
        {
            CreateLocalPlayer(player);
        }
        else 
        {
            CreatePlayer(player);
        }
    }
    ~PlayersPool() 
    {
        GameClient.Instanse.OnReceivePlayerTemplate -= OnReceivePlayerTemplate;
    }
}
