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
            bool qTusunaBasildiMi = Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.Q);

            // KÖPRÜ: Lazer masaya çarpsa bile dürüm sarma/malzeme koyma iþlemini tepsiye yönlendir
            if (qTusunaBasildiMi || eldekiMalzeme != null)
            {
                ustundekiTepsi.Interact(oyuncu);
                SayaciGuncelle();
                return;
            }

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