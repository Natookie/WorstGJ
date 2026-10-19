using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Serializable]
    public struct ScreenNodePair
    {
        public string deviceName;
        public Transform nodeTarget;
    }

    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private List<ScreenNodePair> ScreenNodes;
    [SerializeField] private float enterTransitionTime = 1f;

    [SerializeField] private Dictionary<string, Transform> screenNodeDictionary = 
        new Dictionary<string, Transform>();
    private Transform currentScreenNode;
    private Transform prevScreenNode;
    private Coroutine enterRoutine;

    public bool IsTransitioning { get; private set; }

    private void Start()
    {
        // Map out ScreenNodes to screenNodeDictionary
        screenNodeDictionary.Clear();
        foreach (ScreenNodePair pair in ScreenNodes)
        {
            if (string.IsNullOrEmpty(pair.deviceName) || pair.nodeTarget == null)
            {
                Debug.LogWarning("ScreenNodes contains an entry with a missing name or target.", this);
                continue;
            }
            if (!screenNodeDictionary.TryAdd(pair.deviceName, pair.nodeTarget))
            {
                Debug.LogWarning($"Duplicate screen node '{pair.deviceName}' ignored.", this);
            }
        }

        // Initialize ScreenNodes
        foreach (Transform screenNode in screenNodeDictionary.Values)
        {
            screenNode.gameObject.SetActive(false);
        }
    }

    public void EnterScreenMode(string deviceName){
        // Validate deviceName
        if (deviceName == "")
        {
            Debug.LogWarning("Device name is empty");
            return;
        }
        if (!screenNodeDictionary.Keys.Contains(deviceName))
        {
            Debug.LogWarning($"Screen node target for: {deviceName}, did not exist");
            return;
        }

        if(enterRoutine != null) StopCoroutine(enterRoutine);

        if (prevScreenNode)
        {
            prevScreenNode.gameObject.SetActive(false);
            prevScreenNode = null;
        }
        currentScreenNode = screenNodeDictionary[deviceName];
        currentScreenNode.gameObject.SetActive(true);

        confiner.enabled = false;
        cam.Follow = currentScreenNode;
        cam.LookAt = currentScreenNode;

        IsTransitioning = true;
        enterRoutine = StartCoroutine(EndEnterAfter(enterTransitionTime));
    }

    public void ExitScreenMode(){
        if(enterRoutine != null){
            StopCoroutine(enterRoutine);
            enterRoutine = null;
        }

        if (currentScreenNode)
        {
            prevScreenNode = currentScreenNode;
            currentScreenNode = null;
        }

        cam.Follow = playerTarget;
        cam.LookAt = playerTarget;
        confiner.enabled = true;

        var brain = FindFirstObjectByType<CinemachineBrain>();
        if(brain != null) brain.ResetState();

        IsTransitioning = false;
    }

    IEnumerator EndEnterAfter(float seconds){
        yield return new WaitForSeconds(seconds);
        IsTransitioning = false;
        enterRoutine = null;
    }
}