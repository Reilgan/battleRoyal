using battleRoyalServer.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private string Error { get; set; }
    private string Name { get; set; }

    private Text LoginText;
    private Button LoginButton;

    // Start is called before the first frame update
    [System.Obsolete("Login.cs: Спросить у знающих что не так с моей подпиской на onLoginResponce")]
    void Start()
    {
        LoginButton =  GameObject.Find("ButtonLogin").GetComponent<Button>();
        LoginButton.onClick.AddListener(onClickLoginButton);
        PhotonClient.Instanse.OnLoginResponce += OnLoginHandler;
        Name = "";
    }

    void onClickLoginButton()
    {
        LoginText = GameObject.Find("InputLogin/Text").GetComponent<Text>();
        Name = LoginText.text;
        if (Name == "Single")
        {
            loadStartScene();
            return;
        }
        PhotonClient.Instanse.SendLoginOperation(Name);
    }

    // Update is called once per frame
    void Update()
    {

        //PhotonClient.Instanse.SendLoginOperation(Name);
    }


    [System.Obsolete]
    private void OnLoginHandler(object sender, LoginEventArgs e)
    {
        if (e.Error != ErrorCode.Success)
        {
            Error = "Error:" + e.Error.ToString();
            return;
        }
        PhotonClient.Instanse.OnLoginResponce -= OnLoginHandler;
        loadStartScene();
    }

    private void loadStartScene()
    {
        SceneManager.LoadScene("Game");
    }
}
