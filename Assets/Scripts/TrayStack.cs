using UnityEngine;
using System.Collections.Generic;

public class TrayStack : MonoBehaviour, IInteractable
{
    // Inspector panelinden 7 tabaðý bu listeye sürükle
    public List<GameObject> tabaklar;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        if (tabaklar.Count > 0)
        {
            // Listenin son elemanýný (fiziksel olarak en üstteki tabaðý) seç
            int sonIndex = tabaklar.Count - 1;
            GameObject ustTabak = tabaklar[sonIndex];

            // Tabaðý yýðýndan çýkar
            tabaklar.RemoveAt(sonIndex);

            // Oyuncuya tabaðý ver
            oyuncu.PickUpItem(ustTabak);
            Debug.Log("Yýðýndan 1 tabak alýndý. Kalan tabak: " + tabaklar.Count);
        }
        else
        {
            Debug.Log("Yýðýnda tabak kalmadý!");
        }
    }
}