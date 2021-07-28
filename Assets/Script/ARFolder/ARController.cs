using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARController : MonoBehaviour
{
    
    private List<Vector3> vectorList;
    public GameObject ARContentObject;
    public Camera ARCamera;
    public static Camera ARCameraStatic;
    private List<HistoricStreet> historicLocations;

    public GameObject ARContentPopUp;
    

    private int indexloop = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        vectorList = MapController.listVectors;
        historicLocations = MapController.staticHistoricList;
        
       

        SpawnHistoricCube();
        
       

        //TODO: Make sure the functions of ARContent class
        
        ARCameraStatic = ARCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        
        ProcessTouch();
    }

    private GameObject PopUp;
     void ProcessTouch()
    {
        if ((Input.touchCount > 0 ) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = ARCamera.ScreenPointToRay(Input.GetTouch(0).position); 
            //Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            
            RaycastHit raycastHit;

            if (Physics.Raycast(raycast, out raycastHit))
            { 
                // first implementation

                if (raycastHit.collider.CompareTag(ConstantString.MapPointerTag))
                {
                    
                    /*raycastHit.collider.GetComponentInParent<ARContent>().MapPointerClicked();
                    
                    raycastHit.collider.GetComponentInChildren<ARContent>().MapPointerClicked();*/

                    if (PopUp != null)
                    {
                        Destroy(PopUp);
                    }

                    ARContent content = raycastHit.collider.GetComponentInParent<ARContent>();

                    if (content != null)
                    {
                        PopUp = Instantiate(ARContentPopUp);
                        PopUp.GetComponent<ARHistoryPopUp>().SetUp(content, content.GetHistoricStreet());
                        
                        _ShowAndroidToastMessage("Map Pointer is Raycasted");
                    }
                    else
                    {
                        ARContent contentchild = raycastHit.collider.GetComponentInChildren<ARContent>();

                        if (contentchild != null)
                        {
                           PopUp = Instantiate(ARContentPopUp);
                           PopUp.GetComponent<ARHistoryPopUp>().SetUp(contentchild, contentchild.GetHistoricStreet());
                        }
                    }


                    //_ShowAndroidToastMessage("Completed FIRST call");
                }

                
            }
        }
    }
     

    void SpawnHistoricCube()
    {
        for (int i = 0; i < vectorList.Count; i++)
        {

            Vector3 vectorY = new Vector3(vectorList[i].x, -1, vectorList[i].z);
            GameObject ARsContent =  Instantiate(ARContentObject, vectorY, Quaternion.identity);
            
            ARContent arContent = ARsContent.GetComponent<ARContent>();
            arContent.SetUpContent(historicLocations[i]);
            
            _ShowAndroidToastMessage($"{vectorY.ToString()}");
            //TODO: Make sure to call the functions of ARContent class
            
            
            
        }
    }

    public void BackClick()
    {
        SceneManager.LoadScene(ConstantString.MAPSCENE);
    }


    public static Transform GetARCameraTransform()
    {
        return ARCameraStatic.transform;
    }
    
    public static void _ShowAndroidToastMessage(string message)
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
