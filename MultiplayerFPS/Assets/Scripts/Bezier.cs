using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier {

	

    public static List<Vector3> GenerateBezier(List<Vector3> points, int numSteps)
    {
        float timeStep = 1f / numSteps;

        if(points.Count == 3)
        {
            return GenerateBezierQuadratic(points, timeStep);
        }
        else if (points.Count == 4)
        {
            return GenerateBezierCubic(points, timeStep);
        }
        else
        {
            return GenerateBezierPolynomial(points, timeStep);
        }
    }

    private static List<Vector3> GenerateBezierQuadratic(List<Vector3> points, float timeStep)
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        float t = 0f;

        while (t < 1f)
        {
            float x = (1 - t) * (1 - t) * points[0].x + 2 * (1 - t) * t * points[1].x + t * t * points[2].x;
            float y = (1 - t) * (1 - t) * points[0].y + 2 * (1 - t) * t * points[1].y + t * t * points[2].y;
            float z = (1 - t) * (1 - t) * points[0].z + 2 * (1 - t) * t * points[1].z + t * t * points[2].z;

            bezierPoints.Add(new Vector3(x, y, z));
            t += timeStep;
        }
        return bezierPoints;
    }
    
    private static List<Vector3> GenerateBezierCubic(List<Vector3> points, float timeStep)
    {
        List<Vector3> bezierPoints = new List<Vector3>();
        float t = 0f;

        while (t < 1f)
        {
            float x = (1 - t) * (1 - t) * (1 - t) * points[0].x + 3 * (1 - t) * (1 - t) * t * points[1].x + (1 - t) * t * t * points[2].x + t * t * t * points[3].x;
            float y = (1 - t) * (1 - t) * (1 - t) * points[0].y + 3 * (1 - t) * (1 - t) * t * points[1].y + (1 - t) * t * t * points[2].y + t * t * t * points[3].y;
            float z = (1 - t) * (1 - t) * (1 - t) * points[0].z + 3 * (1 - t) * (1 - t) * t * points[1].z + (1 - t) * t * t * points[2].z + t * t * t * points[3].z;

            bezierPoints.Add(new Vector3(x, y, z));
            t += timeStep;
        }
        return bezierPoints;
    }

    private static List<Vector3> GenerateBezierPolynomial(List<Vector3> points, float timeStep)
    {
        return new List<Vector3>();
    }

    private List<float> GenerateBezierBionomialCoefficients(int numPoints)
    {
        List<float> binomCoeff = new List<float>();
        for(int i = 0; i <= numPoints; i++)
        {
            float binom = Factorial(numPoints) / (Factorial(i) * Factorial(numPoints - i));
            binomCoeff.Add(binom);
        }
        return binomCoeff;
    }

    private float Factorial(int num)
    {
        float result = 1;
        while(num != 1)
        {
            result *= num;
            num--;
        }
        return result;
    }
}
