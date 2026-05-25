using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Tray : MonoBehaviour
{
    [Header("Tepsi Ýçerik Verileri")]
    public int tepsidekiEtSayisi = 0;
    public bool isMeatCold = false;
    public bool isDurum = false;
    public List<string> eklenenMalzemeler = new List<string>();

    [Header("Transform Verileri")]
    public Vector3 orijinalBoyut; // Silinen ve hataya sebep olan deðiþken geri eklendi

    [Header("Eski 3D Görseller")]
    public GameObject etGorselleriGrubu;
    public GameObject durumGorseli;
    public TMP_Text etSayaciYazisi;

    [Header("Yeni Fiziksel Et Yýðýný")]
    public GameObject kesilmisEtPrefab;
    public Transform etlerinBirikecegiNokta;
    public float etKalinligi = 0.02f;

    private List<GameObject> birikenEtGorselleri = new List<GameObject>();

    void Start()
    {
        // Oyun baþladýðýnda orijinal boyut ayarlanmamýþsa (0,0,0 ise), 
        // tepsinin o anki gerçek boyutunu orijinal boyut olarak hafýzaya kazýr.
        if (orijinalBoyut == Vector3.zero)
        {
            orijinalBoyut = transform.localScale;
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
            etSayaciYazisi.text = tepsidekiEtSayisi.ToString();
        }
    }

    public void EtEkle()
    {
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

        foreach (GameObject et in birikenEtGorselleri)
        {
            Destroy(et);
        }
        birikenEtGorselleri.Clear();

        GorselleriGuncelle();
    }
}