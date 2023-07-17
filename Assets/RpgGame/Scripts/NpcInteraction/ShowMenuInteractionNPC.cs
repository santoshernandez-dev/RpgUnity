using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenuInteractionNPC : MonoBehaviour
{
    private GameObject canvas;
    private GameObject chatGPT;
    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("GameManager");
        chatGPT = GameObject.FindGameObjectWithTag("ChatGPT");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            chatGPT.GetComponent<ChatGPT>().NPC = transform.gameObject;
            canvas.GetComponent<GameChatManager>().NPCCamera = transform.Find("CameraPoint").Find("Main Camera NPC").gameObject;
            canvas.GetComponent<ManageCanvas>().ActivateMenuPanel();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canvas.GetComponent<ManageCanvas>().ActivateDefault();
        }
    }
}
