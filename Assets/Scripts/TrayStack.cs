using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TrayStack : MonoBehaviour, IInteractable
{
    [Header("Yýðýn Ayarlarý")]
    public GameObject tepsiPrefab;

    [HideInInspector] public List<GameObject> gorselTepsiler = new List<GameObject>();
    [HideInInspector] public int mevcutTepsiSayisi;

    [Header("Arayüz Ayarlarý")]
    public TextMeshPro yiginSayaciYazisi;

    void Start()
    {
        gorselTepsiler.Clear();

        // 1. Yýðýnýn altýndaki tüm nesneleri tarayarak listeyi otomatik oluþturur
        foreach (Transform child in transform)
        {
            if (child.name.ToLower().Contains("tepsi"))
            {
                gorselTepsiler.Add(child.gameObject);

                // YENÝ KORUMA SÝSTEMÝ: Yýðýndaki tepsilerin içinde açýk kalmýþ 
                // içecek veya yiyecek modelleri varsa oyun baþýnda otomatik olarak temizler.
                Tray trayScript = child.GetComponent<Tray>();
                if (trayScript != null)
                {
                    trayScript.ayranVarMi = false;
                    trayScript.suVarMi = false;
                    trayScript.kolaVarMi = false;
                    trayScript.patatesVarMi = false;
                    trayScript.isDurum = false;
                    trayScript.tepsidekiEtSayisi = 0;
                    trayScript.GorselleriGuncelle();
                }
                else
                {
                    // Eðer tepsi üzerinde Tray scripti olmasýna raðmen gizlenmediyse, 
                    // isme göre alt objeleri tarayýp kaba kuvvetle kapatýyoruz.
                    foreach (Transform altObje in child)
                    {
                        string altObjeAdi = altObje.name.ToLower();
                        if (altObjeAdi.Contains("kola") || altObjeAdi.Contains("cola") ||
                            altObjeAdi.Contains("ayran") || altObjeAdi.Contains("su") ||
                            altObjeAdi.Contains("fries") || altObjeAdi.Contains("patates"))
                        {
                            altObje.gameObject.SetActive(false);
                        }
                    }
                }

                // Yýðýndaki dekoratif tepsilerin lazeri sabote etmesini engellemek için
                // kendi bireysel collider bileþenlerini oyun baþýnda kapatýyoruz.
                Collider childCol = child.GetComponent<Collider>();
                if (childCol != null)
                {
                    childCol.enabled = false;
                }
            }
        }

        // 2. Tepsileri yüksekliklerine (Y ekseni) göre alttan üste doðru hizalar
        gorselTepsiler.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));

        // 3. Sayacý tam mevcut adet üzerinden eþitleyip çalýþtýrýr
        mevcutTepsiSayisi = gorselTepsiler.Count;
        SayaciGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Tray eldekiTepsi = oyuncu.GetHeldTray();

        // DURUM 1: Oyuncunun halihazýrda DOLU bir tepsisi varsa yerleþtiremez
        if (eldekiTepsi != null && !eldekiTepsi.TepsiBosMu())
        {
            Debug.Log("Dolu tepsiyi istasyona koyamazsýn! Önce içindekileri çöpe dökmelisin.");
            return;
        }

        // DURUM 2: Oyuncunun Eli Tamamen Boþsa (Yýðýndan tepsi al)
        if (eldekiTepsi == null && !oyuncu.bicakVarMi && oyuncu.GetHeldMalzeme() == null)
        {
            if (mevcutTepsiSayisi > 0)
            {
                mevcutTepsiSayisi--;
                if (mevcutTepsiSayisi < gorselTepsiler.Count && gorselTepsiler[mevcutTepsiSayisi] != null)
                {
                    gorselTepsiler[mevcutTepsiSayisi].SetActive(false);
                }

                GameObject yeniTepsi = Instantiate(tepsiPrefab);
                oyuncu.PickUpItem(yeniTepsi);

                SayaciGuncelle();
                Debug.Log("Tepsi baþarýyla alýndý. Kalan temiz adet: " + mevcutTepsiSayisi);
            }
            else
            {
                Debug.Log("Yýðýnda alýnacak tepsi kalmadý!");
            }
        }
        // DURUM 3: Oyuncunun Elinde BOÞ Bir Tepsi Varsa (Yýðýna geri býrak)
        else if (eldekiTepsi != null && eldekiTepsi.TepsiBosMu())
        {
            oyuncu.EldenBirak();
            Destroy(eldekiTepsi.gameObject);

            if (mevcutTepsiSayisi < gorselTepsiler.Count && gorselTepsiler[mevcutTepsiSayisi] != null)
            {
                gorselTepsiler[mevcutTepsiSayisi].SetActive(true);
            }

            mevcutTepsiSayisi++;
            SayaciGuncelle();
            Debug.Log("Tepsi yýðýna geri konuldu. Toplam: " + mevcutTepsiSayisi);
        }
    }

    public void SayaciGuncelle()
    {
        if (yiginSayaciYazisi == null) return;

        if (mevcutTepsiSayisi > 0)
        {
            yiginSayaciYazisi.gameObject.SetActive(true);
            yiginSayaciYazisi.text = mevcutTepsiSayisi.ToString();
            yiginSayaciYazisi.color = Color.white;
        }
        else
        {
            yiginSayaciYazisi.text = "Tepsi Bitti!";
            yiginSayaciYazisi.color = Color.red;
        }
    }
}