using UnityEngine;

public class uiController : MonoBehaviour
{
    TMPro.TMP_Dropdown modeDropdown;
    BoidSimulationControl boidSimulationControl;

    private void Start()
    {
        boidSimulationControl = FindFirstObjectByType<BoidSimulationControl>();
        modeDropdown = GetComponent<TMPro.TMP_Dropdown>();
        modeDropdown.onValueChanged.AddListener(boidSimulationControl.SetControlMode); 
    }

    void Update()
    {
        modeDropdown.value = (int)boidSimulationControl.controlMode;

    }
}
