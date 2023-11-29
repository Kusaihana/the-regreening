using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private TMP_Text _npcText;

    private Farmer _farmer;

    void Awake()
    {
        _farmer = FindObjectOfType<Farmer>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            _npcText.text = "Press 'Space' to interact";
            _npcText.gameObject.SetActive(true);
            other.GetComponent<NPC>().canTalk = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            _npcText.gameObject.SetActive(false);
            other.GetComponent<NPC>().canTalk = false;
            other.GetComponent<NPC>().inConversation = false;
        }
    }
}
