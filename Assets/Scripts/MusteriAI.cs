using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // Image bileþenini kontrol etmek için bu þart!
public class MusteriAI : MonoBehaviour
{
    public enum MusteriDurumu { MasayaGidiyor, SiparisBekliyor, YemekYiyor, KasayaGidiyor, KasadaBekliyor, Ayriliyor }
    [Header("Müþteri Durumu")]
    public MusteriDurumu suAnkiDurum;

    [Header("Zamanlayýcýlar")]
    public float sabirSuresi = 20f;
    private float sabirSayaci;
    public float yemekYemeSuresi = 5f;
    private float yemekSayaci;

    [Header("Hedef Noktalarý")]
    public Transform hedefSandalye;
    public Transform cikisNoktasi;

    [Header("UI ve Etkileþim")]
    [Header("Sipariþ UI ve Menü")]
    public Image yemekIkonu; // Dönerin çýkacaðý çerçeve
    public Image icecekIkonu; // Ýçeceðin çýkacaðý çerçeve

    public Sprite donerResmi; // Fix menü: Sabit döner resmimiz
    public Sprite[] icecekResimleri; // Rastgele seçilecek içecekler (Kola, Ayran, Þalgam)
    public GameObject siparisCanvas; // Kafasýndaki baloncuk
    public float etkilesimMesafesi = 3f; // E'ye basabilmek için gereken mesafe
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

        // Oyuncuyu Tag (Player) sayesinde otomatik bulur
        GameObject oyuncuObje = GameObject.FindGameObjectWithTag("Player");
        if (oyuncuObje != null) oyuncu = oyuncuObje.transform;

        if (sabirSuresi <= 0) sabirSuresi = 20f;
        if (yemekYemeSuresi <= 0) yemekYemeSuresi = 5f;

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
                        if (suAnkiDurum == MusteriDurumu.MasayaGidiyor && hedefSandalye != null)
                        {
                            float gercekMesafe = Vector3.Distance(transform.position, hedefSandalye.position);
                            if (gercekMesafe > 2.5f) return;
                        }

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

        // ==========================================
        // YENÝ: E TUÞU ÝLE SÝPARÝÞ ALMA (BALONCUÐU KAPATIP FÝÞ BASMA)
        // ==========================================
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor && !siparisAlindiMi)
        {
            if (oyuncu != null && Vector3.Distance(transform.position, oyuncu.position) <= etkilesimMesafesi)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SiparisiOyuncuyaVer();
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
                if (sabirSayaci <= 0)
                {
                    DurumDegistir(MusteriDurumu.Ayriliyor);
                }
                break;

            case MusteriDurumu.YemekYiyor:
                yemekSayaci -= Time.deltaTime;
                if (yemekSayaci <= 0)
                {
                    DurumDegistir(MusteriDurumu.KasayaGidiyor);
                }
                break;
        }
    }

    void SiparisiOyuncuyaVer()
    {
        siparisAlindiMi = true;
        sabirSayaci = sabirSuresi; // Sipariþ alýndý, sabýr süresi sýfýrlandý (yemek bekleme süresi baþladý)

        // 1. Kafasýndaki baloncuðu gizle
        if (siparisCanvas != null) siparisCanvas.SetActive(false);

        // 2. Sað üstte fiþ oluþtur (FisYonetici üzerinden)
        if (FisYonetici.Instance != null) FisYonetici.Instance.YeniFisOlustur(gameObject.name);
    }

    void HedefeUlasincaTetikle()
    {
        switch (suAnkiDurum)
        {
            case MusteriDurumu.MasayaGidiyor:
                DurumDegistir(MusteriDurumu.SiparisBekliyor);
                break;
            case MusteriDurumu.KasayaGidiyor:
                DurumDegistir(MusteriDurumu.KasadaBekliyor);
                break;
            case MusteriDurumu.Ayriliyor:
                Destroy(gameObject);
                break;
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
                // YAPAY ZEKAYI DURDUR
                if (agent != null) agent.enabled = false;

                // --- YENÝ KOD: KOMBOLU SÝPARÝÞ SÝSTEMÝ ---
                // 1. Herkes döner yiyecek (Zorunlu)
                if (yemekIkonu != null && donerResmi != null)
                {
                    yemekIkonu.sprite = donerResmi;
                }

                // 2. Yanýna rastgele bir içecek seçecek
                if (icecekResimleri.Length > 0 && icecekIkonu != null)
                {
                    int rastgeleIcecek = Random.Range(0, icecekResimleri.Length);
                    icecekIkonu.sprite = icecekResimleri[rastgeleIcecek];
                }

                // Baloncuðu ve animasyonu aç
                if (siparisCanvas != null) siparisCanvas.SetActive(true);
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", true);
                break;

            case MusteriDurumu.YemekYiyor:
                if (siparisCanvas != null) siparisCanvas.SetActive(false);
                break;

            case MusteriDurumu.KasayaGidiyor:
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", false);
                if (KasaYonetici.Instance != null) KasaYonetici.Instance.KuyrugaGir(this);
                break;

            case MusteriDurumu.KasadaBekliyor:
                break;

            case MusteriDurumu.Ayriliyor:
                if (siparisCanvas != null) siparisCanvas.SetActive(false);
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", false);
                if (cikisNoktasi != null) agent.SetDestination(cikisNoktasi.position);
                break;
        }
    }

    public void SiparisTeslimEdildi()
    {
        // Yemeði cidden verdiðimizde çaðýracaðýmýz yer
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor && siparisAlindiMi)
        {
            DurumDegistir(MusteriDurumu.YemekYiyor);
        }
    }
    // ==========================================
    // KASA YÖNETÝCÝSÝ ÝÇÝN GEREKLÝ FONKSÝYONLAR
    // ==========================================

    public void OdemeYapVeGit()
    {
        DurumDegistir(MusteriDurumu.Ayriliyor);
    }

    public void NavigasyonHedefiVer(Vector3 yeniPozisyon)
    {
        if (agent != null)
        {
            // Eðer müþteri masada oturduðu için yapay zekasý kapanmýþsa, yürüyebilmesi için geri açýyoruz
            if (!agent.enabled) agent.enabled = true;

            agent.SetDestination(yeniPozisyon);
        }
    }
}