using UnityEngine;
using System.Linq; // Sýralama yapabilmemiz için bu kütüphaneyi ekledik

public class OyuncuEnvanter : MonoBehaviour
{
    [Header("Býçak Durumu")]
    public bool bicakVarMi = false;

    [Header("Gerekli Bileþenler")]
    public Transform elNoktasi;
    public Animator oyuncuAnimator;
    [HideInInspector] public Bicak suAnkiBicakScripti;

    [Header("Etkileþim Ayarlarý (Lazer)")]
    public Transform isinCikisNoktasi;
    public float etkilesimMesafesi = 3f;

    private GameObject eldeTutulanObje;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TasimaAksiyonu();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            IslemAksiyonu();
        }
    }

    void TasimaAksiyonu()
    {
        if (isinCikisNoktasi == null) return;

        RaycastHit[] hits = Physics.RaycastAll(isinCikisNoktasi.position, isinCikisNoktasi.forward, etkilesimMesafesi);

        // =========================================================================
        // JÝLET GÝBÝ SIRALAMA: Çarptýðýmýz her þeyi mesafeye göre (yakýndan uzaða) diziyoruz!
        // =========================================================================
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // F Tuþunun iþlerini es geçiyoruz
            if (hit.collider.GetComponentInParent<KasaYonetici>() != null) continue;
            if (hit.collider.GetComponentInParent<MusteriAI>() != null) continue;

            // =========================================================================
            // ÝÇECEK/TEPSÝ DÜZELTMESÝ: Eðer elimde bir þey varsa ve baktýðým yerde Tepsi (Tray) varsa, 
            // direkt tepsiye odaklan, araya giren boþ kutularý yok say!
            // =========================================================================
            Tray tepsi = hit.collider.GetComponentInParent<Tray>();
            if (tepsi != null && eldeTutulanObje != null)
            {
                IInteractable tepsiEtkilesim = tepsi.GetComponent<IInteractable>();
                if (tepsiEtkilesim != null)
                {
                    tepsiEtkilesim.Interact(this);
                    return; // Ýçeceði koyduk, çýk!
                }
            }

            // Normal etkileþimler (Buzdolabýndan alma vb.)
            IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
            if (etkilesimliObje != null)
            {
                etkilesimliObje.Interact(this);
                return;
            }

            // Doðrudan malzeme (Lavaþ, Kola vb.) yerden/dolaptan alma
            Malzeme malzemeScripti = hit.collider.GetComponentInParent<Malzeme>();
            if (malzemeScripti != null && eldeTutulanObje == null)
            {
                PickUpItem(malzemeScripti.gameObject);
                return;
            }
        }

        if (eldeTutulanObje != null)
        {
            Debug.Log("<color=orange>Uyarý: Elindeki eþyayý boþluða býrakamazsýn!</color>");
            return;
        }
    }

    void IslemAksiyonu()
    {
        if (isinCikisNoktasi == null) return;

        RaycastHit[] hits = Physics.RaycastAll(isinCikisNoktasi.position, isinCikisNoktasi.forward, etkilesimMesafesi);

        // F tuþu için de ayný kusursuz sýralamayý yapýyoruz
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            MusteriAI musteri = hit.collider.GetComponentInParent<MusteriAI>();
            if (musteri != null)
            {
                musteri.Interact(this);
                return;
            }

            bool islemIstasyonuMu = hit.collider.GetComponentInParent<Tray>() != null ||
                                    hit.collider.GetComponentInParent<KasaYonetici>() != null;

            if (islemIstasyonuMu)
            {
                IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
                if (etkilesimliObje != null)
                {
                    etkilesimliObje.Interact(this);
                    return;
                }
            }
        }
    }

    public void PickUpItem(GameObject alinacakObje)
    {
        if (eldeTutulanObje != null) return;

        eldeTutulanObje = alinacakObje;

        // =========================================================================
        // JÝLET GÝBÝ ÇÖZÜM: Obje klonlanýrken kapalý gelmiþse bile zorla GÖRÜNÜR YAP!
        // =========================================================================
        eldeTutulanObje.SetActive(true);

        eldeTutulanObje.transform.SetParent(elNoktasi);
        eldeTutulanObje.transform.localPosition = Vector3.zero;
        eldeTutulanObje.transform.localRotation = Quaternion.identity;

        Collider col = eldeTutulanObje.GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = eldeTutulanObje.GetComponentInChildren<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }
    public void EldenBirak()
    {
        if (eldeTutulanObje != null)
        {
            Kepce kepceScript = eldeTutulanObje.GetComponentInChildren<Kepce>();
            eldeTutulanObje.transform.SetParent(null);

            if (kepceScript != null)
            {
                kepceScript.IstasyonaDon();
            }
            else
            {
                Collider col = eldeTutulanObje.GetComponentInChildren<Collider>();
                if (col != null) col.enabled = true;

                Rigidbody rb = eldeTutulanObje.GetComponentInChildren<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
            }
            eldeTutulanObje = null;
        }
    }

    public void EldenBirakVeSil()
    {
        if (eldeTutulanObje != null)
        {
            Destroy(eldeTutulanObje);
            eldeTutulanObje = null;
        }
    }

    public Tray GetHeldTray()
    {
        if (eldeTutulanObje != null)
        {
            return eldeTutulanObje.GetComponentInChildren<Tray>();
        }
        return null;
    }

    public Malzeme GetHeldMalzeme()
    {
        if (eldeTutulanObje != null)
        {
            return eldeTutulanObje.GetComponentInChildren<Malzeme>();
        }
        return null;
    }

    public void NesneyiEleYapistirEvent()
    {
        if (suAnkiBicakScripti != null)
        {
            suAnkiBicakScripti.BicagiEleIsinla();
        }
        if (oyuncuAnimator != null)
        {
            oyuncuAnimator.ResetTrigger("isPickingUp");
        }
    }
}