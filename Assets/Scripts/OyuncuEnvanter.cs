using UnityEngine;

public class OyuncuEnvanter : MonoBehaviour
{
    [Header("Býçak Durumu")]
    public bool bicakVarMi = false;

    [Header("Gerekli Bileþenler")]
    public Transform elNoktasi;
    public Animator oyuncuAnimator;

    [HideInInspector] public Bicak suAnkiBicakScripti;

    [Header("Yeni Etkileþim Ayarlarý (Tepsi)")]
    public Transform isinCikisNoktasi;
    public float etkilesimMesafesi = 3f;

    private GameObject eldeTutulanObje;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EtkilesimiKontrolEt();
        }
    }

    void EtkilesimiKontrolEt()
    {
        if (isinCikisNoktasi == null) return;

        RaycastHit hit;
        if (Physics.Raycast(isinCikisNoktasi.position, isinCikisNoktasi.forward, out hit, etkilesimMesafesi))
        {
            Debug.Log("LAZER ÞUNA ÇARPTI: " + hit.collider.gameObject.name);

            IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
            if (etkilesimliObje != null)
            {
                etkilesimliObje.Interact(this);
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

        Collider col = eldeTutulanObje.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = eldeTutulanObje.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    // ARTIK TABAK DEÐÝL TEPSÝ DÖNDÜRÜYOR
    public Tray GetHeldTray()
    {
        if (eldeTutulanObje != null)
        {
            return eldeTutulanObje.GetComponent<Tray>();
        }
        return null;
    }

    public void NesneyiEleYapistirEvent()
    {
        if (suAnkiBicakScripti != null)
        {
            suAnkiBicakScripti.BicagiEleIsinla();
            suAnkiBicakScripti = null;
        }

        if (oyuncuAnimator != null)
        {
            oyuncuAnimator.ResetTrigger("isPickingUp");
        }
    }

    // Oyuncunun elindeki objeyi referanstan temizler, böylece býçak veya baþka eþya alabilir
    public void EldenBirak()
    {
        eldeTutulanObje = null;
    }
}