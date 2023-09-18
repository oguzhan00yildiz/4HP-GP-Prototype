using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionFinder : MonoBehaviour
{

    private TMPro.TMP_Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TMPro.TMP_Text>();
        _text.text = Application.productName + " " + Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
