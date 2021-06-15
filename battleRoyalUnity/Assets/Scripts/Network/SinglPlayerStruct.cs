using gameServer.Common;
using System.Collections.Generic;

public class SinglePlayerStruct : StructPlayer
{
    public static SinglePlayerStruct _instance;
    public static SinglePlayerStruct Instanse
    {
        get { return _instance; }
    }

    public SinglePlayerStruct() {
        _instance = this;
    }
}
