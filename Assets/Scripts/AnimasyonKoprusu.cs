using UnityEngine;

public class AnimasyonKoprusu : MonoBehaviour
{
    private OyuncuEnvanter anaEnvanter;

    void Start()
    {
        // En dýþtaki (parent) ana oyuncu objesindeki envanter kodunu bulur
        anaEnvanter = GetComponentInParent<OyuncuEnvanter>();
    }

    // Animasyon Event'i doðrudan bu fonksiyonu çaðýracak
    public void NesneyiEleYapistirEvent()
    {
        if (anaEnvanter != null)
        {
            anaEnvanter.NesneyiEleYapistirEvent();
        }
        else
        {
            Debug.LogError("Ana oyuncu objesinde OyuncuEnvanter kodu bulunamadý!");
        }
    }
}