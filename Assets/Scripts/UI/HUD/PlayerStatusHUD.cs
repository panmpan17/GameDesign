using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using TMPro;

public class PlayerStatusHUD : MonoBehaviour
{
    [SerializeField]
    private EventReference inventoryChangeEvent;
    [SerializeField]
    private TextMeshProUGUI itemCountText;

    void Awake()
    {
        inventoryChangeEvent.InvokeIntEvents += ChangeInventory;
    }

    void OnDestroy()
    {
        inventoryChangeEvent.InvokeIntEvents -= ChangeInventory;
    }

    public void ChangeInventory(int count)
    {
        itemCountText.text = count.ToString();
    }
}
