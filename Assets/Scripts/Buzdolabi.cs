using UnityEngine;
using System.Collections;

public class Buzdolabi : MonoBehaviour, IInteractable
{
    [Header("Kapak Ayarlarý")]
    public Transform dolapKapagi;
    public float acikAci = -90f;
    private bool kapakAcikMi = false;
    private Coroutine kapakAnimasyonu;

    [Header("Arayüz (Canvas)")]
    public GameObject dolapMenusuCanvas;
    private OyuncuEnvanter islemYapanOyuncu;

    public float kapanmaMesafesi = 4f;

    [Header("Malzeme Prefablarý")]
    public GameObject marulPrefab;
    public GameObject tursuPrefab;
    public GameObject soganPrefab;
    public GameObject lavasPrefab;
    public GameObject cigTavukPrefab;

    void Start()
    {
        if (dolapMenusuCanvas != null) dolapMenusuCanvas.SetActive(false);
    }

    void Update()
    {
        if (islemYapanOyuncu != null && dolapMenusuCanvas != null && dolapMenusuCanvas.activeSelf)
        {
            float mesafe = Vector3.Distance(transform.position, islemYapanOyuncu.transform.position);

            if (mesafe > kapanmaMesafesi)
            {
                Debug.Log("Oyuncu dolaptan uzaklaþtý, kapak örtülüyor.");
                Buton_MenuyuKapat();
            }
        }
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        if (oyuncu.GetHeldMalzeme() != null)
        {
            oyuncu.EldenBirakVeSil();
            Debug.Log("Malzeme dolaba geri kondu.");
            return;
        }

        if (oyuncu.GetHeldTray() != null || oyuncu.bicakVarMi)
        {
            Debug.Log("Dolabý kullanmak için elindeki eþyayý tezgaha býrakmalýsýn!");
            return;
        }

        islemYapanOyuncu = oyuncu;

        if (!kapakAcikMi)
        {
            if (kapakAnimasyonu != null) StopCoroutine(kapakAnimasyonu);
            kapakAnimasyonu = StartCoroutine(KapakDondur(acikAci));
        }

        if (dolapMenusuCanvas != null) dolapMenusuCanvas.SetActive(true);
    }

    IEnumerator KapakDondur(float hedefAci)
    {
        // HATALI SATIR BURASIYDI. Düzeltildi ve eksi açýlarý da algýlamasý saðlandý.
        kapakAcikMi = Mathf.Abs(hedefAci) > 0.1f;

        Quaternion baslangicRot = dolapKapagi.localRotation;
        Quaternion hedefRot = Quaternion.Euler(0, hedefAci, 0);

        float gecenSure = 0f;
        float animasyonSuresi = 0.4f;

        while (gecenSure < animasyonSuresi)
        {
            gecenSure += Time.deltaTime;
            dolapKapagi.localRotation = Quaternion.Slerp(baslangicRot, hedefRot, gecenSure / animasyonSuresi);
            yield return null;
        }
        dolapKapagi.localRotation = hedefRot;
    }

    public void Buton_MarulAl() { MalzemeVer(marulPrefab); }
    public void Buton_tursuAl() { MalzemeVer(tursuPrefab); }
    public void Buton_SoganAl() { MalzemeVer(soganPrefab); }
    public void Buton_LavasAl() { MalzemeVer(lavasPrefab); }
    public void Buton_CigTavukAl() { MalzemeVer(cigTavukPrefab); }

    public void Buton_MenuyuKapat()
    {
        if (dolapMenusuCanvas != null) dolapMenusuCanvas.SetActive(false);
        islemYapanOyuncu = null;

        if (kapakAcikMi)
        {
            if (kapakAnimasyonu != null) StopCoroutine(kapakAnimasyonu);
            kapakAnimasyonu = StartCoroutine(KapakDondur(0f));
        }
    }

    private void MalzemeVer(GameObject secilenPrefab)
    {
        if (islemYapanOyuncu != null && secilenPrefab != null)
        {
            GameObject yeniMalzeme = Instantiate(secilenPrefab);
            islemYapanOyuncu.PickUpItem(yeniMalzeme);
            Buton_MenuyuKapat();
        }
    }
}