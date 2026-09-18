using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;
    private Coroutine bloodyScreenCoroutine;

    public TextMeshProUGUI playerHealthUI;
    public TextMeshProUGUI gameOverUI;

    public bool isDead;


    public void Start()
    {
        playerHealthUI.text = $"Health : {HP}";
    }


    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        Debug.Log("Player HP: " + HP);

        if (HP <= 0)
        {
            HP = 0;

            playerHealthUI.text = $"Health : {HP}";

            Debug.Log("Player Dead");

            PlayerDead();
        }
        else
        {
            Debug.Log("Player Hit");

            if (bloodyScreenCoroutine != null)
            {
                StopCoroutine(bloodyScreenCoroutine);
            }

            bloodyScreenCoroutine = StartCoroutine(BloodyScreenEffect());

            playerHealthUI.text = $"Health : {HP}";
        }
    }


    private IEnumerator BloodyScreenEffect()
    {
        if (!bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(true);
        }


        Image image = bloodyScreen.GetComponentInChildren<Image>();

        if (image == null)
        {
            Debug.LogWarning("Bloody Screen Image component not found.");

            bloodyScreenCoroutine = null;

            yield break;
        }


        Color color = image.color;

        color.a = 1f;

        image.color = color;


        float duration = 3f;

        float elapsedTime = 0f;


        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(
                1f,
                0f,
                elapsedTime / duration
            );


            color = image.color;

            color.a = alpha;

            image.color = color;


            elapsedTime += Time.deltaTime;


            yield return null;
        }


        color = image.color;

        color.a = 0f;

        image.color = color;


        bloodyScreen.SetActive(false);


        bloodyScreenCoroutine = null;
    }


    private void PlayerDead()
    {
        isDead = true;


        // -----------------------------------------------------
        // DISABLE MOUSE MOVEMENT
        // -----------------------------------------------------

        MouseMovement mouseMovement = GetComponent<MouseMovement>();

        if (mouseMovement != null)
        {
            mouseMovement.enabled = false;
        }


        // -----------------------------------------------------
        // DISABLE PLAYER MOVEMENT
        //
        // Your movement script is PlayerController,
        // NOT PlayerMovement.
        // -----------------------------------------------------

        PlayerController playerController = GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.enabled = false;
        }


        // -----------------------------------------------------
        // PLAYER DEATH ANIMATION
        // -----------------------------------------------------

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.enabled = true;
        }


        // -----------------------------------------------------
        // HIDE HEALTH UI
        // -----------------------------------------------------

        if (playerHealthUI != null)
        {
            playerHealthUI.gameObject.SetActive(false);
        }


        // -----------------------------------------------------
        // SCREEN FADER
        //
        // Commented temporarily because ScreenFader
        // does not exist in the project yet.
        // -----------------------------------------------------

        // GetComponent<ScreenFader>().StartFade();


        StartCoroutine(showGameOverUI());
    }


    private IEnumerator showGameOverUI()
    {
        yield return new WaitForSeconds(1f);


        if (gameOverUI != null)
        {
            gameOverUI.gameObject.SetActive(true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "TRIGGER ENTERED: " + other.gameObject.name
        );


        if (other.CompareTag("ZombieHand"))
        {
            if (isDead == false)
            {
                // -------------------------------------------------
                // ZombieHand script does not exist currently.
                //
                // Keep this here for later.
                // -------------------------------------------------

                /*
                ZombieHand hand = other.GetComponent<ZombieHand>();

                if (hand != null)
                {
                    TakeDamage(hand.damage);
                }
                */


                // TEMPORARY TEST:
                // You can uncomment this if you want the player
                // to take damage whenever something tagged
                // "ZombieHand" touches the player.

                // TakeDamage(10);
            }
        }
    }
}