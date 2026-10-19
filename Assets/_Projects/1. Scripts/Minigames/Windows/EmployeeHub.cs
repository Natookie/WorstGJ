using UnityEngine;
using Nova;
using System.Collections.Generic;

public class EmployeeHub : MonoBehaviour
{
    [Header("PAGES")]
    [SerializeField] private UIBlock2D loginPage;
    [SerializeField] private UIBlock2D todoListPage;

    [Header("LOGIN")]
    [SerializeField] private TextBlock invalidRequirementText;

    private bool isLoggedIn;
    private string invalidText;

    void Start(){
        isLoggedIn = false;
        ShowLoginPage();
    }

    public bool IsLoggedIn => isLoggedIn;

    public void ClearLoginFields(){
        TextFieldManager.ClearAll("loginemployeehub");
        invalidRequirementText.Text = string.Empty;
        invalidRequirementText.Visible = false;
    }

    public void SubmitLogin(){
        if(isLoggedIn) return;

        if(TextFieldManager.TrySubmitGroup("loginemployeehub", out var failures)){
            isLoggedIn = true;
            invalidRequirementText.Text = string.Empty;
            invalidRequirementText.Visible = false;
            ShowTodoListPage();
            return;
        }

        invalidRequirementText.Text = FormatFailures(failures);
        invalidRequirementText.Visible = true;
    }

    void ShowLoginPage(){
        if(loginPage != null) loginPage.gameObject.SetActive(true);
        if(todoListPage != null) todoListPage.gameObject.SetActive(false);
    }

    void ShowTodoListPage(){
        if(loginPage != null) loginPage.gameObject.SetActive(false);
        if(todoListPage != null) todoListPage.gameObject.SetActive(true);
    }

    string FormatFailures(List<TextFieldManager.FieldFailure> failures){
        var sb = new System.Text.StringBuilder();

        foreach(var fail in failures){
            if(fail.FailedRules == null || fail.FailedRules.Length == 0) continue;
            if(sb.Length > 0) sb.Append('\n');

            sb.Append(fail.Field.DisplayName).Append(':');
            foreach(var rule in fail.FailedRules)
                sb.Append('\n').Append("• ").Append(rule);
        }

        return sb.ToString();
    }
}