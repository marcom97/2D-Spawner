using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public SpriteRenderer[] objectsToSpawn;
    public Vector3 SpawnAreaSize;
    public float frequencyMin;
    public float frequencyMax;
    public int quantityMin;
    public int quantityMax;
    public float scaleMin;
    public float scaleMax;
    public int depthMin;
    public int depthMax;
    public float depthScale;
    public bool alphaScaleEnabled;
    
    public Vector3 moveDirection;
    public float moveSpeedMin;
    public float moveSpeedMax;
    public float stoppingDistance;
    public OnStop onStop = (x) => Destroy(x);

    public delegate void OnStop(GameObject spawnee);

    private bool isSpawning;

    // Use this for initialization
    void Start () {
        StartSpawning();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartSpawning() {
        isSpawning = true;
        StartCoroutine(Spawn());
    }

    public void StopSpawning(bool destroyCurrent) {
        isSpawning = false;
        StopCoroutine(Spawn());

        if (destroyCurrent) {
            foreach (Move move in transform.
                     GetComponentsInChildren<Move>()) {
                Destroy(move.gameObject);
            }
        }
    }

    private IEnumerator Spawn() {
        var halfSize = SpawnAreaSize / 2;

        var normalizedDirection = moveDirection.normalized;
        var projection = Mathf.Abs(Vector3.Dot(normalizedDirection, halfSize)) * 
                                normalizedDirection;

        while (isSpawning) {
            for (int i = 0; i < Random.Range(quantityMin, quantityMax + 1); i++) {
                var objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.
                                                                GetUpperBound(0))];
                var spawnee = Instantiate(objectToSpawn).gameObject;
                spawnee.transform.parent = transform;

                var depth = Random.Range(depthMin, depthMax);

                var scale = Random.Range(scaleMin, scaleMax);
                spawnee.transform.localScale = Vector3.one * scale * Mathf.Pow(depthScale,
                                                                                 depth);

                var pos = new Vector3(Random.Range(-halfSize.x, halfSize.x),
                                      Random.Range(-halfSize.y, halfSize.y),
                                      Random.Range(-halfSize.z, halfSize.z));
                spawnee.transform.localPosition = pos - projection;

                var spriteRenderer = spawnee.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder -= depth;
                var extentProj = 
                    Mathf.Abs(Vector3.Dot(spriteRenderer.bounds.extents, 
                                          normalizedDirection)) 
                         * normalizedDirection;
                spawnee.transform.localPosition -= extentProj;

                if (alphaScaleEnabled)
                {
                    var color = spriteRenderer.color;
                    color.a *= Mathf.Pow(depthScale, depth);
                    spriteRenderer.color = color;
                }

                var speed = Random.Range(moveSpeedMin, moveSpeedMax);

                var maxDistance = stoppingDistance + (extentProj.magnitude * 2)
                                    + projection.magnitude;

                var move = spawnee.AddComponent<Move>();
                move.speedScale = Mathf.Pow(depthScale, depth);
                move.MoveBy(normalizedDirection * speed, maxDistance, () => onStop(spawnee));
            }

            yield return new WaitForSeconds(Random.Range(frequencyMin, frequencyMax));
        }
    }

}
