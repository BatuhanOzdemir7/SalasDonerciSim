using UnityEngine;
using TMPro;

public class FisYonetici : MonoBehaviour
{
    public static FisYonetici Instance;

    [Header("Fiþ Sistemi Ayarlarý")]
    public GameObject fisPrefab;
    public Transform fislikPaneli;

    // Maksimum alýnabilecek fiþ sýnýrý
    public int maksimumFisSayisi = 6;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Müþteri AI E'ye basýlýnca buraya soracak: "Mutfakta yer var mý?"
    public bool FisIcinYerVarMi()
    {
        if (fislikPaneli != null)
        {
            // Panelin içindeki çocuk (fiþ) objelerini sayar
            return fislikPaneli.childCount < maksimumFisSayisi;
        }
        return false;
    }

    // YENÝ: 'bool sos' parametresi eklendi
    public GameObject YeniFisOlustur(string musteriAdi, bool tursu, bool marul, bool sogan, bool patates, bool sos, string icecekAdi, int dilimSayisi)
    {
        if (fisPrefab != null && fislikPaneli != null)
        {
            GameObject yeniFis = Instantiate(fisPrefab, fislikPaneli);
            yeniFis.transform.SetAsLastSibling();

            TextMeshProUGUI fisYazisi = yeniFis.GetComponentInChildren<TextMeshProUGUI>();

            if (fisYazisi != null)
            {
                string siparisBilgisi = "<align=left><size=110%><b>1x DÜRÜM</b></size> <i>(" + dilimSayisi + " Dilim)</i>\nÝçindekiler:\n";

                if (tursu) siparisBilgisi += " - Turþu\n";
                if (marul) siparisBilgisi += " - Marul\n";
                if (sogan) siparisBilgisi += " - Soðan\n";
                if (sos) siparisBilgisi += " - Sos\n"; // Fiþe sos yazdýrýlýyor

                // Eðer hiçbir malzeme ve sos yoksa SADE yazdýr
                if (!tursu && !marul && !sogan && !sos) siparisBilgisi += " - SADE\n";

                siparisBilgisi += "\n<b>YAN ÜRÜNLER:</b>\n";
                if (patates) siparisBilgisi += " - Patates Kýzartmasý\n";
                siparisBilgisi += " - " + icecekAdi + "</align>";

                fisYazisi.text = siparisBilgisi;
            }

            return yeniFis; // Çýkan fiþi müþterinin aklýnda tutmasý için geri gönderiyoruz
        }

        return null;
    }
}