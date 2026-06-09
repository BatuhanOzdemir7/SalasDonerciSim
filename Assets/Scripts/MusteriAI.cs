using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MusteriAI : MonoBehaviour
{
    public enum MusteriDurumu { MasayaGidiyor, SiparisBekliyor, YemekYiyor, KasayaGidiyor, KasadaBekliyor, Ayriliyor }
    [Header("Müþteri Durumu")]
    public MusteriDurumu suAnkiDurum;

    [Header("Sipariþi (Fiþin Ýçeriði)")]
    private bool tursuIsterMi;
    private bool marulIsterMi;
    private bool soganIsterMi;
    private bool patatesIsterMi;
    private string secilenIcecekAdi;
    private int dilimSayisi;

    [Header("Müþterinin Fiþi")]
    private GameObject benimFisim;

    [Header("Zamanlayýcýlar")]
    public float sabirSuresi = 60f;
    private float sabirSayaci;
    public float yemekYemeSuresi = 10f;
    private float yemekSayaci;

    [Header("Memnuniyet Sistemi")]
    public float memnuniyet = 100f;

    [Header("Hedef Noktalar")]
    public Transform hedefSandalye;
    public Transform cikisNoktasi;

    [Header("UI ve Etkileþim")]
    public Image yemekIkonu;
    public Image icecekIkonu;
    public Sprite donerResmi;
    public Sprite[] icecekResimleri;
    public GameObject siparisCanvas;
    public float etkilesimMesafesi = 3f;
    private Transform oyuncu;
    private bool siparisAlindiMi = false;
    private NavMeshAgent agent;
    private Animator musteriAnimator;
    private bool hedefeUlasildiMi = false;

    [Header("Oturma Ayarlarý")]
    public float hizalamaHizi = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        musteriAnimator = GetComponent<Animator>();
        GameObject oyuncuObje = GameObject.FindGameObjectWithTag("Player");
        if (oyuncuObje != null) oyuncu = oyuncuObje.transform;

        sabirSayaci = sabirSuresi;
        yemekSayaci = yemekYemeSuresi;

        if (siparisCanvas != null) siparisCanvas.SetActive(false);
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

        // STANDART: Sipariþ Alma Aksiyonu "F" tuþu oldu
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor && !siparisAlindiMi)
        {
            if (oyuncu != null && Vector3.Distance(transform.position, oyuncu.position) <= etkilesimMesafesi)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (FisYonetici.Instance != null && FisYonetici.Instance.FisIcinYerVarMi())
                    {
                        SiparisiOyuncuyaVer();
                    }
                }
            }
        }
        StatusKontrol();
    }

    void StatusKontrol()
    {
        switch (suAnkiDurum)
        {
            case MusteriDurumu.SiparisBekliyor:
                sabirSayaci -= Time.deltaTime;
                float gecenSure = sabirSuresi - sabirSayaci;
                memnuniyet = 100f - (Mathf.Floor(gecenSure / 12f) * 20f);
                memnuniyet = Mathf.Clamp(memnuniyet, 0f, 100f);

                if (sabirSayaci <= 0)
                {
                    Debug.Log("<color=red>Müþteri: Sabrým bitti, yemek falan istemiyorum!</color>");
                    memnuniyet = 0f;
                    if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(memnuniyet);
                    DurumDegistir(MusteriDurumu.Ayriliyor);
                }
                break;
            case MusteriDurumu.YemekYiyor:
                yemekSayaci -= Time.deltaTime;
                if (yemekSayaci <= 0) DurumDegistir(MusteriDurumu.KasayaGidiyor);
                break;
        }
    }

    void SiparisiOyuncuyaVer()
    {
        siparisAlindiMi = true;
        sabirSayaci = sabirSuresi;
        if (siparisCanvas != null) siparisCanvas.SetActive(false);

        if (FisYonetici.Instance != null)
        {
            benimFisim = FisYonetici.Instance.YeniFisOlustur(gameObject.name, tursuIsterMi, marulIsterMi, soganIsterMi, patatesIsterMi, secilenIcecekAdi, dilimSayisi);
        }
    }

    public void TabagiDegerlendir(GameObject masayaKonanObje)
    {
        if (suAnkiDurum != MusteriDurumu.SiparisBekliyor || !siparisAlindiMi) return;

        Durum icindekiDurum = masayaKonanObje.GetComponentInChildren<Durum>(true);
        if (icindekiDurum == null)
        {
            Debug.Log("Müþteri: Usta bu tepside dürüm yok!");
            return;
        }

        // YENÝ ZEHÝRLENME CEZASI (Hem Memnuniyet Hem Hijyen Düþer)
        if (icindekiDurum.donerZehirliMi)
        {
            Debug.Log("Müþteri: Aaaðh! Bu et bozuk! Zehirlendim!");
            memnuniyet = 0f;
            if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(memnuniyet);

            if (HijyenYonetici.Instance != null) HijyenYonetici.Instance.mevcutHijyen -= 1.0f;

            DurumDegistir(MusteriDurumu.Ayriliyor);
            return;
        }

        bool siparisDogruMu = true;
        if (icindekiDurum.kullanilanDonerSayisi < dilimSayisi) siparisDogruMu = false;
        if (tursuIsterMi != icindekiDurum.tursuVarMi) siparisDogruMu = false;
        if (marulIsterMi != icindekiDurum.marulVarMi) siparisDogruMu = false;
        if (soganIsterMi != icindekiDurum.soganVarMi) siparisDogruMu = false;
        if (patatesIsterMi != icindekiDurum.patatesVarMi) siparisDogruMu = false;

        if (siparisDogruMu)
        {
            Debug.Log("<color=green>Müþteri: Sipariþim doðru, eline saðlýk usta!</color>");
            if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(memnuniyet);

            Collider objeCol = masayaKonanObje.GetComponent<Collider>();
            if (objeCol != null) objeCol.enabled = false;

            DurumDegistir(MusteriDurumu.YemekYiyor);
        }
        else
        {
            Debug.Log("<color=red>Müþteri: Yanlýþ veya eksik dürüm sarmýþsýn usta! Memnuniyet %0, kalkýp gidiyorum.</color>");
            memnuniyet = 0f;
            if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(memnuniyet);
            DurumDegistir(MusteriDurumu.Ayriliyor);
        }
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

    public void DurumDegistir(MusteriDurumu yeniDurum)
    {
        suAnkiDurum = yeniDurum;
        switch (yeniDurum)
        {
            case MusteriDurumu.MasayaGidiyor:
                hedefeUlasildiMi = false;
                if (agent != null) { agent.enabled = true; if (hedefSandalye != null) agent.SetDestination(hedefSandalye.position); }
                break;
            case MusteriDurumu.SiparisBekliyor:
                if (agent != null) agent.enabled = false;
                tursuIsterMi = Random.value > 0.5f;
                marulIsterMi = Random.value > 0.5f;
                soganIsterMi = Random.value > 0.5f;
                patatesIsterMi = Random.value > 0.5f;
                dilimSayisi = Random.Range(1, 8);
                if (yemekIkonu != null && donerResmi != null) yemekIkonu.sprite = donerResmi;
                if (icecekResimleri.Length > 0 && icecekIkonu != null)
                {
                    int r = Random.Range(0, icecekResimleri.Length);
                    icecekIkonu.sprite = icecekResimleri[r];
                    secilenIcecekAdi = icecekResimleri[r].name;
                }
                if (siparisCanvas != null) siparisCanvas.SetActive(true);
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", true);
                break;
            case MusteriDurumu.YemekYiyor:
                if (siparisCanvas != null) siparisCanvas.SetActive(false);
                if (benimFisim != null) Destroy(benimFisim);
                break;
            case MusteriDurumu.KasayaGidiyor:
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", false);
                if (KasaYonetici.Instance != null) KasaYonetici.Instance.KuyrugaGir(this);
                break;
            case MusteriDurumu.Ayriliyor:
                if (siparisCanvas != null) siparisCanvas.SetActive(false);
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", false);
                if (cikisNoktasi != null) agent.SetDestination(cikisNoktasi.position);
                if (benimFisim != null) Destroy(benimFisim);
                break;
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
        Debug.Log("Müþteri: Yemeði yedim, hesabý da ödedim. Kolay gelsin usta!");
        DurumDegistir(MusteriDurumu.Ayriliyor);
    }
}