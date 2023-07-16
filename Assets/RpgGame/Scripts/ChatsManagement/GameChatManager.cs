using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameChatManager : MonoBehaviour
{
    public GameObject NPCCompa;
    public GameObject NPCCamera;
    public GameObject PlayerCamera;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void ActivateChatNPC() { 
        GetComponent<ManageCanvas>().ActivateIaChat();
        NPCCamera.SetActive(true);
        PlayerCamera.SetActive(false);
        NPCCompa.SetActive(false);
        player.SetActive(false);
    }
    public void ActivateChatCompa()
    {
        GetComponent<ManageCanvas>().ActivateIaChat();
        if(NPCCamera != null)
            NPCCamera.SetActive(false);
        PlayerCamera.SetActive(false);
        NPCCompa.SetActive(true);
        player.SetActive(false);
    }
    public void CloseChat() {
        GetComponent<ManageCanvas>().ActivateDefault();
        PlayerCamera.SetActive(true);
        if (NPCCamera != null)
            NPCCamera.SetActive(false);        
        NPCCompa.SetActive(false);
        player.SetActive(true);
    }
}
