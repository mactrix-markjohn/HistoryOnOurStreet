using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MainSceneScript : MonoBehaviour
{
    public static HistoricLocation selectedLocation;
    public GameObject Listitem;
    public GameObject ContentContainer;

    private List<HistoricLocation> listOfHistoricLocatios;
    private bool loadlock = true;
    
    // Start is called before the first frame update
    void Start()
    {
        requestLocationPermission();
    }

    // Update is called once per frame
    void Update()
    {
        LoadList();
    }



     void LoadList()
    {

        listOfHistoricLocatios = LoadHistoricLocation.ListHistoricLocations;

        if (listOfHistoricLocatios != null && loadlock)
        {
            foreach (var location in listOfHistoricLocatios)
            {
                GameObject historicList = Instantiate(Listitem);
                historicList.transform.SetParent(ContentContainer.transform, false);
                
                historicList.GetComponent<ListItemScript>().SetUp(location,this);
                
                
            }

            loadlock = false;

        }

    }


    void requestLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    public void ItemClickHandle(HistoricLocation clickedLocation)
    {
        selectedLocation = clickedLocation;
        SceneManager.LoadScene(ConstantVariables.MAPSCENE);
    }

    public void AddLocationButton()
    {
        SceneManager.LoadScene(ConstantVariables.ADDLOCATIONSCENE);
    }

    public void MyLocationGoogleMapButton()
    {
        selectedLocation = null;
        SceneManager.LoadScene(ConstantVariables.MAPSCENE);
    }
}
