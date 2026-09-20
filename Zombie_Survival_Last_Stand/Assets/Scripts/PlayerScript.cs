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

        if(HP <= 0)
        {
            print("Player Dead");
            PlayerDead();
        }
        else
        {
            print("Player Hit");
            if (bloodyScreenCoroutine != null)
            {
                StopCoroutine(bloodyScreenCoroutine);
            }

            bloodyScreenCoroutine = StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health : {HP}"; 
            AudioManager.Instance.playerChannel.PlayOneShot(AudioManager.Instance.playerHurt);
        }
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (!bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(true);
        }

        Image image = bloodyScreen.GetComponentInChildren<Image>();

        Color color = image.color;
        color.a = 1f;
        image.color = color;

        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

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
        AudioManager.Instance.playerChannel.PlayOneShot(AudioManager.Instance.playerDie);
        StartCoroutine(PlayGameOverSound());


        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        GetComponentInChildren<Animator>().enabled = true;

        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(showGameOverUI());
    }

    private IEnumerator PlayGameOverSound()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.playerChannel.PlayOneShot(AudioManager.Instance.gameOver);
    }

    private IEnumerator showGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTERED: " + other.gameObject.name);

        if (other.CompareTag("ZombieHand"))
        {
            if(isDead == false)
            {
                //Debug.Log("Zombie hand hit player!");
                ZombieHand hand = other.GetComponent<ZombieHand>();

                if (hand != null)
                {
                    TakeDamage(hand.damage);
                }
            }
        }
    }
}