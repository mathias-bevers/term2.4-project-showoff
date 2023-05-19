using UnityEngine;

[ExecuteInEditMode]
public class LevelPoint : MonoBehaviour
{
    [SerializeField] float _forwardStrenth = 0.25f;
    [SerializeField] bool _smooth = false;
    [SerializeField] bool _isEnd = false;
    [SerializeField] ConnectionPoint[] _connectionPoints;

    public float forwardStrength => _forwardStrenth;
    public ConnectionPoint[] connectionPoints => _connectionPoints;
    public bool isSmooth => _smooth;
    public bool isEnd => _isEnd;

    //I could use an automated system like before, but it's quite frankly just sooo much easier to make this a boolean.
    public bool isChoiceNode;

    Transform _transform;
    public new Transform transform => _transform;
    public Vector3 position => _transform.position;

    private void Awake()
    {
        _transform = base.transform;
    }
}

[System.Serializable]
public struct ConnectionPoint
{
    [SerializeField]
    public LevelPoint nextPoint;
    [SerializeField]
    public MapSides requiredSides;
}
