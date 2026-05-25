using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Radyo : MonoBehaviour, IInteractable
{
    [Header("Ses Ayarlarý")]
    public AudioClip[] sarkilar;
    private AudioSource audioSource;
    private int sarkiIndeksi = -1;

    [Header("Titreþim Ayarlarý")]
    public Transform radyoGorseli;
    public float titresimHizi = 30f;
    public float titresimSiddeti = 0.015f;
    private Vector3 orijinalPozisyon;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Sesin sadece radyonun yanýndayken duyulmasý için 3D ayarlarý
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 15f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        if (radyoGorseli != null)
        {
            orijinalPozisyon = radyoGorseli.localPosition;
        }
    }

    void Update()
    {
        // Eðer müzik çalýyorsa modeli titret
        if (audioSource.isPlaying && radyoGorseli != null)
        {
            float xOffset = Mathf.Sin(Time.time * titresimHizi) * titresimSiddeti;
            float yOffset = Mathf.Cos(Time.time * titresimHizi * 1.2f) * titresimSiddeti;
            radyoGorseli.localPosition = orijinalPozisyon + new Vector3(xOffset, yOffset, 0);
        }
        // Çalmýyorsa modeli orijinal, düz konumuna geri getir
        else if (radyoGorseli != null && radyoGorseli.localPosition != orijinalPozisyon)
        {
            radyoGorseli.localPosition = orijinalPozisyon;
        }
    }

    public void Interact(OyuncuEnvanter oyuncu)
    {
        if (sarkilar.Length == 0)
        {
            Debug.LogWarning("Radyoya hiç MP3 yüklemedin!");
            return;
        }

        sarkiIndeksi++;

        // Eðer listedeki tüm þarkýlar çalýndýysa, radyoyu tamamen kapat ve sýfýrla
        if (sarkiIndeksi >= sarkilar.Length)
        {
            RadyoyuKapat();
            return;
        }

        // Yeni þarkýyý seç ve çal
        audioSource.clip = sarkilar[sarkiIndeksi];
        audioSource.Play();
        Debug.Log("Radyo çalýyor: Þarký " + (sarkiIndeksi + 1));
    }

    private void RadyoyuKapat()
    {
        sarkiIndeksi = -1;
        audioSource.Stop();

        if (radyoGorseli != null)
        {
            radyoGorseli.localPosition = orijinalPozisyon;
        }

        Debug.Log("Radyo kapatýldý.");
    }
}