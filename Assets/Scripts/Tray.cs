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

    [Header("Ýçecek Verileri")]
    public bool ayranVarMi = false;
    public bool suVarMi = false;
    public bool kolaVarMi = false;

    [Header("Transform Verileri")]
    public Vector3 orijinalBoyut;

    [Header("3D Görseller")]
    public GameObject etGorselleriGrubu;
    public GameObject durumGorseli;
    public GameObject patatesGorseli;
    public GameObject ayranGorseli;
    public GameObject suGorseli;
    public GameObject kolaGorseli;
    public TMP_Text etSayaciYazisi;

    [Header("Yeni Fiziksel Et Yýðýný")]
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
        ayranVarMi = false;
        suVarMi = false;
        kolaVarMi = false;
        patatesVarMi = false;
        GorselleriGuncelle();
        DurumVerileriniGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

        // STANDART: Dürüm sarmak ve hazýrlamak artýk F tuþu
        bool fTusunaBasildiMi = Input.GetKey(KeyCode.F) || Input.GetKeyDown(KeyCode.F);

        if (fTusunaBasildiMi)
        {
            if (eldekiMalzeme != null)
            {
                string objeAdi = eldekiMalzeme.name.ToLower();

                // 1. LAVAÞ
                if (objeAdi.Contains("lavas") || objeAdi.Contains("lavaþ"))
                {
                    if (tepsidekiEtSayisi > 0 && !isDurum)
                    {
                        oyuncu.EldenBirakVeSil();
                        isDurum = true;
                        foreach (GameObject et in birikenEtGorselleri) Destroy(et);
                        birikenEtGorselleri.Clear();
                        GorselleriGuncelle();
                        Debug.Log("<color=green>BAÞARILI: Lavaþ eklendi, tepsideki etler dürüme dönüþtü!</color>");
                    }
                    else Debug.LogWarning("Tepside et yok veya zaten dürüm var!");
                }
                // 2. PATATES
                else if (objeAdi.Contains("patates") || objeAdi.Contains("frenchfries") || objeAdi.Contains("fries"))
                {
                    if (objeAdi.Contains("cig") || objeAdi.Contains("yanik")) Debug.LogWarning("Çið veya yanýk patates eklenemez!");
                    else
                    {
                        if (!patatesVarMi)
                        {
                            patatesVarMi = true;
                            oyuncu.EldenBirakVeSil();
                            GorselleriGuncelle();
                            Debug.Log("<color=green>BAÞARILI: Tepsiye patates kýzartmasý eklendi.</color>");
                        }
                    }
                }
                // 3. SOÐAN
                else if (objeAdi.Contains("sogan") || objeAdi.Contains("soðan") || objeAdi.Contains("onion"))
                {
                    if (!soganVarMi) { soganVarMi = true; oyuncu.EldenBirakVeSil(); Debug.Log("<color=green>Soðan eklendi.</color>"); }
                }
                // 4. MARUL
                else if (objeAdi.Contains("marul") || objeAdi.Contains("lettuce"))
                {
                    if (!marulVarMi) { marulVarMi = true; oyuncu.EldenBirakVeSil(); Debug.Log("<color=green>Marul eklendi.</color>"); }
                }
                // 5. TURÞU
                else if (objeAdi.Contains("tursu") || objeAdi.Contains("turþu") || objeAdi.Contains("cucumber") || objeAdi.Contains("pickle"))
                {
                    if (!tursuVarMi) { tursuVarMi = true; oyuncu.EldenBirakVeSil(); Debug.Log("<color=green>Turþu eklendi.</color>"); }
                }
                // 6. SOS (KEPÇE)
                else if (objeAdi.Contains("kepce") || objeAdi.Contains("kepçe") || objeAdi.Contains("ladle"))
                {
                    if (isDurum)
                    {
                        Kepce kepceScript = eldekiMalzeme.GetComponent<Kepce>();
                        if (kepceScript != null)
                        {
                            if (kepceScript.doluMu)
                            {
                                if (!sosVarMi)
                                {
                                    sosVarMi = true;
                                    kepceScript.SosuKullan();
                                    Debug.Log("<color=green>BAÞARILI: Dürüme F tuþu ile sos döküldü!</color>");
                                }
                                else Debug.LogWarning("Dürümde zaten sos var!");
                            }
                            else Debug.LogWarning("Kepçe boþ, önce kazandan sos doldurmalýsýn.");
                        }
                    }
                    else Debug.LogWarning("Tepside dürüm yok, sos sadece dürüme dökülebilir!");
                }
                // 7. AYRAN
                else if (objeAdi.Contains("ayran"))
                {
                    if (!ayranVarMi && !suVarMi && !kolaVarMi)
                    {
                        ayranVarMi = true;
                        oyuncu.EldenBirakVeSil();
                        GorselleriGuncelle();
                        Debug.Log("<color=green>BAÞARILI: Tepsiye Ayran eklendi.</color>");
                    }
                    else Debug.LogWarning("UYARI: Tepside zaten bir içecek var!");
                }
                // 8. KOLA
                else if (objeAdi.Contains("kola") || objeAdi.Contains("cola"))
                {
                    if (!ayranVarMi && !suVarMi && !kolaVarMi)
                    {
                        kolaVarMi = true;
                        oyuncu.EldenBirakVeSil();
                        GorselleriGuncelle();
                        Debug.Log("<color=green>BAÞARILI: Tepsiye Kola eklendi.</color>");
                    }
                    else Debug.LogWarning("UYARI: Tepside zaten bir içecek var!");
                }
                // 9. SU
                else if (objeAdi.Contains("su") || objeAdi.Contains("water"))
                {
                    if (!ayranVarMi && !suVarMi && !kolaVarMi)
                    {
                        suVarMi = true;
                        oyuncu.EldenBirakVeSil();
                        GorselleriGuncelle();
                        Debug.Log("<color=green>BAÞARILI: Tepsiye Su eklendi.</color>");
                    }
                    else Debug.LogWarning("UYARI: Tepside zaten bir içecek var!");
                }
                else Debug.LogWarning("F'ye bastýn ama elindeki obje tanýnmadý! Adý: " + eldekiMalzeme.name);

                DurumVerileriniGuncelle();
            }
            return;
        }
    }

    public bool TepsiBosMu()
    {
        // Et, dürüm, malzemeler, içecekler veya patates varsa tepsi BOÞ DEÐÝLDÝR!
        return tepsidekiEtSayisi == 0 &&
               !isDurum &&
               eklenenMalzemeler.Count == 0 &&
               !ayranVarMi &&
               !suVarMi &&
               !kolaVarMi &&
               !patatesVarMi;
    }

    public void GorselleriGuncelle()
    {
        if (etSayaciYazisi != null)
        {
            if (isDurum) etSayaciYazisi.gameObject.SetActive(false);
            else if (tepsidekiEtSayisi > 0) { etSayaciYazisi.gameObject.SetActive(true); etSayaciYazisi.text = tepsidekiEtSayisi.ToString(); }
            else etSayaciYazisi.gameObject.SetActive(false);
        }

        if (durumGorseli != null) durumGorseli.SetActive(isDurum);
        if (etGorselleriGrubu != null) etGorselleriGrubu.SetActive(!isDurum);
        if (patatesGorseli != null) patatesGorseli.SetActive(patatesVarMi);
        if (ayranGorseli != null) ayranGorseli.SetActive(ayranVarMi);
        if (suGorseli != null) suGorseli.SetActive(suVarMi);
        if (kolaGorseli != null) kolaGorseli.SetActive(kolaVarMi);
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
        DurumVerileriniGuncelle();
    }

    public void TepsiyiSifirla()
    {
        tepsidekiEtSayisi = 0; isDurum = false; isMeatCold = false; eklenenMalzemeler.Clear();
        zehirliEtVarMi = false; sosVarMi = false; soganVarMi = false; marulVarMi = false; tursuVarMi = false; patatesVarMi = false;
        ayranVarMi = false; suVarMi = false; kolaVarMi = false;

        foreach (GameObject et in birikenEtGorselleri) Destroy(et);
        birikenEtGorselleri.Clear();

        GorselleriGuncelle();
        DurumVerileriniGuncelle();
    }

    public void DurumVerileriniGuncelle()
    {
        Durum icindekiDurum = GetComponentInChildren<Durum>(true);
        if (icindekiDurum != null)
        {
            icindekiDurum.kullanilanDonerSayisi = tepsidekiEtSayisi;
            icindekiDurum.sosKullanildiMi = sosVarMi;
            icindekiDurum.soganVarMi = soganVarMi;
            icindekiDurum.marulVarMi = marulVarMi;
            icindekiDurum.tursuVarMi = tursuVarMi;
            icindekiDurum.patatesVarMi = patatesVarMi;
            icindekiDurum.donerZehirliMi = zehirliEtVarMi;
        }
        else
        {
            Debug.LogWarning("UYARI: Tepsinin altýnda 'Durum' scripti bulunamadý.");
        }
    }
}