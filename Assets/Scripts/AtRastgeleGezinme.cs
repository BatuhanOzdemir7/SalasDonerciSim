using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AtRastgeleGezinme : MonoBehaviour
{
    [Header("Gezinme Ayarları")]
    public float gezinmeYaricapi = 15f;
    public float beklemeSuresiMin = 1f; // At artık daha az bekleyecek
    public float beklemeSuresiMax = 3.5f;

    [Header("Canlılık (Idle) Ayarları")]
    public float sagaSolaBakmaHizi = 1.5f;

    private NavMeshAgent agent;
    private Animator anim;
    private float beklemeSayaci;
    private bool hedefeGidiyorMu = false;

    // Etrafa bakma mekaniği için
    private Quaternion rastgeleBakisAcisi;
    private bool etrafaBakiyorMu = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // HATA DÜZELTMESİ: Animator'ü iskeletten (child objeden) alıyoruz
        anim = GetComponentInChildren<Animator>();

        // Atın robot gibi keskin dönmemesi için dönüş hızını biraz yumuşatıyoruz
        agent.angularSpeed = 120f;

        YeniHedefBelirle();
    }

    void Update()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Hedefe ulaştıysa veya duruyorsa
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Yeni durduysa bekleme süresini ve bakacağı rastgele yönü ayarla
            if (hedefeGidiyorMu)
            {
                hedefeGidiyorMu = false;
                beklemeSayaci = Random.Range(beklemeSuresiMin, beklemeSuresiMax);

                // Durduğu yerde kafasını/vücudunu çevireceği rastgele bir açı hesapla (Sağa veya Sola maks 75 derece)
                float rastgeleY = transform.eulerAngles.y + Random.Range(-75f, 75f);
                rastgeleBakisAcisi = Quaternion.Euler(0, rastgeleY, 0);
                etrafaBakiyorMu = true;
            }

            beklemeSayaci -= Time.deltaTime;

            // At beklerken dümdüz duvara bakmak yerine, hesaplanan rastgele açıya doğru yavaşça döner (Otlama/Bakınma hissi)
            if (etrafaBakiyorMu)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rastgeleBakisAcisi, Time.deltaTime * sagaSolaBakmaHizi);
            }

            // Bekleme süresi dolunca tekrar yürüyüşe geç
            if (beklemeSayaci <= 0f)
            {
                etrafaBakiyorMu = false;
                YeniHedefBelirle();
            }
        }
    }

    // EKSİK OLAN KISIM TAMAMLANDI
    void YeniHedefBelirle()
    {
        Vector3 rastgeleYon = Random.insideUnitSphere * gezinmeYaricapi;
        rastgeleYon += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rastgeleYon, out hit, gezinmeYaricapi, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            hedefeGidiyorMu = true;
        }
        else
        {
            beklemeSayaci = 0.1f;
        }
    }
}