using UnityEngine;

public class DonerDilimi : MonoBehaviour, IInteractable
{
    public bool sogukMu = false;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Tray eldeTutulanTepsi = oyuncu.GetHeldTray();

        if (eldeTutulanTepsi != null)
        {
            // Tepside henüz lavaþ basýlmadýysa et toplanabilir
            if (!eldeTutulanTepsi.isDurum)
            {
                eldeTutulanTepsi.tepsidekiEtSayisi++;
                eldeTutulanTepsi.isMeatCold = this.sogukMu;
                eldeTutulanTepsi.GorselleriGuncelle();

                Debug.Log("Et tepsiye alýndý. Toplam Et: " + eldeTutulanTepsi.tepsidekiEtSayisi);
                Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("Lavaþ sarýlmýþ tepsiye ekstra çýplak et ekleyemezsin!");
            }
        }
        else
        {
            Debug.Log("Eti yerden almak için elinde bir TEPSÝ olmalý!");
        }
    }
}