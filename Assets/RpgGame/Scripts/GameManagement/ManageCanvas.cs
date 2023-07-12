using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageCanvas : MonoBehaviour
{
    public List<GameObject> panels;
    public GameObject defaultPanel;
    public GameObject iaChat;
    public GameObject menuPanel;

    // Start is called before the first frame update
    void Start()
    {
        ActivateDefault();
    }
    public void ActivateDefault()
    {
        DeActivateAll();
        defaultPanel.SetActive(true);
    }
    public void ActivateIaChat() {
        DeActivateAll();
        iaChat.SetActive(true);
    }
    public void ActivateMenuPanel()
    {
        DeActivateAll();
        menuPanel.SetActive(true);
    }
    private void DeActivateAll() {
        panels.ForEach(s => s.SetActive(false));
    }
}
