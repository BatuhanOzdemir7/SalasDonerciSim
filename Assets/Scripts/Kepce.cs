using UnityEngine;

public class Kepce : MonoBehaviour
{
    [Header("Sos Görseli")]
    public GameObject sosGorseli; // Kepçenin altındaki Cylinder (sos) buraya sürüklenecek
    public bool doluMu = true;    // Başlangıçta sos dolu olarak varsayıyoruz

    public void SosuKullan()
    {
        doluMu = false;
        if (sosGorseli != null)
        {
            sosGorseli.SetActive(false); // Sos görselini gizle
        }
    }

    public void SosuDoldur()
    {
        doluMu = true;
        if (sosGorseli != null)
        {
            sosGorseli.SetActive(true); // Sos görselini geri aç
        }
    }
}