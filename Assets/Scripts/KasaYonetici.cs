using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KasaYonetici : MonoBehaviour, IInteractable
{
    public static KasaYonetici Instance;
    public Transform kasaBeklemeNoktasi;

    [Header("Para Sistemi")]
    public float toplamCiro = 0f;
    public float durumFiyati = 150f;    // Taban menü fiyatý (Müþteri çarpaný bunun üzerinden hesaplar)
    public TMP_Text ciroYazisi;

    [Header("Memnuniyet Sistemi")]
    public TMP_Text memnuniyetYazisi;
    private float toplamMemnuniyet = 0f;
    private int hizmetAlanMusteriSayisi = 0;

    public float GetOrtalamaMemnuniyet()
    {
        if (hizmetAlanMusteriSayisi == 0) return 100f;
        return toplamMemnuniyet / hizmetAlanMusteriSayisi;
    }

    public void MemnuniyetPuaniniIsle(float musteriPuani)
    {
        toplamMemnuniyet += musteriPuani;
        hizmetAlanMusteriSayisi++;

        float ortalama = toplamMemnuniyet / hizmetAlanMusteriSayisi;

        if (memnuniyetYazisi != null)
        {
            memnuniyetYazisi.text = "%" + Mathf.RoundToInt(ortalama).ToString();
        }
    }

    private Queue<MusteriAI> kasaKuyrugu = new Queue<MusteriAI>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
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

            // Müþterinin kalitesine göre hesaplanan kendi tutarýný alýyoruz
            float alinacakHesap = siradakiMusteri.odenecekTutar;

            siradakiMusteri.OdemeYapVeGit();
            KuyruguGuncelle();

            toplamCiro += alinacakHesap;
            CiroYazisiniGuncelle();

            Debug.Log("<color=green>KASÝYER: Hesap alýndý! Kasaya " + alinacakHesap + " TL eklendi. Toplam: " + toplamCiro + " TL</color>");
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
            Vector3 yeniPozisyon = kasaBeklemeNoktasi.position - (kasaBeklemeNoktasi.forward * (index * 0.8f));
            musteri.NavigasyonHedefiVer(yeniPozisyon);
            index++;
        }
    }

    public void CiroYazisiniGuncelle()
    {
        if (ciroYazisi != null)
        {
            ciroYazisi.text = toplamCiro.ToString() + " TL";
        }
    }
}