using UnityEngine;
using UnityEngine.SceneManagement; // Sahneler arası geçiş için gerekli kütüphane

public class MainMenuManager : MonoBehaviour
{
    // Oyuna Başla butonuna tıklanınca çalışacak fonksiyon
    public void OyunaBasla()
    {
        // "OyunSahnesi" yazan yere oyununun asıl sahnesinin adını veya Build Index numarasını yazmalısın.
        SceneManager.LoadScene("SampleScene");
    }

    // Ayarlar butonuna tıklanınca çalışacak fonksiyon
    public void Ayarlar()
    {
        Debug.Log("Ayarlar menüsü açıldı!");
        // İleride buraya ayarlar panelini (GameObject.SetActive(true)) açacak kodu ekleyeceğiz.
    }

    // Hakkında butonuna tıklanınca çalışacak fonksiyon
    public void Hakkinda()
    {
        Debug.Log("Hakkında paneli açıldı!");
        // İleride buraya hakkında panelini açacak kodu ekleyeceğiz.
    }

    // Çıkış butonuna tıklanınca çalışacak fonksiyon
    public void Cikis()
    {
        Debug.Log("Oyundan çıkılıyor...");
        Application.Quit(); // Bu kod Unity Editörde çalışmaz, sadece oyunu build aldığında çalışır.
    }
}