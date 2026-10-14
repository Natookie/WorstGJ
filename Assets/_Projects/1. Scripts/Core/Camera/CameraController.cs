using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform computerTarget;
    [SerializeField] private float enterTransitionTime = 1f;

    private Coroutine enterRoutine;

    public bool IsTransitioning { get; private set; }

    public void EnterComputerMode(){
        if(enterRoutine != null) StopCoroutine(enterRoutine);

        confiner.enabled = false;
        cam.Follow = computerTarget;
        cam.LookAt = computerTarget;

        IsTransitioning = true;
        enterRoutine = StartCoroutine(EndEnterAfter(enterTransitionTime));
    }

    public void ExitComputerMode(){
        if(enterRoutine != null){
            StopCoroutine(enterRoutine);
            enterRoutine = null;
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