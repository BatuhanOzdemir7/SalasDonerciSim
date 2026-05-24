using UnityEngine;

public class Bicak : MonoBehaviour, IInteractable
{
    private OyuncuEnvanter bicagiAlanOyuncu;

    public void Interact(OyuncuEnvanter oyuncu)
    {
        if (oyuncu.GetHeldTray() != null)
        {
            Debug.Log("Bıçağı almak için önce elindeki tepsiyi tezgaha bırak!");
            return;
        }

        if (!oyuncu.bicakVarMi)
        {
            bicagiAlanOyuncu = oyuncu;

            // Animasyonu tetikle (Görsel olarak oynasın)
            if (oyuncu.oyuncuAnimator != null)
            {
                oyuncu.oyuncuAnimator.SetTrigger("isPickingUp");
            }

            // HATA ÇÖZÜMÜ: Animasyon event'ini beklemeden bıçağı doğrudan ele bağlıyoruz
            BicagiEleIsinla();
        }
    }

    public void BicagiEleIsinla()
    {
        if (bicagiAlanOyuncu == null) return;

        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().isKinematic = true;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        // El noktasına sıfır hata ile sabitleme
        transform.SetParent(bicagiAlanOyuncu.elNoktasi);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one; // Elindeyken küçülmesini önler

        bicagiAlanOyuncu.bicakVarMi = true;
        Debug.Log("Bıçak başarıyla ele alındı!");
    }
}