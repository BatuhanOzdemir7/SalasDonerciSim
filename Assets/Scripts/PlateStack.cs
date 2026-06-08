using UnityEngine;
using System.Collections.Generic;

public class PlateStack : MonoBehaviour, IInteractable
{
    // Inspector panelinden 7 tabaðý bu listeye sürükle
    public List<GameObject> tabaklar;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // EKLENEN KISIM: Oyuncunun elleri tamamen boþ deðilse iþlemi reddet
        if (oyuncu.GetHeldMalzeme() != null || oyuncu.GetHeldTray() != null || oyuncu.bicakVarMi)
        {
            Debug.Log("Elinde bir þey varken yeni tabak alamazsýn!");
            return;
        }

        if (tabaklar.Count > 0)
        {
            int sonIndex = tabaklar.Count - 1;
            GameObject ustTabak = tabaklar[sonIndex];

            tabaklar.RemoveAt(sonIndex);
            oyuncu.PickUpItem(ustTabak);
            Debug.Log("Yýðýndan 1 tabak alýndý. Kalan tabak: " + tabaklar.Count);
        }
        else
        {
            Debug.Log("Yýðýnda tabak kalmadý!");
        }
    }
}