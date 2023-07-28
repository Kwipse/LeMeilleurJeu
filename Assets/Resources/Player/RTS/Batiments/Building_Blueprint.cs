using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Building_Blueprint : MonoBehaviour
{
    /*
	en cours blueprint chantier batiment ruine
    A attacher à un blueprint de batiment

    le blueprint de reste sous le curseur que sur un objet contenant un collider et sur le layer 8
    ce script maintient le blueprint sous la souris et detruit le blueprint pour mettre le batiment correspondant
    */
    /*a faire
     * prix modulaire 
     * condition de terrain modulaire
     */
   
    RaycastHit hit;
    Vector3 movePoint;
    public GameObject chantierPrefab;
    public Animator RTSPlayerAnimator;
    public Material prebuild, notAllowed;
    public bool isAllowed=false;
    private RTSRessourceManager ressourceManager;
    
    void Start()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 50000.0f, (1<<8)))
        {
            transform.position = new Vector3(hit.point.x, 0, hit.point.z);
        }
        RTSPlayerAnimator = GameObject.Find("RTSPlayer(Clone)").GetComponent<Animator>();
        ressourceManager = GameObject.Find("RTSManager").GetComponent<RTSRessourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(CheckConstructionAllowance())
        {
            // mode prebuild
            isAllowed = true;
            ApplyMaterialToAllChildren(prebuild);
            PLayerClick();
        }
        else 
        {
            //mode NotAllowed
            isAllowed = false;
            ApplyMaterialToAllChildren(notAllowed);

        }
        FollowMouseForBlueprint();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);

        }
    }

    private void FollowMouseForBlueprint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50000.0f, (1 << 8)))
        {
            transform.position = hit.point;
        }
    }

    private void PLayerClick()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //le joueur essaye de creer le batiment
            GameObject go = Instantiate(chantierPrefab, new Vector3(hit.point.x, 0, hit.point.z), transform.rotation);
            go.GetComponent<NetworkObject>().Spawn();

            if (RTSPlayerAnimator != null)
            {
                RTSPlayerAnimator.SetBool("IsConstructed", true);
                RTSPlayerAnimator.SetBool("mineConstructionEnd", true);
            }
            else
            {
                Debug.Log("RTS Animator not detected");
            }
            Destroy(gameObject);
            ressourceManager.AddGold(-400);
        }
    }

    private bool CheckConstructionAllowance()
    {
        if(CheckTerrainAllowance() && ressourceManager.MoreGoldThan(400))
        { return true; }
        else { return false; }
    }

    private bool CheckTerrainAllowance()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 4000.0f, (1 << 9)))
        {
            if (hit.transform.root.CompareTag("Minerai"))
            {
                return true;
            }
        }
        return false;
    }

    private void ApplyMaterialToAllChildren(Material mat)
    {
        for (int i = 0;i < gameObject.transform.childCount;i++)
        {
            transform.GetChild(i).GetComponent<Renderer>().material = mat;
        }
    }
}
