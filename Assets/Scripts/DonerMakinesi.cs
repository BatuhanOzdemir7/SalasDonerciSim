using UnityEngine;
using UnityEngine.UI;

public class DonerMakinesi : MonoBehaviour
{
    [Header("Makine Durumu")]
    public bool makineAcikMi = false;
    public Animator donerAnimator;

    [Header("Sýcaklýk ve Soðuma Ayarlarý")]
    public bool donerSogukMu = false;
    public float sogumaSayaci = 0f;
    public float sogumaSiniri = 5f; // 5 saniye kuralý

    [Header("Isýtma Arayüzü")]
    public GameObject isinmaArayuzObjesi;
    public Image isinmaBarGorseli;
    private float isinmaIlerlemesi = 0f;
    private bool isiniyorMu = false;

    [Header("Etkileþim Ayarlarý")]
    public bool oyuncuYakindaMi = false;
    public GameObject eTusuGorseli;
    private OyuncuEnvanter makineyeYakinOyuncu;

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
    public bool kesimYapiliyorMu = false;

    [Header("Düþen Et Ayarlarý")]
    public GameObject donerDilimPrefab;
    public Transform kesimNoktasi;

    void Start()
    {
        if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        if (eTusuGorseli != null) eTusuGorseli.SetActive(false);
        if (isinmaArayuzObjesi != null) isinmaArayuzObjesi.SetActive(false);
    }

    void Update()
    {
        if (isiniyorMu)
        {
            DöneriIsit();
            return;
        }

        // 1. YAZI KONTROLÜ (GÜNCELLENDÝ: Mantýk hatasý giderildi, yakýndayken yazý hep tetiklenecek)
        if (oyuncuYakindaMi && !kesimYapiliyorMu)
        {
            if (eTusuGorseli != null) eTusuGorseli.SetActive(true);
        }
        else
        {
            if (eTusuGorseli != null) eTusuGorseli.SetActive(false);
        }

        // 2. E TUÞU (Açma/Kapama ve Isýtma)
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            if (donerSogukMu && !makineAcikMi)
            {
                if (eTusuGorseli != null) eTusuGorseli.SetActive(false);
                isiniyorMu = true;
                if (isinmaArayuzObjesi != null) isinmaArayuzObjesi.SetActive(true);
            }
            else
            {
                makineAcikMi = !makineAcikMi;
            }
        }

        // 3. DÖNME, PÝÞME ve SOÐUMA
        if (makineAcikMi)
        {
            if (donerAnimator != null) donerAnimator.SetBool("isSpinning", true);
            sogumaSayaci = 0f;

            if (pismeSuresi < cokPismisOlmaSiniri)
            {
                pismeSuresi += Time.deltaTime;
            }

            if (pismeSuresi >= cokPismisOlmaSiniri) donerEtiRenderer.material = cokPismisMat;
            else if (pismeSuresi >= pismisOlmaSiniri) donerEtiRenderer.material = pismisMat;
            else donerEtiRenderer.material = azPismisMat;
        }
        else
        {
            if (donerAnimator != null) donerAnimator.SetBool("isSpinning", false);

            sogumaSayaci += Time.deltaTime;
            if (sogumaSayaci >= sogumaSiniri)
            {
                donerSogukMu = true;
            }
        }

        // 4. Q TUÞU (Kesim Baþlatma)
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.Q))
        {
            if (makineyeYakinOyuncu != null && makineyeYakinOyuncu.bicakVarMi)
            {
                kesimYapiliyorMu = !kesimYapiliyorMu;

                if (makineyeYakinOyuncu.oyuncuAnimator != null)
                {
                    makineyeYakinOyuncu.oyuncuAnimator.SetBool("isCutting", kesimYapiliyorMu);
                }
            }
        }

        // 5. OTOMATÝK KESÝM
        if (kesimYapiliyorMu)
        {
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(true);

            kesmeIlerlemesi += Time.deltaTime * kesmeHizi;
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = kesmeIlerlemesi;

            if (kesmeIlerlemesi >= 1f)
            {
                EtKesipDusur();
                DurdurKesimveAnimasyon();
            }
        }
        else
        {
            kesmeIlerlemesi = 0f;
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        }
    }

    void DöneriIsit()
    {
        isinmaIlerlemesi += Time.deltaTime * 0.5f;
        if (isinmaBarGorseli != null) isinmaBarGorseli.fillAmount = isinmaIlerlemesi;

        if (isinmaIlerlemesi >= 1f)
        {
            donerSogukMu = false;
            sogumaSayaci = 0f;
            isinmaIlerlemesi = 0f;
            isiniyorMu = false;
            makineAcikMi = true;
            if (isinmaArayuzObjesi != null) isinmaArayuzObjesi.SetActive(false);
        }
    }

    void DurdurKesimveAnimasyon()
    {
        kesimYapiliyorMu = false;
        kesmeIlerlemesi = 0f;
        if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
        if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);

        if (makineyeYakinOyuncu != null && makineyeYakinOyuncu.oyuncuAnimator != null)
        {
            makineyeYakinOyuncu.oyuncuAnimator.SetBool("isCutting", false);
        }
    }

    void EtKesipDusur()
    {
        if (donerDilimPrefab != null && kesimNoktasi != null)
        {
            GameObject yeniDilim = Instantiate(donerDilimPrefab, kesimNoktasi.position, Quaternion.identity);
            MeshRenderer dilimRenderer = yeniDilim.GetComponent<MeshRenderer>();
            if (dilimRenderer != null && donerEtiRenderer != null) dilimRenderer.material = donerEtiRenderer.material;

            DonerDilimi dilimScript = yeniDilim.GetComponent<DonerDilimi>();
            if (dilimScript != null)
            {
                dilimScript.sogukMu = this.donerSogukMu;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // GÜNCELLENDÝ: Hem objenin kendisinde hem de üst/alt objelerinde Player tag'i arayan garanti kontrol
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            oyuncuYakindaMi = true;
            makineyeYakinOyuncu = other.GetComponentInChildren<OyuncuEnvanter>() ?? other.GetComponentInParent<OyuncuEnvanter>() ?? other.GetComponent<OyuncuEnvanter>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            oyuncuYakindaMi = false;
            DurdurKesimveAnimasyon();
            makineyeYakinOyuncu = null;

            if (isiniyorMu)
            {
                isiniyorMu = false;
                isinmaIlerlemesi = 0f;
                if (isinmaArayuzObjesi != null) isinmaArayuzObjesi.SetActive(false);
            }
        }
    }
}