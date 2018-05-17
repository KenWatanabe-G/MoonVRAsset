using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ThrowSample : MonoBehaviour
{

    public Transform ballSpawnPosition;
    public GameObject ballPrefab;

    private GameObject ball;
    private Vector3 ballV;
    private Vector3 ballLastPosition;

    void Start()
    {
        ballV = Vector3.zero;
    }

    void Update()
    {
        if (GvrControllerInput.ClickButtonDown)
        {
            Vector2 touchPos = GvrControllerInput.TouchPos;
            if (0.25f < touchPos.x && touchPos.x < 0.75f
                && 0.25f < touchPos.y && touchPos.y < 0.75f)
            {
                spawnBall();
            }
        }
        if (GvrControllerInput.ClickButtonUp && ball != null)
        {
            throwBall();
        }

        if (ball != null)
        {
            ballV = (ball.transform.position - ballLastPosition) / Time.deltaTime;
            ballLastPosition = ball.transform.position;
        }
    }

    private void spawnBall()
    {
        ball = Instantiate(ballPrefab, ballSpawnPosition);
    }

    private void throwBall()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(ballV.x * 2.0f, ballV.y * 2.0f, ballV.z * 2.0f, ForceMode.Impulse);
        Destroy(ball, 60);
        ball.transform.parent = null;
        ball = null;
    }
}