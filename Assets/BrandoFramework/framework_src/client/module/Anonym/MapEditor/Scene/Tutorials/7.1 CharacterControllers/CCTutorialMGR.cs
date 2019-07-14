using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Anonym.Isometric;

public class CCTutorialMGR : MonoBehaviour {
    [Header("CC Prefabs")]
    [SerializeField]
    IsometricMovement simpleCC;

    [SerializeField]
    IsometricMovement advancedCC;

    [SerializeField]
    IsometricMovement navCC;

    [Anonym.Util.ConditionalHide("ccInstance", "null", hideInInspector:false)]
    IsometricMovement ccInstance;

    [Header("Starting")]
    [SerializeField]
    Transform startingPoint;

    [SerializeField]
    float fDropHeight = 1;

    [Header("Singleton")]
    [SerializeField]
    KeyInputAssist inputAssist;

    //[Header("UI")]
    //[SerializeField]
    //Text CCTypeText;

    //[SerializeField]
    //Text CCTypeDescText;

    //[SerializeField]
    //Text KeyText;

    private void Start()
    {
        NavMeshAgentCC();
    }

    void ClearCC()
    {
        if (ccInstance != null)
        {
            Destroy(ccInstance.gameObject);
            ccInstance = null;
        }
    }

    void CreateCCInstance(IsometricMovement prefab)
    {
        ClearCC();

        ccInstance = GameObject.Instantiate(prefab);
        ccInstance.transform.position = startingPoint.position + Vector3.up * fDropHeight;

        inputAssist.SetTarget(ccInstance);

        //CCTypeText.text = CCType;
    }

    public void SimpleCC()
    {
        CreateCCInstance(simpleCC);
    }

    public void AdvanceCC()
    {
        CreateCCInstance(advancedCC);
    }

    public void NavMeshAgentCC()
    {
        CreateCCInstance(navCC);
    }

    public string CCType
    {
        get {
            return ccInstance != null ? ccInstance.GetComponent<IsometricMovement>().GetType().Name : "null";
        }
    }
}
