using UnityEngine;
using Nova;
using NaughtyAttributes;
using System.Collections.Generic;

public class TelephonePanel : MonoBehaviour
{
    [Header("NUMBER DIAL")]
    [SerializeField] private float radius = 3f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private UIBlock2D[] numberCircle;

    [Header("NUMBER BLOCK")]
    [SerializeField] private TextBlock[] numberBlock;

    [Header("POINTER")]
    [SerializeField] private UIBlock2D pointer;
    [SerializeField] private float pointerRadius = 3f;

    private UIBlock2D[] _children;
    private float _rotationAngle;
    private bool _initialized;
    private bool _hasPointer;
    private float _pointerScreenAngle;

    void Start(){
        Initialize();
        RandomizePointer();
    }

    void Update(){
        _rotationAngle += rotationSpeed * Time.deltaTime;
        SetChildTransform();
    }

    [Button("SetChildTransform", EButtonEnableMode.Editor)]
    void SetChildTransform(){
        if(!_initialized) Initialize();
        if(_children == null || _children.Length == 0) return;

        int count = _children.Length;
        float step = 360f / count;

        for(int i = 0; i < count; i++){
            var child = _children[i];
            if(child == null) continue;
            float radians = (step * i + _rotationAngle) * Mathf.Deg2Rad;
            child.Position.X.Value = Mathf.Cos(radians) * radius;
            child.Position.Y.Value = Mathf.Sin(radians) * radius;
        }

        if(pointer != null && _hasPointer){
            float radians = _pointerScreenAngle * Mathf.Deg2Rad;
            pointer.Position.X.Value = Mathf.Cos(radians) * pointerRadius;
            pointer.Position.Y.Value = Mathf.Sin(radians) * pointerRadius;
        }
    }

    void Initialize(){
        int count = transform.childCount;
        int numberCount = numberCircle.Length;

        _children = new UIBlock2D[numberCount];
        for(int i = 0; i < numberCount; i++){
            var child = transform.GetChild(i);
            _children[i] = child.GetComponent<UIBlock2D>();
        }

        _initialized = true;
    }

    public void RandomizePointer(){
        if(_children == null || _children.Length == 0) return;
        if(pointer == null) return;

        int count = _children.Length;
        float step = 360f / count;
        int randomIndex = Random.Range(0, count);

        _pointerScreenAngle = Mathf.Repeat(step * randomIndex + _rotationAngle, 360f);
        _hasPointer = true;

        float radians = _pointerScreenAngle * Mathf.Deg2Rad;
        pointer.Position.X.Value = Mathf.Cos(radians) * pointerRadius;
        pointer.Position.Y.Value = Mathf.Sin(radians) * pointerRadius;
    }

    public string GetClosestChildText(){
        if(_children == null || _children.Length == 0) return null;
        if(pointer == null) return null;

        Vector3 pointerPos = pointer.transform.position;
        int closestIdx = -1;
        float closestDistSq = float.MaxValue;

        for(int i = 0; i < _children.Length; i++){
            var child = _children[i];
            if(child == null) continue;

            Vector3 p = child.transform.position;
            float dx = p.x - pointerPos.x;
            float dy = p.y - pointerPos.y;
            float distSq = dx * dx + dy * dy;

            if(distSq < closestDistSq){
                closestDistSq = distSq;
                closestIdx = i;
            }
        }

        if(closestIdx < 0) return null;

        var text = _children[closestIdx].GetComponentInChildren<TextBlock>();
        return text != null ? text.Text : null;
    }

    public void SetNumberBlock(int index, string value) => numberBlock[index].Text = value;
    public void ResetNumberBlock(){
        for(int i = 0; i < GetNumberBlockChildCount(); i++) numberBlock[i].Text = string.Empty;
    }
    public int GetNumberBlockChildCount() => numberBlock.Length;
}