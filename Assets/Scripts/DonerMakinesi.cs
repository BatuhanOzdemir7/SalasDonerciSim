using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMPro kütüphanesini ekledik ki yazý metnini deðiþtirebilelim
using System.Collections;

public class DonerMakinesi : MonoBehaviour
{
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
    public TextMeshProUGUI eTusuYazisi; // GameObject yerine TextMeshProUGUI olarak güncelledik!
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

    [Header("Tepsi Baðlantýsý")]
    public TepsiBirakmaNoktasi tepsiNoktasi; // Kýrmýzý alandaki scripti buraya sürükleyeceðiz

    void Start()
    {
        if (arkaplanObjesi != null) arkaplanObjesi.SetActive(false);
        if (eTusuYazisi != null) eTusuYazisi.gameObject.SetActive(false);
        if (isinmaArayuzObjesi != null) isinmaArayuzObjesi.SetActive(false);
    }

    void Update()
    {
        if (isiniyorMu)
        {
            DöneriIsit();
            return;
        }

        // 1. DÝNAMÝK YAZI KONTROLÜ (DÜZELTÝLDÝ: Duruma göre yazý metni deðiþiyor)
        if (oyuncuYakindaMi && !kesimYapiliyorMu)
        {
            if (eTusuYazisi != null)
            {
                eTusuYazisi.gameObject.SetActive(true);

                // Eðer döner soðuksa öncelik ýsýtma uyarýsý olsun
                if (donerSogukMu && !makineAcikMi)
                {
                    eTusuYazisi.text = "Döneri Isýtmak Ýçin E'ye Bas";
                }
                // Makine zaten açýk durumdaysa kapatma uyarýsý yazsýn
                else if (makineAcikMi)
                {
                    eTusuYazisi.text = "Makineyi Kapatmak Ýçin E'ye Bas";
                }
                // Makine kapalý ve döner sýcaksa açma uyarýsý yazsýn
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
            // 1. Eti havada, kesim noktasýnda fiziksel 3D model olarak yarat
            GameObject yeniDilim = Instantiate(donerDilimPrefab, kesimNoktasi.position, Quaternion.identity);

            // 2. Piþmiþlik rengini yeni düþen ete aktar
            MeshRenderer dilimRenderer = yeniDilim.GetComponent<MeshRenderer>();
            if (dilimRenderer != null && donerEtiRenderer != null)
            {
                dilimRenderer.material = donerEtiRenderer.material;
            }

            DonerDilimi dilimScript = yeniDilim.GetComponent<DonerDilimi>();
            if (dilimScript != null)
            {
                dilimScript.sogukMu = this.donerSogukMu;
            }

            // 3. Kýrmýzý alanda tepsi var mý kontrol et
            if (tepsiNoktasi != null && tepsiNoktasi.ustundekiTepsi != null)
            {
                // Tepsi varsa animasyonlu düþüþ sürecini baþlat
                StartCoroutine(TepsiyeDusmeAnimasyonu(yeniDilim, tepsiNoktasi.ustundekiTepsi));
            }
            else
            {
                // Tepsi yoksa et serbest düþüþle (fizikle) tezgaha/yere düþer
                Debug.LogWarning("Ocaðýn yanýna tepsi koymadýðýn için et tezgaha düþtü!");
            }
        }
    }

    // Zaman ayarlý düþme animasyon sistemi
    IEnumerator TepsiyeDusmeAnimasyonu(GameObject dilim, Tray hedefTepsi)
    {
        // Etin saða sola sekmesini engellemek için fiziksel çarpýþmasýný geçici kapatýyoruz
        Rigidbody rb = dilim.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = dilim.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float gecenSure = 0f;
        float animasyonSuresi = 0.35f;
        Vector3 baslangicPozisyonu = dilim.transform.position;

        // Etin, tepsinin merkezine deðil doðrudan "Et Noktasýna" gitmesini saðlýyoruz
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

        // ESKÝ HATA BURADAYDI: Sadece sayacý artýrýp eti "Destroy" yapýyorduk.
        // YENÝ SÝSTEM: Uçuþ animasyonu bitince o sahte animasyon etini siliyoruz ve 
        // Tray.cs içindeki, eti rastgele açýlarla üst üste dizen fiziksel EtEkle() sistemini tetikliyoruz.
        Destroy(dilim);

        hedefTepsi.EtEkle();
        hedefTepsi.isMeatCold = this.donerSogukMu;

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