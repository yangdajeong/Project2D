using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] ObjectPool pooler;
    [SerializeField] float Speed;

    [SerializeField] Grunt grunt;



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

    public void GrundCreateHitEffect()
    {
        Vector3 gruntPosition = transform.position;

        Vector3 direction = grunt.GruntDirVec.normalized;

        Vector3 startPosition = gruntPosition - direction * 10f;


        PooledObject instance = pooler.GetPool(startPosition, Quaternion.identity);


        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            instance.transform.right = direction;

            rb.AddForce(direction * Speed, ForceMode2D.Impulse);
        }
    }
}
