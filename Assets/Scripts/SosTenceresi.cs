using UnityEngine;

public class SosTenceresi : MonoBehaviour, IInteractable
{
    [Header("Tencere Ayarları")]
    public Transform kepceninDuracagiYer;
    public GameObject icindekiKepce;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

        // DURUM 1: Oyuncunun elleri boş ve kazanda kepçe varsa -> Kepçeyi Al
        if (eldekiMalzeme == null && icindekiKepce != null)
        {
            Collider[] colliders = icindekiKepce.GetComponentsInChildren<Collider>(true);
            foreach (Collider c in colliders) c.enabled = false;

            oyuncu.PickUpItem(icindekiKepce);
            icindekiKepce = null;
            Debug.Log("Kepçe kazandan ele alındı ve lazeri engellememesi için TÜM collider'lar kapatıldı.");
        }
        // DURUM 2: Oyuncunun elinde bir 'Malzeme' varsa -> Kepçeyi Kazana Geri Koy
        else if (eldekiMalzeme != null)
        {
            string objeAdi = eldekiMalzeme.name.ToLower();

            if (objeAdi.Contains("kepce") || objeAdi.Contains("kepçe") || objeAdi.Contains("ladle"))
            {
                GameObject kepceObjesi = eldekiMalzeme.gameObject;

                oyuncu.EldenBirak();

                icindekiKepce = kepceObjesi;
                icindekiKepce.transform.SetParent(kepceninDuracagiYer != null ? kepceninDuracagiYer : transform);
                icindekiKepce.transform.localPosition = Vector3.zero;
                icindekiKepce.transform.localRotation = Quaternion.identity;

                Collider[] colliders = icindekiKepce.GetComponentsInChildren<Collider>(true);
                foreach (Collider c in colliders) c.enabled = true;

                Kepce kepceScript = icindekiKepce.GetComponent<Kepce>();
                if (kepceScript != null) kepceScript.SosuDoldur();

                Debug.Log("<color=green>BAŞARILI: Kepçe kazana geri kondu ve sosla dolduruldu.</color>");
            }
            else
            {
                Debug.LogWarning("UYARI: Kazana bir şey koymak istiyorsun ama elindeki obje Kepçe olarak tanınmadı! Adı: " + eldekiMalzeme.name);
            }
        }
        // DURUM 3: SESSİZ HATA YAKALAYICI (TUZAK)
        else if (eldekiMalzeme == null && icindekiKepce == null)
        {
            Debug.LogError("HATA YAKALANDI: Tencereye tıkladın ama oyun ellerinin BOMBOŞ olduğunu sanıyor! Lütfen Hiyerarşideki 'kepce' objesine tıklayıp Inspector panelinden 'Malzeme' (Malzeme.cs) scriptini eklediğinden emin ol.");
        }
    }
}