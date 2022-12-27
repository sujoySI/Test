using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound; //Sound that we will play when picking up a new checkpoint
    private Transform currentCheckpoint; //We will store last checkpoint here
    private Health playerHealth;
    private UiManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindObjectOfType<UiManager>();
    }

    public void CheckRespawn()
    {
        //check if checkpoint available
        if(currentCheckpoint == null)
        {
            //Show game over screen
            uiManager.GameOver();

            return; //Don't execute the rest of this functions
        }
        
        transform.position = currentCheckpoint.position; //Move player to checkpoint position
        playerHealth.Respawn(); //Restore player healthand reset animation

        //Move camera to checkpoint room
        //(** for this to work checkpoint has to be placed as a child of the room object **)
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }

    //Activate checkpoints
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform; //Store the checkpoint that we activated as current one
            SoundManager.instance.PlaySound(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false; //Deactivate checkpoint collider
            collision.GetComponent<Animator>().SetTrigger("appear"); //Triger checkpoint animation
        }
    }
}
