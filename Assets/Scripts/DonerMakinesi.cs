using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DonerMakinesi : MonoBehaviour
{
    public enum DonerDurumu { Cig, Pisti, Yandi }

    [Header("Döner Durumu (Katmanlar)")]
    public DonerDurumu anlikDurum = DonerDurumu.Cig;
    public bool zehirliMi = true;
    public int atilanKesikSayisi = 0;

    [Header("Döner Kapasitesi")]
    public int maxYaprakSayisi = 100;
    public int kalanYaprakSayisi = 100;
    public bool donereCigTavukEklendiMi = false;
    public TextMeshProUGUI kalanYaprakText;

    [Header("Makine Durumu")]
    public bool makineAcikMi = false;
    public Animator donerAnimator;
    public TextMeshProUGUI makineDurumText;

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
    public Material azPismisMat;
    public Material pismisMat;
    public Material cokPismisMat;

    public float pismeSuresi = 0f;
    public float pismesiIcinGerekenSure = 10f;
    public float yanmasiIcinGerekenSure = 30f; // BATUHAN'IN YENÝ YANMA SÜRESÝ

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

        DurumuGuncelle(DonerDurumu.Cig);
    }

    void Update()
    {
        // 1. YAPRAK SAYACI GÜNCELLEMESÝ
        if (kalanYaprakText != null)
        {
            if (kalanYaprakSayisi > 0)
                kalanYaprakText.text = kalanYaprakSayisi + " / " + maxYaprakSayisi;
            else
                kalanYaprakText.text = "DÖNER BÝTTÝ!";
        }

        // MAKÝNE DURUMU GÜNCELLEMESÝ (Açýk/Kapalý ve Renkli)
        if (makineDurumText != null)
        {
            if (makineAcikMi)
            {
                makineDurumText.text = "<color=green>MAKÝNE AÇIK</color>";
            }
            else
            {
                makineDurumText.text = "<color=red>MAKÝNE KAPALI</color>";
            }
        }

        if (isiniyorMu)
        {
            DöneriIsit();
            return;
        }

        // TAVUK DETEKTÝF SÝSTEMÝ
        bool elindeTavukVarMi = false;
        if (oyuncuYakindaMi && makineyeYakinOyuncu != null)
        {
            var eldekiObje = makineyeYakinOyuncu.GetHeldMalzeme();
            if (eldekiObje != null)
            {
                string nesneAdi = eldekiObje.name.ToLower();
                if (nesneAdi.Contains("tavuk") || nesneAdi.Contains("cig") || nesneAdi.Contains("chicken") || nesneAdi.Contains("doner"))
                {
                    elindeTavukVarMi = true;
                }
            }
        }

        // DÝNAMÝK YAZI KONTROLÜ
        if (oyuncuYakindaMi && !kesimYapiliyorMu)
        {
            if (eTusuYazisi != null)
            {
                eTusuYazisi.gameObject.SetActive(true);

                if (elindeTavukVarMi)
                    eTusuYazisi.text = "Çið Tavuðu Ocaða Takmak Ýçin E'ye Bas";
                else if (donerSogukMu && !makineAcikMi)
                    eTusuYazisi.text = "Döneri Isýtmak Ýçin E'ye Bas";
                else if (makineAcikMi)
                    eTusuYazisi.text = "Makineyi Kapatmak Ýçin E'ye Bas";
                else
                    eTusuYazisi.text = "Makineyi Açmak Ýçin E'ye Bas";
            }
        }
        else
        {
            if (eTusuYazisi != null) eTusuYazisi.gameObject.SetActive(false);
        }

        // E TUÞU KONTROLÜ
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            if (elindeTavukVarMi)
            {
                if (kalanYaprakSayisi > 0 && kalanYaprakSayisi < maxYaprakSayisi)
                {
                    donereCigTavukEklendiMi = true;
                    Debug.Log("<color=red><b>[ÇAPRAZ BULAÞMA]:</b> Bitmemiþ dönerin üstüne çið tavuk basýldý! Müþteriler zehirlenecek!</color>");
                }
                else
                {
                    donereCigTavukEklendiMi = false;
                    Debug.Log("[OCAK]: Boþ ocaða yeni, temiz çið tavuk takýldý.");
                }

                kalanYaprakSayisi = maxYaprakSayisi;
                DurumuGuncelle(DonerDurumu.Cig);

                if (makineyeYakinOyuncu != null)
                {
                    makineyeYakinOyuncu.EldenBirakVeSil();
                }

                if (donerEtiRenderer != null) donerEtiRenderer.enabled = true;
            }
            else
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
        }

        // 3. DÖNME, PÝÞME ve SOÐUMA
        if (makineAcikMi && kalanYaprakSayisi > 0)
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
                // BATUHAN'IN YANMA MEKANÝÐÝ: Artýk yanma süresi ayrý bir deðiþkenden kontrol ediliyor (30s)
                else if (anlikDurum == DonerDurumu.Pisti && pismeSuresi >= yanmasiIcinGerekenSure)
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

        // Q TUÞU
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.Q))
        {
            if (kalanYaprakSayisi > 0 && makineyeYakinOyuncu != null && makineyeYakinOyuncu.bicakVarMi)
            {
                kesimYapiliyorMu = !kesimYapiliyorMu;

                if (makineyeYakinOyuncu.oyuncuAnimator != null)
                {
                    makineyeYakinOyuncu.oyuncuAnimator.SetBool("isCutting", kesimYapiliyorMu);
                }
            }
        }

        // OTOMATÝK KESÝM
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
    }

    void EtKesipDusur()
    {
        if (donerDilimPrefab != null && kesimNoktasi != null)
        {
            atilanKesikSayisi++;
            kalanYaprakSayisi--;

            // BATUHAN'IN MEKANÝÐÝ: Eðer döner piþmiþ durumdaysa ve kesik atýldýysa, yanma süresini sýfýrla!
            if (anlikDurum == DonerDurumu.Pisti)
            {
                pismeSuresi = 0f;
            }

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
                if (donereCigTavukEklendiMi) dilimScript.zehirliMi = true;
                else dilimScript.zehirliMi = this.zehirliMi;
            }

            if (atilanKesikSayisi >= 10)
            {
                if (anlikDurum == DonerDurumu.Yandi) DurumuGuncelle(DonerDurumu.Pisti);
                else if (anlikDurum == DonerDurumu.Pisti) DurumuGuncelle(DonerDurumu.Cig);
            }

            if (tepsiNoktasi != null && tepsiNoktasi.ustundekiTepsi != null)
            {
                StartCoroutine(TepsiyeDusmeAnimasyonu(yeniDilim, tepsiNoktasi.ustundekiTepsi));
            }

            if (kalanYaprakSayisi <= 0)
            {
                if (donerEtiRenderer != null) donerEtiRenderer.enabled = false;
                DurdurKesimveAnimasyon();
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

        if (this.zehirliMi || this.donereCigTavukEklendiMi)
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