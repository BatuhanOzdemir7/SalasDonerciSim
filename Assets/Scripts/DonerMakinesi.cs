using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DonerMakinesi : MonoBehaviour
{
    // --- YENÝ EKLENEN DURUM YÖNETÝMÝ ---
    public enum DonerDurumu { Cig, Pisti, Yandi }

    [Header("Döner Durumu (Katmanlar)")]
    public DonerDurumu anlikDurum = DonerDurumu.Cig;
    public bool zehirliMi = true; // Baþlangýçta çið olduðu için zehirli
    public int atilanKesikSayisi = 0; // Bu katmanda kaç kere kesik atýldý?

    [Header("Makine Durumu")]
    public bool makineAcikMi = false;
    public Animator donerAnimator;

    [Header("Sýcaklýk ve Soðuma Ayarlarý")]
    public bool donerSogukMu = false;
    public float sogumaSayaci = 0f;
    public float sogumaSiniri = 5f;

    [Header("Isýtma Arayüzü")]
    public GameObject isinmaArayuzObjesi;
    public Image isinmaBarGorseli;
    private float isinmaIlerlemesi = 0f;
    private bool isiniyorMu = false;

    [Header("Etkileþim Ayarlarý")]
    public bool oyuncuYakindaMi = false;
    public TextMeshProUGUI eTusuYazisi;
    private OyuncuEnvanter makineyeYakinOyuncu;

    [Header("Piþme Ayarlarý")]
    public MeshRenderer donerEtiRenderer;
    public Material azPismisMat; // Çið
    public Material pismisMat;   // Piþmiþ
    public Material cokPismisMat; // Yanýk

    public float pismeSuresi = 0f;
    public float pismesiIcinGerekenSure = 10f; // Akýþ þemasýndaki 10 saniye

    [Header("Kesme ve Yükleme Çubuðu")]
    public GameObject arkaplanObjesi;
    public Image yuklemeCubugu;
    public float kesmeHizi = 1f;
    private float kesmeIlerlemesi = 0f;
    public bool kesimYapiliyorMu = false;

    [Header("Düþen Et Ayarlarý")]
    public GameObject donerDilimPrefab;
    public Transform kesimNoktasi;

    [Header("Tepsi Baðlantýsý")]
    public TepsiBirakmaNoktasi tepsiNoktasi;

    void Start()
    {
        if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        if (eTusuYazisi != null) eTusuYazisi.gameObject.SetActive(false);
        if (isinmaArayuzObjesi != null) isinmaArayuzObjesi.SetActive(false);

        DurumuGuncelle(DonerDurumu.Cig); // Baþlangýç durumunu ayarla
    }

    void Update()
    {
        if (isiniyorMu)
        {
            DöneriIsit();
            return;
        }

        // 1. DÝNAMÝK YAZI KONTROLÜ
        if (oyuncuYakindaMi && !kesimYapiliyorMu)
        {
            if (eTusuYazisi != null)
            {
                eTusuYazisi.gameObject.SetActive(true);

                if (donerSogukMu && !makineAcikMi)
                {
                    eTusuYazisi.text = "Döneri Isýtmak Ýçin E'ye Bas";
                }
                else if (makineAcikMi)
                {
                    eTusuYazisi.text = "Makineyi Kapatmak Ýçin E'ye Bas";
                }
                else
                {
                    eTusuYazisi.text = "Makineyi Açmak Ýçin E'ye Bas";
                }
            }
        }
        else
        {
            if (eTusuYazisi != null) eTusuYazisi.gameObject.SetActive(false);
        }

        // 2. E TUÞU
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            if (donerSogukMu && !makineAcikMi)
            {
                if (eTusuYazisi != null) eTusuYazisi.gameObject.SetActive(false);
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

            if (anlikDurum != DonerDurumu.Yandi)
            {
                pismeSuresi += Time.deltaTime;

                if (anlikDurum == DonerDurumu.Cig && pismeSuresi >= pismesiIcinGerekenSure)
                {
                    DurumuGuncelle(DonerDurumu.Pisti);
                }
                else if (anlikDurum == DonerDurumu.Pisti && pismeSuresi >= pismesiIcinGerekenSure)
                {
                    DurumuGuncelle(DonerDurumu.Yandi);
                }
            }
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

        // 4. Q TUÞU
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

    // --- YENÝ: DURUM GÜNCELLEME MERKEZÝ ---
    private void DurumuGuncelle(DonerDurumu yeniDurum)
    {
        anlikDurum = yeniDurum;
        pismeSuresi = 0f;
        atilanKesikSayisi = 0;

        switch (anlikDurum)
        {
            case DonerDurumu.Cig:
                zehirliMi = true;
                if (donerEtiRenderer != null) donerEtiRenderer.material = azPismisMat;
                break;
            case DonerDurumu.Pisti:
                zehirliMi = false;
                if (donerEtiRenderer != null) donerEtiRenderer.material = pismisMat;
                break;
            case DonerDurumu.Yandi:
                zehirliMi = true;
                if (donerEtiRenderer != null) donerEtiRenderer.material = cokPismisMat;
                break;
        }
        Debug.Log("Döner durumu deðiþti: " + anlikDurum);
    }

    void EtKesipDusur()
    {
        if (donerDilimPrefab != null && kesimNoktasi != null)
        {
            atilanKesikSayisi++;
            Debug.Log("Et kesildi! Mevcut katmandaki kesik: " + atilanKesikSayisi);

            GameObject yeniDilim = Instantiate(donerDilimPrefab, kesimNoktasi.position, Quaternion.identity);

            MeshRenderer dilimRenderer = yeniDilim.GetComponent<MeshRenderer>();
            if (dilimRenderer != null && donerEtiRenderer != null)
            {
                dilimRenderer.material = donerEtiRenderer.material;
            }

            DonerDilimi dilimScript = yeniDilim.GetComponent<DonerDilimi>();
            if (dilimScript != null)
            {
                dilimScript.sogukMu = this.donerSogukMu;
                dilimScript.zehirliMi = this.zehirliMi;
            }

            // 10 KESÝK KONTROLÜ
            if (atilanKesikSayisi >= 10)
            {
                if (anlikDurum == DonerDurumu.Yandi) DurumuGuncelle(DonerDurumu.Pisti);
                else if (anlikDurum == DonerDurumu.Pisti) DurumuGuncelle(DonerDurumu.Cig);
            }

            if (tepsiNoktasi != null && tepsiNoktasi.ustundekiTepsi != null)
            {
                StartCoroutine(TepsiyeDusmeAnimasyonu(yeniDilim, tepsiNoktasi.ustundekiTepsi));
            }
            else
            {
                Debug.LogWarning("Ocaðýn yanýna tepsi koymadýðýn için et tezgaha düþtü!");
            }
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

    IEnumerator TepsiyeDusmeAnimasyonu(GameObject dilim, Tray hedefTepsi)
    {
        Rigidbody rb = dilim.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = dilim.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float gecenSure = 0f;
        float animasyonSuresi = 0.35f;
        Vector3 baslangicPozisyonu = dilim.transform.position;

        Vector3 hedefPozisyon = hedefTepsi.etlerinBirikecegiNokta != null
            ? hedefTepsi.etlerinBirikecegiNokta.position
            : hedefTepsi.transform.position;

        while (gecenSure < animasyonSuresi)
        {
            gecenSure += Time.deltaTime;
            float oran = gecenSure / animasyonSuresi;
            dilim.transform.position = Vector3.Lerp(baslangicPozisyonu, hedefPozisyon, oran);
            yield return null;
        }

        Destroy(dilim);

        hedefTepsi.EtEkle();
        hedefTepsi.isMeatCold = this.donerSogukMu;

        // EÐER MAKÝNEDEKÝ ET ZEHÝRLÝYSE (Çið veya Yanýk), TEPSÝYÝ DE ZEHÝRLE
        if (this.zehirliMi)
        {
            hedefTepsi.zehirliEtVarMi = true;
        }

        if (tepsiNoktasi != null)
        {
            tepsiNoktasi.SayaciGuncelle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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