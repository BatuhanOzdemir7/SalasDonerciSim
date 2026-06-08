        using UnityEngine;

public class HijyenYonetici : MonoBehaviour
{


    public static HijyenYonetici Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    [Header("Hijyen Ayarları (0.0 - 5.0 Arası)")]
    public float mevcutHijyen = 5.0f;
    public float saniyelikKirlenmeHizi = 0.02f; // Kendi kendine düşme hızı
    public int dukkanCopSayisi = 0; // Yerdeki çöp/leke miktarı

    // Not sistemini bir Enum (Liste) olarak tanımlıyoruz
    public enum SaglikNotu { A, B, C, D, F }

    [Header("Güncel Durum")]
    public SaglikNotu guncelNot = SaglikNotu.A;

    void Update()
    {
        // 1. DÜŞÜŞ MANTIĞI: Yerdeki çöp sayısına göre düşüş hızı katlanır
        float toplamDusus = saniyelikKirlenmeHizi + (dukkanCopSayisi * 0.015f);
        mevcutHijyen -= toplamDusus * Time.deltaTime;

        // 2. SINIRLANDIRMA: Puan 0'ın altına veya 5'in üstüne çıkamaz
        mevcutHijyen = Mathf.Clamp(mevcutHijyen, 0f, 5f);

        // 3. NOTU SÜREKLİ KONTROL ET
        NotuHesapla();
    }

    void NotuHesapla()
    {
        // Eski notu aklımızda tutalım ki, not değiştiğinde ekrana uyarı verebilelim
        SaglikNotu eskiNot = guncelNot;

        // 5 üzerinden senin belirlediğin aralıklara göre harf veriyoruz
        if (mevcutHijyen >= 4.0f) guncelNot = SaglikNotu.A;
        else if (mevcutHijyen >= 3.0f) guncelNot = SaglikNotu.B;
        else if (mevcutHijyen >= 2.0f) guncelNot = SaglikNotu.C;
        else if (mevcutHijyen >= 1.0f) guncelNot = SaglikNotu.D;
        else guncelNot = SaglikNotu.F;

        // EĞER NOT DÜŞERSE VEYA YÜKSELİRSE:
        if (eskiNot != guncelNot)
        {
            Debug.Log("<color=orange>DİKKAT: Dükkanın Sağlık Notu Değişti! Yeni Not: " + guncelNot + "</color>");

            // F'ye düşerse GDD'deki "Oyun Erken Biter" kuralı çalışır
            if (guncelNot == SaglikNotu.F)
            {
                DukkaniKapat();
            }
        }
    }

    // Paspas yapınca veya el yıkayınca diğer scriptlerden bu fonksiyonu çağıracağız
    public void TemizlikYap(float eklenecekPuan)
    {
        mevcutHijyen += eklenecekPuan;
        Debug.Log("Temizlik yapıldı! Hijyen arttı.");
    }

    void DukkaniKapat()
    {
        Debug.LogError("SAĞLIK BAKANLIĞI DÜKKANI MÜHÜRLEDİ! OYUN BİTTİ.");
        // Buraya Game Over ekranını çağırma kodunu ekleyeceğiz
        Time.timeScale = 0f; // Oyunu durdur
    }
}