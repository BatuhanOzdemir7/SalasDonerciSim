using UnityEngine;

public class UstaHareket : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float yürümeHýzý = 5f;
    public float dönmeHýzý = 10f; // Karakterin saða sola dönerkenki yumuþaklýðý

    private Rigidbody rb;
    private Animator anim;
    private Vector3 hareketYonu;

    void Start()
    {
        // Bileþenleri kodun içine alýyoruz
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // W, A, S, D tuþlarýndan gelen girdileri al (1 veya -1 olarak)
        float yatay = Input.GetAxisRaw("Horizontal"); // A ve D
        float dikey = Input.GetAxisRaw("Vertical");   // W ve S

        // Vektörü oluþtur ve normalize et (Çapraz giderken hýzlanmasýný engeller)
        hareketYonu = new Vector3(yatay, 0f, dikey).normalized;

        // Eðer karakter hareket ediyorsa animasyonu baþlat, duruyorsa durdur
        if (hareketYonu.magnitude >= 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    // Fizik iþlemleri her zaman FixedUpdate içinde yapýlýr (Duvarlardan titreyerek geçmemesi için)
    void FixedUpdate()
    {
        if (hareketYonu.magnitude >= 0.1f)
        {
            // 1. Karakterin yüzünü gittiði yöne doðru yumuþakça çevir
            Quaternion hedefDonus = Quaternion.LookRotation(hareketYonu);
            rb.rotation = Quaternion.Slerp(rb.rotation, hedefDonus, dönmeHýzý * Time.fixedDeltaTime);

            // 2. Karakteri o yöne doðru ilerlet
            rb.MovePosition(rb.position + hareketYonu * yürümeHýzý * Time.fixedDeltaTime);
        }
    }
}

