using UnityEngine;

public class FisYonetici : MonoBehaviour
{
    public static FisYonetici Instance;

    [Header("Fiþ Sistemi Ayarlarý")]
    public GameObject fisPrefab; // Klasördeki SiparisFisi prefabý
    public Transform fislikPaneli; // Sahnedeki FislikPaneli

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void YeniFisOlustur(string musteriAdi)
    {
        if (fisPrefab != null && fislikPaneli != null)
        {
            // Fiþi oluþtur ve FislikPaneli'nin içine koy
            GameObject yeniFis = Instantiate(fisPrefab, fislikPaneli);

            // Fiþ panelde en son sýraya (en saða) geçsin
            yeniFis.transform.SetAsLastSibling();

            Debug.Log("Mutfaktan sesler geliyor... " + musteriAdi + " için fiþ basýldý!");
        }
    }
}