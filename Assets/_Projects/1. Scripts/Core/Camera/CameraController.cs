using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float enterTransitionTime = 1f;

    private Transform currentScreenNode;
    private Transform prevScreenNode;
    private Coroutine enterRoutine;

    public bool IsTransitioning { get; private set; }

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    public void EnterScreenMode(Transform screenNode){
        if(screenNode == null){
            Debug.LogWarning("[CameraController] screenNode is null");
            return;
        }

        if(enterRoutine != null) StopCoroutine(enterRoutine);

        if(prevScreenNode != null){
            prevScreenNode.gameObject.SetActive(false);
            prevScreenNode = null;
        }

        currentScreenNode = screenNode;
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

        if(currentScreenNode != null){
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

    public void FocusOnWorldTarget(Transform worldTarget, bool autoRestore){
        if(worldTarget == null) return;
        if(enterRoutine != null) StopCoroutine(enterRoutine);
        
        cam.Follow = worldTarget;
        cam.LookAt = worldTarget;

        IsTransitioning = true;
        enterRoutine = StartCoroutine(EndFocusAfter(worldTarget, autoRestore));
    }

    IEnumerator EndFocusAfter(Transform worldTarget, bool autoRestore){
        yield return new WaitForSeconds(enterTransitionTime);
        IsTransitioning = false;
        enterRoutine = null;
    }
}