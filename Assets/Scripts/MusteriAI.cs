using UnityEngine;
using UnityEngine.AI;

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

    private NavMeshAgent agent;
    private Animator musteriAnimator;
    private bool hedefeUlasildiMi = false;

    [Header("Oturma Ayarlarý")]
    public float hizalamaHizi = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        musteriAnimator = GetComponent<Animator>();

        // GÜVENLÝK: Inspector'da yanlýþlýkla 0 yapýldýysa oyunu bozmamasý için zorla düzeltiyoruz
        if (sabirSuresi <= 0) sabirSuresi = 20f;
        if (yemekYemeSuresi <= 0) yemekYemeSuresi = 5f;

        sabirSayaci = sabirSuresi;
        yemekSayaci = yemekYemeSuresi;

        DurumDegistir(MusteriDurumu.MasayaGidiyor);
    }

    void Update()
    {
        // 1. KUSURSUZ HEDEF KONTROLÜ (Unity Bug'ýný Engelleyen Kýsým)
        if (agent != null && agent.enabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    if (!hedefeUlasildiMi)
                    {
                        // EKSTRA GÜVENLÝK: Eðer masaya gidiyorsa, sandalyeye gerçekten fiziksel olarak yaklaþtý mý?
                        if (suAnkiDurum == MusteriDurumu.MasayaGidiyor && hedefSandalye != null)
                        {
                            float gercekMesafe = Vector3.Distance(transform.position, hedefSandalye.position);
                            if (gercekMesafe > 2.5f) return; // 2.5 metreden uzaktaysa hedefe vardým sanýp masayý iptal etme!
                        }

                        hedefeUlasildiMi = true;
                        HedefeUlasincaTetikle();
                    }
                }
            }
        }

        // 2. MASADA OTURMA (Pürüzsüz Hizalama)
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor || suAnkiDurum == MusteriDurumu.YemekYiyor)
        {
            if (hedefSandalye != null)
            {
                transform.position = Vector3.Lerp(transform.position, hedefSandalye.position, Time.deltaTime * hizalamaHizi);
                transform.rotation = Quaternion.Slerp(transform.rotation, hedefSandalye.rotation, Time.deltaTime * hizalamaHizi);
            }
        }

        // 3. ANÝMASYON HIZI
        if (musteriAnimator != null)
        {
            float anlikHiz = (agent != null && agent.enabled) ? agent.velocity.magnitude : 0f;
            musteriAnimator.SetFloat("Speed", anlikHiz);
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
                    Debug.Log(gameObject.name + ": Usta açlýktan öldük, ben kaçar!");
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
                if (agent != null)
                {
                    agent.enabled = true;
                    if (hedefSandalye != null) agent.SetDestination(hedefSandalye.position);
                }
                break;

            case MusteriDurumu.SiparisBekliyor:
                if (agent != null) agent.enabled = false;
                break;

            case MusteriDurumu.YemekYiyor:
                break;

            case MusteriDurumu.KasayaGidiyor:
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (KasaYonetici.Instance != null) KasaYonetici.Instance.KuyrugaGir(this);
                break;

            case MusteriDurumu.KasadaBekliyor:
                break;

            case MusteriDurumu.Ayriliyor:
                hedefeUlasildiMi = false;
                if (agent != null) agent.enabled = true;
                if (cikisNoktasi != null) agent.SetDestination(cikisNoktasi.position);
                break;
        }
    }

    public void SiparisTeslimEdildi()
    {
        if (suAnkiDurum == MusteriDurumu.SiparisBekliyor)
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