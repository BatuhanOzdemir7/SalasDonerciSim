using UnityEngine;

public class SosTenceresi : MonoBehaviour, IInteractable
{
    [Header("Tencere Ayarları")]
    public Transform kepceninDuracagiYer;
    public GameObject icindekiKepce;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

        // 1. KEPÇE ALMA MANTIĞI
        if (eldekiMalzeme == null && icindekiKepce != null)
        {
            // Kepçeyi oyuncuya ver
            oyuncu.PickUpItem(icindekiKepce);

            // Tenceredeki referansı temizle ki artık "boş" bilsin
            icindekiKepce = null;

            Debug.Log("Kepçe tencereden alındı.");
        }
        // 2. KEPÇE KOYMA MANTIĞI
        else if (eldekiMalzeme != null)
        {
            // Elindeki nesnenin kepçe olup olmadığını kontrol et
            Kepce kepceScript = eldekiMalzeme.GetComponentInChildren<Kepce>();

            if (kepceScript != null)
            {
                // Oyuncunun elindeki kepçeyi tencerenin konumuna sabitle
                GameObject kepceObjesi = eldekiMalzeme.gameObject;
                oyuncu.EldenBirak(); // Oyuncu elinden bırakır

                // Tencere artık kepçeyi "içinde" tutuyor
                icindekiKepce = kepceObjesi;

                // Kepçeyi tencereye ışınla
                icindekiKepce.transform.SetParent(kepceninDuracagiYer != null ? kepceninDuracagiYer : transform);
                icindekiKepce.transform.localPosition = Vector3.zero;
                icindekiKepce.transform.localRotation = Quaternion.identity;

                // Fiziği dondur ve istasyona dönme kodunu tetikle
                kepceScript.IstasyonaDon();

                Debug.Log("Kepçe tencereye başarıyla geri konuldu.");
            }
            else
            {
                Debug.Log("Bu tencereye sadece kepçe konulabilir.");
            }
        }
        // 3. BOŞ TENCERE DURUMU
        else if (icindekiKepce == null)
        {
            Debug.Log("Tencere şu an boş.");
        }
    }
}