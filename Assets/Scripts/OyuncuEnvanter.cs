using UnityEngine;

public class OyuncuEnvanter : MonoBehaviour
{
    [Header("Býçak Durumu")]
    public bool bicakVarMi = false;

    [Header("Gerekli Bileþenler")]
    public Transform elNoktasi;
    public Animator oyuncuAnimator;

    [HideInInspector] public Bicak suAnkiBicakScripti;

    [Header("Yeni Etkileþim Ayarlarý (Tabak/Tepsi)")]
    public Transform isinCikisNoktasi;
    public float etkilesimMesafesi = 3f;

    // Eldeki tabaðý veya tepsiyi takip etmek için
    private GameObject eldeTutulanObje;

    void Update()
    {
        // F tuþuna basýldýðýnda kameranýn baktýðý yerdeki objeyle etkileþime gir
        if (Input.GetKeyDown(KeyCode.F))
        {
            EtkilesimiKontrolEt();
        }
    }

    void EtkilesimiKontrolEt()
    {
        if (isinCikisNoktasi == null)
        {
            Debug.LogError("HATA: 'Isin Cikis Noktasi' Inspector panelinden atanmamýþ!");
            return;
        }

        RaycastHit hit;

        // Görmek için test amaçlý mesafeyi 10 birim yapýyoruz (Inspector'dan da deðiþtirebilirsin)
        if (Physics.Raycast(isinCikisNoktasi.position, isinCikisNoktasi.forward, out hit, etkilesimMesafesi))
        {
            // KONTROL 1: Lazer sahnedeki herhangi bir þeye çarpýyor mu?
            Debug.Log("LAZER ÞUNA ÇARPTI: " + hit.collider.gameObject.name);

            IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
            if (etkilesimliObje != null)
            {
                // KONTROL 2: Çarptýðý objede kodlar kurulu mu?
                Debug.Log("BAÞARILI: Objede IInteractable bulundu, Interact tetikleniyor.");
                etkilesimliObje.Interact(this);
            }
            else
            {
                Debug.LogWarning("UYARI: Objeye çarptým ama üzerinde IInteractable scripti yok!");
            }
        }
        else
        {
            // KONTROL 3: Lazer hiçbir þeye çarpmýyor mu?
            Debug.Log("BOÞA GÝTTÝ: Lazer menzilinde hiçbir collider bulamadý.");
        }

        // Scene (Sahne) ekranýnda kýrmýzý çizgiyi net olarak 2 saniye boyunca gösterir
        Debug.DrawRay(isinCikisNoktasi.position, isinCikisNoktasi.forward * etkilesimMesafesi, Color.red, 2f);
    }

    // --- YENÝ EKLENEN FONKSÝYONLAR (Hatalarý Çözen Kýsým) ---

    // Tabak ve Tepsilerin anýnda ele alýnmasýný saðlayan fonksiyon
    public void PickUpItem(GameObject alinacakObje)
    {
        if (eldeTutulanObje != null)
        {
            Debug.Log("Elinde zaten bir obje var!");
            return;
        }

        eldeTutulanObje = alinacakObje;

        // Objeyi ele sabitle
        eldeTutulanObje.transform.SetParent(elNoktasi);
        eldeTutulanObje.transform.localPosition = Vector3.zero;
        eldeTutulanObje.transform.localRotation = Quaternion.identity;

        // Objeyi tutarken saða sola çarpýp fiziði bozmamasý için ayarlar
        Collider col = eldeTutulanObje.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = eldeTutulanObje.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    // Çöp, Fritöz ve Salata Barýnýn eldeki tabaðýn verisine ulaþmasý için
    public Plate GetHeldPlate()
    {
        if (eldeTutulanObje != null)
        {
            return eldeTutulanObje.GetComponent<Plate>();
        }
        return null;
    }

    // --- SENÝN MEVCUT ANÝMASYON FONKSÝYONUN (Hiç Dokunulmadý) ---

    // Animasyon tam elin nesneye deðdiði an bu fonksiyonu çaðýracak (Animation Event)
    public void NesneyiEleYapistirEvent()
    {
        // 1. BIÇAK ÝÇÝN ALMA MANTIÐI
        if (suAnkiBicakScripti != null)
        {
            suAnkiBicakScripti.BicagiEleIsinla();
            suAnkiBicakScripti = null; // Ýþ bitince hafýzayý temizle
        }

        // BU SATIR SÝHRÝ YAPACAK: Animasyonun loopta kalmasýný engeller, tetikleyiciyi sýfýrlar!
        if (oyuncuAnimator != null)
        {
            oyuncuAnimator.ResetTrigger("isPickingUp");
        }
    }
}