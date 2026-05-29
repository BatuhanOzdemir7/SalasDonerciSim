using UnityEngine;

public class Kepce : MonoBehaviour
{
    [Header("Sos Görseli")]
    public GameObject sosGorseli;
    public bool doluMu = true;

    // YENİ: Başlangıç konumu hafızası
    private Vector3 baslangicPozisyonu;
    private Quaternion baslangicRotasyonu;

    void Start()
    {
        // Oyun başladığı an durduğu o ilk yeri ve açıyı hafızaya kazır
        baslangicPozisyonu = transform.position;
        baslangicRotasyonu = transform.rotation;
    }

    public void SosuKullan()
    {
        doluMu = false;
        if (sosGorseli != null) sosGorseli.SetActive(false);
    }

    public void SosuDoldur()
    {
        doluMu = true;
        if (sosGorseli != null) sosGorseli.SetActive(true);
    }

    // YENİ: G'ye basılınca çağrılacak ışınlanma fonksiyonu
    public void IstasyonaDon()
    {
        transform.position = baslangicPozisyonu;
        transform.rotation = baslangicRotasyonu;

        // Yerçekiminden etkilenip düşmemesi için
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Yeniden elimize alabilelim diye collider'ı açıyoruz
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }
}