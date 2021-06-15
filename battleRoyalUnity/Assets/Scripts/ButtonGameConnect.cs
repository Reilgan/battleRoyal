using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGameConnect : MonoBehaviour
{
    private Button Button;
    // Start is called before the first frame update
    void Start()
    {
        Button = GameObject.Find("ButtonGameConnect").GetComponent<Button>();
        Button.onClick.AddListener(onClickButton);
    }

    void onClickButton()
    {
        MasterClient.Instanse.GetGameServerIP();
    }

    // Update is called once per frame
    void Update()
    {

        //PhotonClient.Instanse.SendLoginOperation(Name);
    }
}
