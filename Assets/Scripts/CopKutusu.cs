using UnityEngine;
using System.Collections;

public class CopKutusu : MonoBehaviour, IInteractable
{
    [Header("Animasyon Ayarlarý")]
    public Transform copKapagi;
    public float acikAci = -60f;
    public float acikKalmaSuresi = 0.5f;

    private bool isAnimating = false;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // Kapak zaten hareket halindeyse ardýþýk týklamalarý engelle
        if (isAnimating) return;

        // DURUM 1: Býçak engeli
        if (oyuncu.bicakVarMi)
        {
            Debug.Log("Býçaðý çöpe atamazsýn!");
            return;
        }

        // DURUM 2: Tepsi temizleme
        Tray eldekiTepsi = oyuncu.GetHeldTray();
        if (eldekiTepsi != null)
        {
            if (eldekiTepsi.tepsidekiEtSayisi == 0 && !eldekiTepsi.isDurum && eldekiTepsi.eklenenMalzemeler.Count == 0)
            {
                Debug.Log("Tepsi zaten temiz!");
            }
            else
            {
                eldekiTepsi.TepsiyiSifirla();
                Debug.Log("Tepsinin içindekiler çöpe döküldü, tepsi temizlendi.");
                StartCoroutine(KapakAnimasyonu());
            }
            return;
        }

        // DURUM 3: Malzeme atma
        if (oyuncu.GetHeldMalzeme() != null)
        {
            oyuncu.EldenBirakVeSil();
            Debug.Log("Malzeme çöpe atýldý!");
            StartCoroutine(KapakAnimasyonu());
            return;
        }

        Debug.Log("Çöpe atacak bir þeyin yok.");
    }

    private IEnumerator KapakAnimasyonu()
    {
        isAnimating = true;

        // Kapaðýn mevcut kapalý açýsýný hafýzaya alýyoruz
        Quaternion baslangicRot = copKapagi.localRotation;

        // Çöp kutusu kapaklarý genelde X ekseninde yukarý doðru açýlýr. 
        // Eðer senin modelinde kapak yana doðru veya saçma bir yöne açýlýrsa,
        // acikAci deðerini Y (0, acikAci, 0) veya Z (0, 0, acikAci) kýsmýna kaydýrabilirsin.
        Quaternion hedefRot = Quaternion.Euler(acikAci, 0, 0);

        float animasyonSuresi = 0.2f; // Kapaðýn ne kadar sürede açýlacaðý (çok hýzlý)
        float gecenSure = 0f;

        // 1. KAPAÐI AÇ
        while (gecenSure < animasyonSuresi)
        {
            gecenSure += Time.deltaTime;
            copKapagi.localRotation = Quaternion.Slerp(baslangicRot, hedefRot, gecenSure / animasyonSuresi);
            yield return null;
        }
        copKapagi.localRotation = hedefRot;

        // 2. AÇIK BEKLE (Çöpün içine düþme hissi için)
        yield return new WaitForSeconds(acikKalmaSuresi);

        // 3. KAPAÐI KAPAT
        gecenSure = 0f;
        while (gecenSure < animasyonSuresi)
        {
            gecenSure += Time.deltaTime;
            copKapagi.localRotation = Quaternion.Slerp(hedefRot, baslangicRot, gecenSure / animasyonSuresi);
            yield return null;
        }
        copKapagi.localRotation = baslangicRot;

        isAnimating = false;
    }
}