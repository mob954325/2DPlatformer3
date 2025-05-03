using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public float parallaxSpeed = 0.5f;
    public float viewZone = 10f; // ī�޶�� �󸶳� ��������� ����������

    private Transform cam;
    private float backgroundSize;

    private Transform[] layers;
    private int leftIndex;
    private int rightIndex;

    void Start()
    {
        cam = Camera.main.transform;
        backgroundSize = GetComponentInChildren<SpriteRenderer>().bounds.size.x;

        // ���� ������Ʈ �������� ���̾� �迭 �����
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

        // ���������� �̵� ��
        if (cam.position.x > (layers[rightIndex].position.x - viewZone))
            ScrollRight();

        // �������� �̵� ��
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