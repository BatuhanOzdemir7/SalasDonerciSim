using UnityEngine;

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
        // YENÝ STANDART E TUÞU: Sadece Taþýma (Alma / Býrakma)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TasimaAksiyonu();
        }

        // YENÝ STANDART F TUÞU: Sadece Ýþlem (Dürüm Sarma / Hesap Alma)
        if (Input.GetKeyDown(KeyCode.F))
        {
            IslemAksiyonu();
        }
    }

    void TasimaAksiyonu()
    {
        if (isinCikisNoktasi == null) return;

        RaycastHit hit;
        if (Physics.Raycast(isinCikisNoktasi.position, isinCikisNoktasi.forward, out hit, etkilesimMesafesi))
        {
            // FÝLTRE 1: Kasa iþlemi F tuþuna aittir. E tuþu kasayý tetiklemesin.
            if (hit.collider.GetComponentInParent<KasaYonetici>() != null) return;

            IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
            if (etkilesimliObje != null)
            {
                etkilesimliObje.Interact(this);
                return;
            }
        }

        // AKILLI DÜÞÜÞ
        if (eldeTutulanObje != null)
        {
            Tray eldekiTepsi = GetHeldTray();
            if (eldekiTepsi != null)
            {
                Debug.Log("Uyarý: Tepsiyi sadece uygun istasyonlara býrakabilirsiniz!");
                return;
            }
            EldenBirak();
        }
    }

    void IslemAksiyonu()
    {
        if (isinCikisNoktasi == null) return;

        RaycastHit hit;
        if (Physics.Raycast(isinCikisNoktasi.position, isinCikisNoktasi.forward, out hit, etkilesimMesafesi))
        {
            // FÝLTRE 2: F tuþu sadece ÝÞLEM yapýlan objelerde (Tepsi, Ýstasyon, Kasa) lazeri çalýþtýrýr!
            // Buzdolabý, býçak, fritöz, kepçe gibi "Taþýma" objelerini tamamen yok sayar.
            bool islemIstasyonuMu = hit.collider.GetComponentInParent<Tray>() != null ||
                                    hit.collider.GetComponentInParent<TepsiBirakmaNoktasi>() != null ||
                                    hit.collider.GetComponentInParent<KasaYonetici>() != null;

            if (islemIstasyonuMu)
            {
                IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
                if (etkilesimliObje != null)
                {
                    etkilesimliObje.Interact(this);
                }
            }
        }
    }

    public void PickUpItem(GameObject alinacakObje)
    {
        if (eldeTutulanObje != null) return;

        eldeTutulanObje = alinacakObje;
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