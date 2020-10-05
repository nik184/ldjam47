using UnityEngine;

public class RopeController : MonoBehaviourWrapper
{
    // Start is called before the first frame update
    private LineRenderer _lineRenderer;
    private BalloonController _balloon;
    private AnchorController _anchor;
    

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _balloon = FindObjectOfType<BalloonController>();
        _anchor = FindObjectOfType<AnchorController>();
    }

    private void Update()
    {
        if(!_balloon.IsAlive) return;
        
        Vector3[] points = new Vector3[101];
        _lineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = _anchor.transform.position * (101 - i) / 101 + _balloon.transform.position * i / 101;
            points[i].y += (((float)i-50)*((float)i-50) - 2500) / 1000;
        }

        _lineRenderer.SetPositions(points);
    }
}
