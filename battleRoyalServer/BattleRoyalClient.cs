using ExitGames.Logging;
using System.Collections.Generic;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using battleRoyalServer.Common;
using battleRoyalServer.Operations;
using System.Linq;

namespace battleRoyalServer
{
    public class BattleRoyalClient : ClientPeer
    {
        private readonly ILogger log = LogManager.GetCurrentClassLogger();

        public string CharactedName { get; private set; }

        public BattleRoyalClient(InitRequest initRequest) : base(initRequest)
        {
            log.Debug("Player connection ip: " + initRequest.RemoteIP);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            ClientsPool.Instance.RemoveClient(this);
            log.Debug("Disconnected: " + CharactedName);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case (byte)OperationCode.Login:
                    {
                        LoginHandler(operationRequest, sendParameters);
                        break;
                    }

                case (byte)OperationCode.SendChatMessage:
                    {
                        var chatRequest = new ChatMessage(Protocol, operationRequest);

                        if (!chatRequest.IsValid)
                        {
                            SendOperationResponse(chatRequest.GetResponse(ErrorCode.InvalidParametrs), sendParameters);
                            return;
                        }

                        string message = chatRequest.Message;

                        string[] param = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (param.Length == 2)
                        {
                            string targetName = param[0];
                            message = param[1];

                            if (ClientsPool.Instance.IsContain(targetName))
                            {
                                var targetClient = ClientsPool.Instance.getByName(targetName);
                                if (targetClient == null)
                                {
                                    return;
                                }

                                message = CharactedName + "[PM]: " + message;

                                // Создание event для отправителя и получателя
                                var personalEventData = new EventData((byte)EventCode.ChatMessage);
                                personalEventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
                                // Отправка eventa с заданными получателями
                                personalEventData.SendTo(new BattleRoyalClient[] { this, targetClient }, sendParameters);
                                return;
                            }
                        }

                        message = CharactedName + ": " + message;
                        Chat.Instance.AddMessage(message);
                        // Создание event для каждого client
                        var eventData = new EventData((byte)EventCode.ChatMessage);
                        eventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
                        //SendEvent(eventData, sendParameters);
                        eventData.SendTo(ClientsPool.Instance.Clients, sendParameters);
                        break;
                    }

                case (byte)OperationCode.GetRecentChatMessages:
                    {
                        // Отправка истории сообщений
                        // Создание event для каждого client
                        string allMessages = Chat.Instance.GetRecentMessages().Aggregate((i, j) => i + "\r\n" + j);
                        var allMessagesEventData = new EventData((byte)EventCode.ChatMessage);
                        allMessagesEventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, allMessages } };
                        allMessagesEventData.SendTo(new BattleRoyalClient[] { this }, sendParameters);
                        break;
                    }

                case (byte)OperationCode.GetLocalPlayerTemplate:
                    {
                        var eventDataPlayerTemp = new EventData((byte)EventCode.PlayerTemplate);
                        Dictionary<string, object> template = new Dictionary<string, object>();
                        template.Add(CharactedName, null);
                        eventDataPlayerTemp.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.PlayerTemplate, template } };
                        eventDataPlayerTemp.SendTo(ClientsPool.Instance.Clients, sendParameters);
                        break;
                    }

                case (byte)OperationCode.Move:
                    {
                        var moveRequest = new Move(Protocol, operationRequest);
                        var eventDataMove = new EventData((byte)EventCode.Move);
                        eventDataMove.Parameters = moveRequest.GetParametersForEvent();
                        List<BattleRoyalClient> clientsForEven = new List<BattleRoyalClient>();
                        clientsForEven.AddRange(ClientsPool.Instance.Clients);
                        clientsForEven.Remove(this);
                        eventDataMove.SendTo(clientsForEven, sendParameters);
                        break;
                    }
                case (byte)OperationCode.GetPlayersTemplate:
                    {
                        List<BattleRoyalClient> clientsForEven = new List<BattleRoyalClient>();
                        clientsForEven.AddRange(ClientsPool.Instance.Clients);
                        clientsForEven.Remove(this);
                        foreach (BattleRoyalClient client in clientsForEven) 
                        {
                            OperationResponse resp = new OperationResponse(operationRequest.OperationCode);
                            Dictionary<string, object> template = new Dictionary<string, object>();
                            template.Add(client.CharactedName, null);
                            resp.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.PlayerTemplate, template } };
                            SendOperationResponse(resp, sendParameters);
                        }

                        break;
                    }

                default:
                    log.Debug("Unknown OperationRequest recv: " + operationRequest.OperationCode);
                    break;
            }
        }

        #region handler for requests
        private void LoginHandler(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var loginRequest = new Login(Protocol, operationRequest);

            if (!loginRequest.IsValid)
            {
                SendOperationResponse(loginRequest.GetResponse(ErrorCode.InvalidParametrs), sendParameters);
                return;
            }
            CharactedName = loginRequest.CharactedName;
            if (ClientsPool.Instance.IsContain(CharactedName))
            {
                SendOperationResponse(loginRequest.GetResponse(ErrorCode.NameIsExist), sendParameters);
                return;
            }

            ClientsPool.Instance.AddClient(this);
            OperationResponse resp = new OperationResponse(operationRequest.OperationCode);
            resp.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.CharactedName, CharactedName } };
            SendOperationResponse(resp, sendParameters);
            log.Info("user with name:" + CharactedName);
        }
        #endregion
    }
}

//TODO Добавить определение шаблона локального игрока
//Dictionary<string, object> playerTemplate = new Dictionary<string, object>();
//playerTemplate.Add(loginRequest.CharactedName, null);
//resp.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.Player, playerTemplate } };
