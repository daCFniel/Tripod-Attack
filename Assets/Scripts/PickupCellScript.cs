using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCellScript : MonoBehaviour
{
    public Shooter shooter;

    public void PickupCell()
    {
        shooter.numberOfCells++;
        //AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        print(shooter.numberOfCells);
        GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Score>().UpdateScoreText(shooter.numberOfCells);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the other object entering our trigger zone
        //has a tag called "Pick Up"
        if (other.gameObject.CompareTag("Pickup"))
        {
            //deactivate the other object
            other.gameObject.SetActive(false);
            //add score
            PickupCell();
        }
    }
}
