using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Parts_fly : MonoBehaviour
{
    Rigidbody2D Rigidbody;

    private float fly_speed2 = -2.0f;
    private float fly_speed = 1.0f;
    private float fly_angle = 45.0f;
    private float fly_angle2 = 60.0f;
    private float fly_angle3 = 30.0f;
    private float height;

    public float fly_gravity = 9.8f;
    private float flying_time = 0f;

    public float destroy_time;

    private Vector2 pos1 = new Vector2(0, 0);

    public int pos_Num = 0;

    private Vector3 plus_scale;

    void Start()
    {
        plus_scale = new Vector3(0.0003f, 0.0003f, 0f);

        destroy_time = 0f;
        Cloud_movement cloud_Movement = GameObject.Find("MoveCloud").GetComponent<Cloud_movement>();
        pos_Num = cloud_Movement.ran_num;

        Rigidbody = GetComponent<Rigidbody2D>();

        if (pos_Num == 1 || pos_Num == 3)
        {
            float x = fly_speed * Mathf.Cos(fly_angle * Mathf.Deg2Rad);
            float y = 2.8f * fly_speed * Mathf.Sin(fly_angle * Mathf.Deg2Rad);
            height = 2.0f * y * y * y * y / (2f * fly_gravity);

            Rigidbody.gravityScale = fly_gravity / Physics2D.gravity.magnitude;
        }
        if (pos_Num == 2 || pos_Num == 4)
        {
            float x = fly_speed * Mathf.Cos(fly_angle * Mathf.Deg2Rad);
            float y = 2.8f * fly_speed * Mathf.Sin(fly_angle * Mathf.Deg2Rad);
            height = 2.0f * y * y * y * y / (2f * fly_gravity);
            Rigidbody.gravityScale = fly_gravity / Physics2D.gravity.magnitude;
        }

    }

    void FixedUpdate()
    {
        flying_time += Time.fixedDeltaTime;

        if (pos_Num == 1 || pos_Num == 3)
        {
            float x2 = fly_speed2 * Mathf.Cos(fly_angle3 * Mathf.Deg2Rad);
            float y2 = height - (0.5f * fly_gravity * flying_time * flying_time);
            Vector2 moveVector = new Vector2(x2, y2) * Time.fixedDeltaTime;
            Vector2 pos2 = Rigidbody.position + moveVector;
            destroy_time += Time.deltaTime;
            Rigidbody.MovePosition(pos2);
            this.gameObject.transform.localScale += plus_scale;
            if (destroy_time > 0.5f)
            {
                Destroy(gameObject);
                destroy_time = 0f;
            }
        }
        if (pos_Num == 2 || pos_Num == 4)
        {
            float x1 = fly_speed * Mathf.Cos(fly_angle2 * Mathf.Deg2Rad);
            float y1 = height - (0.5f * fly_gravity * flying_time * flying_time);
            Vector2 moveVector = new Vector2(x1, y1) * Time.fixedDeltaTime;
            Vector2 pos1 = Rigidbody.position + moveVector;
            destroy_time += Time.deltaTime;
            Rigidbody.MovePosition(pos1);
            this.gameObject.transform.localScale += plus_scale;
            if (destroy_time > 0.5f)
            {
                Destroy(gameObject);
                destroy_time = 0f;
            }
        }
    }

}
