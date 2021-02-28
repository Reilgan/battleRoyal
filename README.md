## Алгоритм определения запроса

1. Определить код операции
```C#
public enum OperationCode : byte
{
    Login,
    SendChatMessage,
    GetRecentChatMessages
}
```
2. Определить код параметра
```C#
public enum ParameterCode : byte
{
    CharactedName,
    ChatMessage
}
```
3. Определить код события, если запрос подразумевает ответ нескольким пользователям
```C#
public enum EventCode : byte
{
    ChatMessage
}
```
4. Созданать класс операции, класс будет содержать передаваемые данные унаследован от BaseOperations
```C#
namespace battleRoyalServer.Operations
{
    class ChatMessage:BaseOperations
    {
        public ChatMessage(IRpcProtocol protocol, OperationRequest request) : base(protocol, request)
        { 
        }

        [DataMember(Code = (byte)ParameterCode.ChatMessage)]
        public string Message { get; set; }

    }
}
```
5. Если есть необходимость создать подписку на определенное событие в коде клиента, нужно создать класс унаследованный от EventArgs(см. Документацию C#)
```C#
namespace battleRoyalServer.Common
{
    public class ChatMessageEventArgs:EventArgs
    {
        public string Message { get; set; }

        public ChatMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
```
