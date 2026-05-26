using UnityEngine;

public class DonerDilimi : MonoBehaviour, IInteractable
{
    public bool sogukMu = false;

    // ÝÞTE EKSÝK OLAN VE HATAYA SEBEP OLAN SATIR BURASI:
    public bool zehirliMi = false;

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

                // NOT: Ýleride Tray kodunuza "isMeatPoisonous" tarzý bir deðiþken eklerseniz, 
                // bu dilimdeki zehir bilgisini o tepsiye þöyle aktarabilirsiniz:
                // eldeTutulanTepsi.isMeatPoisonous = this.zehirliMi; 

                eldeTutulanTepsi.GorselleriGuncelle();

                Debug.Log("Et tepsiye alýndý. Zehirli mi?: " + zehirliMi);
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