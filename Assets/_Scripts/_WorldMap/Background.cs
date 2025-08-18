using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform[] positions;
    public Transform[] minMaxY;
    public Transform canvas;
    public Transform foregroundCanvas;
    public Camera cam;
    public GameObject cloudPrefab;
    public int count;
    public Sprite[] cloudSprites;
    public float[] minMaxSpeed = new float[2] {3, 5};
    public float[] minMaxScale = new float[2] {0.5f, 2};
    public float[] minMaxWait = new float[2] {5, 10};
    public List<GameObject> clouds = new List<GameObject>();
    public List<Clouds> cloudsScript = new List<Clouds>();
    public List<Transform> destinations = new List<Transform>();

    private float originalSize;

    public void Start()
    {
        for(int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(cloudPrefab, positions[0].position, Quaternion.identity);
            int canvasChoice = Random.Range(0, 2);
            Transform _canvas = canvasChoice == 1 ? foregroundCanvas : canvas;
            go.transform.SetParent(_canvas);
            if(go.TryGetComponent(out Clouds goScript))
            {
                GameObject destination = Instantiate(positions[1].gameObject, positions[1].position, Quaternion.identity);
                destination.transform.SetParent(_canvas);
                goScript.icon.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
                goScript.speed = Random.Range(minMaxSpeed[0], minMaxSpeed[1]);
                go.SetActive(false);
                destinations.Add(destination.transform);
                cloudsScript.Add(goScript);
            }
            clouds.Add(go);
        }
        StartCoroutine(SpawnClouds());
    }

    void Update()
    {
        for(int i = 0; i < clouds.Count; i++)
        {
            if(!clouds[i].activeInHierarchy)
            {
                continue;
            }

            float multiplyer = originalSize / cam.orthographicSize; 
            Vector3 newScale = new Vector3(cloudsScript[i].originalLocalScale.x * multiplyer, cloudsScript[i].originalLocalScale.y * multiplyer, cloudsScript[i].originalLocalScale.z * multiplyer);
            clouds[i].transform.localScale = Vector3.Lerp(clouds[i].transform.localScale, newScale, Time.deltaTime * cloudsScript[i].scaleSpeed);
    
            float alpha = Mathf.InverseLerp(cloudsScript[i].disappearZoom, cloudsScript[i].appearZoom, cam.orthographicSize); 
            Color color = cloudsScript[i].icon.color;
            color.a = cloudsScript[i].baseAlpha * alpha; 
            cloudsScript[i].icon.color = color;
            cloudsScript[i].icon.enabled = alpha == 0 ? false : true;
    
            clouds[i].transform.position = Vector3.MoveTowards(clouds[i].transform.position, destinations[i].position, Time.deltaTime * cloudsScript[i].speed);
            if(Vector2.Distance(clouds[i].transform.position, destinations[i].position) <= 0f)
            {
                clouds[i].SetActive(false);
            }
        }
    }

    void SpawnRandomCloudChance()
    {
        int amount = Mathf.RoundToInt(count * 0.3f);
        for(int i = 0; i < amount; i++)
        {
            clouds[i].SetActive(true);
            // cloudsScript[i].enabled = true;
            cloudsScript[i].originalSize = this.originalSize;
            float randomY = Random.Range(minMaxY[0].position.y, minMaxY[1].position.y);
            clouds[i].transform.position = new Vector3(positions[0].position.x, randomY, positions[0].position.z);
            float randomScale = Random.Range(minMaxScale[0], minMaxScale[1]);
            clouds[i].transform.localScale = new Vector3(randomScale, randomScale, 0);
            destinations[i].transform.position = new Vector3(destinations[i].transform.position.x, randomY, destinations[i].transform.position.z);
            clouds[i].transform.position = Vector3.Lerp(clouds[i].transform.position, destinations[i].position, Random.Range(0, 1f));
        }
    }

    IEnumerator SpawnClouds()
    {
        yield return null;
        originalSize = cam.orthographicSize;
        SpawnRandomCloudChance();

        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minMaxWait[0], minMaxWait[1]));
            for(int i = 0; i < clouds.Count; i++)
            {
                if(!clouds[i].activeInHierarchy)
                {
                    clouds[i].SetActive(true);
                    // cloudsScript[i].enabled = true;
                    cloudsScript[i].originalSize = this.originalSize;
                    float randomY = Random.Range(minMaxY[0].position.y, minMaxY[1].position.y);
                    clouds[i].transform.position = new Vector3(positions[0].position.x, randomY, positions[0].position.z);
                    float randomScale = Random.Range(minMaxScale[0], minMaxScale[1]);
                    clouds[i].transform.localScale = new Vector3(randomScale, randomScale, 0);
                    destinations[i].transform.position = new Vector3(destinations[i].transform.position.x, randomY, destinations[i].transform.position.z);
                    break;
                }
            }
        }
    }
}
