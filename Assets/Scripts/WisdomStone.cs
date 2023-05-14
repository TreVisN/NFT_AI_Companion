using System;
using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using Gamekit3D.Cameras;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class WisdomStone : MonoBehaviour
{
    public static WisdomStone Instance;
    public GameObject DialogPanel;
    public float TriggerRadius = 2f;
    private bool _isDialogShown = false;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if(_isDialogShown) return;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, TriggerRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Player"))
            {
                ShowDialog();
            }
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TriggerRadius);
    }

    private void ShowDialog()
    {
        _isDialogShown = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerInput.Instance.ReleaseControl();
        Time.timeScale = 0;
        DialogPanel.SetActive(true);
    }
    
    public void HideDialog()
    {
        DialogPanel.SetActive(false);
        Invoke("ChangeDialogShown", 6f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        PlayerInput.Instance.GainControl();
        
    }
    
    
    private void ChangeDialogShown()
    {
        _isDialogShown = !_isDialogShown;
    }
    
}
