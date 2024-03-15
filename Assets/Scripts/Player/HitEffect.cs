using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] ObjectPool pooler;
    [SerializeField] float Speed;


    public void CreateHitEffect()
    {
        Vector3 playerPosition = transform.position;

        Vector3 direction = playerAttack.DirVec.normalized;

        Vector3 startPosition = playerPosition - direction * 10f;


        PooledObject instance = pooler.GetPool(startPosition, Quaternion.identity);


        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            instance.transform.right = direction;

            rb.AddForce(direction * Speed, ForceMode2D.Impulse);
        }
    }
}
