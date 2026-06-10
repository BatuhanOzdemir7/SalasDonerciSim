using UnityEngine;

public class SupurgeIstasyonu : MonoBehaviour, IInteractable
{
    public Transform supurgeDurmaNoktasi; // İstasyonun neresine oturacağı

    public void Interact(OyuncuEnvanter oyuncu)
    {
        Malzeme eldekiMalzeme = oyuncu.GetHeldMalzeme();

        // Eğer oyuncunun elindeki şey süpürgeyse, istasyona geri koy
        if (eldekiMalzeme != null && eldekiMalzeme.malzemeTipi.ToLower() == "supurge")
        {
            oyuncu.EldenBirak(); // Oyuncunun elinden çıkarır

            // Süpürgeyi istasyona sabitle
            eldekiMalzeme.transform.SetParent(supurgeDurmaNoktasi);
            eldekiMalzeme.transform.localPosition = Vector3.zero;
            eldekiMalzeme.transform.localRotation = Quaternion.identity;

            // Fiziğini tekrar dondur ki yere düşmesin
            Rigidbody rb = eldekiMalzeme.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            // Tekrar alınabilmesi için lazerin çarpacağı collider'ı aç
            Collider col = eldekiMalzeme.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            Debug.Log("Süpürge güvenli bir şekilde yerine asıldı.");
        }
        else
        {
            Debug.Log("Buraya sadece süpürgeyi asabilirsin!");
            }
        }
}