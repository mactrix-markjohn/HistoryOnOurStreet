using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARHistoryPopUp : MonoBehaviour
{
    private ARContent arContent;
    private HistoricStreet historicStreet;

    public Text heading;
    public Text body;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(ARContent ARcontent, HistoricStreet street )
    {

        if ((ARcontent != null) && (street != null))
        {
            arContent = ARcontent;
            historicStreet = street;
        
            // set the info

            heading.text = street.LocationName;
            body.text = street.Textcontent;


        }

        
        
        

    }

    public void ARModeClicked()
    {
        arContent.OnHistoryBody();
        Destroy(gameObject);
    }

    public void CancelARClicked()
    {
        arContent.OffHistoryBody();
    }

    public void ClearContentClicked()
    {
        arContent.ClearContent();
        Destroy(gameObject);
    }

}
