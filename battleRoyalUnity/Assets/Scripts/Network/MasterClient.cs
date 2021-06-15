using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using gameServer.Common;

public class MasterClient : MonoBehaviour, IPhotonPeerListener
{
    //public const string CONNECTION = "31.28.27.99:5055";
    public const string CONNECTION = "localhost:5055";
    private const string APP_NAME = "MasterServer";

    public static MasterClient _instance;
    public static MasterClient Instanse
    {
        get { return _instance; }
    }

    public PhotonPeer PhotonPeer { get; set; }
    
    void Awake()
    {
        if (Instanse != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        _instance = this;
        new SinglePlayerStruct();

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

    void OnApplicationQuit() {
        Disconnect();
    }


    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Master DebugRetutn: " + message);
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.Login:
                {
                    LoginHandler(operationResponse);
                    break;
                }
            case (byte)OperationCode.GetGameServerIP:
                {
                    GetGameServerIPHandler(operationResponse);
                    break;
                }
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("Connect to master server");
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnect to master server");
                break;
            case StatusCode.TimeoutDisconnect:
                Debug.Log("TimeoutDisconnect from master server");
                break;
            case StatusCode.DisconnectByServer:
                Debug.Log("DisconnectByServer from master server");
                break;
            case StatusCode.DisconnectByServerUserLimit:
                Debug.Log("DisconnectByServerUserLimit from master server");
                break;
            case StatusCode.DisconnectByServerLogic:
                Debug.Log("DisconnectByServerLogic from master server");
                break;
            case StatusCode.Exception:
                Debug.Log("Exeption master Server");
                break;
            default:
                Debug.Log("Unknown status code master server: " + statusCode);
                break;
        }
    }

    public void OnEvent(EventData eventData)
    {
        throw new NotImplementedException();
    }


    public void SendLoginOperation(string charactedName)
    {
        PhotonPeer.OpCustom((byte)OperationCode.Login,
                             new Dictionary<byte, object> { { (byte)ParameterCode.CharactedName, charactedName } }, 
                             true);
    }

    public void GetGameServerIP()
    {
        PhotonPeer.OpCustom((byte)OperationCode.GetGameServerIP,
                             new Dictionary<byte, object> {}, true);
    }

    #region handler for responce
    private void LoginHandler(OperationResponse operationResponse)
    {
        if (operationResponse.OperationCode != (byte)OperationCode.Login)
        {
            ErrorCode errorCode = (ErrorCode)operationResponse.ReturnCode;
            switch (errorCode)
            {
                case ErrorCode.NameIsExist:
                    {
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
        SinglePlayerStruct.Instanse.DeserializationPlayerFromDict(operationResponse.Parameters);
        Dictionary<byte, object> parms = (Dictionary<byte, object>)operationResponse.Parameters[(byte)ParameterCode.PlayerInfo];
        SceneManager.LoadScene("Garage");
    }

    private void GetGameServerIPHandler(OperationResponse operationResponse)
    {
        string CONNECTION = (string)operationResponse.Parameters[(byte)ParameterCode.GameServerId];
        Debug.Log("GameServerIP: " + CONNECTION);
        //Подлючение к GameServer
        GameClient.Instanse.Connect(CONNECTION);
    }


    #endregion
}
