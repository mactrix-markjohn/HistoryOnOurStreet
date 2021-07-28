using UnityEngine;
using UnityEngine.UI;

namespace Script.MainFolder
{
    public class ItemListSearchPanel : MonoBehaviour
    {
        public Text LocationName;
        public Text Coordinate;
        private HistoricStreet location;
        private SearchPanelScript _searchPanelScript;
        
        
        public void SetUp(HistoricStreet historicLocation, SearchPanelScript searchPanelScript)
        {
            location = historicLocation;
            _searchPanelScript = searchPanelScript;

            LocationName.text = location.LocationName;
            Coordinate.text = $"{location.latitude} , {location.longitude}";

        }

        public void handleClick()
        {
            _searchPanelScript.ItemClickHandle(location);
        }
        
        
    }
    
    
    
    
}