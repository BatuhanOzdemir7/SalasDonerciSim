using UnityEngine;

public class KamerayaBak : MonoBehaviour
{
    private Camera anaKamera;

    void Start()
    {
        anaKamera = Camera.main; // Sahnedeki ana kamerayý otomatik bulur
    }

    void LateUpdate()
    {
        if (anaKamera != null)
        {
            // Canvas'ýn yüzünü sürekli kameraya doðru çevirir
            transform.LookAt(transform.position + anaKamera.transform.rotation * Vector3.forward, anaKamera.transform.rotation * Vector3.up);
        }
    }
}