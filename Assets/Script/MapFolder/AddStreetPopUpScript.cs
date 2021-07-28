using System.Collections;
using System.Collections.Generic;
using Google.Maps.Coord;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddStreetPopUpScript : MonoBehaviour
{
    
    public static UserLatLng currentLocation;
    public static bool clicked = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetUp(LatLng location)
    {
        currentLocation = new UserLatLng(location);

    }

    public void AddHistoricStreet()
    {
        clicked = true;

        SceneManager.LoadScene(ConstantString.ADDLOCATIONSCENE);
        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}
