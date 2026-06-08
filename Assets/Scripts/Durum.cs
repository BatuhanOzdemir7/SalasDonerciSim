using UnityEngine;

public class Durum : MonoBehaviour
{
    [Header("Dürüm İçeriği")]
    public float donerMiktari = 0f;
    public int kullanilanDonerSayisi = 0;
    public bool sosKullanildiMi = false;
    public bool soganVarMi = false;
    public bool marulVarMi = false;
    public bool tursuVarMi = false;
    public bool patatesVarMi = false;

    [Header("Sağlık Durumu")]
    public bool donerZehirliMi = false;

    // Not: Müşteri AI sistemini yazdığında, müşteriler dürümü aldıklarında 
    // doğrudan bu scriptteki bool değerlerini okuyarak sana puan/para verecek.
}