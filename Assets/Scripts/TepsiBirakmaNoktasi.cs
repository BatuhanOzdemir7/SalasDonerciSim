using UnityEngine;
using TMPro;

public class TepsiBirakmaNoktasi : MonoBehaviour, IInteractable
{
    [Header("Ýstasyon Ayarlarý")]
    public Transform tepsininDuracagiYer;
    public Tray ustundekiTepsi;

    [Header("Arayüz Ayarlarý")]
    public TextMeshPro etSayaciYazisi;

    void Start()
    {
        SayaciGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // Lazerin hangi tuþtan geldiðini tespit ediyoruz
        bool fTusunaBasildiMi = Input.GetKey(KeyCode.F) || Input.GetKeyDown(KeyCode.F);

        if (ustundekiTepsi == null)
        {
            // FÝLTRE 3: F tuþuna basýldýysa boþ istasyona tepsiyi "koyma". Tepsi sadece E ile konur!
            if (fTusunaBasildiMi) return;

            Tray eldekiTepsi = oyuncu.GetHeldTray();
            if (eldekiTepsi != null)
            {
                ustundekiTepsi = eldekiTepsi;
                ustundekiTepsi.transform.SetParent(null);
                ustundekiTepsi.transform.position = tepsininDuracagiYer.position;
                ustundekiTepsi.transform.rotation = tepsininDuracagiYer.rotation;
                ustundekiTepsi.transform.localScale = ustundekiTepsi.orijinalBoyut;

                Collider col = ustundekiTepsi.GetComponent<Collider>();
                if (col == null) col = ustundekiTepsi.GetComponentInChildren<Collider>();
                if (col != null) col.enabled = true;

                oyuncu.EldenBirak();
                SayaciGuncelle();
            }
        }
        else
        {
            Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

            // Eðer F'ye basýldýysa VEYA elde malzeme varsa doðrudan Dürüm (Ýþlem) mantýðýna geç
            if (fTusunaBasildiMi || eldekiMalzeme != null)
            {
                ustundekiTepsi.Interact(oyuncu);
                SayaciGuncelle();
                return;
            }

            // Eðer E'ye basýldýysa ve eller tamamen boþsa Tepsiyi Geri Al
            if (oyuncu.GetHeldTray() == null && !oyuncu.bicakVarMi && eldekiMalzeme == null)
            {
                oyuncu.PickUpItem(ustundekiTepsi.gameObject);
                ustundekiTepsi = null;
                SayaciGuncelle();
            }
        }
    }

    public void SayaciGuncelle()
    {
        if (etSayaciYazisi == null) return;

        if (ustundekiTepsi != null && !ustundekiTepsi.isDurum)
        {
            etSayaciYazisi.gameObject.SetActive(true);
            etSayaciYazisi.text = ustundekiTepsi.tepsidekiEtSayisi.ToString();
        }
        else
        {
            etSayaciYazisi.gameObject.SetActive(false);
        }
    }
}