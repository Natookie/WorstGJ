using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("REFERENCES")]
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("TIMING")]
    [SerializeField] private float enterTransitionTime = 1f;

    private Transform currentScreenNode;
    private Coroutine enterRoutine;
    private bool _playerWasEnabled = true;

    public bool IsTransitioning { get; private set; }
    public bool IsInScreenMode => currentScreenNode != null;

    public event System.Action OnScreenModeEnter;
    public event System.Action OnScreenModeExit;

    void Awake(){
        if(Instance != null && Instance != this){ Destroy(gameObject); return; }
        Instance = this;
    }

    public void EnterScreenMode(Transform screenNode){
        if(screenNode == null) return;
        if(enterRoutine != null) StopCoroutine(enterRoutine);

        if(currentScreenNode != null && currentScreenNode != screenNode)
            currentScreenNode.gameObject.SetActive(false);

        currentScreenNode = screenNode;
        currentScreenNode.gameObject.SetActive(true);

        if(playerMovement != null){
            _playerWasEnabled = playerMovement.IsEnabled;
            playerMovement.SetEnabled(false);
        }

        confiner.enabled = false;
        cam.Follow = currentScreenNode;
        cam.LookAt = currentScreenNode;

        IsTransitioning = true;
        enterRoutine = StartCoroutine(EndEnterAfter(enterTransitionTime));

        OnScreenModeEnter?.Invoke();
    }

    public void ExitScreenMode(){
        if(enterRoutine != null){ StopCoroutine(enterRoutine); enterRoutine = null; }

        if(currentScreenNode != null){
            currentScreenNode.gameObject.SetActive(false);
            currentScreenNode = null;
        }

        if(playerMovement != null && _playerWasEnabled)
            playerMovement.SetEnabled(true);

        cam.Follow = playerTarget;
        cam.LookAt = playerTarget;
        confiner.enabled = true;

        var brain = FindFirstObjectByType<CinemachineBrain>();
        if(brain != null) brain.ResetState();

        IsTransitioning = false;

        OnScreenModeExit?.Invoke();
    }

    public void FocusOnWorldTarget(Transform worldTarget){
        if(worldTarget == null) return;
        if(enterRoutine != null) StopCoroutine(enterRoutine);

        cam.Follow = worldTarget;
        cam.LookAt = worldTarget;

        IsTransitioning = true;
        enterRoutine = StartCoroutine(EndEnterAfter(enterTransitionTime));
    }
    IEnumerator EndEnterAfter(float seconds){
        yield return new WaitForSeconds(seconds);
        IsTransitioning = false;
        enterRoutine = null;
    }

}