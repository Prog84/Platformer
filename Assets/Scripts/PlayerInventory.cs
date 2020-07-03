﻿using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public int coinsCount;
    [SerializeField] private Text coinsText;
    public BuffReciever buffReciever;
    private List<Item> items;
    public List<Item> Items
    {
        get { return items; }
    }

    private void Start()
    {
        GameManager.Instance.inventory = this;
        coinsText.text = coinsCount.ToString();
        items = new List<Item>();

    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (GameManager.Instance.coinContainer.ContainsKey(col.gameObject))
        {
            coinsCount++;
            coinsText.text = coinsCount.ToString();
            var coin = GameManager.Instance.coinContainer[col.gameObject];
            coin.StartDestroy();
        }
        if (GameManager.Instance.itemsContainer.ContainsKey(col.gameObject))
        {
            var itemComponent = GameManager.Instance.itemsContainer[col.gameObject];
            items.Add(itemComponent.Item);
            itemComponent.Destroy(col.gameObject);
        }
    }
}
