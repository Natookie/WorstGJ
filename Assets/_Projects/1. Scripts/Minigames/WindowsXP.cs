using UnityEngine;
using Nova;

public class WindowsXP : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D root;
    [SerializeField] private UIBlock2D fileExplorerTab;
    [SerializeField] private UIBlock2D employeeHubTab;
    [SerializeField] private UIBlock2D windowsStartTab;

    [Header("ERROR WINDOW")]
    [SerializeField] private UIBlock2D errorWindow;
    [SerializeField] private int errorWindowCount = 15;
    [SerializeField] private Vector2 scatterRadius = new Vector2(500f, 200f);
    [Space(5)]
    [SerializeField] private bool startWithError;

    void Start(){
        if(startWithError) SpamError(errorWindowCount);
    }

    public void SpamError(int n){
        if(errorWindow == null) return;
        
        int trollCount = 3;
        for(int i = 0; i < n; i++){
            var clone = Instantiate(errorWindow, root.transform);

            clone.Position.X = Random.Range(-scatterRadius.x, scatterRadius.x);
            clone.Position.Y = Random.Range(-scatterRadius.y, scatterRadius.y);

            WindowsErrorPanel error = clone.GetComponent<WindowsErrorPanel>();
            if(i % 3 == 0 && trollCount > 0){
                trollCount--;
                error.Init(WindowsErrorPanel.ErrorType.Troll, this);
            }else error.Init(WindowsErrorPanel.ErrorType.Normal, this);
        }
    }

    public void ToggleFileExplorerTab() => fileExplorerTab.gameObject.SetActive(!fileExplorerTab.gameObject.activeSelf);
    public void ToggleEmployeeHubTab() => employeeHubTab.gameObject.SetActive(!employeeHubTab.gameObject.activeSelf);
    public void ToggleWindowsStartTab() => windowsStartTab.gameObject.SetActive(!windowsStartTab.gameObject.activeSelf);

    public void SubmitLoginEmployeeHub() => TextFieldManager.TrySubmitGroup("loginemployeehub");
    public void ClearLoginEmployeeHub() => TextFieldManager.ClearAll("loginemployeehub");
}