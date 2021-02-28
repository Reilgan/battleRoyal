using battleRoyalServer.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private string Error { get; set; }

    private Text LoginText;
    private Button LoginButton;

    // Start is called before the first frame update
    void Start()
    {
        LoginButton =  GameObject.Find("ButtonLogin").GetComponent<Button>();
        LoginButton.onClick.AddListener(onClickLoginButton);
    }

    void onClickLoginButton()
    {
        LoginText = GameObject.Find("InputLogin/Text").GetComponent<Text>();
        string login = LoginText.text;
        if (login == "Single")
        {
            PhotonClient.Instanse.loadStartScene();
            return;
        }
        PhotonClient.Instanse.SendLoginOperation(login);
    }

    // Update is called once per frame
    void Update()
    {

        //PhotonClient.Instanse.SendLoginOperation(Name);
    }

}
