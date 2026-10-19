using System;
using System.Collections.Generic;
using UnityEngine;
using Nova;

[Serializable]
public class PrintableFile
{
    public string displayName;
    public CoreButton button;
    public UIBlock2D paperPreview;
}

public class PrinterPanel : MonoBehaviour
{
    [Header("FILES")]
    [SerializeField] private List<PrintableFile> files = new();

    [Header("STATE")]
    [SerializeField] private UIBlock2D paperPreviewRoot;

    private PrintableFile activeFile;

    void Awake(){
        foreach(var file in files){
            if(file == null || file.paperPreview == null) continue;
            file.paperPreview.gameObject.SetActive(false);
        }

        if(paperPreviewRoot != null) paperPreviewRoot.gameObject.SetActive(true);
    }

    void OnEnable(){
        foreach(var file in files){
            if(file == null || file.button == null) continue;
            var captured = file;
            file.button.OnClick.AddListener(() => OnFileClicked(captured));
        }
    }

    void OnDisable(){
        foreach(var file in files){
            if(file == null || file.button == null) continue;
            var captured = file;
            file.button.OnClick.RemoveListener(() => OnFileClicked(captured));
        }
    }

    void OnFileClicked(PrintableFile file) => ShowPaper(file);
    void ShowPaper(PrintableFile file){
        if(file == null) return;

        foreach(var f in files){
            if(f == null || f.paperPreview == null) continue;
            bool active = f == file;
            f.paperPreview.gameObject.SetActive(active);
            f.button?.SetSelected(active);
        }

        activeFile = file;
        if(paperPreviewRoot != null) paperPreviewRoot.gameObject.SetActive(true);
    }
    public void ClosePaper(){
        foreach(var f in files){
            if(f == null || f.paperPreview == null) continue;
            f.paperPreview.gameObject.SetActive(false);
            f.button?.SetSelected(false);
        }

        activeFile = null;
        if(paperPreviewRoot != null) paperPreviewRoot.gameObject.SetActive(false);
    }

    public PrintableFile ActiveFile => activeFile;
}