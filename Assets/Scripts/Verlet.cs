using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public Vector3 pos;
    public Vector3 oldPos;
    public bool pinned = false;
}

public class Stick
{
    public Point p0;
    public Point p1;
    public float length;
}

// resource:https://www.youtube.com/watch?v=YgRZDCBLDfs&list=PL7wAPgl1JVvUEb0dIygHzO4698tmcwLk9&index=39
public class Verlet : MonoBehaviour {

    public List<GameObject> objs = new List<GameObject>();
    List<Point> points = new List<Point>();
    List<Stick> sticks = new List<Stick>();
    float bounce = 0.9f;
    float gravity = 0.001f;
    float friction = 0.999f;
    float timeElapsed = 0f;
    // Use this for initialization
    void Start () {
        CreatePointsAndSticks();
    }
	
    private void CreatePointsAndSticks()
    {
        var p = new Point();
        p.pos = new Vector3(-0.1f, 0.8f, 0.0f);
        p.oldPos = new Vector3(-0.05f, 0.75f, 0.0f);
        points.Add(p);

        p = new Point();
        p.pos = new Vector3(0.1f, 0.8f, 0.0f);
        p.oldPos = new Vector3(0.1f, 0.8f, 0.0f);
        points.Add(p);

        p = new Point();
        p.pos = new Vector3(0.1f, 0.6f, 0.0f);
        p.oldPos = new Vector3(0.1f, 0.6f, 0.0f);
        points.Add(p);

        p = new Point();
        p.pos = new Vector3(-0.1f, 0.6f, 0.0f);
        p.oldPos = new Vector3(-0.1f, 0.6f, 0.0f);
        points.Add(p);

        //

        p = new Point();
        p.pos = new Vector3(0.6f, 0.8f, 0.0f);
        p.oldPos = new Vector3(0.6f, 0.8f, 0.0f);
        p.pinned = true;
        points.Add(p);

        p = new Point();
        p.pos = new Vector3(0.4f, 0.8f, 0.0f);
        p.oldPos = new Vector3(0.4f, 0.8f, 0.0f);
        points.Add(p);

        p = new Point();
        p.pos = new Vector3(0.25f, 0.8f, 0.0f);
        p.oldPos = new Vector3(0.25f, 0.8f, 0.0f);
        points.Add(p);

        // sticks
        var s = new Stick();
        s.p0 = points[0];
        s.p1 = points[1];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        s = new Stick();
        s.p0 = points[1];
        s.p1 = points[2];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        s = new Stick();
        s.p0 = points[2];
        s.p1 = points[3];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        s = new Stick();
        s.p0 = points[3];
        s.p1 = points[0];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        s = new Stick();
        s.p0 = points[0];
        s.p1 = points[2];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        //
        s = new Stick();
        s.p0 = points[4];
        s.p1 = points[5];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        s = new Stick();
        s.p0 = points[5];
        s.p1 = points[6];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);

        s = new Stick();
        s.p0 = points[6];
        s.p1 = points[1];
        s.length = Vector3.Distance(s.p0.pos, s.p1.pos);
        sticks.Add(s);
    }

	// Update is called once per frame
	void Update () {
        UpdatePoints();
        for(int i = 0; i<3; i++)
        {
            UpdateSticks();
            ConstrainPoints();
        }
        RenderPoints();
    }

    void UpdatePoints()
    {
        timeElapsed += Time.deltaTime * 1.2f;
        for (int i = 0; i< points.Count; i++)
        {
            var p = points[i];
            if (!p.pinned)
            {
                var vx = (p.pos.x - p.oldPos.x) * friction;
                var vy = (p.pos.y - p.oldPos.y) * friction;

                p.oldPos.x = p.pos.x;
                p.oldPos.y = p.pos.y;

                p.pos.x += vx;
                p.pos.y += vy;

                p.pos.y -= gravity;
            } else
            {
                p.oldPos.x = p.pos.x;
                p.pos.x += (0.6f+Mathf.Sin(timeElapsed) *0.5f)- p.oldPos.x;
            }
        }
    }

    void ConstrainPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            if (!p.pinned)
            {
                var vx = (p.pos.x - p.oldPos.x) * friction;
                var vy = (p.pos.y - p.oldPos.y) * friction;

                float w = 2f;
                float h = 1f;
                if (p.pos.x > w)
                {
                    p.pos.x = w;
                    p.oldPos.x = p.pos.x + vx * bounce;
                }
                else if (p.pos.x < -w)
                {
                    p.pos.x = -w;
                    p.oldPos.x = p.pos.x + vx * bounce;
                }

                if (p.pos.y > h)
                {
                    p.pos.y = h;
                    p.oldPos.y = p.pos.y + vy * bounce;
                }
                else if (p.pos.y < -h)
                {
                    p.pos.y = -h;
                    p.oldPos.y = p.pos.y + vy * bounce;
                }
            }
        }
    }

    void UpdateSticks()
    {
        for (int i = 0; i < sticks.Count; i++)
        {
            var s = sticks[i];
            var dx = s.p1.pos.x - s.p0.pos.x;
            var dy = s.p1.pos.y - s.p0.pos.y;
            var dist = Mathf.Sqrt(dx * dx + dy * dy);
            var diff = s.length - dist;
            var percent = diff / dist / 2f;
            var offsetX = dx * percent;
            var offsetY = dy * percent;

            if (!s.p0.pinned)
            {
                s.p0.pos.x -= offsetX;
                s.p0.pos.y -= offsetY;
            }

            if (!s.p1.pinned)
            {
                s.p1.pos.x += offsetX;
                s.p1.pos.y += offsetY;
            }
        }
    }

    void RenderPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            objs[i].transform.position = points[i].pos;
        }
    }
}
