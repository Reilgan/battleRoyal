using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using gameServer.Common;

public class GameClient : MonoBehaviour, IPhotonPeerListener
{
    private const string APP_NAME = "GameServer";

    public static GameClient _instance;
    public static GameClient Instanse
    {
        get { return _instance; }
    } 

    public PhotonPeer PhotonPeer { get; set; }
    public event EventHandler<ChatMessageEventArgs> OnReceiveChatMessage;
    public event EventHandler<PlayerTemlateEventArgs> OnReceivePlayerTemplate;
    public event EventHandler<MoveEventArgs> onReceiveMoveEventArgs;

    void Awake()
    {
        if (Instanse != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        _instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
    }

    public void Connect(string CONNECTION)
    {
        if (PhotonPeer != null)
        {
            PhotonPeer.Connect(CONNECTION, APP_NAME);

        }
    }

    private void Disconnect()
    {
        if (PhotonPeer != null) 
        {
            PhotonPeer.Disconnect();
        }
    }

    void FixedUpdate()
    {
        PhotonPeer.Service();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        ExitFromGameServer();
    }

    #region IPhotonPeerListener
    public void OnOperationResponse(OperationResponse operationResponse)
    {

        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.GetPlayersInRoom:
                GetPlayersTemplateHandler(operationResponse);
                break;
            case (byte)OperationCode.EnterInGameServer:
                FindRoom();
                break;
            default:
                Debug.Log("Unknown OperationResponse:" + operationResponse.OperationCode);
                break;
        }
    }
    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("Connect to game server");
                EnterInGameServer(SinglePlayerStruct.Instanse);
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnect to game server");
                break;
            case StatusCode.TimeoutDisconnect:
                Debug.Log("TimeoutDisconnect from game server");
                break;
            case StatusCode.DisconnectByServer:
                Debug.Log("DisconnectByServer from game server");
                break;
            case StatusCode.DisconnectByServerUserLimit:
                Debug.Log("DisconnectByServerUserLimit from game server");
                break;
            case StatusCode.DisconnectByServerLogic:
                Debug.Log("DisconnectByServerLogic from game server");
                break;
            case StatusCode.Exception:
                Debug.Log("Exeption game Server");
                break;
            default:
                Debug.Log("Unknown status code game server: " + statusCode);
                break;
        }
    }

    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.ChatMessage:
                ChatMessageHandler(eventData);
                break;
            case (byte)EventCode.PlayerTemplate:
                PlayerTemplateHandler(eventData); 
                break;
            case (byte)EventCode.Move:
                moveEventHandler(eventData);
                break;
            case (byte)EventCode.RoomReady:
                RoomReadyHandler(eventData);
                break;
            case (byte)EventCode.PlayerExitFromGameServer:
                PlayerExitFromGameServerHandler(eventData);
                break;
            default:
                Debug.Log("Unknown event: " + eventData.Code);
                break;
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        return;
    }
    #endregion


    #region handler for event
    private void RoomReadyHandler(EventData eventData)
    {
        Debug.Log("Enter in Room with id: " + eventData.Parameters[(byte)ParameterCode.IdRoom]);
        SceneManager.LoadScene("Game");
    }

    private void PlayerExitFromGameServerHandler(EventData eventData)
    {
        int id = (int)eventData.Parameters[(byte)ParameterCode.Id];
        PlayersPool.Instanse.Exit(id);
        if (id == SinglePlayerStruct.Instanse.Id) {
            Disconnect();
        }
    }

    #endregion

    #region handler for responce
    private void ChatMessageHandler(EventData eventData)
    {
        string massege = (string)eventData.Parameters[(byte)ParameterCode.ChatMessage];

        if (OnReceiveChatMessage != null) 
        {
            OnReceiveChatMessage(this, new ChatMessageEventArgs(massege));
        }
    }

    private void PlayerTemplateHandler(EventData eventData)
    {
        if (OnReceivePlayerTemplate != null)
        {
            OnReceivePlayerTemplate(this, new PlayerTemlateEventArgs(eventData.Parameters));
        }
    }

    private void moveEventHandler(EventData eventData)
    {
        if (onReceiveMoveEventArgs != null)
        {
            Dictionary<byte, object> parametrs = new Dictionary<byte, object>();
            parametrs = eventData.Parameters;
            onReceiveMoveEventArgs(this, new MoveEventArgs(parametrs));
        }
    }

    private void GetPlayersTemplateHandler(OperationResponse response)
    {

        if (OnReceivePlayerTemplate != null)
        {
            OnReceivePlayerTemplate(this, new PlayerTemlateEventArgs(response.Parameters));
        }
    }
    #endregion

    #region Up-level Api
    private void FindRoom()
    {
        Dictionary<byte, object> param = new Dictionary<byte, object>();
        param.Add((byte)ParameterCode.Id, SinglePlayerStruct.Instanse.Id);
        param.Add((byte)ParameterCode.RoomType, RoomTypeCode.Room_1x1);
        PhotonPeer.OpCustom((byte)OperationCode.FindRoom,
                            param,
                            true);
    }

    public void EnterInGameServer(SinglePlayerStruct player)
    {
        PhotonPeer.OpCustom((byte)OperationCode.EnterInGameServer,
                                player.SerializationPlayerToDict(),
                                true);
     
    }

    public void SendChatMessage(string message)
    {
        PhotonPeer.OpCustom((byte)OperationCode.SendChatMessage,
                             new Dictionary<byte, object> { { (byte)ParameterCode.ChatMessage, message } }, true);
    }

    public void GetRecentChatMessage()
    {
        PhotonPeer.OpCustom((byte)OperationCode.GetRecentChatMessages, new Dictionary<byte, object>(), true);
    }

    public void SendMovingToServer(Dictionary<byte, object> parametrs)
    {
        PhotonPeer.OpCustom((byte)OperationCode.Move,
                             parametrs,
                             false);
    }

    public void GetPlayersTemplate()
    {
        PhotonPeer.OpCustom((byte)OperationCode.GetPlayersInRoom, new Dictionary<byte, object>(), true);
    }

    public void ExitFromGameServer() 
    {
        PhotonPeer.OpCustom((byte)OperationCode.ExitFromGameServer, new Dictionary<byte, object>(), true);
    }
    #endregion

}
