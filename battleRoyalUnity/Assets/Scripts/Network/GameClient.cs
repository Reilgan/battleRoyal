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
    //public const string CONNECTION = "31.28.27.99:5055";
    public const string CONNECTION = "localhost:5056";
    private const string APP_NAME = "GameServer";

    public static GameClient _instance;
    public static GameClient Instanse
    {
        get { return _instance; }
    } 
    public StructPlayer Player { get; set; }

    public PhotonPeer PhotonPeer { get; set; }
    public event EventHandler<LoginEventArgs> OnLoginResponce;
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
        Connect();
    }

    private void Connect()
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

    #region IPhotonPeerListener
    public void OnOperationResponse(OperationResponse operationResponse)
    {

        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.GetPlayersTemplate:
                GetPlayersTemplateHandler(operationResponse);
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

    public void loadStartScene()
    {
        SceneManager.LoadScene("Game");
    }

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
    public void SendLoginOperation(string charactedName)
    {
        PhotonPeer.OpCustom((byte)OperationCode.Login,
                             new Dictionary<byte, object> { { (byte)ParameterCode.CharactedName, charactedName } }, true);
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

    public void RequestLocalPlayerTemplate()
    {
        PhotonPeer.OpCustom((byte)OperationCode.GetLocalPlayerTemplate, new Dictionary<byte, object>(), true);
    }

    public void SendMovingToServer(Dictionary<byte, object> parametrs)
    {
        PhotonPeer.OpCustom((byte)OperationCode.Move,
                             parametrs,
                             false);
    }

    public void GetPlayersTemplate()
    {
        PhotonPeer.OpCustom((byte)OperationCode.GetPlayersTemplate, new Dictionary<byte, object>(), true);
    }

    #endregion

}
