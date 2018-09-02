using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour {

    [SerializeField]
    GameObject bezPoint1;

    [SerializeField]
    GameObject bezPoint2;

    [SerializeField]
    GameObject bezPoint3;

    [SerializeField]
    GameObject bezPoint4;

    [SerializeField]
    GameObject bezObjectMove;

    [SerializeField]
    LineRenderer lineRenderer;


    List<Vector3> pointTransforms;
    List<Vector3> curve;
    public float curveLength;
    public List<float> curveSegments;

    private float startTime;
    private float timer;
    private bool coroutineStart;

    public float speed =20f;
    public float currentTravelDistance = 0f;

    // Update is called once per frame
    void Start() {
        startTime = Time.time;
        timer = Time.time;
        coroutineStart = false;

    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer - startTime > 5)
        {
            pointTransforms = new List<Vector3>();
            curve = new List<Vector3>();
            pointTransforms.Add(bezPoint1.transform.position);
            pointTransforms.Add(bezPoint2.transform.position);
            pointTransforms.Add(bezPoint3.transform.position);
            pointTransforms.Add(bezPoint4.transform.position);
            curve = Bezier.GenerateBezier(pointTransforms, 10);

            if (curve.Count > 0)
            {
                lineRenderer.positionCount = curve.Count;
                for (int i = 0; i < curve.Count; i++)
                {
                    lineRenderer.SetPosition(i, curve[i]);
                }
            }
        }

        if (bezObjectMove != null && timer - startTime > 6 && coroutineStart == false)
        {
            curveLength = Bezier.CalculateCurveLength(curve);
            print(curveLength);
            curveSegments = Bezier.CalculateCurveSegments(curve);

            coroutineStart = true;

        }

        if (bezObjectMove != null && coroutineStart && timer - startTime > 7)
        { 
            bezTravel();
            
        }  
        }

    public void bezTravel()
    {
        while (currentTravelDistance <= curveLength)
        {
            float curveStep = Mathf.Min(Time.deltaTime * speed, curveLength - currentTravelDistance);
            float nextTravelDistance = currentTravelDistance + curveStep;

            float segmentLengthCounter = 0f;
            for (int i = 0; i < curveSegments.Count; i++)
            {
                segmentLengthCounter += curveSegments[i];
                if (segmentLengthCounter >= nextTravelDistance)
                {
                    float amount = Mathf.InverseLerp(0, curveSegments[i], nextTravelDistance - (segmentLengthCounter - curveSegments[i]));
                    print("amount: " + amount);
                    bezObjectMove.transform.position = Vector3.Lerp(curve[i], curve[Mathf.Min(curve.Count, i+1)], amount);

                    currentTravelDistance += curveStep;
                    print("currTra: " + currentTravelDistance);
                    return;
                }
            }
        }
    }
}

    //while (currentTravelDistance<curveLength)
    //    {
    //        float curveStep = Time.deltaTime * speed;
    //float nextTravelDistance = currentTravelDistance + curveStep;

    //float segmentLengthCounter = 0f;
    //        for (int i = 0; i<curveSegments.Count; i++)
    //        {
    //            segmentLengthCounter += curveSegments[i];
    //            if (segmentLengthCounter >= nextTravelDistance)
    //            {
    //                float amount = Mathf.InverseLerp(curveSegments[i], curveSegments[(i + 1) % curveSegments.Count], nextTravelDistance - segmentLengthCounter);
    //currentTravelDistance += nextTravelDistance - segmentLengthCounter;
    //                obj.transform.position = Vector3.Lerp(curve[i], curve[(i + 1) % curve.Count], amount);
    //                yield return null;
    //            }
    //        }



        //else if (bezPoint1 != null && bezPoint2 != null && bezPoint3 != null && bezPoint4 != null)
        //{
        //    print("CUB");
        //    pointTransforms = new List<Vector3>();
        //    pointTransforms.Add(bezPoint1.transform.position);
        //    pointTransforms.Add(bezPoint2.transform.position);
        //    pointTransforms.Add(bezPoint3.transform.position);
        //    pointTransforms.Add(bezPoint4.transform.position);
        //    curve = Bezier.GenerateBezier(pointTransforms, 10);
        //}
    

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    print(curve.Count);

