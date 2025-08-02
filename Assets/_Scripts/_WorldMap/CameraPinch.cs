using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPinch : MonoBehaviour
{
    public static CameraPinch Instance {get; private set;}
    public Camera cam;
    public float zoomSpeed = 0.01f;
    public float panSpeed = 0.01f;
    public float maxSize = 50;
    public float minSize = 0;
    public Vector3 islandPlace;
    public float cooldown = 3f;
    public float distanceTillReturn = 5f;
    public float cameraSpeed = 10f;

    [Header("Desktop specific")]
    public float scrollSpeed = 40f;

    private Vector3 lastPanPosition;
    private bool isPanning = false;

    private Coroutine returnCoroutine;

    [Header("Requirement to show Dots")]
    public float zoomRequired;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        bool interacted = false;

        if(InteractionSystem.Instance.isOpen)
        {
            interacted = false;
            isPanning = false;
            return;
        }

        //Zoom
        if (Input.touchCount == 2)
        {
            isPanning = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMag = (prevTouch0 - prevTouch1).magnitude;
            float currMag = (touch0.position - touch1.position).magnitude;

            float delta = currMag - prevMag;
            HandleZoom(delta);
            interacted = true;
        }

        //Panning
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = cam.ScreenToWorldPoint(touch.position);
                isPanning = true;
                interacted = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
                Vector3 diff = lastPanPosition - currentPos;
                cam.transform.position += diff;
                interacted = true;
            }
        }
        else if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastPanPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                isPanning = true;
                interacted = true;
            }
            else if (Input.GetMouseButton(0) && isPanning)
            {
                Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 diff = lastPanPosition - currentPos;
                cam.transform.position += diff;
                interacted = true;
            }

            float scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > 0f)
            {
                HandleZoom(scrollDelta * scrollSpeed);
                interacted = true;
            }
        }

        if(interacted) //Simply restarts the coroutine
        {
            if(returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }

            if(Vector3.Distance(transform.position, islandPlace) >= distanceTillReturn)
            {
                returnCoroutine = StartCoroutine(ReturnToIslandAfterDelay(cooldown));
            }
        }
    }

    void HandleZoom(float delta)
    {
        if (Mathf.Abs(delta) < 0.01f)
            return;

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta * zoomSpeed, minSize, maxSize);
            if(cam.orthographicSize <= zoomRequired)
            {
                InteractionSystem.Instance.ShowDots();
            }
            else
            {
                InteractionSystem.Instance.HideDots();   
            }
        }
        else
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - delta * zoomSpeed, 15f, 100f);
        }
    }

    IEnumerator ReturnToIslandAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (Vector3.Distance(cam.transform.position, islandPlace) > 0.01f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, islandPlace, Time.deltaTime * cameraSpeed);
            yield return null;
        }

        cam.transform.position = islandPlace;
        returnCoroutine = null;
    }
}
