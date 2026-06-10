using UnityEngine;

public class Cop : MonoBehaviour, IInteractable
{
    private HijyenYonetici hijyenYonetici;
    public float temizlikPuani = 0.5f; // Temizleyince gelecek memnuniyet/hijyen puanı

    void Start()
    {
        hijyenYonetici = FindObjectOfType<HijyenYonetici>();
        if (hijyenYonetici != null)
        {
            hijyenYonetici.dukkanCopSayisi++;
        }
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

        // Eğer elimizde bir malzeme varsa ve bu malzemenin tipi "supurge" ise
        if (eldekiMalzeme != null && eldekiMalzeme.malzemeTipi.ToLower() == "supurge")
        {
            if (hijyenYonetici != null)
            {
                // HijyenYonetici'deki TemizlikYap fonksiyonunu çağırıp puanı veriyoruz
                hijyenYonetici.TemizlikYap(temizlikPuani);
            }

            Debug.Log("<color=green>Çöp süpürüldü! Çöp kayboldu ve hijyen arttı.</color>");
            Destroy(this.gameObject); // Çöp objesini tamamen yok et
        }
        else
        {
            Debug.Log("Bu çöpü elle alamazsın, eline süpürgeyi alman lazım!");
        }
    }

    // Obje yok olduğunda dükkandaki toplam çöp sayısını düşürür
    void OnDestroy()
    {
        if (hijyenYonetici != null && gameObject.scene.isLoaded)
        {
            hijyenYonetici.dukkanCopSayisi--;
        }
    }
}