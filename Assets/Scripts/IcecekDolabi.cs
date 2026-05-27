using UnityEngine;
using System.Collections;

public class IcecekDolabi : MonoBehaviour, IInteractable
{
    [Header("Kapak Ayarları")]
    public Transform dolapKapagi; // Kapağı (veya Menteshe objesini) buraya sürükleyeceğiz
    public float acikAci = 90f;   // Kapağın açılma yönüne göre bunu 90 veya -90 yapabilirsin
    private bool kapakAcikMi = false;
    private Coroutine kapakAnimasyonu;

    [Header("Arayüz (Canvas)")]
    public GameObject dolapMenusuCanvas; // Yeni yapacağımız İçecek menüsü
    private OyuncuEnvanter islemYapanOyuncu;

    public float kapanmaMesafesi = 4f;

    [Header("İçecek Prefabları")]
    public GameObject ayranPrefab;
    public GameObject kolaPrefab;
    public GameObject suPrefab;
    public GameObject salgamPrefab;

    void Start()
    {
        if (dolapMenusuCanvas != null) dolapMenusuCanvas.SetActive(false);
    }

    void Update()
    {
        // Oyuncu uzaklaşırsa menüyü ve kapağı kapat
        if (islemYapanOyuncu != null && dolapMenusuCanvas != null && dolapMenusuCanvas.activeSelf)
        {
            float mesafe = Vector3.Distance(transform.position, islemYapanOyuncu.transform.position);

            if (mesafe > kapanmaMesafesi)
            {
                Buton_MenuyuKapat();
            }
        }
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // Elinde bir şey varsa dolaba geri koy
        if (oyuncu.GetHeldMalzeme() != null)
        {
            oyuncu.EldenBirakVeSil();
            return;
        }

        // Elinde tepsi veya bıçak varsa uyarı ver
        if (oyuncu.GetHeldTray() != null || oyuncu.bicakVarMi)
        {
            Debug.Log("İçecek almak için ellerin boş olmalı!");
            return;
        }

        islemYapanOyuncu = oyuncu;

        // Kapağı aç
        if (!kapakAcikMi)
        {
            if (kapakAnimasyonu != null) StopCoroutine(kapakAnimasyonu);
            kapakAnimasyonu = StartCoroutine(KapakDondur(acikAci));
        }

        // Menüyü göster
        if (dolapMenusuCanvas != null) dolapMenusuCanvas.SetActive(true);
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

    // Buton fonksiyonları
    public void Buton_AyranAl() { MalzemeVer(ayranPrefab); }
    public void Buton_KolaAl() { MalzemeVer(kolaPrefab); }
    public void Buton_SuAl() { MalzemeVer(suPrefab); }
    public void Buton_SalgamAl() { MalzemeVer(salgamPrefab); }

    public void Buton_MenuyuKapat()
    {
        if (dolapMenusuCanvas != null) dolapMenusuCanvas.SetActive(false);
        islemYapanOyuncu = null;

        // Kapağı geri ört
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
            GameObject yeniIcecek = Instantiate(secilenPrefab);
            islemYapanOyuncu.PickUpItem(yeniIcecek);
            Buton_MenuyuKapat();
        }
    }
}