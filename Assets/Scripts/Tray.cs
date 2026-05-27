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

    [Header("Transform Verileri")]
    public Vector3 orijinalBoyut;

    [Header("3D Görseller")]
    // Tepsideki o ham, voxel etlerin olduðu grup.
    public GameObject etGorselleriGrubu;
    // Tepsinin hiyerarþisinde duran, sarýlmýþ dürüm modeli.
    public GameObject durumGorseli;
    // Tepsinin üzerinde yüzen "et sayacý" (TextMeshPro).
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
        GorselleriGuncelle(); // Baþlangýçta görselleri doðru ayarla
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();
        bool qTusunaBasildiMi = Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.Q);

        // Q Tuþu: Lavaþ ile Dürüm Sarma Ýþlemi
        if (qTusunaBasildiMi)
        {
            if (eldekiMalzeme != null)
            {
                // Ýsimlendirme sorununu kökten çözüyoruz (büyük/küçük harf baðýmsýz)
                string objeAdi = eldekiMalzeme.name.ToLower();

                if (objeAdi.Contains("lavas") || objeAdi.Contains("lavaþ"))
                {
                    if (tepsidekiEtSayisi > 0 && !isDurum)
                    {
                        oyuncu.EldenBirakVeSil(); // Lavaþ silindi

                        // YENÝ SÝSYSTEM: Tepsiyi dürüme dönüþtürüyoruz
                        isDurum = true;

                        // Altýndaki ham et görsellerini siliyoruz
                        foreach (GameObject et in birikenEtGorselleri)
                        {
                            Destroy(et);
                        }
                        birikenEtGorselleri.Clear();

                        // !!! Verileri SIFIRLAMIYORUZ! Tepsi zehir bilgisini, malzeme bilgisini koruyor. !!!

                        GorselleriGuncelle(); // Dürüm görselini açar, sayacý gizler

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
            }
            return;
        }

        // F Tuþu: Tepsiye Malzeme Ekleme Ýþlemi (Dürüm sarýlmadan önce yapýlabilir)
        if (eldekiMalzeme != null && !isDurum)
        {
            string objeAdi = eldekiMalzeme.name.ToLower();
            if (objeAdi.Contains("marul"))
            {
                marulVarMi = true;
                oyuncu.EldenBirakVeSil();
                Debug.Log("Tepsiye marul eklendi.");
            }
            else if (objeAdi.Contains("sogan") || objeAdi.Contains("soðan"))
            {
                soganVarMi = true;
                oyuncu.EldenBirakVeSil();
                Debug.Log("Tepsiye soðan eklendi.");
            }
        }
    }

    public bool TepsiBosMu()
    {
        return tepsidekiEtSayisi == 0 && !isDurum && eklenenMalzemeler.Count == 0;
    }

    public void GorselleriGuncelle()
    {
        // 1. Sayacýn Durumu
        if (etSayaciYazisi != null)
        {
            if (isDurum)
            {
                etSayaciYazisi.gameObject.SetActive(false); // Dürümse sayacý gizle
            }
            else if (tepsidekiEtSayisi > 0)
            {
                etSayaciYazisi.gameObject.SetActive(true);
                etSayaciYazisi.text = tepsidekiEtSayisi.ToString();
            }
            else
            {
                etSayaciYazisi.gameObject.SetActive(false);
            }
        }

        // 2. Dürüm Görselinin Durumu
        if (durumGorseli != null)
        {
            // Dürüm görselini sadece dürüme dönüþtüðünde aktif ediyoruz
            durumGorseli.SetActive(isDurum);
        }

        // 3. Ham Et Görsellerinin Durumu
        if (etGorselleriGrubu != null)
        {
            // Dürüm sarýldýysa eski et görsellerini gizle
            etGorselleriGrubu.SetActive(!isDurum);
        }
    }

    public void EtEkle()
    {
        // Tepside dürüm varsa üzerine daha fazla açýk et atýlamaz
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

        foreach (GameObject et in birikenEtGorselleri)
        {
            Destroy(et);
        }
        birikenEtGorselleri.Clear();

        GorselleriGuncelle();
    }
}