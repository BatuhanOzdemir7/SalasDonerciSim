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

        // F TUÞU: Akýllý Sistem - Hem Alýr/Etkileþir hem de (boþluða bakýyorsa) Geri Býrakýr

        if (Input.GetKeyDown(KeyCode.F))

        {

            FTusuAksiyonu();

        }



        // Q TUÞU: Sadece normal etkileþim (Döner kesme mekaniðiyle çakýþmasýn diye)

        if (Input.GetKeyDown(KeyCode.Q))

        {

            EtkilesimiKontrolEt();

        }

    }

    void FTusuAksiyonu()
    {
        if (isinCikisNoktasi == null) return;

        RaycastHit hit;
        // Önümüzde etkileþime geçebileceðimiz bir nesne (Tencere, Çöp, Masa vs.) var mý?
        if (Physics.Raycast(isinCikisNoktasi.position, isinCikisNoktasi.forward, out hit, etkilesimMesafesi))
        {
            IInteractable etkilesimliObje = hit.collider.GetComponentInParent<IInteractable>();
            if (etkilesimliObje != null)
            {
                // Eðer etkileþimli bir nesne varsa onu çalýþtýr 
                etkilesimliObje.Interact(this);
                return; // Etkileþim gerçekleþtiði için býrakma mantýðýna geçme, burada fonksiyonu bitir.
            }
        }

        // AKILLI DÜÞÜÞ GÜNCELLEMESÝ V2: Eðer önümüzde etkileþime geçecek HÝÇBÝR ÞEY yoksa VE elimiz doluysa...
        if (eldeTutulanObje != null)
        {
            // Önceden sadece tepsiyi engelliyordu, artýk Malzeme, Býçak ve Kepçe dahil HÝÇBÝR ÞEYÝN
            // boþlukta F'ye basýlarak yere/havaya atýlmasýna izin vermiyoruz.
            Debug.Log("Uyarý: Elindeki eþyayý boþluða býrakamazsýn! Uygun bir istasyona veya çöpe atmalýsýn.");
            return;
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

                // Kepçeyse bumerang gibi tezgahtaki orijinal yerine ýþýnlanýr

                kepceScript.IstasyonaDon();

            }

            else

            {

                // Diðer malzemelerse (tavuk, kola vs.) fiziksel olarak yere/tezgaha düþer

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