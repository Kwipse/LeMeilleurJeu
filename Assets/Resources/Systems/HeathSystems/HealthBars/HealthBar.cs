using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu]
public class HealthBar : ScriptableObject
{
    public GameObject healthBarPrefab;
    GameObject healthBarGo;

    public Color borderColor;
    public Color healthColor;
    public Color missingHealthColor;

    Image borderImage;
    Image healthImage;
    Image missingHealthImage;

    Bounds ownerBounds;
    
    public void CreateHealthBar(GameObject healthOwner)
    {
        ownerBounds = healthOwner.GetComponent<Collider>().bounds;

        healthBarGo = Instantiate(healthBarPrefab);
        healthBarGo.transform.position = healthOwner.transform.position + new Vector3(0, 2.5f*ownerBounds.extents.y, 0);
        healthBarGo.transform.rotation = healthOwner.transform.rotation;
        healthBarGo.transform.SetParent(healthOwner.transform);

        foreach(var img in healthBarGo.GetComponentsInChildren<Image>())
        {
            switch (img.gameObject.name) {
                case "Border":
                    img.color = borderColor;
                    break;
                case "CurrentHealth":
                    img.color = healthColor;
                    healthImage = img;
                    break;
                case "MissingHealth":
                    img.color = missingHealthColor;
                    break;
            }
        }
    }

    public void LookAtPosition(Vector3 pos)
    {
        healthBarGo?.transform.LookAt(pos);
    }

    //Set Health Amount
    public void SetHealth(float hp)
    {
        if (healthBarGo) { healthImage.fillAmount = hp; }
    }

    public void DestroyHealthBar()
    {
        Destroy(healthBarGo);
    } 
    
}
