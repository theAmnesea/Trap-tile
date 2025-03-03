using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetUICursor : MonoBehaviour
{
    public GameObject objectToSelect;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(objectToSelect);
    }
}
