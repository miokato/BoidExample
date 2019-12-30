using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public int objectNum = 100;

    public GameObject prefab;
    public Param param;

    public List<Boid> boids = new List<Boid>();

    void Start()
    {

        
    }

    void Update()
    {
        while(boids.Count < objectNum)
        {
            CreateBoid();
        }
        while(boids.Count > objectNum)
        {
            RemoveBoid();
        }
        
    }

    void CreateBoid()
    {
        var obj = Instantiate(prefab, Vector3.zero, Random.rotation);
        obj.transform.SetParent(transform);
        var boid = obj.GetComponent<Boid>();
        boid.env = this;
        boid.param = param;
        boids.Add(boid);

    }

    void RemoveBoid()
    {
        // FIXME: null exception
        var lastIndex = boids.Count - 1;
        var boid = boids[lastIndex];
        Destroy(boid.gameObject);
        boids.RemoveAt(lastIndex);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * param.wallScale);
    }

} 

