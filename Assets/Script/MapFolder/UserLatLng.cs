using System.Collections;
using System.Collections.Generic;
using Google.Maps.Coord;
using UnityEngine;

public class UserLatLng
{
    public LatLng latLng;

    public UserLatLng(LatLng lng)
    {
        latLng = lng;
    }

    public LatLng LatLng
    {
        get => latLng;
        set => latLng = value;
    }

}
