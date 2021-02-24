using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersPool : MonoBehaviour
{
    public static PlayersPool _instance;
    public static PlayersPool Instanse
    {
        get { return _instance; }
    }

    public Player LocalPlayer { get; private set; }

    public void CreateLocalPlayer(object objectTemplate) 
    {
        LocalPlayer = new Player(objectTemplate);
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

        //TODO Дать запрос на получение шаблона игрового объекта перенести loadStartScene в соответствующий handler
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
