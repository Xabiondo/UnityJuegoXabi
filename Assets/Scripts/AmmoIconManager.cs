using UnityEngine;
using UnityEngine.UI;

public class AmmoIconManager : MonoBehaviour
{
    public GameObject arrowIconPrefab;
    public int maxAmmo = 10;
    public float spacing = 25f;

    private GameObject[] iconObjects; // Guardamos los GameObjects, no los Images

    void Start()
    {
        CreateAmmoIcons();
        SetAmmo(maxAmmo);
    }

    void CreateAmmoIcons()
    {
        iconObjects = new GameObject[maxAmmo];

        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject icon = Instantiate(arrowIconPrefab, transform);
            icon.name = "ArrowIcon_" + i;

            // Posicionar
            RectTransform rt = icon.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(i * spacing, 0);
            }

            iconObjects[i] = icon; // Guardamos el GameObject entero
        }
    }

    public void SetAmmo(int currentAmmo)
    {
        // Aseguramos que currentAmmo esté en rango
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);

        for (int i = 0; i < maxAmmo; i++)
        {
            if (iconObjects[i] != null)
            {
                // ✅ ACTIVAR/DESACTIVAR EL GAMEOBJECT ENTERO
                iconObjects[i].SetActive(i < currentAmmo);
            }
        }
    }
}