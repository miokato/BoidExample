using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 accel = Vector3.zero;

    public Environment env;
    public Param param;

    List<Boid> neighbors = new List<Boid>();

    void Start()
    {

        position = transform.position;
        velocity = transform.forward * param.initSpeed;
        
    }

    void Update()
    {
        // 毎回近隣の個体をクリア
        neighbors.Clear();

        // 近隣の個体を取得
        UpdateNeighbors();
        // 壁に近づいたら反対方向の力を加える
        UpdateWall();
        // 分離
        UpdateSeparation();
        // 整列
        UpdateAlignment();
        // 凝集
        UpdateCohesion();
        // 十字キーで上下左右に力を加える
        UpdateUserInput();
        // 適用
        UpdateMove();
        
    }

    void UpdateUserInput()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            accel += Vector3.right * 5f;
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            accel += Vector3.left * 5f;
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            accel += Vector3.up * 5f;
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            accel += Vector3.down * 5f;
        }
    }

    void UpdateNeighbors()
    {
        neighbors.Clear();

        var prodThresh = Mathf.Cos(param.fov * Mathf.Deg2Rad);
        var distThresh = param.distance;

        foreach(var other in env.boids)
        {
            if(other == this) continue;
            var to = other.position - position;
            var dist = to.magnitude;
            if(dist < distThresh)
            {
                var dir = to.normalized;
                var fwd = velocity.normalized;
                var prod = Vector3.Dot(fwd, dir);
                if(prod > prodThresh)
                {
                    neighbors.Add(other);
                }
            }
        }
    }

    void UpdateSeparation()
    {
        // これないとNaNが発生
        if(neighbors.Count == 0) return;

        Vector3 force = Vector3.zero;
        foreach(var neighbor in neighbors)
        {
            // 近隣の個体から自分に向かうベクトルを足し合わせる
            force += (position - neighbor.position).normalized;
        }
        force /= neighbors.Count;
        // 要するに群れから離れる方向へ力を加える
        accel += force * param.SeparationWeight;

    }

    void UpdateAlignment()
    {
        if(neighbors.Count == 0) return;

        Vector3 averageVelocity = Vector3.zero;
        foreach(var neighbor in neighbors)
        {
            averageVelocity += neighbor.velocity;
        }
        averageVelocity /= neighbors.Count;
        // 自分から近隣個体の平均の向きへ向かうベクトルを加える
        // ベクトルの引き算は順番を逆にすると逆向きのベクトルになるので注意
        accel += (averageVelocity - velocity) * param.AlignmentWeight;

    }

    void UpdateCohesion()
    {
        if(neighbors.Count == 0) return;

        Vector3 averagePosition = Vector3.zero;
        foreach(var neighbor in neighbors)
        {
            averagePosition += neighbor.position;
        }
        averagePosition /= neighbors.Count;
        // 自分から近隣個体の位置の中心へ向かうベクトルを加える
        // ベクトルの引き算は順番を逆にすると逆向きのベクトルになるので注意
        accel += (averagePosition - position) * param.CohesionWeight;

    }

    void UpdateWall()
    {
        var scale = param.wallScale * 0.5f;

        accel += 
            CalcAccelAgainstWall(-scale - position.x, Vector3.right) +
            CalcAccelAgainstWall(-scale - position.y, Vector3.up) +
            CalcAccelAgainstWall(-scale - position.z, Vector3.forward) +
            CalcAccelAgainstWall(+scale - position.x, Vector3.left) +
            CalcAccelAgainstWall(+scale - position.y, Vector3.down) +
            CalcAccelAgainstWall(+scale - position.z, Vector3.back); 
    }

    Vector3 CalcAccelAgainstWall(float distance, Vector3 dir)
    {
        if(distance < param.wallDistance)
        {
            return dir * (param.wallWeight / Mathf.Abs(distance / param.wallDistance));
        }
        return Vector3.zero;
    }

    void UpdateMove()
    {
        var dt = Time.deltaTime;
        velocity += accel * dt;

        var dir = velocity.normalized;
        var speed = velocity.magnitude;
        velocity = Mathf.Clamp(speed, param.minSpeed, param.maxSpeed) * dir;
        position += velocity * dt;

        var rot = Quaternion.LookRotation(velocity);
        transform.SetPositionAndRotation(position, rot);

        accel = Vector3.zero;
    }
}
