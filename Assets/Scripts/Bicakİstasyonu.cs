using UnityEngine;

public class BicakIstasyonu : MonoBehaviour, IInteractable
{
    [Header("İstasyon Ayarları")]
    public Transform bicakDurmaNoktasi; // Bıçağın masada tam oturacağı yer

    public void Interact(OyuncuEnvanter oyuncu)
    {
        // Sadece oyuncunun elinde bıçak varsa çalışır
        if (oyuncu.bicakVarMi && oyuncu.suAnkiBicakScripti != null)
        {
            Bicak eldekiBicak = oyuncu.suAnkiBicakScripti;

            // 1. Oyuncunun elini ve envanterini boşalt
            oyuncu.EldenBirak();
            oyuncu.bicakVarMi = false;
            oyuncu.suAnkiBicakScripti = null;

            // 2. Bıçağı masadaki noktaya fiziksel olarak sabitle
            eldekiBicak.transform.SetParent(bicakDurmaNoktasi);
            eldekiBicak.transform.localPosition = Vector3.zero;
            eldekiBicak.transform.localRotation = Quaternion.identity;
            eldekiBicak.transform.localScale = Vector3.one;

            // 3. Bıçağın sonradan tekrar alınabilmesi için lazer çarpışmasını (Collider) aktif et
            Collider col = eldekiBicak.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            Debug.Log("Bıçak başarıyla tezgaha bırakıldı.");
        }
        else
        {
            Debug.Log("Tezgaha bırakacak bir bıçağın yok!");
        }
    }
}