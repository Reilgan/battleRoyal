using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public GameObject SourceObject { get; private set; }
    public string CharactedName { get; private set; }

    public Player(object objectTemplate)
    {
        Dictionary<string, object> dict = (Dictionary<string, object>)objectTemplate;
        var keys = dict.Keys.ToList();
        CharactedName = keys[0];
        if (CharactedName == null) 
        {
            Debug.Log("Design error: Attempt to create a player without a name");
            return;
        }
        if (dict[CharactedName] == null)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SourceObject = gameObject;
            return;
        }
        //TODO Распарсить пришедший словарь и собрать из него объект

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
