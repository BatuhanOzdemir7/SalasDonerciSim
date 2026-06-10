using UnityEngine;

public class KirlenmeSistemi : MonoBehaviour
{
    [Header("Kirlilik Ayarları")]
    public GameObject[] copPrefablari; // Leke ve peçete prefablarını buraya at
    public Transform[] spawnNoktalari; // Dükkandaki boş objeleri buraya at
    public float kirlenmeAraligi;

    private float zamanSayaci;

    void Start()
    {
        // HATA ÇÖZÜMÜ: Başka scriptleri beklemek yerine, gün bilgisini 
        // doğrudan oyunun kayıt defterinden (PlayerPrefs) çekiyoruz.
        int gun = PlayerPrefs.GetInt("KayitliGun", 1);

        if (gun == 1) kirlenmeAraligi = 60f;
        else if (gun == 2) kirlenmeAraligi = 45f;
        else if (gun >= 3) kirlenmeAraligi = 30f;

        Debug.Log("<color=yellow>Kirlenme Sistemi Başladı: " + gun + ". Gün zorluğu aktif (" + kirlenmeAraligi + " saniyede bir çöp çıkacak).</color>");
    }

    void Update()
    {
        zamanSayaci += Time.deltaTime;

        // Süre dolduğunda yeni bir çöp oluştur
        if (zamanSayaci >= kirlenmeAraligi)
        {
            CopOlustur();
            zamanSayaci = 0f; // Sayacı sıfırla ve baştan başla
        }
    }

    void CopOlustur()
    {
        if (spawnNoktalari.Length == 0 || copPrefablari.Length == 0) return;

        // Rastgele bir nokta ve rastgele bir leke türü seç
        int rastgeleNokta = Random.Range(0, spawnNoktalari.Length);
        int rastgeleCop = Random.Range(0, copPrefablari.Length);

        Transform secilenNokta = spawnNoktalari[rastgeleNokta];

        // Eğer o noktada zaten çöp varsa üst üste çıkmasın
        Collider[] etraftakiler = Physics.OverlapSphere(secilenNokta.position, 0.5f);
        bool burasiDolu = false;

        foreach (Collider col in etraftakiler)
        {
            if (col.GetComponent<Cop>() != null) burasiDolu = true;
        }

        // Eğer nokta boşsa çöpü fırlat!
        if (!burasiDolu)
        {
            Instantiate(copPrefablari[rastgeleCop], secilenNokta.position, secilenNokta.rotation);
            Debug.Log("Dükkanda yeni bir kirlilik oluştu!");
        }
    }
}