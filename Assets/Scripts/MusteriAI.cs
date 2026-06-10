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

    [Header("Zamanlayýcýlar ve Fiyat")]
    public float beklemeSuresi = 0f;
    public float yemekYemeSuresi = 10f;
    private float yemekSayaci;
    public float odenecekTutar = 0f; // Müþterinin kasada ödeyeceði dinamik tutar

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
                // Müþteri masaya oturduðu andan itibaren sayar
                beklemeSuresi += Time.deltaTime;

                // 100 Saniye Aþýmý (Akýþ Þemasýndaki ">100 saniye" koþulu)
                if (beklemeSuresi > 100f)
                {
                    Debug.Log("<color=red>Müþteri: 100 saniyeden fazla bekledim, dükkaný terk ediyorum!</color>");

                    // Þemadaki "memn += -15" kuralý (100 taban puan üzerinden ortalamaya yansýtýyoruz)
                    if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(-15f);

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

        if (icindekiDurum.donerZehirliMi)
        {
            Debug.Log("Müþteri: Aaaðh! Bu et bozuk! Zehirlendim!");
            if (KasaYonetici.Instance != null) KasaYonetici.Instance.MemnuniyetPuaniniIsle(-15f);
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

        float paraCarpani = 1f;
        float eklenecekMemnuniyet = 0f;

        // AKIÞ ÞEMASI KURALLARI
        if (beklemeSuresi <= 50f)
        {
            if (siparisDogruMu)
            {
                eklenecekMemnuniyet = 10f;
                paraCarpani = 1.5f;
                Debug.Log("<color=green>Müþteri: Hýzlý ve doðru servis! (0-50 sn)</color>");
            }
            else
            {
                eklenecekMemnuniyet = -5f;
                paraCarpani = 0.5f;
                Debug.Log("<color=orange>Müþteri: Hýzlý geldi ama yanlýþ! (0-50 sn)</color>");
            }
        }
        else if (beklemeSuresi <= 100f)
        {
            if (siparisDogruMu)
            {
                eklenecekMemnuniyet = 5f;
                paraCarpani = 1f;
                Debug.Log("<color=green>Müþteri: Sipariþ doðru ama gecikti. (50-100 sn)</color>");
            }
            else
            {
                eklenecekMemnuniyet = -10f;
                paraCarpani = 0.25f;
                Debug.Log("<color=red>Müþteri: Hem geç hem yanlýþ! (50-100 sn)</color>");
            }
        }

        // Kasa Yöneticisine gönderilecek verilerin iþlenmesi
        if (KasaYonetici.Instance != null)
        {
            // Fiyatý müþterinin kendi deðiþkenine kaydediyoruz (Kasaya gidince bu tutarý ödeyecek)
            odenecekTutar = KasaYonetici.Instance.durumFiyati * paraCarpani;

            // Taban memnuniyet (100) üzerine þemadaki artý/eksi deðeri ekleyip ortalama sistemine yolluyoruz
            KasaYonetici.Instance.MemnuniyetPuaniniIsle(100f + eklenecekMemnuniyet);
        }

        Collider objeCol = masayaKonanObje.GetComponent<Collider>();
        if (objeCol != null) objeCol.enabled = false;

        DurumDegistir(MusteriDurumu.YemekYiyor);
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
                beklemeSuresi = 0f; // Masaya oturunca sayacý baþlat
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