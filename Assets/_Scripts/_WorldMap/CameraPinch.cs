using UnityEngine;

public class CameraPinch : MonoBehaviour
{
    public Camera cam;
    public float zoomSpeed = 0.01f;
    public float panSpeed = 0.01f;

    public float maxSize = 50;
    public float minSize = 0;

    public Vector3 islandPlace;
    public float cooldown;
    private float o_cooldown;
    public float cameraSpeed = 10f;
    private bool returnToIsland;

    [Header("Desktop specific")]
    public float scrollSpeed = 40f;

    private Vector3 lastPanPosition;
    private bool isPanning;

    void Start()
    {
        returnToIsland = false;
        o_cooldown = cooldown;
    }

    void Update()
    {
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
        }
        else if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastPanPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                isPanning = true;
            }
            else if (Input.GetMouseButton(0) && isPanning)
            {
                Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 diff = lastPanPosition - currentPos;
                cam.transform.position += diff;
                returnToIsland = false;
            }

            float scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > 0f)
            {
                HandleZoom(scrollDelta * scrollSpeed);
            }
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = cam.ScreenToWorldPoint(touch.position);
                isPanning = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
                Vector3 diff = lastPanPosition - currentPos;
                cam.transform.position += diff;
                returnToIsland = false;
            }
        }

        if(cooldown > 0 && !returnToIsland)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            cooldown = o_cooldown;
            returnToIsland = true;
        }
    }

    void FixedUpdate()
    {
        if(!returnToIsland)
        {
            return;
        }

        float distance = Vector3.Distance(cam.transform.position, islandPlace);
        if(distance < 0.1)
        {
            cam.transform.position = Vector3.MoveTowards(GetComponent<Camera>().transform.position, islandPlace, Time.deltaTime * cameraSpeed);
            if(distance == 0)
            {
                returnToIsland = false;
            }
        }
        else
        {
            cam.transform.position = Vector3.Lerp(GetComponent<Camera>().transform.position, islandPlace, Time.deltaTime * cameraSpeed);
        }
    }


    void HandleZoom(float delta)
    {
        if(Mathf.Abs(delta) < 0.01f) 
        {
            return;
        }

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta * zoomSpeed, minSize, maxSize);
        }
        else
        {
            cam.fieldOfView -= delta * zoomSpeed;
        }
    }
}
