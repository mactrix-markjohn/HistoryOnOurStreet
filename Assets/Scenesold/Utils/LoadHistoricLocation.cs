using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;
using Firebase.Unity.Editor;

public class LoadHistoricLocation : MonoBehaviour
{
    /// <summary>
    /// The purpose of this class is to load all the Historic location into a List once it is called
    /// </summary>


    public static List<HistoricLocation> ListHistoricLocations;

    private DatabaseReference dataReference;


    private void Start()
    {
        ListHistoricLocations = new List<HistoricLocation>();
        
        LoadFromFirebase();
    }


   

    void LoadFromFirebase()
    {
        // Set up the Editor before calling into the realtime database
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(ConstantVariables.DATABASE_URL);
        
        dataReference = FirebaseDatabase.DefaultInstance.RootReference.Child(ConstantVariables.HISTORIC_LOCATION);


        StartCoroutine(LoadLocations());


    }

    IEnumerator LoadLocations()
    {
        var data = LoadDataSnap(dataReference);
        
        yield return  new WaitUntil(() => data.IsCompleted);

        DataSnapshot dataSnapshot = data.Result;

        foreach (var datasnap in dataSnapshot.Children)
        {
            HistoricLocation historicLocation = JsonUtility.FromJson<HistoricLocation>(datasnap.GetRawJsonValue());
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









    void AddHistoricLocations(string name, double lat, double lng, string textcontent, int videoindex)
    {
    
        HistoricLocation historicLocation = new HistoricLocation(name,lat,lng,textcontent);
        ListHistoricLocations.Add(historicLocation);
    }

    void AddHistoricLoationswithVideo(string name, double lat, double lng, string videourl, string textcotent)
    {
        HistoricLocation historicLocation = new HistoricLocation(name,lat, lng,videourl,textcotent);
        ListHistoricLocations.Add(historicLocation);
    }

    void AddHistoricLoation(HistoricLocation historicLocation)
    {
        ListHistoricLocations.Add(historicLocation);
        
    }

    public List<HistoricLocation> GetHistoricLocations()
    {
        
        
        
        return ListHistoricLocations;
    }
    
    
    
}
