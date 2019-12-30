using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="Boid/Param")]
public class Param : ScriptableObject
{
    public float initSpeed = 2f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float distance = 1f;
    public float fov = 90f;
    public float wallScale = 5f;
    public float wallDistance = 3f;
    public float wallWeight = 1f;
    public float SeparationWeight = 5f;
    public float AlignmentWeight = 2f;
    public float CohesionWeight = 3f;
}