using UnityEngine;

public class Bicak : MonoBehaviour, IInteractable
{
    private OyuncuEnvanter bicagiAlanOyuncu;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // GÜNCELLENEN KISIM: Hem tepsi hem malzeme kontrolü yapılıyor
        if (oyuncu.GetHeldTray() != null || oyuncu.GetHeldMalzeme() != null)
        {
            Debug.Log("Bıçağı almak için önce elindeki eşyayı tezgaha bırak!");
            return;
        }

        if (!oyuncu.bicakVarMi)
        {
            bicagiAlanOyuncu = oyuncu;

            // EKSİK OLAN VE SORUNU ÇÖZEN HAYATİ SATIR: 
            // Oyuncuya sadece "bıçağın var" demiyoruz, "elimdeki bıçak TAM OLARAK BU" diyoruz.
            oyuncu.suAnkiBicakScripti = this;

            if (oyuncu.oyuncuAnimator != null)
            {
                oyuncu.oyuncuAnimator.SetTrigger("isPickingUp");
            }

            BicagiEleIsinla();
        }
    }

    public void BicagiEleIsinla()
    {
        if (bicagiAlanOyuncu == null) return;

        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().isKinematic = true;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        transform.SetParent(bicagiAlanOyuncu.elNoktasi);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        bicagiAlanOyuncu.bicakVarMi = true;
        Debug.Log("Bıçak başarıyla ele alındı ve envantere kaydedildi!");
    }
}