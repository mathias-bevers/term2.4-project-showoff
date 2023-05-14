using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelPoint : MonoBehaviour
{
    [SerializeField] float _forwardStrenth = 0.25f;
    [SerializeField] bool _smooth = false;
    [SerializeField] bool _isEnd = false;
    [SerializeField] LevelPoint[] _connectionPoints;

    public float forwardStrength => _forwardStrenth;
    public LevelPoint[] connectionPoints => _connectionPoints;
    public bool isSmooth => _smooth;
    public bool isEnd => _isEnd;

    public bool isChoiceNode => connectionPoints.Length > 1;

    Transform _transform;
    public new Transform transform => _transform;
    public Vector3 position => _transform.position;

    [InfoBox("Only applicable if there are multiple endpoints")]
    [SerializeField] MapSides _winningSides;
    public MapSides winningSides => _winningSides;

    

    private void Awake()
    {
        _transform = base.transform;
    }
}
