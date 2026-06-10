using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KasaYonetici : MonoBehaviour, IInteractable
{
    public static KasaYonetici Instance;
    public Transform kasaBeklemeNoktasi;

    [Header("Para Sistemi")]
    public float toplamCiro = 0f;
    public float durumFiyati = 150f;
    public TMP_Text ciroYazisi;

    [Header("Memnuniyet Sistemi")]
    public TMP_Text memnuniyetYazisi;
    public float genelMemnuniyet = 50f; // Dükkanýn puaný 50'den baþlar

    // Diðer scriptler hata vermesin diye ismini deðiþtirmedik ama direkt ana puaný yollar
    public float GetOrtalamaMemnuniyet()
    {
        return genelMemnuniyet;
    }

    public void MemnuniyetPuaniniIsle(float degisimMiktari)
    {
        // Gelen -5, +10 gibi deðerleri doðrudan ana puana ekliyoruz
        genelMemnuniyet += degisimMiktari;

        // Puanýn 0'ýn altýna düþmesini veya 100'ü geçmesini engelliyoruz
        genelMemnuniyet = Mathf.Clamp(genelMemnuniyet, 0f, 100f);

        if (memnuniyetYazisi != null)
        {
            memnuniyetYazisi.text = "%" + Mathf.RoundToInt(genelMemnuniyet).ToString();
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
        // Oyun baþlarken ekranda %50 yazsýn
        MemnuniyetPuaniniIsle(0f);
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