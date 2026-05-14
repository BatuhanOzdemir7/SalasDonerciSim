using UnityEngine;
using UnityEngine.UI;

public class DonerMakinesi : MonoBehaviour
{
    [Header("Makine Durumu")]
    public bool makineAcikMi = false;
    public Animator donerAnimator;

    [Header("Etkileþim Ayarlarý")]
    public bool oyuncuYakindaMi = false;
    public GameObject eTusuGorseli;

    [Header("Piþme Ayarlarý")]
    public MeshRenderer donerEtiRenderer;
    public Material azPismisMat;
    public Material pismisMat;
    public Material cokPismisMat;

    public float pismeSuresi = 0f;
    public float pismisOlmaSiniri = 10f;
    public float cokPismisOlmaSiniri = 25f;

    [Header("Kesme ve Yükleme Çubuðu")]
    public GameObject arkaplanObjesi;
    public Image yuklemeCubugu;
    public float kesmeHizi = 1f;
    private float kesmeIlerlemesi = 0f;

    // YENÝ: Otomatik kesim durumunu takip eden anahtar
    public bool kesimYapiliyorMu = false;

    [Header("Düþen Et Ayarlarý")]
    public GameObject donerDilimPrefab;
    public Transform kesimNoktasi;

    void Start()
    {
        if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        if (eTusuGorseli != null) eTusuGorseli.SetActive(false);
    }

    void Update()
    {
        // 1. YAZI KONTROLÜ
        if (oyuncuYakindaMi && !makineAcikMi)
            if (eTusuGorseli != null) eTusuGorseli.SetActive(true);
            else
            if (eTusuGorseli != null) eTusuGorseli.SetActive(false);

        // 2. E TUÞU (Makineyi Aç/Kapat)
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            makineAcikMi = !makineAcikMi;
            if (!makineAcikMi) kesimYapiliyorMu = false; // Makine kapanýrsa kesimi de iptal et
        }

        // 3. DÖNME ve PÝÞME
        if (makineAcikMi)
        {
            if (donerAnimator != null) donerAnimator.SetBool("isSpinning", true);
            pismeSuresi += Time.deltaTime;

            if (pismeSuresi >= cokPismisOlmaSiniri) donerEtiRenderer.material = cokPismisMat;
            else if (pismeSuresi >= pismisOlmaSiniri) donerEtiRenderer.material = pismisMat;
            else donerEtiRenderer.material = azPismisMat;
        }
        else
        {
            if (donerAnimator != null) donerAnimator.SetBool("isSpinning", false);
        }

        // 4. Q TUÞU (YENÝ MEKANÝK: Tek týkla baþlat veya iptal et)
        if (oyuncuYakindaMi && makineAcikMi && Input.GetKeyDown(KeyCode.Q))
        {
            // True ise False yapar, False ise True yapar. (Açma/Kapatma þalteri gibi)
            kesimYapiliyorMu = !kesimYapiliyorMu;
        }

        // 5. OTOMATÝK KESÝM ÝÞLEMÝ
        if (kesimYapiliyorMu)
        {
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(true);

            kesmeIlerlemesi += Time.deltaTime * kesmeHizi; // Bar kendi kendine dolar
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = kesmeIlerlemesi;

            if (kesmeIlerlemesi >= 1f) // Çubuk %100 olduðunda
            {
                EtKesipDusur();
                kesmeIlerlemesi = 0f;
                if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
                pismeSuresi = 0f;
                kesimYapiliyorMu = false; // Et düþtü, sistemi bekleme moduna geri al
            }
        }
        else
        {
            // Kesim iptal edildiyse her þeyi gizle ve sýfýrla
            kesmeIlerlemesi = 0f;
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        }
    }

    void EtKesipDusur()
    {
        if (donerDilimPrefab != null && kesimNoktasi != null)
        {
            GameObject yeniDilim = Instantiate(donerDilimPrefab, kesimNoktasi.position, Quaternion.identity);
            MeshRenderer dilimRenderer = yeniDilim.GetComponent<MeshRenderer>();
            if (dilimRenderer != null && donerEtiRenderer != null) dilimRenderer.material = donerEtiRenderer.material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) oyuncuYakindaMi = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakindaMi = false;
            kesimYapiliyorMu = false; // Adam uzaklaþýrsa kesimi hemen iptal et
            kesmeIlerlemesi = 0f;
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        }
    }
}