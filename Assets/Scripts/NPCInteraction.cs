using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private TMP_Text _npcText;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            _npcText.text = "Press 'E' to interact";
            _npcText.gameObject.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            _npcText.gameObject.SetActive(false);
        }
    }
}
