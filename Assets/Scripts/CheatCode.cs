using UnityEngine;
using System.Collections.Generic;

public class CheatCode : MonoBehaviour {

    public KeyCode[] codeMiniRadar;

    private List<KeyCode> codeMiniRadarTemp;

	// Use this for initialization
	void Start () {
        Reset();
	}

    void Reset()
    {
        codeMiniRadarTemp = new List<KeyCode>(codeMiniRadar);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(codeMiniRadarTemp[0]))
            {
                codeMiniRadarTemp.RemoveAt(0);
                if (codeMiniRadarTemp.Count == 0)
                {
                    ActiveCode();
                }
            }
            else
            {
                Reset();
            }
        }
	}

    void ActiveCode()
    {
        Debug.Log("cheat code !");
    }
}
