using UnityEngine;

public class OyuncuEnvanter : MonoBehaviour
{
    [Header("Býçak Durumu")]
    public bool bicakVarMi = false;

    [Header("Gerekli Bileþenler")]
    public Transform elNoktasi;
    public Animator oyuncuAnimator;

    [HideInInspector] public Bicak suAnkiBicakScripti;

    // Animasyon tam elin nesneye deðdiði an bu fonksiyonu çaðýracak (Animation Event)
    public void NesneyiEleYapistirEvent()
    {
        // 1. BIÇAK ÝÇÝN ALMA MANTIÐI
        if (suAnkiBicakScripti != null)
        {
            suAnkiBicakScripti.BicagiEleIsinla();
            suAnkiBicakScripti = null; // Ýþ bitince hafýzayý temizle
        }

        // 2. YARIN BÝR GÜN TABAK EKLEDÝÐÝNDE SADECE BURAYA ÞUNU YAZACAKSIN:
        /*
        if (suAnkiTabakScripti != null)
        {
            suAnkiTabakScripti.TabagiEleIsinla();
            suAnkiTabakScripti = null;
        }
        */

        // BU SATIR SÝHRÝ YAPACAK: Animasyonun loopta kalmasýný engeller, tetikleyiciyi sýfýrlar!
        if (oyuncuAnimator != null)
        {
            oyuncuAnimator.ResetTrigger("isPickingUp");
        }
    }
}