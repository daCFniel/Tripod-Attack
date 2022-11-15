using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class Shooter : MonoBehaviour
{
    public int numberOfCells { get; private set; } //number of power cells owned
    public GameObject powerCell; //link to the powerCell prefab
    public AudioClip throwSound; //throw sound
    public AudioClip pickupSound; //pickup sound
    public float throwSpeed;//throw speed

    public UnityEvent<Shooter> onCellEvent;

    private void Start()
    {
        numberOfCells = 5;
    }

    void Update()
    {
        //if left control (fire1) pressed, and we still have at least 1 cell
        if (Input.GetButtonDown("Fire1") && numberOfCells > 0)
        {
            numberOfCells--; //reduce the cell
                             //play throw sound
            onCellEvent.Invoke(this);
            AudioSource.PlayClipAtPoint(throwSound, transform.position);
            //instantaite the power cell as game object
            GameObject cell = Instantiate(powerCell, transform.position,
            transform.rotation) as GameObject;
            //ask physics engine to ignore collison between
            //power cell and our FPSControler
            Physics.IgnoreCollision(transform.root.GetComponent<Collider>(), cell.GetComponent<Collider>(), true);
            //give the powerCell a velocity so that it moves forward
            cell.GetComponent<Rigidbody>().AddForce(cell.transform.forward * throwSpeed * Time.deltaTime, ForceMode.Impulse);
        }
    }

    public void PickupCell()
    {
        numberOfCells++;
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        onCellEvent.Invoke(this);
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
