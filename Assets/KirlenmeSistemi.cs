using UnityEngine;

public class KirlenmeSistemi : MonoBehaviour
{
    [Header("Kirlilik Ayarları")]
    public GameObject[] copPrefablari; // Leke ve peçete prefablarını buraya at
    public Transform[] spawnNoktalari; // Dükkandaki boş objeleri buraya at
    public float kirlenmeAraligi = 20f; // Kaç saniyede bir çöp çıkacak? (Örn: 20 sn)

    private float zamanSayaci;

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

        // İPUCU: Eğer o noktada zaten çöp varsa üst üste çıkmasın
        // (Bunu küçük bir görünmez küre ile o noktayı kontrol ederek yapıyoruz)
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