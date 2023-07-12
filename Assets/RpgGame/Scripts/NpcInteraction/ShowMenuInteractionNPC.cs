using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenuInteractionNPC : MonoBehaviour
{
    private GameObject canvas;
    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("GameManager");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
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
