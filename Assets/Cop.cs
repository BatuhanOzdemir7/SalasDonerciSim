using UnityEngine;

public class Cop : MonoBehaviour
{
    private HijyenYonetici hijyenYonetici;

    void Start()
    {
        // Sahnedeki ana hijyen yöneticisini bul
        hijyenYonetici = FindObjectOfType<HijyenYonetici>();

        if (hijyenYonetici != null)
        {
            // Obje spawn olduğu an dükkandaki toplam çöp sayısını 1 artır
            // Bu sayede HijyenYonetici'deki puan düşüş hızı katlanarak artar
            hijyenYonetici.dukkanCopSayisi++;
        }
    }

    // Paspasla bu çöpü sildiğimizde (Destroy edildiğinde) bu fonksiyon OTOMATİK çalışır
    void OnDestroy()
    {
        if (hijyenYonetici != null)
        {
            hijyenYonetici.dukkanCopSayisi--; // Çöp silindi, dükkan biraz rahatladı
        }
    }
}