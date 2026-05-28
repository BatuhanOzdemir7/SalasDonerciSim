using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Tray : MonoBehaviour, IInteractable
{
    [Header("Tepsi Ýçerik Verileri")]
    public int tepsidekiEtSayisi = 0;
    public bool isMeatCold = false;
    public bool isDurum = false;
    public List<string> eklenenMalzemeler = new List<string>();

    [Header("Dürüm Ýçerik Verileri")]
    public bool zehirliEtVarMi = false;
    public bool sosVarMi = false;
    public bool soganVarMi = false;
    public bool marulVarMi = false;
    public bool tursuVarMi = false;
    public bool patatesVarMi = false;

    [Header("Transform Verileri")]
    public Vector3 orijinalBoyut;

    [Header("3D Görseller")]
    public GameObject etGorselleriGrubu;
    public GameObject durumGorseli;
    public GameObject patatesGorseli;
    public TMP_Text etSayaciYazisi;

    [Header("Yeni Fiziksel Et Yýðýný (Fritözden Düþenler Ýçin)")]
    public GameObject kesilmisEtPrefab;
    public Transform etlerinBirikecegiNokta;
    public float etKalinligi = 0.02f;

    private List<GameObject> birikenEtGorselleri = new List<GameObject>();

    void Start()
    {
        if (orijinalBoyut == Vector3.zero)
        {
            orijinalBoyut = transform.localScale;
        }
        GorselleriGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();
        bool qTusunaBasildiMi = Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.Q);

        // Q Tuþu Ýþlemleri: Lavaþ, Patates ve Tüm Yeþillik/Malzemeler
        if (qTusunaBasildiMi)
        {
            if (eldekiMalzeme != null)
            {
                string objeAdi = eldekiMalzeme.name.ToLower();

                // 1. LAVAÞ KONTROLÜ
                if (objeAdi.Contains("lavas") || objeAdi.Contains("lavaþ"))
                {
                    if (tepsidekiEtSayisi > 0 && !isDurum)
                    {
                        oyuncu.EldenBirakVeSil();
                        isDurum = true;

                        foreach (GameObject et in birikenEtGorselleri)
                        {
                            Destroy(et);
                        }
                        birikenEtGorselleri.Clear();

                        GorselleriGuncelle();
                        Debug.Log("<color=green>BAÞARILI: Lavaþ eklendi, tepsideki etler dürüme dönüþtü!</color>");
                    }
                    else if (isDurum)
                    {
                        Debug.LogWarning("UYARI: Tepside zaten bir dürüm var!");
                    }
                    else
                    {
                        Debug.LogWarning("UYARI: Tepside et olmadýðý için dürüm sarýlamaz!");
                    }
                }
                // 2. PATATES KIZARTMASI KONTROLÜ
                else if (objeAdi.Contains("patates") || objeAdi.Contains("frenchfries") || objeAdi.Contains("fries"))
                {
                    if (objeAdi.Contains("cig") || objeAdi.Contains("çið") || objeAdi.Contains("yanik") || objeAdi.Contains("yanýk"))
                    {
                        Debug.LogWarning("UYARI: Çið veya yanýk patatesi tepsiye ekleyemezsin!");
                    }
                    else
                    {
                        if (!patatesVarMi)
                        {
                            patatesVarMi = true;
                            oyuncu.EldenBirakVeSil();
                            GorselleriGuncelle();
                            Debug.Log("<color=green>BAÞARILI: Tepsiye Q tuþu ile patates kýzartmasý eklendi.</color>");
                        }
                        else
                        {
                            Debug.LogWarning("Tepside zaten patates var!");
                        }
                    }
                }
                // 3. SOÐAN KONTROLÜ
                else if (objeAdi.Contains("sogan") || objeAdi.Contains("soðan") || objeAdi.Contains("onion"))
                {
                    if (!soganVarMi)
                    {
                        soganVarMi = true;
                        oyuncu.EldenBirakVeSil();
                        Debug.Log("<color=green>BAÞARILI: Dürüme/Tepsiye Q tuþu ile soðan eklendi.</color>");
                    }
                    else
                    {
                        Debug.LogWarning("Tepside zaten soðan var!");
                    }
                }
                // 4. MARUL KONTROLÜ
                else if (objeAdi.Contains("marul") || objeAdi.Contains("lettuce"))
                {
                    if (!marulVarMi)
                    {
                        marulVarMi = true;
                        oyuncu.EldenBirakVeSil();
                        Debug.Log("<color=green>BAÞARILI: Dürüme/Tepsiye Q tuþu ile marul eklendi.</color>");
                    }
                    else
                    {
                        Debug.LogWarning("Tepside zaten marul var!");
                    }
                }
                // 5. TURÞU KONTROLÜ
                else if (objeAdi.Contains("tursu") || objeAdi.Contains("turþu") || objeAdi.Contains("cucumber") || objeAdi.Contains("pickle"))
                {
                    if (!tursuVarMi)
                    {
                        tursuVarMi = true;
                        oyuncu.EldenBirakVeSil();
                        Debug.Log("<color=green>BAÞARILI: Dürüme/Tepsiye Q tuþu ile turþu eklendi.</color>");
                    }
                    else
                    {
                        Debug.LogWarning("Tepside zaten turþu var!");
                    }
                }
                // 6. ÝSÝM UYUÞMAZLIÐI YAKALAYICI
                else
                {
                    Debug.LogWarning("UYARI: Q'ya bastýn ama elindeki obje Lavaþ, Patates veya Malzeme olarak tanýnmadý! Objenin Unity'deki tam adý: " + eldekiMalzeme.name);
                }
            }
            return;
        }
    }

    public bool TepsiBosMu()
    {
        return tepsidekiEtSayisi == 0 && !isDurum && eklenenMalzemeler.Count == 0;
    }

    public void GorselleriGuncelle()
    {
        if (etSayaciYazisi != null)
        {
            if (isDurum) etSayaciYazisi.gameObject.SetActive(false);
            else if (tepsidekiEtSayisi > 0)
            {
                etSayaciYazisi.gameObject.SetActive(true);
                etSayaciYazisi.text = tepsidekiEtSayisi.ToString();
            }
            else etSayaciYazisi.gameObject.SetActive(false);
        }

        if (durumGorseli != null) durumGorseli.SetActive(isDurum);
        if (etGorselleriGrubu != null) etGorselleriGrubu.SetActive(!isDurum);

        if (patatesGorseli != null) patatesGorseli.SetActive(patatesVarMi);
    }

    public void EtEkle()
    {
        if (isDurum) return;

        tepsidekiEtSayisi++;

        if (kesilmisEtPrefab != null && etlerinBirikecegiNokta != null)
        {
            GameObject yeniEtGorseli = Instantiate(kesilmisEtPrefab, etlerinBirikecegiNokta);
            float rastgeleAci = Random.Range(0f, 360f);
            float yukariKayma = (tepsidekiEtSayisi - 1) * etKalinligi;

            yeniEtGorseli.transform.localPosition = new Vector3(0, yukariKayma, 0);
            yeniEtGorseli.transform.localRotation = Quaternion.Euler(0, rastgeleAci, 0);

            birikenEtGorselleri.Add(yeniEtGorseli);
        }

        GorselleriGuncelle();
    }

    public void TepsiyiSifirla()
    {
        tepsidekiEtSayisi = 0;
        isDurum = false;
        isMeatCold = false;
        eklenenMalzemeler.Clear();

        zehirliEtVarMi = false;
        sosVarMi = false;
        soganVarMi = false;
        marulVarMi = false;
        tursuVarMi = false;
        patatesVarMi = false;

        foreach (GameObject et in birikenEtGorselleri)
        {
            Destroy(et);
        }
        birikenEtGorselleri.Clear();

        GorselleriGuncelle();
    }
}