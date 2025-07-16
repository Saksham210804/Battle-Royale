using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera cam;
    public ParticleSystem muzzleFlash;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Play muzzle flash first
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Visual ray for debugging
        Debug.DrawRay(cam.transform.position, cam.transform.forward * range, Color.red, 1f);

        // Raycast shoot logic
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, ~0)) // ~0 = all layers
        {
            Debug.Log("Hit object: " + hit.transform.name + " | Tag: " + hit.transform.tag);

            // Get root in case the hit object is a child
            Transform root = hit.transform.root;

            if (root.CompareTag("Enemy"))
            {
                Enemy_AI enemy = root.GetComponent<Enemy_AI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log("Enemy hit! Damage applied.");
                }
                else
                {
                    Debug.LogWarning("Hit Enemy tag but no Enemy_AI script found on root.");
                }
            }
        }
        else
        {
            Debug.Log("Raycast missed.");
        }
    }
}
