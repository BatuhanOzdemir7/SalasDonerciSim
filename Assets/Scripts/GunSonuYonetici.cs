using UnityEngine;
using TMPro;

public class GunSonuYonetici : MonoBehaviour
{
    public static GunSonuYonetici Instance;

    [Header("Zaman Ayarları (Vardiya Saniyeleri)")]
    // Artık 3 günün süresini de direkt Unity arayüzünden değiştirebilirsin
    public float gun1Suresi = 180f; // Deneme için 3 dakika
    public float gun2Suresi = 420f; // 7 dakika
    public float gun3Suresi = 300f; // 5 dakika

    private float kalanSure;
    private bool gunDevamEdiyorMu = false;

    public TMP_Text dijitalSaatYazisi;

    [Header("UI Panelleri")]
    public GameObject gunSonuCanvas;

    [Header("Gün Sonu Metinleri")]
    public TMP_Text gunYazisi;
    public TMP_Text hijyenSkoruYazisi;
    public TMP_Text kazanilanParaYazisi;

    [Header("Yıldız Sistemi (Grup)")]
    public GameObject[] doluYildizlar;

    [Header("Gün Verileri")]
    public int mevcutGun = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (gunSonuCanvas != null) gunSonuCanvas.SetActive(false);
        KalanSureyiSifirla();
    }

    void Update()
    {
        if (gunDevamEdiyorMu)
        {
            kalanSure -= Time.deltaTime;
            SaatiEkranaYazdir();

            if (kalanSure <= 0f)
            {
                kalanSure = 0f;
                gunDevamEdiyorMu = false;
                SaatiEkranaYazdir();
                Debug.Log("Vardiya bitti! Gün Sonu paneli açılıyor...");
                GunuBitir();
            }
        }
    }

    void SaatiEkranaYazdir()
    {
        if (dijitalSaatYazisi != null)
        {
            int dakika = Mathf.FloorToInt(kalanSure / 60);
            int saniye = Mathf.FloorToInt(kalanSure % 60);
            dijitalSaatYazisi.text = string.Format("{0:00}:{1:00}", dakika, saniye);
        }
    }

    // YENİ: Süre sıfırlanırken hangi gündeysek o günün public değişkenini çekecek
    public void KalanSureyiSifirla()
    {
        if (mevcutGun == 1) kalanSure = gun1Suresi;
        else if (mevcutGun == 2) kalanSure = gun2Suresi;
        else if (mevcutGun >= 3) kalanSure = gun3Suresi;

        gunDevamEdiyorMu = true;
    }

    public void GunuBitir()
    {
        gunDevamEdiyorMu = false;
        Time.timeScale = 0f;

        if (gunSonuCanvas != null) gunSonuCanvas.SetActive(true);

        if (gunYazisi != null) gunYazisi.text = mevcutGun + ". GÜN SKORU";

        if (HijyenYonetici.Instance != null && hijyenSkoruYazisi != null)
        {
            hijyenSkoruYazisi.text = HijyenYonetici.Instance.guncelNot.ToString();
        }

        if (KasaYonetici.Instance != null)
        {
            if (kazanilanParaYazisi != null)
                kazanilanParaYazisi.text = KasaYonetici.Instance.toplamCiro.ToString() + " TL";

            float ortalama = KasaYonetici.Instance.GetOrtalamaMemnuniyet();
            int acilacakYildizSayisi = Mathf.RoundToInt((ortalama / 100f) * 5f);

            for (int i = 0; i < doluYildizlar.Length; i++)
            {
                if (i < acilacakYildizSayisi)
                {
                    doluYildizlar[i].SetActive(true);
                }
                else
                {
                    doluYildizlar[i].SetActive(false);
                }
            }
        }
    }

    public void SonrakiGuneGec()
    {
        if (mevcutGun >= 3)
        {
            Debug.Log("3 GÜNLÜK VARDİYA BİTTİ! OYUN KAZANILDI!");
            return;
        }

        mevcutGun++;

        if (gunSonuCanvas != null) gunSonuCanvas.SetActive(false);
        Time.timeScale = 1f;

        // Yeni vardiyayı ve o günün ayarlı süresini başlat
        KalanSureyiSifirla();

        Debug.Log("Yeni vardiya başladı! Mevcut Gün: " + mevcutGun);
    }
}