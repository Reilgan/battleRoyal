using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChat : MonoBehaviour
{
    private GameObject panel;
    private bool panelVisible = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(onClickedButton);
        panel = GameObject.Find("Panel");
        panel.SetActive(panelVisible);
    }

    public void onClickedButton()
    {
        panelVisible = !panelVisible;
        panel.SetActive(panelVisible);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
