using UnityEngine;

public class RopeController : MonoBehaviourWrapper
{
    // Start is called before the first frame update
    private LineRenderer _lineRendererComponent;
    private BalloonController _balloon;
    private AnchorController _anchor;
    

    private void Start()
    {
        _lineRendererComponent = GetComponent<LineRenderer>();
        _balloon = FindObjectOfType<BalloonController>();
        _anchor = FindObjectOfType<AnchorController>();
    }

    private void Update()
    {
        Vector3[] points = new Vector3[100];
        _lineRendererComponent.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = _anchor.transform.position * (100 - i) / 100 + _balloon.transform.position * i / 100;
            _lineRendererComponent.positionCount = i+1;
        }

        _lineRendererComponent.SetPositions(points);
    }
}
