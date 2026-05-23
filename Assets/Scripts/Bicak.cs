using UnityEngine;

public class Bicak : MonoBehaviour
{
    public bool oyuncuYakindaMi = false;
    public GameObject bicakAlYazisi;
    private OyuncuEnvanter yakindakiOyuncu;

    void Start()
    {
        if (bicakAlYazisi != null) bicakAlYazisi.SetActive(false);
    }

    void Update()
    {
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            // GARANTİ ADIM: Eğer referans bir şekilde boşaldıysa, o an yakındaki oyuncuyu tekrar bulmayı dene
            if (yakindakiOyuncu == null)
            {
                // Çevredeki Player tag'li objeden envanteri bulur
                Collider[] yakinlardakiObjeler = Physics.OverlapSphere(transform.position, 3f);
                foreach (var obje in yakinlardakiObjeler)
                {
                    if (obje.CompareTag("Player"))
                    {
                        yakindakiOyuncu = obje.GetComponentInChildren<OyuncuEnvanter>() ?? obje.GetComponentInParent<OyuncuEnvanter>() ?? obje.GetComponent<OyuncuEnvanter>();
                        break;
                    }
                }
            }

            // Eğer hala oyuncu bulunduysa ve bıçağı yoksa alma sürecini başlat
            if (yakindakiOyuncu != null && !yakindakiOyuncu.bicakVarMi)
            {
                yakindakiOyuncu.suAnkiBicakScripti = this;

                if (yakindakiOyuncu.oyuncuAnimator != null)
                {
                    yakindakiOyuncu.oyuncuAnimator.SetTrigger("isPickingUp");
                }

                if (bicakAlYazisi != null) bicakAlYazisi.SetActive(false);
                oyuncuYakindaMi = false;
            }
        }
    }

    public void BicagiEleIsinla()
    {
        // GÜVENLİK KONTROLÜ: Eğer animasyon event tetiklendiğinde yakindakiOyuncu hala null ise hatayı engelle
        if (yakindakiOyuncu == null)
        {
            Debug.LogError("Bıçak alınmaya çalışılıyor ama oyuncu referansı bulunamadı!");
            return;
        }

        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().isKinematic = true;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        // Oyuncunun el noktasına jilet gibi yapış
        transform.SetParent(yakindakiOyuncu.elNoktasi);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        yakindakiOyuncu.bicakVarMi = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            yakindakiOyuncu = other.GetComponentInChildren<OyuncuEnvanter>() ?? other.GetComponentInParent<OyuncuEnvanter>() ?? other.GetComponent<OyuncuEnvanter>();

            if (yakindakiOyuncu != null && !yakindakiOyuncu.bicakVarMi)
            {
                oyuncuYakindaMi = true;
                if (bicakAlYazisi != null) bicakAlYazisi.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakindaMi = false;
            if (bicakAlYazisi != null) bicakAlYazisi.SetActive(false);
            yakindakiOyuncu = null;
        }
    }
}