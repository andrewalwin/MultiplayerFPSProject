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
    LineRenderer lineRenderer;


    List<Vector3> pointTransforms;
    List<Vector3> curve;

    private float startTime;
    private float timer;


    // Update is called once per frame
    void Start() {
        startTime = Time.time;
        timer = Time.time;

    }

    private void Update()
    {
        timer += Time.deltaTime;
        print(timer - startTime);
        if (timer - startTime > 5)
        {
            print("QUAD");
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
    }



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

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    print(curve.Count);

