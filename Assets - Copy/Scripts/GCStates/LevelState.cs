using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        GCScript.inst.messageCanvas.SetActive(true);
        GCScript.inst.SetMessageText("Level " + GCScript.inst.level);

        // We need to build the level
        GCScript.inst.ActivateLayout(1);

        GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameObject temp;
        for(int i =0; i<spawns.Length; i++)
        {
            int swap = Random.Range(0, spawns.Length);
            temp = spawns[i];
            spawns[i] = spawns[swap];
            spawns[swap] = temp;
        }

        //just create 6 monsters for now
        for(int i=0; i<6; i++)
        {
            spawns[i].GetComponent<ParticleSystem>().Play();
            GameObject go = Instantiate(GCScript.inst.redMonster, spawns[i].transform);
        }

        GCScript.inst.activePlayer = Instantiate(GCScript.inst.playerPrefab);
        GCScript.inst.activePlayer.transform.position = new Vector3(-1.5f, 1f, 0);


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GCScript.inst.messageCanvas.SetActive(false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
