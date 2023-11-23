using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private int _itemIndex;
    
    private Farmer _farmer;

    private void Awake()
    {
        _farmer = FindObjectOfType<Farmer>();
        GetComponent<Button>().onClick.AddListener(SetItem);
    }

    private void SetItem()
    {
        _farmer.OnItemClick(_itemIndex);
    }
}
