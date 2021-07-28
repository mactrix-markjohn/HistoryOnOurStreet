using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItemScript : MonoBehaviour
{ 
    public Text LocationName;
    public Text Coordinate;
    private HistoricLocation location;
    private MainSceneScript mainScript;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(HistoricLocation historicLocation, MainSceneScript mainSceneScript)
    {
        location = historicLocation;
        mainScript = mainSceneScript;

        LocationName.text = location.LocationName;
        Coordinate.text = $"{location.latitude} , {location.longitude}";

    }

    public void handleClick()
    {
        mainScript.ItemClickHandle(location);
    }
}
