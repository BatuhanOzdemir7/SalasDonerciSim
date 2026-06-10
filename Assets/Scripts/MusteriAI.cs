using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MusteriAI : MonoBehaviour, IInteractable
{
    public enum MusteriDurumu { MasayaGidiyor, SiparisBekliyor, YemekYiyor, KasayaGidiyor, KasadaBekliyor, Ayriliyor }
    [Header("Müþteri Durumu")]
    public MusteriDurumu suAnkiDurum;

    [Header("Sipariþi (Fiþin Ýçeriði)")]
    private bool tursuIsterMi;
    private bool marulIsterMi;
    private bool soganIsterMi;
    private bool patatesIsterMi;
    private bool sosIsterMi; 
    private string secilenIcecekAdi;
    private int dilimSayisi;

    [Header("Müþterinin Fiþi")]
    private GameObject benimFisim;

    [Header("Zamanlayýcýlar ve Fiyat")]
    public float beklemeSuresi = 0f;
    public float yemekYemeSuresi = 10f;
    private float yemekSayaci;
    public float odenecekTutar = 0f; 

    [Header("Memnuniyet Sistemi")]
    public float memnuniyet = 50f;

    [Header("Hedef Noktalar")]
    public Transform hedefSandalye;
    public Transform cikisNoktasi;

    [Header("UI ve Etkileþim")]
    public GameObject siparisCanvas;
    public Image yemekIkonu;
    public Image icecekIkonu;
    public TMPro.TextMeshProUGUI baloncukYazisi; // Unity'den eklediðin Text'i buraya baðla
    public Sprite donerResmi;
    public Sprite[] icecekResimleri;
    public float etkilesimMesafesi = 3f;
    
    private Transform oyuncu;
    private bool siparisAlindiMi = false;
    private NavMeshAgent agent;
    private Animator musteriAnimator;
    private bool hedefeUlasildiMi = false;
    private Tray onumdekiTepsi;

    [Header("Oturma Ayarlarý")]
    public float hizalamaHizi = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        musteriAnimator = GetComponent<Animator>();
        GameObject oyuncuObje = GameObject.FindGameObjectWithTag("Player");
        if (oyuncuObje != null) oyuncu = oyuncuObje.transform;

        toplamYemekSuresiAyarla();

        // BAÞLANGIÇTA BALONCUÐU TAMAMEN KAPATIYORUZ
        BaloncukArayuzuGuncelle(false, false, false);
        DurumDegistir(MusteriDurumu.MasayaGidiyor);
    }

    void Update()
    {
        if (agent != null && agent.enabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    if (!hedefeUlasildiMi)
                    {
                        hedefeUlasildiMi = true;
                        HedefeUlasincaTetikle();
                    }
                }
            }
        }

        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor || suAnkiDurum == MusteriDurumu.YemekYiyor)
        {
            if (hedefSandalye != null)
            {
                transform.position = Vector3.Lerp(transform.position, hedefSandalye.position, Time.deltaTime * hizalamaHizi);
                transform.rotation = Quaternion.Slerp(transform.rotation, hedefSandalye.rotation, Time.deltaTime * hizalamaHizi);
            }
        }

        if (musteriAnimator != null)
        {
            float anlikHiz = (agent != null && agent.enabled) ? agent.velocity.magnitude : 0f;
            musteriAnimator.SetFloat("Speed", anlikHiz);
        }

        StatusKontrol();
    }

    // =========================================================================
    // MERKEZÝ BALONCUK KONTROLÖRÜ (Hatalarý Engelleyen Sihirli Fonksiyon)
    // =========================================================================
    void BaloncukArayuzuGuncelle(bool canvasAcik, bool ikonlarAcik, bool yaziAcik, string metin = "")
    {
        if (siparisCanvas != null) siparisCanvas.SetActive(canvasAcik);
        if (yemekIkonu != null) yemekIkonu.gameObject.SetActive(ikonlarAcik);
        if (icecekIkonu != null) icecekIkonu.gameObject.SetActive(ikonlarAcik);
        
        if (baloncukYazisi != null)
        {
            baloncukYazisi.gameObject.SetActive(yaziAcik);
            if (yaziAcik) baloncukYazisi.text = metin;
        }
    }

    public void Interact(OyuncuEnvanter oyuncuEnvanteri)
    {
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor && !siparisAlindiMi)
        {
            if (FisYonetici.Instance != null && FisYonetici.Instance.FisIcinYerVarMi())
            {
                SiparisiOyuncuyaVer();
            }
        }
    }

    void SiparisiOyuncuyaVer()
    {
        siparisAlindiMi = true;
        
        // SÝPARÝÞ ALINDIÐINDA BALONCUK TAMAMEN KAPANIR
        BaloncukArayuzuGuncelle(false, false, false);

        if (FisYonetici.Instance != null)
        {
            benimFisim = FisYonetici.Instance.YeniFisOlustur(gameObject.name, tursuIsterMi, marulIsterMi, soganIsterMi, patatesIsterMi, sosIsterMi, secilenIcecekAdi, dilimSayisi);
        }
    }

    public void DurumDegistir(MusteriDurumu yeniDurum)
    {
        suAnkiDurum = yeniDurum;
        switch (yeniDurum)
        {
            case MusteriDurumu.MasayaGidiyor:
                hedefeUlasildiMi = false;
                BaloncukArayuzuGuncelle(false, false, false); // Yürürken kapalý
                if (agent != null) { agent.enabled = true; if (hedefSandalye != null) agent.SetDestination(hedefSandalye.position); }
                break;

            case MusteriDurumu.SiparisBekliyor:
                beklemeSuresi = 0f; 
                if (agent != null) agent.enabled = false;
                
                tursuIsterMi = Random.value > 0.5f;
                marulIsterMi = Random.value > 0.5f;
                soganIsterMi = Random.value > 0.5f;
                patatesIsterMi = Random.value > 0.5f;
                sosIsterMi = Random.value > 0.5f; 
                dilimSayisi = Random.Range(1, 8);
                
                if (yemekIkonu != null && donerResmi != null) yemekIkonu.sprite = donerResmi;
                if (icecekResimleri.Length > 0 && icecekIkonu != null)
                {
                    int r = Random.Range(0, icecekResimleri.Length);
                    icecekIkonu.sprite = icecekResimleri[r];
                    secilenIcecekAdi = icecekResimleri[r].name;
                }

                // SÝPARÝÞ BEKLERKEN: Canvas Açýk, Ýkonlar Açýk, Yazý Kapalý!
                BaloncukArayuzuGuncelle(true, true, false);

                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", true);
                break;

            case MusteriDurumu.YemekYiyor:
                string[] replikler = { 
                    "Eline saðlýk ustam!", 
                    "Þifa þifa! Muazzam olmuþ.", 
                    "Ustam þifa mý yapýyorsun ?", 
                    "Aradýðým lezzet buydu!", 
                    "On numara dürüm usta!" 
                };
                string secilenReplik = replikler[Random.Range(0, replikler.Length)];

                // YEMEK YERKEN: Süreli baloncuk coroutine'ini baþlatýyoruz
                StartCoroutine(SureliYemekRepligi(secilenReplik, 5f));
                break;

            case MusteriDurumu.KasayaGidiyor:
                hedefeUlasildiMi = false;
                BaloncukArayuzuGuncelle(false, false, false); // Kasaya giderken kapalý
                if (agent != null) agent.enabled = true;
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", false);
                if (KasaYonetici.Instance != null) KasaYonetici.Instance.KuyrugaGir(this);
                break;

            case MusteriDurumu.Ayriliyor:
                BaloncukArayuzuGuncelle(false, false, false); // Ayrýlýrken kapalý
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", false);
                if (cikisNoktasi != null) agent.SetDestination(cikisNoktasi.position);
                if (benimFisim != null) Destroy(benimFisim);
                break;
        }
    }

    private System.Collections.IEnumerator SureliYemekRepligi(string metin, float kalmaSuresi)
    {
        // Canvas Açýk, Ýkonlar Kapalý, Yazý Açýk!
        BaloncukArayuzuGuncelle(true, false, true, metin);

        yield return new WaitForSeconds(kalmaSuresi);

        // 2 Saniye dolunca baloncuk tamamen yok olur!
        BaloncukArayuzuGuncelle(false, false, false);
    }

    public void TabagiDegerlendir(GameObject masayaKonanObje)
    {
        if (suAnkiDurum != MusteriDurumu.SiparisBekliyor || !siparisAlindiMi) return;

        Durum icindekiDurum = masayaKonanObje.GetComponentInChildren<Durum>(true);
        if (icindekiDurum == null) return;

        if (icindekiDurum.donerZehirliMi && HijyenYonetici.Instance != null) 
            HijyenYonetici.Instance.mevcutHijyen -= 1.0f;

        bool siparisDogruMu = true;
        if (icindekiDurum.kullanilanDonerSayisi < dilimSayisi) siparisDogruMu = false;
        if (tursuIsterMi != icindekiDurum.tursuVarMi) siparisDogruMu = false;
        if (marulIsterMi != icindekiDurum.marulVarMi) siparisDogruMu = false;
        if (soganIsterMi != icindekiDurum.soganVarMi) siparisDogruMu = false;
        if (patatesIsterMi != icindekiDurum.patatesVarMi) siparisDogruMu = false;
        if (sosIsterMi != icindekiDurum.sosKullanildiMi) siparisDogruMu = false; 

        if (!siparisDogruMu)
        {
            if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(-15f);
            DurumDegistir(MusteriDurumu.Ayriliyor);
            return;
        }

        float paraCarpani = 1f;
        float eklenecekMemnuniyet = 0f;

        if (beklemeSuresi <= 50f) { eklenecekMemnuniyet = 10f; paraCarpani = 1.5f; }
        else if (beklemeSuresi <= 100f) { eklenecekMemnuniyet = 5f; paraCarpani = 1f; }

        if (KasaYonetici.Instance != null)
        {
            odenecekTutar = KasaYonetici.Instance.durumFiyati * paraCarpani;
            KasaYonetici.Instance.MemnuniyetPuaniniIsle(eklenecekMemnuniyet);
        }

        Collider objeCol = masayaKonanObje.GetComponent<Collider>();
        if (objeCol != null) objeCol.enabled = false;

        onumdekiTepsi = masayaKonanObje.GetComponent<Tray>();
        if (onumdekiTepsi == null) onumdekiTepsi = masayaKonanObje.GetComponentInChildren<Tray>();
        
        DurumDegistir(MusteriDurumu.YemekYiyor);
    }

    void StatusKontrol()
    {
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor)
        {
            beklemeSuresi += Time.deltaTime;
            if (beklemeSuresi > 100f)
            {
                if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(-15f);
                DurumDegistir(MusteriDurumu.Ayriliyor);
            }
        }
        else if (suAnkiDurum == MusteriDurumu.YemekYiyor)
        {
            toplamYemekSuresiAyarla();
            if (yemekSayaci <= 0)
            {
                if (onumdekiTepsi != null)
                {
                    onumdekiTepsi.TepsiyiSifirla();
                    Collider col = onumdekiTepsi.GetComponent<Collider>();
                    if (col != null) col.enabled = true;
                    onumdekiTepsi = null;
                }
                DurumDegistir(MusteriDurumu.KasayaGidiyor);
            }
        }
    }

    void toplamYemekSuresiAyarla()
    {
        if (suAnkiDurum == MusteriDurumu.YemekYiyor) yemekSayaci -= Time.deltaTime;
        else yemekSayaci = yemekYemeSuresi;
    }

    void HedefeUlasincaTetikle()
    {
        switch (suAnkiDurum)
        {
            case MusteriDurumu.MasayaGidiyor: DurumDegistir(MusteriDurumu.SiparisBekliyor); break;
            case MusteriDurumu.KasayaGidiyor: DurumDegistir(MusteriDurumu.KasadaBekliyor); break;
            case MusteriDurumu.Ayriliyor: Destroy(gameObject); break;
        }
    }

    public void NavigasyonHedefiVer(Vector3 yeniPozisyon)
    {
        if (agent != null)
        {
            if (!agent.enabled) agent.enabled = true;
            agent.SetDestination(yeniPozisyon);
        }
    }

    public void OdemeYapVeGit()
    {
        DurumDegistir(MusteriDurumu.Ayriliyor);
    }
}