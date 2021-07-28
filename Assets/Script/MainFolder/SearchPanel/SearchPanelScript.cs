using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Script.MainFolder;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SearchPanelScript : MonoBehaviour
{

    public static HistoricStreet selectedLocation;
    public GameObject Listitem;
    public GameObject ContentContainer;
    public GameObject ResultPanel;

    private List<HistoricStreet> ListOfSearchedStreet;
    private bool loadlock = true;
    

    public static List<HistoricStreet> ListHistoricLocations;
    private List<GameObject> ResultGameObject;


    public InputField searchField;
    public Text result;
    
    private DatabaseReference dataReference;
    // Start is called before the first frame update
    void Start()
    {
        
        ListHistoricLocations = new List<HistoricStreet>();
        ResultGameObject = new List<GameObject>();
        ResultPanel.SetActive(false);

        LoadFromFirebase(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void LoadFromFirebase()
    {
        
        // Set up the Editor before calling into the realtime database
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(ConstantString.DATABASE_URL);
        
        dataReference = FirebaseDatabase.DefaultInstance.RootReference.Child(ConstantString.HISTORIC_LOCATION);


        StartCoroutine(LoadLocations());


    }
    
    IEnumerator LoadLocations()
    {
        var data = LoadDataSnap(dataReference);
        
        yield return  new WaitUntil(() => data.IsCompleted);

        DataSnapshot dataSnapshot = data.Result;

        foreach (var datasnap in dataSnapshot.Children)
        {
            HistoricStreet historicLocation = JsonUtility.FromJson<HistoricStreet>(datasnap.GetRawJsonValue());
            AddHistoricLoation(historicLocation);
        }
        
        
    }
    
    private async Task<DataSnapshot> LoadDataSnap(DatabaseReference reference)
    {
        var datasnap = await reference.GetValueAsync();

        if (!datasnap.Exists)
        {
            return null;
        }

        return datasnap;
    }
    
    void AddHistoricLoation(HistoricStreet historicLocation)
    {
        ListHistoricLocations.Add(historicLocation);
        
    }

    void SpawnListItems()
    {
        
        foreach (var location in ListOfSearchedStreet)
        {
            GameObject historicList = Instantiate(Listitem);
            historicList.transform.SetParent(ContentContainer.transform, false);
                
            historicList.GetComponent<ItemListSearchPanel>().SetUp(location,this);
            
            ResultGameObject.Add(historicList);
                
                
        }
        
        
    }
    
    
    
    public void ItemClickHandle(HistoricStreet clickedLocation)
    {
        selectedLocation = clickedLocation;
        SceneManager.LoadScene(ConstantString.MAPSCENE);
    }

    public void SearchStreetClicked()
    {
        
        ListOfSearchedStreet = new List<HistoricStreet>();

        
        // Destroy all old result before spawning a new one
        foreach (var resultedobject in ResultGameObject)
        {
            Destroy(resultedobject);
        }
        
        ResultPanel.SetActive(false); // Make the panel visible
        
        string searchentry =  searchField.text;

        if (String.IsNullOrEmpty(searchentry))
        {
            // The string or seaerch is empty
            _ShowAndroidToastMessage("Sorry, there is no text in the Search bar");
        }
        else
        {
            // The string contains something, so search the ListHistoricLocation

            foreach (var street in ListHistoricLocations)
            {
                string streetname = street.LocationName;

                if (String.IsNullOrEmpty(streetname))
                {
                    continue;
                }

                if (streetname.ToLower().Contains(searchentry.ToLower()))
                {
                    // The Search entry is contained in the Street name
                    
                    ListOfSearchedStreet.Add(street);
                    
                }

            }
            
            
            // once the List is got, check if it has anything in it. If yes, then spawn the items

            if (ListOfSearchedStreet.Count > 0)
            {
                
                result.text = $"Results: {ListOfSearchedStreet.Count}";
                ResultPanel.SetActive(true);
                SpawnListItems();
            }
            else
            {
                _ShowAndroidToastMessage("Sorry, Nothing on this Street name was Found");
            }



        }



    }



    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    
}
