using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunSonuYonetici : MonoBehaviour
{
    public static GunSonuYonetici Instance;

    [Header("Zaman Ayarları (Vardiya Saniyeleri)")]
    public float gun1Suresi = 180f;
    public float gun2Suresi = 420f;
    public float gun3Suresi = 300f;

    private float kalanSure;
    private bool gunDevamEdiyorMu = false;

    public TMP_Text dijitalSaatYazisi;

    [Header("UI Panelleri")]
    public GameObject gunSonuCanvas;
    public GameObject finalUICanvas; // BATUHAN'IN FİNAL EKRANI BURAYA GELECEK

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
        // İŞTE KRİTİK NOKTA BURASI: Oyun başlarken hafızadaki günü çekiyoruz!
        mevcutGun = PlayerPrefs.GetInt("KayitliGun", 1);

        if (gunSonuCanvas != null) gunSonuCanvas.SetActive(false);
        if (finalUICanvas != null) finalUICanvas.SetActive(false);

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

    private void DukkaniTemizle()
    {
        // 1. İçerideki tüm müşterileri anında yok et
        MusteriAI[] musteriler = FindObjectsOfType<MusteriAI>();
        foreach (MusteriAI musteri in musteriler)
        {
            Destroy(musteri.gameObject);
        }

        // 2. Hijyen skorunu tamamen sıfırla
        if (HijyenYonetici.Instance != null)
        {
            HijyenYonetici.Instance.mevcutHijyen = 5.0f;
            HijyenYonetici.Instance.dukkanCopSayisi = 0;
        }

        // 3. Oyuncuyu dükkanın başlangıç noktasına ışınla (Sahnede "BaslangicNoktasi" objesi olmalı)
        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");
        GameObject baslangic = GameObject.Find("BaslangicNoktasi");

        if (oyuncu != null && baslangic != null)
        {
            CharacterController cc = oyuncu.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            oyuncu.transform.position = baslangic.transform.position;
            oyuncu.transform.rotation = baslangic.transform.rotation;

            if (cc != null) cc.enabled = true;
        }
    }

    // DEVAM BUTONUNA TIKLANINCA ÇALIŞACAK FONKSİYON
    public void SonrakiGuneGec()
    {
        // 1. FİNAL UI İÇİN O GÜNÜN VERİLERİNİ HAFIZAYA KAYDET
        if (KasaYonetici.Instance != null && HijyenYonetici.Instance != null)
        {
            PlayerPrefs.SetFloat("Gun" + mevcutGun + "_Ciro", KasaYonetici.Instance.toplamCiro);
            PlayerPrefs.SetFloat("Gun" + mevcutGun + "_Memnuniyet", KasaYonetici.Instance.GetOrtalamaMemnuniyet());
            PlayerPrefs.SetString("Gun" + mevcutGun + "_Hijyen", HijyenYonetici.Instance.guncelNot.ToString());
        }

        // 2. EĞER 3. GÜN BİTTİYSE FİNAL EKRANINI AÇ!
        if (mevcutGun >= 3)
        {
            Debug.Log("3 GÜNLÜK VARDİYA BİTTİ! FİNAL EKRANI AÇILIYOR...");

            if (gunSonuCanvas != null) gunSonuCanvas.SetActive(false); // Eski gün sonunu gizle
            if (finalUICanvas != null) finalUICanvas.SetActive(true);  // Masalı Final Ekranını patlat!

            return; // Kodun aşağıya inip sahneyi yeniden yüklemesini engelliyoruz
        }

        // 3. EĞER 3. GÜN DEĞİLSE SONRAKİ GÜNE GEÇ
        mevcutGun++;

        // Sadece kaçıncı günde olduğumuzu hafızada tutuyoruz
        PlayerPrefs.SetInt("KayitliGun", mevcutGun);
        PlayerPrefs.Save();

        // Zamanı normale döndür ve sahneyi sıfırla
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}