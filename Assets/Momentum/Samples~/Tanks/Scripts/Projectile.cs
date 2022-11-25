using UnityEngine;

namespace Mirage.Examples.Tanks
{
    public class Projectile : MonoBehaviour
    {
        public float destroyAfter = 1;
        public Rigidbody rigidBody;
        public float force = 1000;

        void Awake()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        [Header("Game Stats")]
        public int damage;
        public GameObject source;

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        void Start()
        {
            rigidBody.AddForce(transform.forward * force);
        }

        // destroy for everyone on the server
        void DestroySelf()
        {
            Destroy(gameObject);
        }

        // [Server] because we don't want a warning if OnTriggerEnter is
        // called on the client
        void OnTriggerEnter(Collider co)
        {
            //Hit another player
            if (co.CompareTag("Player") && co.gameObject != source)
            {
                //Apply damage
                co.GetComponent<Tank>().health -= damage;

                //update score on source
                source.GetComponent<Tank>().score += damage;
            }

            Destroy(gameObject);
        }
    }
}
