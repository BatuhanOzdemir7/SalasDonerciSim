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
    private OyuncuEnvanter makineyeYakinOyuncu; // YENÝ: Yakýndaki oyuncunun envanter referansý

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
    }

    void Update()
    {
        // 1. YAZI KONTROLÜ
        if (oyuncuYakindaMi && !makineAcikMi)
        {
            if (eTusuGorseli != null) eTusuGorseli.SetActive(true);
        }
        else
        {
            if (eTusuGorseli != null) eTusuGorseli.SetActive(false);
        }

        // 2. E TUÞU (Makineyi Aç/Kapat)
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            makineAcikMi = !makineAcikMi;
            if (!makineAcikMi) DurdurKesimveAnimasyon();
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

        // 4. Q TUÞU (BIÇAK ZORUNLULUÐU EKLENDÝ)
        if (oyuncuYakindaMi && makineAcikMi && Input.GetKeyDown(KeyCode.Q))
        {
            // EÐER OYUNCUNUN ELÝNDE BIÇAK VARSA ÇALIÞ
            if (makineyeYakinOyuncu != null && makineyeYakinOyuncu.bicakVarMi)
            {
                kesimYapiliyorMu = !kesimYapiliyorMu;

                // Oyuncunun kesme animasyonunu baþlat/durdur
                if (makineyeYakinOyuncu.oyuncuAnimator != null)
                {
                    makineyeYakinOyuncu.oyuncuAnimator.SetBool("isCutting", kesimYapiliyorMu);
                }
            }
            else
            {
                Debug.LogWarning("Usta! Elinde býçak yok, döneri neyle keseceksin?");
            }
        }

        // 5. OTOMATÝK KESÝM ÝÞLEMÝ
        if (kesimYapiliyorMu)
        {
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(true);

            kesmeIlerlemesi += Time.deltaTime * kesmeHizi;
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = kesmeIlerlemesi;

            if (kesmeIlerlemesi >= 1f)
            {
                EtKesipDusur();
                DurdurKesimveAnimasyon();
                pismeSuresi = 0f;
            }
        }
        else
        {
            kesmeIlerlemesi = 0f;
            if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
            if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        }
    }

    void DurdurKesimveAnimasyon()
    {
        kesimYapiliyorMu = false;
        kesmeIlerlemesi = 0f;
        if (yuklemeCubugu != null) yuklemeCubugu.fillAmount = 0f;
        if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);

        // Kesim bittiðinde veya makine kapandýðýnda oyuncunun animasyonunu durdur
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakindaMi = true;
            makineyeYakinOyuncu = other.GetComponent<OyuncuEnvanter>(); // Oyuncuyu kaydet
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakindaMi = false;
            DurdurKesimveAnimasyon();
            makineyeYakinOyuncu = null;
        }
    }
}