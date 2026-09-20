using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    public ZombieHand zombieHand;
    public int zombieDamage;

    private void Start()
    {
        zombieHand.damage = zombieDamage;
    }
}
