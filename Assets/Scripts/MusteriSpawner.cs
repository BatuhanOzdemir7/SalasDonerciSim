using System.Collections.Generic;
using UnityEngine;

public class MusteriSpawner : MonoBehaviour
{
    [Header("Müþteri Modelleri (Prefablar)")]
    public GameObject[] musteriPrefablar;

    [Header("Doðuþ ve Çýkýþ Noktalarý")]
    public Transform dogusNoktasi;
    public Transform cikisNoktasi;

    [Header("Dükkandaki TÜM Sandalyeler (12 Adet)")]
    public List<Transform> tumSandalyeler;
    private List<Transform> bosSandalyeler = new List<Transform>();

    [Header("Zamanlama Ayarlarý")]
    public float musteriGelmeAraligi = 7f;
    private float timer;

    void Start()
    {
        timer = musteriGelmeAraligi;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = musteriGelmeAraligi;
            MusteriYaratmayiDene();
        }
    }

    void MusteriYaratmayiDene()
    {
        BosSandalyeleriGuncelle();

        if (bosSandalyeler.Count > 0)
        {
            int rastgeleMusteriIndex = Random.Range(0, musteriPrefablar.Length);
            GameObject secilenPrefab = musteriPrefablar[rastgeleMusteriIndex];

            int rastgeleSandalyeIndex = Random.Range(0, bosSandalyeler.Count);
            Transform secilenSandalye = bosSandalyeler[rastgeleSandalyeIndex];

            GameObject yeniMusteri = Instantiate(secilenPrefab, dogusNoktasi.position, dogusNoktasi.rotation);

            MusteriAI musteriScript = yeniMusteri.GetComponent<MusteriAI>();
            if (musteriScript != null)
            {
                musteriScript.hedefSandalye = secilenSandalye;
                musteriScript.cikisNoktasi = cikisNoktasi;
            }

            Debug.Log($"Yeni müþteri belirdi! Boþ olan {secilenSandalye.parent.name} -> {secilenSandalye.gameObject.name} koltuðuna gidiyor.");
        }
        else
        {
            Debug.Log("Dükkanda oturacak tek bir boþ sandalye bile yok usta! Müþteri kapýdan döndü.");
        }
    }

    void BosSandalyeleriGuncelle()
    {
        bosSandalyeler.Clear();
        MusteriAI[] sahnendekiMusteriler = FindObjectsOfType<MusteriAI>();

        foreach (Transform sandalye in tumSandalyeler)
        {
            bool sandalyeDoluMu = false;

            foreach (MusteriAI musteri in sahnendekiMusteriler)
            {
                if (musteri.hedefSandalye == sandalye &&
                    musteri.suAnkiDurum != MusteriAI.MusteriDurumu.Ayriliyor &&
                    musteri.suAnkiDurum != MusteriAI.MusteriDurumu.KasayaGidiyor &&
                    musteri.suAnkiDurum != MusteriAI.MusteriDurumu.KasadaBekliyor)
                {
                    sandalyeDoluMu = true;
                    break;
                }
            }

            if (!sandalyeDoluMu)
            {
                bosSandalyeler.Add(sandalye);
            }
        }
    }
}