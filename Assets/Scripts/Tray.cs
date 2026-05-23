using UnityEngine;
using System.Collections.Generic;

public class Tray : MonoBehaviour, IInteractable
{
    // Tabak Veri Mantýðý
    public int tabaktakiEtSayisi = 0;
    public bool isMeatCold = false;
    public bool isDurum = false;

    // Eklenen malzemelerin listesi
    public List<string> eklenenMalzemeler = new List<string>();

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // Tabak Alma Sistemi: Týpký býçak gibi ele alma fonksiyonu
        oyuncu.PickUpItem(this.gameObject);
        Debug.Log("Boþ/Dolu tabak ele alýndý.");
    }

    public void MalzemeEkle(string malzemeAdi)
    {
        if (!eklenenMalzemeler.Contains(malzemeAdi))
        {
            eklenenMalzemeler.Add(malzemeAdi);
            Debug.Log(malzemeAdi + " tabaða eklendi.");
        }
    }

    public void TabagiSifirla()
    {
        tabaktakiEtSayisi = 0;
        isMeatCold = false;
        isDurum = false;
        eklenenMalzemeler.Clear();
    }
}