using UnityEngine;
using System.Collections;
using TMPro;

public class Fritoz : MonoBehaviour, IInteractable
{
    [Header("Sepet 1 (Plane.001)")]
    public Transform sepet1Transform;
    public TextMeshPro s1UyariYazisi;
    public GameObject s1CigGorsel;
    public GameObject s1PismisGorsel;
    public GameObject s1YanikGorsel;

    // Durumlar -> 0: Boş, 1: Çiğ, 2: Pişmiş, 3: Yanık
    public int s1Durum = 0;
    private Coroutine s1Zamanlayici;
    private Vector3 s1BaslangicPozisyonu;
    [HideInInspector] public float s1GecenZaman = 0f; // Öncelik hesabı için zamanı takip eder

    [Header("Sepet 2 (Plane.002)")]
    public Transform sepet2Transform;
    public TextMeshPro s2UyariYazisi;
    public GameObject s2CigGorsel;
    public GameObject s2PismisGorsel;
    public GameObject s2YanikGorsel;

    public int s2Durum = 0;
    private Coroutine s2Zamanlayici;
    private Vector3 s2BaslangicPozisyonu;
    [HideInInspector] public float s2GecenZaman = 0f;

    [Header("Pişirme Ayarları")]
    public float pismeSuresi = 10f;
    public float yanmaSiniri = 15f;
    public float dalmaDerinligi = 0.3f;

    [Header("Çıktı Prefabları")]
    public GameObject cigPatatesPrefab;
    public GameObject pismisPatatesPrefab;
    public GameObject yanikPatatesPrefab;

    void Start()
    {
        if (sepet1Transform != null) s1BaslangicPozisyonu = sepet1Transform.localPosition;
        if (sepet2Transform != null) s2BaslangicPozisyonu = sepet2Transform.localPosition;

        SepetiSifirla(1);
        SepetiSifirla(2);
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();
        bool elBosMu = (eldekiMalzeme == null && oyuncu.GetHeldTray() == null && !oyuncu.bicakVarMi);

        // 1. DURUM: ELLER DOLUYKEN PATATES KOYMA
        if (eldekiMalzeme != null && eldekiMalzeme.name.Contains("CigPatates"))
        {
            if (s1Durum == 0)
            {
                oyuncu.EldenBirakVeSil();
                s1Zamanlayici = StartCoroutine(PisirmeDongusu(1));
                return;
            }
            if (s2Durum == 0)
            {
                oyuncu.EldenBirakVeSil();
                s2Zamanlayici = StartCoroutine(PisirmeDongusu(2));
                return;
            }

            Debug.Log("Fritözün iki gözü de şu an dolu!");
        }

        // 2. DURUM: ELLER BOŞKEN PATATES ALMA (ÖNCELİK SİSTEMİ)
        else if (elBosMu)
        {
            int alinacakSepet = 0;

            if (s1Durum != 0 && s2Durum != 0)
            {
                // ÖNCELİK 1: Pişmiş (Eksili sayılar)
                if (s1Durum == 2 && s2Durum != 2) alinacakSepet = 1;
                else if (s2Durum == 2 && s1Durum != 2) alinacakSepet = 2;
                else if (s1Durum == 2 && s2Durum == 2)
                {
                    // İkisi de eksili sayılardaysa yanmaya EN YAKIN olanı seç
                    alinacakSepet = (s1GecenZaman >= s2GecenZaman) ? 1 : 2;
                }
                // ÖNCELİK 2: Yanık (Çöpe atmak için temizle)
                else if (s1Durum == 3 && s2Durum != 3) alinacakSepet = 1;
                else if (s2Durum == 3 && s1Durum != 3) alinacakSepet = 2;
                // ÖNCELİK 3: İkisi de aynı durumdaysa (ikisi de yanık veya ikisi de çiğ)
                else alinacakSepet = 1;
            }
            else if (s1Durum != 0) alinacakSepet = 1;
            else if (s2Durum != 0) alinacakSepet = 2;

            if (alinacakSepet != 0) SepettenAl(alinacakSepet, oyuncu);
        }
    }

