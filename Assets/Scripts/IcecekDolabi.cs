using UnityEngine;
using System.Collections;

public class İcecekDolabi : MonoBehaviour, IInteractable
{
    [Header("Kapak Ayarları")]
    public Transform dolapKapagi;
    public float acikAci = -90f;
    private bool kapakAcikMi = false;
    private Coroutine kapakAnimasyonu;

    [Header("Arayüz (Canvas)")]
    public GameObject İcecek_Menusu;
    private OyuncuEnvanter islemYapanOyuncu;

    public float kapanmaMesafesi = 4f;

    [Header("Malzeme Prefabları")]
    public GameObject ayranPrefab;
    public GameObject suPrefab;
    public GameObject kolaPrefab;

    void Start()
    {
        if (İcecek_Menusu != null) İcecek_Menusu.SetActive(false);
    }

    void Update()
    {
        if (islemYapanOyuncu != null && İcecek_Menusu != null && İcecek_Menusu.activeSelf)
        {
            float mesafe = Vector3.Distance(transform.position, islemYapanOyuncu.transform.position);

            if (mesafe > kapanmaMesafesi)
            {
                Debug.Log("Oyuncu dolaptan uzaklaştı, kapak örtülüyor.");
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
            Debug.Log("Dolabı kullanmak için elindeki eşyayı tezgaha bırakmalısın!");
            return;
        }

        islemYapanOyuncu = oyuncu;

        if (!kapakAcikMi)
        {
            if (kapakAnimasyonu != null) StopCoroutine(kapakAnimasyonu);
            kapakAnimasyonu = StartCoroutine(KapakDondur(acikAci));
        }

        if (İcecek_Menusu != null) İcecek_Menusu.SetActive(true);
    }

    IEnumerator KapakDondur(float hedefAci)
    {
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

    public void Buton_AyranAl() { MalzemeVer(ayranPrefab); }
    public void Buton_SuAl() { MalzemeVer(suPrefab); }
    public void Buton_KolaAl() { MalzemeVer(kolaPrefab); }

    public void Buton_MenuyuKapat()
    {
        if (İcecek_Menusu != null) İcecek_Menusu.SetActive(false);
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