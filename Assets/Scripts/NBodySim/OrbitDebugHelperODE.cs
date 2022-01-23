using Microsoft.Research.Oslo;
using UnityEngine;
//sebastian lagues code modificed so i can run it during play
[ExecuteInEditMode]
public class OrbitDebugHelperODE : MonoBehaviour
{
    public int numSteps = 1000;
    public float timeStep = 0.1f;
    public bool usePhysicsTimeStep;
    public bool runDuringPlay;

    public bool relativeToBody;
    public StellarBody centralBody;
    public float width = 100;
    public bool useThickLines;

    private static  StellarBody[] bodies;

    private static  VirtualBody[] virtualBodies;
    //var virtualBodyVelocities_ODE = NBodySimulation.Instance.TwoBody_solve_ODE(bodies, timeStep * numSteps, timeStep * numSteps);
    private static Vector3[][] drawVeclocities;
    private static  Vector3[][] drawPoints;
    private  static Vector3[][] lastDrawPoints;
    private  static int referenceFrameIndex;
    private static  Vector3 referenceBodyInitialPosition;

    void Start()
    {
        // if (Application.isPlaying)
        // {
        //     for (int i = 0; i < virtualBodies.Length; i++)
        //     {
        //         var lineRenderer = bodies[i].gameObject.GetComponentInChildren<LineRenderer>();
        //         if (lineRenderer)
        //         {
        //             lineRenderer.enabled = false;
        //         }
        //     }
        // }
    }

    void Update()
    {
        if (!Application.isPlaying || runDuringPlay)
        {
            DrawOrbits();
        }
    }

    void SimulateOrbits()
    {
        lastDrawPoints = drawPoints;
        bodies = FindObjectsOfType<StellarBody>();
        virtualBodies = new VirtualBody[bodies.Length];
        //var virtualBodyVelocities_ODE = NBodySimulation.Instance.TwoBody_solve_ODE(bodies, timeStep * numSteps, timeStep * numSteps);
        drawVeclocities = new Vector3[bodies.Length][];
        drawPoints = new Vector3[bodies.Length][];
        referenceFrameIndex = 0;
        referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies (don't want to move the actual bodies)
        for (int i = 0; i < virtualBodies.Length; i++)
        {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[numSteps];


            drawVeclocities[i] = new Vector3[numSteps];

            if (bodies[i] == centralBody && relativeToBody)
            {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].position;
            }
        }

        // Simulate
        for (int step = 0; step < numSteps; step++)
        {
            Vector3 referenceBodyPosition =
                (relativeToBody) ? virtualBodies[referenceFrameIndex].position : Vector3.zero;
            // Update velocities
            for (int i = 0; i < virtualBodies.Length; i++)
            {
                virtualBodies[i].velocity += CalculateAcceleration(i, virtualBodies) * timeStep;
                drawVeclocities[i][step] = virtualBodies[i].velocity;
            }

            // Update positions
            for (int i = 0; i < virtualBodies.Length; i++)
            {
                Vector3 newPos = virtualBodies[i].position + virtualBodies[i].velocity * timeStep;
                virtualBodies[i].position = newPos;
                if (relativeToBody)
                {
                    var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }

                if (relativeToBody && i == referenceFrameIndex)
                {
                    newPos = referenceBodyInitialPosition;
                }

                drawPoints[i][step] = newPos;
            }
        }
    }

    void DrawOrbits()
    {
        //if (!Application.isPlaying)
            SimulateOrbits();

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++)
        {
            var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial
                .color; //
            var velColour = new Color(255, 255, 0);
            var velODEColour = new Color(115, 48, 216);

            if (useThickLines)
            {
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                lineRenderer.enabled = true;
                lineRenderer.positionCount = drawPoints[bodyIndex].Length;
                lineRenderer.SetPositions(drawPoints[bodyIndex]);
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.widthMultiplier = width;
            }
            else
            {
                for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++)
                {
                    Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], velColour);
                    //Debug.DrawLine (drawVeclocities[bodyIndex][i], drawVeclocities[bodyIndex][i + 1], velColour);
                    //Debug.DrawLine (vectorToVector3(virtualBodyVelocities_ODE[bodyIndex][i].X), vectorToVector3(virtualBodyVelocities_ODE[bodyIndex][i+1].X), velODEColour);
                }

                // Hide renderer
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                if (lineRenderer)
                {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    Vector3 CalculateAcceleration(int i, VirtualBody[] virtualBodies)
    {
        Vector3 acceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Length; j++)
        {
            if (i == j)
            {
                continue;
            }

            Vector3 forceDir = (virtualBodies[j].position - virtualBodies[i].position).normalized;
            float sqrDst = (virtualBodies[j].position - virtualBodies[i].position).sqrMagnitude;
            acceleration += forceDir * NBodySimulation.gravitationalConstant * virtualBodies[j].mass / sqrDst;
        }

        return acceleration;
    }

    void HideOrbits()
    {
        StellarBody[] bodies = FindObjectsOfType<StellarBody>();

        // Draw paths
        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
        {
            var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }

    void OnValidate()
    {
        if (usePhysicsTimeStep)
        {
            timeStep = NBodySimulation.timeStep;
        }
    }

    class VirtualBody
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;

        public VirtualBody(StellarBody body)
        {
            position = body.transform.position;
            if (!Application.isPlaying)
                velocity = body.initialVelocity;
            else
                velocity = body.velocity;
            mass = body.mass;
        }
    }
}