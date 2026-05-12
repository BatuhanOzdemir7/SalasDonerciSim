using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Rigidbody yoksa otomatik ekler
public class UstaHareket : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float yürümeHýzý = 5f;
    public float dönmeHýzý = 20f;

    private Rigidbody rb;
    private Animator anim;
    private Vector3 hareketYonu;

    void Awake()
    {
        // Ana objedeki Rigidbody'yi al
        rb = GetComponent<Rigidbody>();

        // Ýçteki modelde (Child) bulunan Animator'ü bul (Parenting hilesi için)
        anim = GetComponentInChildren<Animator>();

        // FÝZÝK DÜZELTMESÝ: Karakterin devrilmemesi için rotasyonu koddan dondur
        rb.freezeRotation = true;
    }

    void Update()
    {
        // --- ESKÝ SÝSTEM (OLD INPUT SYSTEM) GÝRDÝLERÝ ---
        // Baþlarýna eksi (-) iþareti konularak W-S ve A-D yönleri tam tersine çevrildi
        float yatay = -Input.GetAxisRaw("Horizontal"); // D sola, A saða gider
        float dikey = -Input.GetAxisRaw("Vertical");   // W geriye, S ileriye gider

        // Yön vektörünü oluþtur ve normalize et (çapraz giderken ekstra hýzlanmayý önler)
        hareketYonu = new Vector3(yatay, 0f, dikey).normalized;

        // Animasyon kontrolü
        if (anim != null)
        {
            // Karakter hareket ediyorsa yürüme animasyonunu tetikle
            anim.SetBool("isWalking", hareketYonu.magnitude >= 0.1f);
        }
    }

    void FixedUpdate()
    {
        // Eðer usta hareket etmeye çalýþýyorsa
        if (hareketYonu.magnitude >= 0.1f)
        {
            // A- DÖNME ÝÞLEMÝ
            // Gidilen yöne doðru bakýþ açýsýný hesapla
            Quaternion targetRotation = Quaternion.LookRotation(hareketYonu);

            // Ustayý o yöne doðru fiziken yumuþakça çevir
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, dönmeHýzý * Time.fixedDeltaTime));

            // B- ÝLERLEME ÝÞLEMÝ
            rb.MovePosition(rb.position + hareketYonu * yürümeHýzý * Time.fixedDeltaTime);
        }
    }
}