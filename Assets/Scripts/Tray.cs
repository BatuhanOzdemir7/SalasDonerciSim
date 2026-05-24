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

    [Header("3D Görseller")]
    public GameObject etGorselleriGrubu;
    public GameObject durumGorseli;
    public TextMeshPro etSayaciYazisi;

    // EKSÝK OLAN VE HATAYA SEBEP OLAN DEÐÝÞKEN BURASI
    [HideInInspector] public Vector3 orijinalBoyut;

    void Start()
    {
        // Oyun baþladýðý an tepsinin sahnede duran normal boyutunu (2.5) kaydet
        orijinalBoyut = transform.localScale;
        GorselleriGuncelle();
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        oyuncu.PickUpItem(this.gameObject);
    }

    public void TepsiyiSifirla()
    {
        tepsidekiEtSayisi = 0;
        isMeatCold = false;
        isDurum = false;
        eklenenMalzemeler.Clear();
        GorselleriGuncelle();
    }

    public void GorselleriGuncelle()
    {
        if (isDurum)
        {
            if (etGorselleriGrubu != null) etGorselleriGrubu.SetActive(false);
            if (durumGorseli != null) durumGorseli.SetActive(true);
        }
        else
        {
            if (durumGorseli != null) durumGorseli.SetActive(false);

            // Tepside et varsa etlerin biriktiði 3D modeli görünür yapýyoruz
            if (etGorselleriGrubu != null)
            {
                etGorselleriGrubu.SetActive(tepsidekiEtSayisi > 0);
            }
        }
    }
}