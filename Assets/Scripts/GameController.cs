using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum GameStates
    { 
        Menu,
        Starting,
        AreaTransition,
        Playing,
        Dead,
        Paused
    }

    [System.Serializable]
    public class ObjectInstance
    {
        public GameObject objectToSpawn;
        public int spawnChance;
    }

    [System.Serializable]
    public class LayerInstance
    {
        public ObjectInstance[] objects;
        public int totalObjects;
        public int mindepth;
        public int maxdepth;
        public int layerWidth;
        public float depthScalar;
        public float playerSpeed;
        public float turnSpeed;
        public float areaTransition;
        public Color areaColor;
        public float cameraZ;
    }

    public LayerInstance[] ObjectsToSpawn;


    public TextMeshProUGUI DebugUIText;
    public Camera camera;
    public Player player;
    public float startingTimer;
    public float areaTransitionShake;
    public GameObject mainMenuUI;
    public GameObject inGameUI;
    public AudioSource mainMusic;

    public TextMeshProUGUI depthText;

    private List<Vector3> points;
    private LineRenderer lineRenderer;

    private Vector2 screenDim;
    private GameStates currentGameState;

    private float cameraTargetPosition;
    private float startingDelta;

    private float currentDepth;
    private float currentDepthScalar;

    private float areaTransitionDelta;
    private float areaTransitionTimer;
    private Color lastAreaColor;
    public float lastCameraZ;
    private Color newAreaColor;
    public float newCameraZ;

    public int health;
    public float invincabilityTimer;


    public Image deathBackground;
    public GameObject retryButton;
    public float deathTimer;

    public void DealDamage()
    {
        if (invincabilityTimer > 0) return;
        invincabilityTimer = 0.1f;
        health--;
    }

    private Dictionary<int, LayerInstance> layers;
    public GameStates GetState()
    {
        return currentGameState;
    }
    // Start is called before the first frame update
    void Start()
    {
        invincabilityTimer = 0;
        health = 3;
        lineRenderer = GetComponent<LineRenderer>();
        points = new List<Vector3>();
        layers = new Dictionary<int, LayerInstance>();

        foreach(LayerInstance inst in ObjectsToSpawn)
        {
            layers[inst.mindepth] = inst;
        }

        areaTransitionDelta = 0.0f;

        currentDepth = -90.0f;
        currentDepthScalar = 1.0f;

        screenDim = new Vector2();
        screenDim.y = camera.orthographicSize * 2;
        screenDim.x = screenDim.y * camera.aspect;
        currentGameState = GameStates.Menu;
        cameraTargetPosition = screenDim.y / 4;
        mainMenuUI.SetActive(true);
        inGameUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGameState)
        {
            case GameStates.Starting:
                startingDelta += Time.deltaTime;
                camera.transform.localPosition = new Vector3(0, Mathf.Lerp(0, cameraTargetPosition, startingDelta / startingTimer), -10);
                if (startingDelta > startingTimer)
                {
                    startingDelta = 0.0f;
                    currentGameState = GameStates.Playing;
                    camera.transform.localPosition = new Vector3(0, cameraTargetPosition, -10);
                    inGameUI.SetActive(true);
                }
                break;
            case GameStates.AreaTransition:
                areaTransitionDelta += Time.deltaTime;
                camera.transform.localPosition += new Vector3(Mathf.Sin(areaTransitionDelta * 20.0f)* areaTransitionShake, 0, 0);

                lastAreaColor = camera.backgroundColor;
                lastCameraZ = camera.transform.position.z;

                float lerpAmout = areaTransitionDelta / areaTransitionTimer;

                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y,
                    Mathf.Lerp(lastCameraZ, newCameraZ, lerpAmout));

                camera.backgroundColor = Color.Lerp(lastAreaColor, newAreaColor, lerpAmout);

                if (areaTransitionDelta > areaTransitionTimer)
                {

                    camera.transform.localPosition = new Vector3(0, cameraTargetPosition, newCameraZ);
                    camera.backgroundColor = newAreaColor;
                    areaTransitionDelta = 0.0f;
                    currentGameState = GameStates.Playing;
                    inGameUI.SetActive(true);
                }
                break;
            case GameStates.Playing:
                InGame();
                break;
            case GameStates.Dead:
                deathTimer -= Time.deltaTime;
                deathBackground.gameObject.SetActive(true);
                Color background = deathBackground.color;
                background.a = Mathf.Lerp(1.0f, 0.0f, deathTimer / 3);
                deathBackground.color = background;
                if (deathTimer<0.0f)
                {
                    retryButton.SetActive(true);
                }
                break;
        }
        
    }

    public void Retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void InGame()
    {

        if(health<=0)
        {
            mainMusic.Stop();
            SFXController.instance.Play("Defeat");
            currentGameState = GameStates.Dead;
        }

        lineRenderer.material.color = Color.Lerp(new Color(0.5f, 0.5f, 1.0f), Color.green, (float)(((float)health) / 3));
        invincabilityTimer -= Time.deltaTime;


        currentDepth += player.growthSpeed * Time.deltaTime * currentDepthScalar;

        DebugUIText.text = Input.acceleration.x.ToString();

        string depthString = (currentDepth).ToString("F1") + "cm";
        if(currentDepth>10000)
        {
            depthString = (currentDepth/ 10000).ToString("F1") + "miles";
        }
        else if (currentDepth > 100)
        {
            depthString = (currentDepth / 100).ToString("F1") + "m";
        }

        depthText.text = depthString;

        // how far below the screen before we start unloading data
        float lowScreenBound = camera.transform.position.y - (screenDim.y * 3);
        // Loop through the closest point untill we find one thats far away, when we do, remove all byond that
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if (lowScreenBound > points[i].y)
            {
                points.RemoveRange(0, i);
                break;
            }
        }

        foreach(KeyValuePair<int, LayerInstance> inst in layers)
        {
            if(inst.Key < camera.transform.position.y + screenDim.y)
            {
                SpawnLayer(inst.Value);
                currentGameState = GameStates.AreaTransition;
                lastAreaColor = camera.backgroundColor;
                newAreaColor = inst.Value.areaColor;
                lastCameraZ = camera.transform.position.z;
                newCameraZ = inst.Value.cameraZ;
                health = 3;

                SFXController.instance.Play("MovedToNewArea");

                areaTransitionTimer = inst.Value.areaTransition;
                points.Add(player.transform.position);
                points.Add(player.transform.position);
                GenerateBody(points);
                layers.Remove(inst.Key);
                return;
            }
        }

        points.Add(player.transform.position);
        GenerateBody(points);
        
    }

    private void SpawnLayer(LayerInstance layer)
    {
        currentDepthScalar = layer.depthScalar;
        player.growthSpeed = layer.playerSpeed;
        player.turnSpeed = layer.turnSpeed;

        int objectChance = 0;
        foreach(ObjectInstance inst in layer.objects)
        {
            objectChance += inst.spawnChance;
        }

        List<Vector2> usedPositions = new List<Vector2>();

        int minDepth = layer.mindepth;
        if(minDepth< camera.transform.position.y + (screenDim.y / 2))
        {
            minDepth = (int)(camera.transform.position.y + (screenDim.y / 2));
        }

        for (int i = 0; i < layer.totalObjects; i++)
        {
            int newObjectChance = Random.Range(0, objectChance);
            foreach (ObjectInstance inst in layer.objects)
            {
                if(newObjectChance<inst.spawnChance)
                {

                    GameObject newObject = GameObject.Instantiate(inst.objectToSpawn);
                    Vector2 position = new Vector2();
                    do
                    {
                        position.x = camera.transform.position.x + Random.Range(-layer.layerWidth, layer.layerWidth);
                        position.y = Random.Range(minDepth, layer.maxdepth);
                    } while (usedPositions.Contains(position));
                     
                    usedPositions.Add(position);

                    newObject.transform.position = new Vector3(position.x,position.y,0);

                    break;
                }
                newObjectChance -= inst.spawnChance;
            }
        }
    }

    public void StartGame()
    {
        currentGameState = GameStates.Starting;
        mainMenuUI.SetActive(false);
    }
    public void GenerateBody(List<Vector3> parts)
    {
        lineRenderer.positionCount = parts.Count;
        lineRenderer.SetPositions(parts.ToArray());
    }
}
