using System.Collections.Generic;
using UnityEngine;
using TMPro; // YENÝ: Yazýlarý koddan deðiþtirebilmek için bu kütüphaneyi ekledik

public class KasaYonetici : MonoBehaviour, IInteractable
{
    public static KasaYonetici Instance;
    public Transform kasaBeklemeNoktasi;

    [Header("Para Sistemi")]
    public float toplamCiro = 0f;       // Kasadaki toplam paramýz
    public float durumFiyati = 150f;    // Standart menü fiyatý (Ýstersen Inspector'dan deðiþtirebilirsin)
    public TMP_Text ciroYazisi;         // Ekranda sað üstte duracak o yazý
    [Header("Memnuniyet Sistemi")]
    public TMP_Text memnuniyetYazisi;
    private float toplamMemnuniyet = 0f;
    private int hizmetAlanMusteriSayisi = 0;

    // Bu fonksiyonu scriptin en altýna, diðer fonksiyonlarýn yanýna ekle
    public void MemnuniyetPuaniniIsle(float musteriPuani)
    {
        toplamMemnuniyet += musteriPuani;
        hizmetAlanMusteriSayisi++;

        // Ortalamayý hesapla (Örn: 100 + 80 / 2 = 90)
        float ortalama = toplamMemnuniyet / hizmetAlanMusteriSayisi;

        if (memnuniyetYazisi != null)
        {
            memnuniyetYazisi.text = "%" + Mathf.RoundToInt(ortalama).ToString();
        }
    }
    // Kasadaki kuyruk yapýsý
    private Queue<MusteriAI> kasaKuyrugu = new Queue<MusteriAI>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Oyun baþlarken ekranda 0 TL yazsýn diye
        CiroYazisiniGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        HesapAl();
    }

    public void KuyrugaGir(MusteriAI musteri)
    {
        kasaKuyrugu.Enqueue(musteri);
        KuyruguGuncelle();
    }

    public void HesapAl()
    {
        if (kasaKuyrugu.Count > 0)
        {
            MusteriAI siradakiMusteri = kasaKuyrugu.Dequeue();
            siradakiMusteri.OdemeYapVeGit();
            KuyruguGuncelle();

            // ==========================================
            // YENÝ: PARAYI KASAYA VE EKRANA EKLEME
            // ==========================================
            toplamCiro += durumFiyati;
            CiroYazisiniGuncelle();

            // Ýsteðe baðlý: Kasa sesi (çýnk) eklemek istersen buraya koyabilirsin
            Debug.Log("<color=green>KASÝYER: Hesap alýndý! Kasaya " + durumFiyati + " TL eklendi. Toplam: " + toplamCiro + " TL</color>");
        }
        else
        {
            Debug.Log("Kasada bekleyen müþteri yok usta!");
        }
    }

    void KuyruguGuncelle()
    {
        int index = 0;
        foreach (var musteri in kasaKuyrugu)
        {
            // BURADAKÝ 1.2f DEÐERÝNÝ 0.8f (VEYA 0.7f) YAPARAK MÜÞTERÝLERÝ SIKIÞTIR
            Vector3 yeniPozisyon = kasaBeklemeNoktasi.position - (kasaBeklemeNoktasi.forward * (index * 0.8f));
            musteri.NavigasyonHedefiVer(yeniPozisyon);
            index++;
        }
    }

    // Ekrandaki yazýyý anýnda güncelleyen köprü
    public void CiroYazisiniGuncelle()
    {
        if (ciroYazisi != null)
        {
            ciroYazisi.text = toplamCiro.ToString() + " TL";
        }
    }
}