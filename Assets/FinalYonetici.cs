using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinalYonetici : MonoBehaviour
{
    [Header("UI Elementleri")]
    public TMP_Text zRaporuCiroYazisi;
    public TMP_Text saglikKoduYazisi;

    [Header("PNG Yıldız Sistemi")]
    [Tooltip("5 adet yıldız objesini sırasıyla (Soldan sağa) buraya sürükle")]
    public GameObject[] pngYildizlar; // Senin resim objelerin buraya gelecek

    void Start()
    {
        HesaplaVeEkranaBas();
    }

    void HesaplaVeEkranaBas()
    {
        // 1. TOPLAM CİRO HESABI
        float gun1Ciro = PlayerPrefs.GetFloat("Gun1_Ciro", 0f);
        float gun2Ciro = PlayerPrefs.GetFloat("Gun2_Ciro", 0f);
        float gun3Ciro = PlayerPrefs.GetFloat("Gun3_Ciro", 0f);
        float toplamCiro = gun1Ciro + gun2Ciro + gun3Ciro;

        if (zRaporuCiroYazisi != null)
            zRaporuCiroYazisi.text =toplamCiro.ToString() + "TL"  ;

        // 2. MÜŞTERİ MEMNUNİYETİ (PNG YILDIZ) HESABI
        float gun1Mem = PlayerPrefs.GetFloat("Gun1_Memnuniyet", 100f);
        float gun2Mem = PlayerPrefs.GetFloat("Gun2_Memnuniyet", 100f);
        float gun3Mem = PlayerPrefs.GetFloat("Gun3_Memnuniyet", 100f);
        float ortalamaMemnuniyet = (gun1Mem + gun2Mem + gun3Mem) / 3f;

        // Puan aralığına göre kaç yıldızın yanacağını belirliyoruz
        int yanacakYildizSayisi = 0;
        if (ortalamaMemnuniyet >= 90f) yanacakYildizSayisi = 5;
        else if (ortalamaMemnuniyet >= 70f) yanacakYildizSayisi = 4;
        else if (ortalamaMemnuniyet >= 50f) yanacakYildizSayisi = 3;
        else if (ortalamaMemnuniyet >= 30f) yanacakYildizSayisi = 2;
        else yanacakYildizSayisi = 1; // En kötü ihtimalle 1 yıldız verelim ayıp olmasın :)

        // Döngü ile yıldızları aç/kapat
        for (int i = 0; i < pngYildizlar.Length; i++)
        {
            if (pngYildizlar[i] != null)
            {
                // Eğer i değeri, yanması gereken sayıdan küçükse o yıldızı görünür yap
                pngYildizlar[i].SetActive(i < yanacakYildizSayisi);
            }
        }

        // 3. HİJYEN (SAĞLIK KODU) HESABI
        float toplamHijyen = HarfiPuanaCevir(PlayerPrefs.GetString("Gun1_Hijyen", "A")) +
                             HarfiPuanaCevir(PlayerPrefs.GetString("Gun2_Hijyen", "A")) +
                             HarfiPuanaCevir(PlayerPrefs.GetString("Gun3_Hijyen", "A"));

        float ortalamaHijyen = toplamHijyen / 3f;
        string finalNotu = PuaniHarfeCevir(ortalamaHijyen);

        if (saglikKoduYazisi != null)
            saglikKoduYazisi.text = finalNotu;
    }

    private float HarfiPuanaCevir(string harf)
    {
        if (harf == "A") return 4f;
        if (harf == "B") return 3f;
        if (harf == "C") return 2f;
        if (harf == "D") return 1f;
        return 0f; // F
    }

    private string PuaniHarfeCevir(float puan)
    {
        if (puan >= 3.5f) return "A";
        if (puan >= 2.5f) return "B";
        if (puan >= 1.5f) return "C";
        if (puan >= 0.5f) return "D";
        return "F";
    }

    // LAVAŞ BUTON 1: YENİDEN DENE
    public void YenidenDene()
    {
        // Gizli kasayı (paraları, günleri) tamamen sıfırla
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Oyunun zamanını normale döndür (gün sonunda durdurmuştuk)
        Time.timeScale = 1f;

        // Şu anki sahneyi (SampleScene) baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // LAVAŞ BUTON 2: OYUNU BİTİR
    public void OyunuBitir()
    {
        // Hafızayı temizle
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        // BURADA İKİ SEÇENEĞİN VAR:
        // Seçenek A: Ana Menüye döndürsün dersen bu açık kalsın
        SceneManager.LoadScene("MainMenu");

        // Seçenek B: Oyun direkt masaüstüne kapansın dersen üsttekini silip bunu aç:
        // Application.Quit(); 
    }
}