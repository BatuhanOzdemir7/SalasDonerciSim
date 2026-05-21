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
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E) && yakindakiOyuncu != null)
        {
            if (!yakindakiOyuncu.bicakVarMi)
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
        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().isKinematic = true;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        transform.SetParent(yakindakiOyuncu.elNoktasi);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        yakindakiOyuncu.bicakVarMi = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Değişiklik burada: Hem objeden hem de üst objelerinden kodu arar
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