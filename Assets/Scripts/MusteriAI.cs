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
    private bool sosIsterMi; // EKLENDÝ: Müþteri sos istiyor mu?
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
    public TMPro.TextMeshProUGUI baloncukYazisi; // Unity'den az önce oluþturduðun yazýyý buraya sürükleyeceksin
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
    private Tray onumdekiTepsi;

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
                beklemeSuresi += Time.deltaTime;
                if (beklemeSuresi > 100f)
                {
                    Debug.Log("<color=red>Müþteri: 100 saniyeden fazla bekledim, dükkaný terk ediyorum!</color>");
                    if (KasaYonetici.Instance != null)
                    {
                        KasaYonetici.Instance.MemnuniyetPuaniniIsle(-15f);
                    }
                    DurumDegistir(MusteriDurumu.Ayriliyor);
                }
                break;
            case MusteriDurumu.YemekYiyor:
                yemekSayaci -= Time.deltaTime;
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
                break;
        }
    }

    void SiparisiOyuncuyaVer()
    {
        siparisAlindiMi = true;
        if (siparisCanvas != null) siparisCanvas.SetActive(false);

        if (FisYonetici.Instance != null)
        {
            string masaBilgisi = "Ayakta Müþteri";

            if (hedefSandalye != null)
            {
                // Sandalyenin bir üst objesi (parent) genelde Masadýr (Örn: "Masa 1")
                if (hedefSandalye.parent != null)
                {
                    masaBilgisi = hedefSandalye.parent.name + " \n " + hedefSandalye.name;
                }
                else
                {
                    masaBilgisi = hedefSandalye.name; // Eðer masa objesi yoksa sadece sandalyeyi yaz
                }
            }
            benimFisim = FisYonetici.Instance.YeniFisOlustur(masaBilgisi, tursuIsterMi, marulIsterMi, soganIsterMi, patatesIsterMi, sosIsterMi, secilenIcecekAdi, dilimSayisi);
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
            Debug.Log("Müþteri zehirli et yedi! Hijyen puaný düþtü ama yemeðe devam ediyor.");
            if (HijyenYonetici.Instance != null) HijyenYonetici.Instance.mevcutHijyen -= 1.0f;
        }

        bool siparisDogruMu = true;
        if (icindekiDurum.kullanilanDonerSayisi < dilimSayisi) siparisDogruMu = false;
        if (tursuIsterMi != icindekiDurum.tursuVarMi) siparisDogruMu = false;
        if (marulIsterMi != icindekiDurum.marulVarMi) siparisDogruMu = false;
        if (soganIsterMi != icindekiDurum.soganVarMi) siparisDogruMu = false;
        if (patatesIsterMi != icindekiDurum.patatesVarMi) siparisDogruMu = false;

        // EKLENDÝ: Sos kontrolü (Senin Durum.cs içindeki 'sosKullanildiMi' ile eþleþtirildi)
        if (sosIsterMi != icindekiDurum.sosKullanildiMi) siparisDogruMu = false;

        float paraCarpani = 1f;
        float eklenecekMemnuniyet = 0f;

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
                beklemeSuresi = 0f;
                if (agent != null) agent.enabled = false;

                tursuIsterMi = Random.value > 0.5f;
                marulIsterMi = Random.value > 0.5f;
                soganIsterMi = Random.value > 0.5f;
                patatesIsterMi = Random.value > 0.5f;

                // EKLENDÝ: %50 ihtimalle sos isteme mekanizmasý
                sosIsterMi = Random.value > 0.5f;

                dilimSayisi = Random.Range(1, 8);
                if (yemekIkonu != null && donerResmi != null) yemekIkonu.sprite = donerResmi;
                if (icecekResimleri.Length > 0 && icecekIkonu != null)
                {
                    int r = Random.Range(0, icecekResimleri.Length);
                    icecekIkonu.sprite = icecekResimleri[r];
                    secilenIcecekAdi = icecekResimleri[r].name;
                }
                if (siparisCanvas != null) siparisCanvas.SetActive(true);
                // Yeni sipariþte ikonlarý tekrar görünür yap, eski yazýyý gizle
                if (yemekIkonu != null) yemekIkonu.gameObject.SetActive(true);
                if (icecekIkonu != null) icecekIkonu.gameObject.SetActive(true);
                if (baloncukYazisi != null) baloncukYazisi.gameObject.SetActive(false);
                if (musteriAnimator != null) musteriAnimator.SetBool("Oturuyor", true);
                break;
            case MusteriDurumu.YemekYiyor:
                if (siparisCanvas != null) siparisCanvas.SetActive(false);
                if (benimFisim != null) Destroy(benimFisim);
                if (yemekIkonu != null) yemekIkonu.gameObject.SetActive(false);
                if (icecekIkonu != null) icecekIkonu.gameObject.SetActive(false);
                string[] replikler = {
                    "Eline saðlýk ustam!",
                    "Ustam yine þifa yapmýþsýn bakýyorum.",
                    "Ooo bol kepçe, eyvallah usta!",
                    "Aradýðým lezzet iþte buydu!",
                    "Dönerci Hüseyinn be!"
                };
                string secilenReplik = replikler[Random.Range(0, replikler.Length)];
                StartCoroutine(SureliKonusmaBaloncugu(secilenReplik, 3f));
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
    // =========================================================================
    // SÜRELÝ BALONCUK COROUTINE FONKSÝYONU
    // =========================================================================
    private System.Collections.IEnumerator SureliKonusmaBaloncugu(string metin, float kalmaSuresi)
    {
        if (siparisCanvas != null && baloncukYazisi != null)
        {
            baloncukYazisi.gameObject.SetActive(true); // Yazýyý aç
            baloncukYazisi.text = metin;               // Repliði yazdýr
            siparisCanvas.SetActive(true);             // Baloncuðu zorla görünür yap

            // Oyun motoruna "Belirtilen saniye kadar burada dur, hiçbir þey yapma" diyoruz
            yield return new WaitForSeconds(kalmaSuresi);

            // Süre dolunca baloncuðu ve yazýyý tamamen kapatýyoruz
            siparisCanvas.SetActive(false);
            baloncukYazisi.gameObject.SetActive(false);
        }
    }
}