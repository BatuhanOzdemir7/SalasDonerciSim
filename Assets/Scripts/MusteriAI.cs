using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MusteriAI : MonoBehaviour
{
    public enum MusteriDurumu { MasayaGidiyor, SiparisBekliyor, YemekYiyor, KasayaGidiyor, KasadaBekliyor, Ayriliyor }

    [Header("Müþteri Durumu")]
    public MusteriDurumu suAnkiDurum;

    [Header("Sipariþ Ýçeriði (Fiþ Ýçin)")]
    private bool tursuIsterMi;
    private bool marulIsterMi;
    private bool soganIsterMi;
    private bool patatesIsterMi;
    private string secilenIcecekAdi;
    private int dilimSayisi;

    [Header("Müþterinin Fiþi")]
    private GameObject benimFisim; // YENÝ: Müþterinin ekrandaki fiþini aklýnda tutacaðý deðiþken

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
        // E TUÞU ÝLE SÝPARÝÞ ALMA (KAPASÝTE KONTROLLÜ)
        // ==========================================
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor && !siparisAlindiMi)
        {
            if (oyuncu != null && Vector3.Distance(transform.position, oyuncu.position) <= etkilesimMesafesi)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Önce mutfakta (panelde) yer var mý diye soruyoruz
                    if (FisYonetici.Instance != null && FisYonetici.Instance.FisIcinYerVarMi())
                    {
                        SiparisiOyuncuyaVer();
                    }
                    else
                    {
                        // 6 fiþ doluyken E'ye basarsa burasý çalýþýr
                        Debug.Log("Sipariþ Paneli Dolu! Önce bir yemeði teslim etmelisin.");
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
        sabirSayaci = sabirSuresi;

        if (siparisCanvas != null) siparisCanvas.SetActive(false);

        if (FisYonetici.Instance != null)
        {
            // Matbaadan çýkan fiþi 'benimFisim' deðiþkenine kaydediyoruz!
            benimFisim = FisYonetici.Instance.YeniFisOlustur(gameObject.name, tursuIsterMi, marulIsterMi, soganIsterMi, patatesIsterMi, secilenIcecekAdi, dilimSayisi);
        }
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
                if (agent != null) agent.enabled = false;

                tursuIsterMi = Random.value > 0.5f;
                marulIsterMi = Random.value > 0.5f;
                soganIsterMi = Random.value > 0.5f;
                patatesIsterMi = Random.value > 0.5f;

                dilimSayisi = Random.Range(1, 8);

                if (yemekIkonu != null && donerResmi != null)
                {
                    yemekIkonu.sprite = donerResmi;
                }

                if (icecekResimleri.Length > 0 && icecekIkonu != null)
                {
                    int rastgeleIcecek = Random.Range(0, icecekResimleri.Length);
                    icecekIkonu.sprite = icecekResimleri[rastgeleIcecek];
                    secilenIcecekAdi = icecekResimleri[rastgeleIcecek].name;
                }

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

                // ==========================================
                // YENÝ: Müþteri sinirlenip kalkýyorsa fiþini yýrt at!
                // ==========================================
                if (benimFisim != null)
                {
                    Destroy(benimFisim);
                }
                break;
        }
    }

    public void SiparisTeslimEdildi()
    {
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor && siparisAlindiMi)
        {
            DurumDegistir(MusteriDurumu.YemekYiyor);
        }
    }

    public void OdemeYapVeGit()
    {
        DurumDegistir(MusteriDurumu.Ayriliyor);
    }

    public void NavigasyonHedefiVer(Vector3 yeniPozisyon)
    {
        if (agent != null)
        {
            if (!agent.enabled) agent.enabled = true;
            agent.SetDestination(yeniPozisyon);
        }
    }
}