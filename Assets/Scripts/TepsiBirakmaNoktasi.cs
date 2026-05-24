using UnityEngine;
using TMPro; // TextMeshPro kütüphanesini ekledik

public class TepsiBirakmaNoktasi : MonoBehaviour, IInteractable
{
    [Header("Ýstasyon Ayarlarý")]
    public Transform tepsininDuracagiYer;
    public Tray ustundekiTepsi;

    [Header("Arayüz Ayarlarý")]
    public TextMeshPro etSayaciYazisi; // Havada asýlý duracak 3D yazý bileþeni

    void Start()
    {
        SayaciGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        if (ustundekiTepsi == null)
        {
            Tray eldekiTepsi = oyuncu.GetHeldTray();

            if (eldekiTepsi != null)
            {
                ustundekiTepsi = eldekiTepsi;

                ustundekiTepsi.transform.SetParent(null);
                ustundekiTepsi.transform.position = tepsininDuracagiYer.position;
                ustundekiTepsi.transform.rotation = tepsininDuracagiYer.rotation;
                ustundekiTepsi.transform.localScale = ustundekiTepsi.orijinalBoyut;

                oyuncu.EldenBirak();
                SayaciGuncelle(); // Tepsi konunca sayacý tazeledik
                Debug.Log("Tepsi masaya býrakýldý.");
            }
        }
        else
        {
            if (oyuncu.GetHeldTray() == null && !oyuncu.bicakVarMi)
            {
                oyuncu.PickUpItem(ustundekiTepsi.gameObject);
                ustundekiTepsi = null;
                SayaciGuncelle(); // Tepsi alýnýnca sayacý gizledik
                Debug.Log("Tepsi masadan geri alýndý.");
            }
        }
    }

    // Döner makinesi et kestikçe veya tepsi deðiþtikçe bu fonksiyon çaðrýlacak
    public void SayaciGuncelle()
    {
        if (etSayaciYazisi == null) return;

        // Eðer masada tepsi varsa ve henüz dürüme dönüþmediyse sayýyý göster
        if (ustundekiTepsi != null && !ustundekiTepsi.isDurum)
        {
            etSayaciYazisi.gameObject.SetActive(true);
            etSayaciYazisi.text = ustundekiTepsi.tepsidekiEtSayisi.ToString();
        }
        else
        {
            // Masada tepsi yoksa yazýyý tamamen gizle
            etSayaciYazisi.gameObject.SetActive(false);
        }
    }
}