using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using battleRoyalServer.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PhotonClient : MonoBehaviour, IPhotonPeerListener
{
    public const string CONNECTION = "31.28.27.99:5055";
    //public const string CONNECTION = "localhost:5055";
    private const string APP_NAME = "battleRoyalServer";

    public static PhotonClient _instance;
    public static PhotonClient Instanse
    {
        get { return _instance; }
    } 
    public string CharactedName { get; private set; }

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
            case (byte)OperationCode.Login:
                LoginHandler(operationResponse);
                break;
            case (byte)OperationCode.Move:
                Debug.Log("зхзхзхззхзхз");
                break;
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
                Debug.Log("Connect to server");
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnect to server");
                break;
            case StatusCode.TimeoutDisconnect:
                Debug.Log("TimeoutDisconnect from server");
                break;
            case StatusCode.DisconnectByServer:
                Debug.Log("DisconnectByServer from server");
                break;
            case StatusCode.DisconnectByServerUserLimit:
                Debug.Log("DisconnectByServerUserLimit from server");
                break;
            case StatusCode.DisconnectByServerLogic:
                Debug.Log("DisconnectByServerLogic from server");
                break;
            case StatusCode.Exception:
                Debug.Log("Exeption");
                break;
            default:
                Debug.Log("Unknown status code: " + statusCode);
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
        SceneManager.LoadScene("Garage");
    }

    #region handler for responce
    private void LoginHandler(OperationResponse operationResponse)
    {
        if (operationResponse.OperationCode != 0)
        {
            ErrorCode errorCode = (ErrorCode)operationResponse.ReturnCode;
            switch (errorCode)
            {
                case ErrorCode.NameIsExist:
                    {
                        if (OnLoginResponce != null)
                            OnLoginResponce(this, new LoginEventArgs(ErrorCode.NameIsExist));
                        break;
                    }
                default:
                    {
                        Debug.Log("Error: LoginHandler receive unknown code: " + operationResponse);
                        break;
                    }
                    
            }
            return;
        }
        CharactedName = (string)operationResponse.Parameters[(byte)ParameterCode.CharactedName];
        loadStartScene();
    }

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
        Dictionary<string, object> playerTemplate = (Dictionary<string, object>)eventData.Parameters[(byte)ParameterCode.PlayerTemplate];
        if (OnReceivePlayerTemplate != null)
        {
            OnReceivePlayerTemplate(this, new PlayerTemlateEventArgs(playerTemplate));
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
        Dictionary<string, object> playerTemplate = (Dictionary<string, object>)response.Parameters[(byte)ParameterCode.PlayerTemplate];
        if (OnReceivePlayerTemplate != null)
        {
            OnReceivePlayerTemplate(this, new PlayerTemlateEventArgs(playerTemplate));
        }
    }
    #endregion

    #region Up-level Api
    public void SendLoginOperation(string charactedName = "newName")
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

    public static void PhotonInstantiate(GameObject obj, Transform trans) // ЭТО КОНЕЧНО ЖЕ НАДО НАПИСАТЬ
    {

    }

}
