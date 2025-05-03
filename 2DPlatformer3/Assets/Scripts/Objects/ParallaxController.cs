using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public float parallaxSpeed = 0.5f;
    public float viewZone = 10f; // 카메라와 얼마나 가까워지면 스와핑할지

    private Transform cam;
    private float backgroundSize;

    private Transform[] layers;
    private int leftIndex;
    private int rightIndex;

    void Start()
    {
        cam = Camera.main.transform;
        backgroundSize = GetComponentInChildren<SpriteRenderer>().bounds.size.x;

        // 하위 오브젝트 기준으로 레이어 배열 만들기
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            layers[i] = transform.GetChild(i);

        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }

    void Update()
    {
        float deltaX = cam.position.x * parallaxSpeed;
        transform.position = new Vector3(deltaX, transform.position.y, transform.position.z);

        // 오른쪽으로 이동 중
        if (cam.position.x > (layers[rightIndex].position.x - viewZone))
            ScrollRight();

        // 왼쪽으로 이동 중
        if (cam.position.x < (layers[leftIndex].position.x + viewZone))
            ScrollLeft();
    }

    void ScrollLeft()
    {
        Transform rightmost = layers[rightIndex];
        layers[rightIndex].position = new Vector3(
            layers[leftIndex].position.x - backgroundSize,
            layers[rightIndex].position.y,
            layers[rightIndex].position.z
        );

        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0) rightIndex = layers.Length - 1;
    }

    void ScrollRight()
    {
        Transform leftmost = layers[leftIndex];
        layers[leftIndex].position = new Vector3(
            layers[rightIndex].position.x + backgroundSize,
            layers[leftIndex].position.y,
            layers[leftIndex].position.z
        );

        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex >= layers.Length) leftIndex = 0;
    }
}