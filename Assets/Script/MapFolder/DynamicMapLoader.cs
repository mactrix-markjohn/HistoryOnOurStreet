using System;
using System.Collections;
using System.Collections.Generic;
using Google.Maps.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;

public class DynamicMapLoader : MonoBehaviour
{
    
    [Serializable]
    public class MapRegionUnloadedOutsideCircleEvent : UnityEvent<Vector3, float> {}
    
    /// <summary>
    /// Reference to the base map loader (which knows about MapsService).
    /// The <see cref="BaseMapLoader"/> handles the basic loading of a map region and provides
    /// default styling parameters and loading errors management.
    /// </summary>
    public MapController BaseMapLoader;
    
    
    [Tooltip(
        "The ground plane. We keep this centered underneath Camera.main, so as we move around " +
        "the game world the ground plane stays always underneath us. As such the Material " +
        "applied to this ground plane should either be untextured, or textured using worldspace " +
        "coordinates (as opposed to local uv coordinates), so that we cannot actually see the " +
        "ground plane moving around the world, creating the illusion that there is always ground " +
        "beneath us.")]
    public GameObject Ground;
    
    [Tooltip("Load the map whenever the camera moves more than this distance.")]
    public float CameraMoveDistance = 100f;

    [Tooltip("Load the map when the camera rotates by more than this angle, in degrees.")]
    public float CameraMoveAngle = 10f;
    
    /// <summary>
    /// This event is dispatched the map is unloaded outside a specified circle region.
    /// It is useful to perform housekeeping work in other parts of the game that relies on the map
    /// for gameplay.
    /// </summary>
    public MapRegionUnloadedOutsideCircleEvent UnloadedEvent;

    /// <summary>
    /// Do we need to restart coroutines when the component is next enabled?
    /// </summary>
    private bool RestartCoroutinesOnEnable;

    /// <summary>
    /// Handle to coroutine used to remove unneeded areas of the map.
    /// </summary>
    private Coroutine UnloadUnseenCoroutine;

    ///< summary>Interval in seconds at which unseen geometry is detected and unloaded.</summary>
    private const float UnloadUnseenDelay = 5f;

    /// <summary>
    /// Used to let the unload co-routine that we have loaded additional geometry and that we can
    /// unload older data.
    /// </summary>
    private bool NeedsUnloading = false;

    /// <summary>
    /// Used to indicate that we need to load the map using the current viewport - either because it
    /// hasn't been loaded yet, or because the camera has moved.
    /// </summary>
    private bool NeedsLoading = true;

    /// <summary>
    /// Position of the camera the last time the map was loaded.
    /// </summary>
    private Vector3 LastCameraPosition;

    /// <summary>
    /// Rotation of the camera the last time the map was loaded.
    /// </summary>
    private Quaternion LastCameraRotation;
    
    
    void Awake() {
        // Verify all required parameters are defined and correctly setup, skipping any further setup
        // if any parameter is missing or invalid.
        if (!VerifyParameters()) {
            // Disable this script to prevent error spamming (where Update will producing one or more
            // errors every frame because one or more parameters are undefined).
            enabled = false;
        }

        // Get the required base map loader
        if (BaseMapLoader == null) {
            Debug.LogError(ExampleErrors.MissingParameter(
                this, BaseMapLoader, "Base Map Loader", "is required for this script to work."));
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutines(); 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Quaternion cameraRotation = Camera.main.transform.rotation;
        float distanceSqr = (cameraPosition - LastCameraPosition).sqrMagnitude;
        float angle = Quaternion.Angle(cameraRotation, LastCameraRotation);

        if (distanceSqr > (CameraMoveDistance * CameraMoveDistance) ||
            angle > CameraMoveAngle) {
            NeedsLoading = true;
        }

        if (NeedsLoading) {
            Ground.transform.position = new Vector3(cameraPosition.x, 0f, cameraPosition.z);
            if (BaseMapLoader != null) {
                BaseMapLoader.LoadMap();
            }

            LastCameraPosition = cameraPosition;
            LastCameraRotation = cameraRotation;
            NeedsLoading = false;
            NeedsUnloading = true;
        }
    }
    
    /// <summary>Periodically remove unneeded areas of the map.</summary>
    private IEnumerator UnloadUnseen() {
        while (true) {
            if (BaseMapLoader != null && BaseMapLoader.IsInitialized && NeedsUnloading) {
                // Unload map regions that are not in viewport, and are outside a radius around the
                // camera. This is to avoid unloading geometry that may be reloaded again very shortly (as
                // it is right on the edge of the view).
                BaseMapLoader.mapsService.MakeMapLoadRegion()
                    .AddCircle(Camera.main.transform.position, 1000f)
                    .UnloadOutside();

                if (UnloadedEvent != null)
                    UnloadedEvent.Invoke(Camera.main.transform.position, 1000f);

                // Reset unload flag to prevent unnecessary calls each interval.
                NeedsUnloading = false;
            }

            // Wait for a preset interval before seeing if new geometry needs to be unloaded.
            yield return new WaitForSeconds(UnloadUnseenDelay);
        }
    }
    
    private bool VerifyParameters() {
        // TODO(b/149056787): Standardize parameter verification across scripts.
        // Verify that a Ground plane has been given.
        if (Ground == null) {
            Debug.LogError(ExampleErrors.MissingParameter(this, Ground, "Ground"));

            return false;
        }

        // Verify that there is a Camera.main in the scene (i.e. a Camera that is tagged:
        // "MainCamera").
        if (Camera.main == null) {
            Debug.LogError(ExampleErrors.NullMainCamera(this));

            return false;
        }

        // If have reached this point then we have verified all required parameters.
        return true;
    }
    
    /// <summary>
    /// Start any coroutines needed by this component.
    /// </summary>
    private void StartCoroutines() {
        // Run a coroutine to clean up unseen objects.
        UnloadUnseenCoroutine = StartCoroutine(UnloadUnseen());
    }
    
    /// <summary>
    /// Handle Unity OnDisable event.
    /// </summary>
    void OnDisable() {
        if (UnloadUnseenCoroutine != null) {
            StopCoroutine(UnloadUnseenCoroutine);
            UnloadUnseenCoroutine = null;
            RestartCoroutinesOnEnable = true;
        }
    }

    /// <summary>
    /// Handle Unity OnEnable event.
    /// </summary>
    void OnEnable() {
        if (RestartCoroutinesOnEnable) {
            StartCoroutines();
            RestartCoroutinesOnEnable = false;
        }
    }
}