    IEnumerator PisirmeDongusu(int sepetNo)
    {
        Transform sepet = sepetNo == 1 ? sepet1Transform : sepet2Transform;
        TextMeshPro uyari = sepetNo == 1 ? s1UyariYazisi : s2UyariYazisi;
        Vector3 baslangicPoz = sepetNo == 1 ? s1BaslangicPozisyonu : s2BaslangicPozisyonu;

        sepet.localPosition = new Vector3(baslangicPoz.x, baslangicPoz.y - dalmaDerinligi, baslangicPoz.z);

        if (sepetNo == 1) s1Durum = 1; else s2Durum = 1;
        GorselleriAyarla(sepetNo, true, false, false);

        float gecenZaman = 0f;

        while (gecenZaman < yanmaSiniri)
        {
            gecenZaman += Time.deltaTime;

            // Zamanı öncelik hesabı için küresel değişkene kaydet
            if (sepetNo == 1) s1GecenZaman = gecenZaman; else s2GecenZaman = gecenZaman;

            if (gecenZaman <= pismeSuresi)
            {
                if (sepetNo == 1) s1Durum = 1; else s2Durum = 1;
                int kalanSaniye = Mathf.CeilToInt(pismeSuresi - gecenZaman);
                if (uyari != null) { uyari.text = "<color=green>" + kalanSaniye.ToString() + "</color>"; }
            }
            else if (gecenZaman > pismeSuresi && gecenZaman <= yanmaSiniri)
            {
                if (sepetNo == 1) s1Durum = 2; else s2Durum = 2;
                GorselleriAyarla(sepetNo, false, true, false);

                int eksiSaniye = Mathf.FloorToInt(pismeSuresi - gecenZaman);
                if (uyari != null) { uyari.text = "<color=yellow>" + eksiSaniye.ToString() + "</color>"; }
            }

            yield return null;
        }

        if (sepetNo == 1) s1Durum = 3; else s2Durum = 3;
        GorselleriAyarla(sepetNo, false, false, true);
        if (uyari != null) { uyari.text = "<color=red>!</color>"; }
    }

    void SepettenAl(int sepetNo, OyuncuEnvanter oyuncu)
    {
        int anlikDurum = sepetNo == 1 ? s1Durum : s2Durum;
        GameObject verilecekPrefab = null;

        if (anlikDurum == 1) verilecekPrefab = cigPatatesPrefab;
        else if (anlikDurum == 2) verilecekPrefab = pismisPatatesPrefab;
        else if (anlikDurum == 3) verilecekPrefab = yanikPatatesPrefab;

        if (verilecekPrefab != null)
        {
            GameObject yeniUrun = Instantiate(verilecekPrefab);

            if (anlikDurum == 1)
            {
                yeniUrun.name = "CigPatates";
            }

            oyuncu.PickUpItem(yeniUrun);
        }

        if (sepetNo == 1 && s1Zamanlayici != null) StopCoroutine(s1Zamanlayici);
        if (sepetNo == 2 && s2Zamanlayici != null) StopCoroutine(s2Zamanlayici);

        SepetiSifirla(sepetNo);
    }

    void GorselleriAyarla(int sepetNo, bool cig, bool pismis, bool yanik)
    {
        if (sepetNo == 1)
        {
            if (s1CigGorsel != null) s1CigGorsel.SetActive(cig);
            if (s1PismisGorsel != null) s1PismisGorsel.SetActive(pismis);
            if (s1YanikGorsel != null) s1YanikGorsel.SetActive(yanik);
        }
        else
        {
            if (s2CigGorsel != null) s2CigGorsel.SetActive(cig);
            if (s2PismisGorsel != null) s2PismisGorsel.SetActive(pismis);
            if (s2YanikGorsel != null) s2YanikGorsel.SetActive(yanik);
        }
    }

    void SepetiSifirla(int sepetNo)
    {
        Transform sepet = sepetNo == 1 ? sepet1Transform : sepet2Transform;
        Vector3 baslangicPoz = sepetNo == 1 ? s1BaslangicPozisyonu : s2BaslangicPozisyonu;

        sepet.localPosition = baslangicPoz;

        if (sepetNo == 1)
        {
            s1Durum = 0;
            s1GecenZaman = 0f; // Zamanı sıfırla
            if (s1UyariYazisi != null) s1UyariYazisi.text = "";
            GorselleriAyarla(1, false, false, false);
        }
        else
        {
            s2Durum = 0;
            s2GecenZaman = 0f; // Zamanı sıfırla
            if (s2UyariYazisi != null) s2UyariYazisi.text = "";
            GorselleriAyarla(2, false, false, false);
        }
    }
}