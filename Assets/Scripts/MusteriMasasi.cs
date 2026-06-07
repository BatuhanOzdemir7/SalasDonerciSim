using UnityEngine;

public class MusteriMasasi : MonoBehaviour, IInteractable
{
    [Header("Masa Ayarlarý")]
    public Transform tepsininDuracagiYer;
    public Tray ustundekiTepsi;

    [Header("Müþteri Algýlama (Radar)")]
    public float algilamaYaricapi = 2.5f; // Masanýn etrafýný tarama mesafesi

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // DURUM 1: MASAYA SERVÝS YAPMA
        if (ustundekiTepsi == null)
        {
            Tray eldekiTepsi = oyuncu.GetHeldTray();

            if (eldekiTepsi != null)
            {
                ustundekiTepsi = eldekiTepsi;

                ustundekiTepsi.transform.SetParent(null);
                ustundekiTepsi.transform.position = tepsininDuracagiYer.position;
                ustundekiTepsi.transform.rotation = tepsininDuracagiYer.rotation;
                ustundekiTepsi.transform.localScale = ustundekiTepsi.orijinalBoyut;

                Collider col = ustundekiTepsi.GetComponent<Collider>();
                if (col == null) col = ustundekiTepsi.GetComponentInChildren<Collider>();
                if (col != null) col.enabled = true;

                oyuncu.EldenBirak();

                // ==========================================
                // DÝNAMÝK MÜÞTERÝ BULMA RADARI (Sürükle býrak bitti!)
                // ==========================================
                MusteriAI bulunanMusteri = null;

                // Masanýn etrafýndaki belirli bir çapý tarar
                Collider[] etraftakiler = Physics.OverlapSphere(transform.position, algilamaYaricapi);

                foreach (Collider c in etraftakiler)
                {
                    MusteriAI musteri = c.GetComponentInParent<MusteriAI>();
                    if (musteri == null) musteri = c.GetComponent<MusteriAI>();

                    // Eðer etrafta biri varsa ve o an "Sipariþ Bekliyorsa" bizim müþterimiz odur!
                    if (musteri != null && musteri.suAnkiDurum == MusteriAI.MusteriDurumu.SiparisBekliyor)
                    {
                        bulunanMusteri = musteri;
                        break; // Müþteriyi bulduk, taramayý durdur
                    }
                }

                // Bulduðumuz o dinamik müþteriye tepsiyi kontrol ettiriyoruz
                if (bulunanMusteri != null)
                {
                    bulunanMusteri.TabagiDegerlendir(ustundekiTepsi.gameObject);
                }
                else
                {
                    Debug.Log("Masa: Etrafýmda sipariþ bekleyen bir müþteri bulamadým!");
                }
            }
        }
        // DURUM 2: MASADAN TEPSÝYÝ GERÝ ALMA
        else
        {
            Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

            if (oyuncu.GetHeldTray() == null && !oyuncu.bicakVarMi && eldekiMalzeme == null)
            {
                // ==========================================
                // HIRSIZLIK KORUMASI: Etrafta yemek yiyen biri var mý?
                // ==========================================
                bool musteriYemekYiyorMu = false;
                Collider[] etraftakiler = Physics.OverlapSphere(transform.position, algilamaYaricapi);

                foreach (Collider c in etraftakiler)
                {
                    MusteriAI m = c.GetComponentInParent<MusteriAI>();
                    if (m != null && m.suAnkiDurum == MusteriAI.MusteriDurumu.YemekYiyor)
                    {
                        musteriYemekYiyorMu = true;
                        break;
                    }
                }

                if (musteriYemekYiyorMu)
                {
                    Debug.Log("Hop! Müþteri yemeðini yiyor, tepsiyi çalamazsýn!");
                    return; // Ýptal et
                }

                oyuncu.PickUpItem(ustundekiTepsi.gameObject);
                ustundekiTepsi = null;
                Debug.Log("Tepsi masadan baþarýyla geri alýndý.");
            }
        }
    }

    // Unity Editöründe radarýn ne kadar büyük olduðunu sarý bir küre ile görmeni saðlar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, algilamaYaricapi);
    }
}