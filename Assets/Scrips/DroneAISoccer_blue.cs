﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Panda;



[RequireComponent(typeof(DroneController))]
public class DroneAISoccer_blue : MonoBehaviour
{
    private DroneController m_Drone; // the drone controller we want to use

    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;

    public GameObject[] friends;
    public string friend_tag;
    public GameObject[] enemies;
    public string enemy_tag;

    public GameObject own_goal;
    public GameObject other_goal;
    public GameObject ball;

    PandaBehaviour myPandaBT;

    public float dist;
    public float maxKickSpeed = 40f;
    public float lastKickTime = 0f;

    private void Start()
    {
        myPandaBT = GetComponent<PandaBehaviour>();

        // get the car controller
        m_Drone = GetComponent<DroneController>();
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();


        // note that both arrays will have holes when objects are destroyed
        // but for initial planning they should work
        friend_tag = gameObject.tag;
        if (friend_tag == "Blue")
            enemy_tag = "Red";
        else
            enemy_tag = "Blue";

        friends = GameObject.FindGameObjectsWithTag(friend_tag);
        enemies = GameObject.FindGameObjectsWithTag(enemy_tag);

        ball = GameObject.FindGameObjectWithTag("Ball");


        // Plan your path here
        // ...
    }


    private bool CanKick()
    {
        dist = (transform.position - ball.transform.position).magnitude;
        return dist < 7f && (Time.time - lastKickTime) > 0.5f;
    }


    private void KickBall(Vector3 velocity)
    {
        // impulse to ball object in direction away from agent
        if (CanKick())
        {
            velocity.y = 0f;
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.AddForce(velocity, ForceMode.VelocityChange);
            lastKickTime = Time.time;
            print("ball was kicked ");
        }
    }


    public void GoToPosition(Vector3 target_position)
    {
        // Moves agent toward desired position.
        m_Drone.Move_vect(target_position - transform.position);
    }


    public float GoalkeeperScore(GameObject agent)
    {
        Vector3 own_goal_to_agent = agent.transform.position - own_goal.transform.position;
        //Vector3 own_goal_to_ball = ball.transform.position - own_goal.transform.position;
        return -own_goal_to_agent.magnitude;
    }


    public float ForwardScore(GameObject agent)
    {
        Vector3 ball_to_agent = agent.transform.position - ball.transform.position;
        return -ball_to_agent.magnitude;
    }


    [Task]
    bool IsGoalkeeper()
    {
        return false;
    }


    [Task]
    bool IsChaser()
    {
        float my_chaser_score = ChaserScore(transform.gameObject);
        float my_goalkeeper_score = GoalkeeperScore(transform.gameObject);
        float best_chaser_score = float.MinValue;
        float best_goalkeeper_score = float.MinValue;
        int i = 0;
        foreach (GameObject friend in friends)
        {
            if (friend != transform.gameObject)                                 // TODO: is this check valid? or check pos?
            {
                chaser_score = ChaserScore(friend);
                goalkeeper_score = GoalkeeperScore(friend);
                if (chaser_score > best_chaser_score)
                {
                    best_chaser_score = chaser_score;
                }
                if (goalkeeper_score > best_goalkeeper_score)
                {
                    best_goalkeeper_score = goalkeeper_score;
                }
            } 
        }
        // Check if this agent is the best chaser but at the same time not the best goalkeeper.
        if (my_chaser_score > best_chaser_score && my_goalkeeper_score <= best_goalkeeper_score)        // TODO: is this right?
        {
            return true;
        }
        return false;
    }


    [Task]
    bool IsBallCloserThan(float distance)
    {
        // Checks if the ball is closer than a certain distance to the agent.
        return ((ball.transform.position - transform.position).sqrMagnitude < (distance * distance));
    }


    [Task]
    bool TeammatesHaveBall()
    {

        return false;
    }


    [Task]
    void Defend(float what_the_hell_is_this)
    {
    }


    [Task]
    void InterceptBall()
    {
        Vector3 desired_position;
    }


    [Task]
    void Dribble()
    {
    }


    [Task]
    void GoCenter()
    {
    }


    [Task]
    void GoFishing()
    {
    }


    private void FixedUpdate()
    {


        // Execute your path here
        // ...

        Vector3 avg_pos = Vector3.zero;

        foreach (GameObject friend in friends)
        {
            avg_pos += friend.transform.position;
        }
        avg_pos = avg_pos / friends.Length;
        //Vector3 direction = (avg_pos - transform.position).normalized;
        Vector3 direction = (ball.transform.position - transform.position).normalized;

            

        // this is how you access information about the terrain
        int i = terrain_manager.myInfo.get_i_index(transform.position.x);
        int j = terrain_manager.myInfo.get_j_index(transform.position.z);
        float grid_center_x = terrain_manager.myInfo.get_x_pos(i);
        float grid_center_z = terrain_manager.myInfo.get_z_pos(j);

        Debug.DrawLine(transform.position, ball.transform.position, Color.black);
        Debug.DrawLine(transform.position, own_goal.transform.position, Color.green);
        Debug.DrawLine(transform.position, other_goal.transform.position, Color.yellow);
        Debug.DrawLine(transform.position, friends[0].transform.position, Color.cyan);
        Debug.DrawLine(transform.position, enemies[0].transform.position, Color.magenta);

        if (CanKick())
        {
            Debug.DrawLine(transform.position, ball.transform.position, Color.red);
            //KickBall(maxKickSpeed * Vector3.forward);
        }



        // this is how you control the agent
        m_Drone.Move_vect(direction);

        // this is how you kick the ball (if close enough)
        // Note that the kick speed is added to the current speed of the ball (which might be non-zero)
        Vector3 kickDirection = (other_goal.transform.position - transform.position).normalized;

        // replace the human input below with some AI stuff
        if (Input.GetKeyDown("space"))
        {
            KickBall(maxKickSpeed * kickDirection);
        }
    }


    private void Update()
    {
        myPandaBT.Reset();
        myPandaBT.Tick();
    }
}

