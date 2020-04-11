using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauchTragectory : MonoBehaviour
{
    LineRenderer lr;

    public float velocity;
    public float angle;
    public int reso = 10;
    public Transform playerT;

    float grav;
    float radianAngle;

    [HideInInspector] Vector3 mousePos = Vector3.zero;
    public Vector3 MousePos { get { return mousePos; } }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        grav = Mathf.Abs(Physics.gravity.y);
    }


    void Start()
    {
        RenderArc();
    }

    private void Update()
    {
        RenderArc();
    }

    void RenderArc()
    {        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, 1 << 0))
        {
            if ((hit.point - playerT.position).sqrMagnitude > 16) mousePos = hit.point;
            else mousePos = (hit.point - playerT.position).normalized * 4 + playerT.position;
        }

        lr.positionCount = reso + 1;
        lr.SetPositions(CalculateArcArray(mousePos));
    }

    Vector3[] CalculateArcArray(Vector3 mousePos)
    {
        Vector3[] arcArray = new Vector3[reso + 1];

        //radianAngle = Mathf.Deg2Rad * angle;
        //float distance = (mousePos - transform.position).magnitude;
        //velocity = (3 + distance * 50) / 55;
        //float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / grav;               
        //Debug.Log(distance);

        for (int i = 0; i <= reso; i++)
        {
            float t = (float)i / reso;
            // arcArray[i] = CalculateArcPoint(t, maxDistance);

            Vector3 hermite = Hermite(transform, mousePos, t);

            arcArray[i] = hermite;
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((grav * x * x) / (2 * Mathf.Pow(velocity * Mathf.Cos(radianAngle), 2)));

        return transform.localToWorldMatrix * new Vector4(x, y, 0, 1);
    }

    Vector3 Hermite(Transform ori, Vector3 dest, float time)
    {
        return (2.0f * Mathf.Pow(time, 3) - 3.0f * Mathf.Pow(time, 2) + 1.0f) * ori.position + (-2.0f * Mathf.Pow(time, 3) + 3.0f * Mathf.Pow(time, 2)) * dest +
                (Mathf.Pow(time, 3) - 2.0f * Mathf.Pow(time, 2) + time) * (ori.up * 4 + ori.right) * 4 + (Mathf.Pow(time, 3) - Mathf.Pow(time, 2)) * (-ori.up * 2 + ori.right) * 5;
    }
}
