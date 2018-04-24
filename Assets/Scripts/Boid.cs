using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boid : MonoBehaviour {

	// Use this for initialization
	Vector3 center;
	public float radius = 1.0f;
	public Vector3 velocity=new Vector3(0.0f,0.0f,0.0f);
	int speed=0;
	public float follow = 1.0f;
	public float velocityWeight = 0.03f;
	public float collisionWeight = 0.5f;
	public float centerWeight = 1.0f;


	public Slider radiusSlider;
	public Slider followSlider;
	public Slider velocitySlider;
	public Slider collisionSlider;
	public Slider centerSlider;
	void Start () {
		velocity = speed * transform.position;
		radiusSlider.onValueChanged.AddListener(delegate { ValueChangeCheckRadius(); });
		followSlider.onValueChanged.AddListener(delegate { ValueChangeCheckFollow(); });
		velocitySlider.onValueChanged.AddListener(delegate { ValueChangeCheckVelocity(); });
		collisionSlider.onValueChanged.AddListener(delegate { ValueChangeCheckCollision(); });
		centerSlider.onValueChanged.AddListener(delegate { ValueChangeCheckCenter(); });
	}

	private void ValueChangeCheckCenter()
	{
		centerWeight = (centerSlider.value);
	}

	private void ValueChangeCheckCollision()
	{
		collisionWeight = (collisionSlider.value);
	}

	private void ValueChangeCheckVelocity()
	{
		velocityWeight = (velocitySlider.value);
	}

	private void ValueChangeCheckFollow()
	{
		follow = (followSlider.value);
	}

	public void ValueChangeCheckRadius()
	{
		radius = (radiusSlider.value);
	}

	// Update is called once per frame
	void Update () {
		center = transform.position;
		Collider[] hitsCollider = Physics.OverlapSphere(center, radius);
		int i = 0;
		//Debug.Log(hitsCollider.Length);
		
		change_position(hitsCollider);
		//change_position();
	}

	void change_position(Collider[] hitsCollider)
	{
		Vector3 v1, v2, v3, v4;
		GameObject b = this.gameObject;
		//v1 = rule1(b);
		v1 = rule1(b,hitsCollider);
		v2 = rule2(b, hitsCollider);
		v3 = rule3(b, hitsCollider,velocity);
		v4 = rule4(b, hitsCollider);
		velocity = velocity + v1 + v2 + v3 + v4;
		speedlimit(b);
		transform.position = transform.position + velocity * (Time.deltaTime); 

	}

	Vector3 rule1(GameObject b , Collider[] hitsCollider)
	{
		Vector3 pc =new Vector3(0.0f, 0.0f, 0.0f);
		int i = 0;
		 
		
		while ( i < hitsCollider.Length )
		{
			//Debug.Log("Working");
			if (hitsCollider[i] != b){
				//Debug.Log("Inside");
				pc = pc + hitsCollider[i].transform.position;
			}
			i++;
		}
		pc = pc / hitsCollider.Length;
		return (pc - b.transform.position) *centerWeight/ 100;

	}

	Vector3 rule2(GameObject b, Collider[] hitsCollider)
	{
		Vector3 c = new Vector3(0.0f,0.0f,0.0f);
		int i = 0;
		while (i < hitsCollider.Length)
		{
			if (hitsCollider[i] != b)
			{
				if(Vector3.Distance(hitsCollider[i].transform.position,b.transform.position)<3.0f)
				{
					c = c - ((hitsCollider[i].transform.position-b.transform.position)*collisionWeight);
				}
			}
			i++;
		}
		return c;
	}

	Vector3 rule3(GameObject b, Collider[] hitsCollider, Vector3 velocity)
	{
		Vector3 pv = new Vector3(0.0f, 0.0f, 0.0f);
		int i = 0;


		while (i < hitsCollider.Length)
		{
			//Debug.Log("Working");
			if (hitsCollider[i] != b)
			{
				//Debug.Log("Inside");
				if (hitsCollider[i].GetComponent<Boid>())
				{
					Boid controlscript = hitsCollider[i].GetComponent<Boid>();

					pv = pv + controlscript.velocity;
				}
			}
			i++;
		}
		pv = pv / hitsCollider.Length;
		return (pv - velocity) * velocityWeight;

	}


	Vector3 rule4(GameObject b, Collider[] hitsCollider)
	{
		Vector3 target = new Vector3(0.0f, 0.0f, 0.0f);
		int i = 0;
		while (i < hitsCollider.Length)
		{
			if (hitsCollider[i].GetComponent<Leader>())
			{
				Leader lscript = hitsCollider[i].GetComponent<Leader>();
				target = target + hitsCollider[i].transform.position;
			}
			i++;
		}
		return ((target - b.transform.position)*follow)/100;
	}

	void speedlimit(GameObject b)
	{
		int lim=10;
		Vector3 v = new Vector3(0.0f, 0.0f, 0.0f);
		if(velocity.magnitude > lim)
		{
			velocity = (velocity / velocity.magnitude) * lim;
		}

	}
}
