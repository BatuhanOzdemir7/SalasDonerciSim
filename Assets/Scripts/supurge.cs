using UnityEngine;

// Bu script eklendiğinde Unity otomatik olarak Malzeme scriptini de ekler
[RequireComponent(typeof(Malzeme))]
public class Supurge : MonoBehaviour, IInteractable
{
    public void Interact(OyuncuEnvanter oyuncu)
    {
        // Oyuncunun elleri tamamen boşsa süpürgeyi alabilir
        if (oyuncu.GetHeldMalzeme() == null && oyuncu.GetHeldTray() == null && !oyuncu.bicakVarMi)
        {
            oyuncu.PickUpItem(this.gameObject);
            Debug.Log("<color=cyan>Süpürge ele alındı! Çöplerin üstüne gidip E'ye basarak temizleyebilirsin.</color>");
        }
        else
        {
            Debug.Log("Süpürgeyi almak için ellerin boş olmalı!");
        }
    }
}