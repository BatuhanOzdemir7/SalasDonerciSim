using System.Collections.Generic;
using UnityEngine;

public class KasaYonetici : MonoBehaviour
{
    public static KasaYonetici Instance;
    public Transform kasaBeklemeNoktasi;

    // Kasadaki kuyruk yapýsý
    private Queue<MusteriAI> kasaKuyrugu = new Queue<MusteriAI>();

    void Awake()
    {
        Instance = this;
    }

    // Kuyruða müþteri ekle
    public void KuyrugaGir(MusteriAI musteri)
    {
        kasaKuyrugu.Enqueue(musteri);
        KuyruguGuncelle();
    }

    // Oyuncu kasaya týklayýnca ilk müþteriden hesap alýr
    public void HesapAl()
    {
        if (kasaKuyrugu.Count > 0)
        {
            MusteriAI siradakiMusteri = kasaKuyrugu.Dequeue();
            siradakiMusteri.OdemeYapVeGit();
            KuyruguGuncelle();
        }
        else
        {
            Debug.Log("Kasada bekleyen müþteri yok usta!");
        }
    }

    // Kuyruktakileri arkaya doðru hizalar (isteðe baðlý geliþtirilebilir)
    void KuyruguGuncelle()
    {
        int index = 0;
        foreach (var musteri in kasaKuyrugu)
        {
            Vector3 yeniPozisyon = kasaBeklemeNoktasi.position - (kasaBeklemeNoktasi.forward * (index * 1.2f));
            musteri.NavigasyonHedefiVer(yeniPozisyon);
            index++;
        }
    }
}
