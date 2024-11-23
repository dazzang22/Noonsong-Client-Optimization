using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtonManager : MonoBehaviour
{
    public MapManager mapManager;

    public void OnMapButtonClicked()
    {
        mapManager.OpenMap();
    }

    public void OnCloseButtonClicked()
    {
        mapManager.CloseMap();
    }
}