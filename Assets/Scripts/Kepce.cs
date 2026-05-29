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
        // 1. Rigidbody'i (Fiziği) bul ve yerçekimini/hızını tamamen dondur
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = GetComponentInChildren<Rigidbody>(); // Bulamazsa çocuklara bak

        if (rb != null)
        {
            rb.velocity = Vector3.zero;        // Düşme hızını sıfırla
            rb.angularVelocity = Vector3.zero; // Dönme hızını sıfırla
            rb.isKinematic = true;             // Yerçekimini ve çarpmaları kapat
        }

        // 2. Kepçeyi ilk günkü yerine ışınla
        transform.position = baslangicPozisyonu;
        transform.rotation = baslangicRotasyonu;

        // 3. Etkileşime girebilmek için Collider'ı (Fiziksel Kutuyu) geri aç
        Collider col = GetComponent<Collider>();
        if (col == null) col = GetComponentInChildren<Collider>();

        if (col != null) col.enabled = true;

        // (Ekstra) Yerine dönünce sosu fulle
        SosuDoldur();
    }
}