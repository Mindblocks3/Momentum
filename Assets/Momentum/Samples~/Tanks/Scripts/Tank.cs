using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputSystem.InputAction;

namespace Mirage.Examples.Tanks
{
    public class Tank : NetworkBehaviour
    {
        [Header("Components")]
        public NavMeshAgent agent;
        public Animator animator;

        [Header("Firing")]
        public KeyCode shootKey = KeyCode.Space;

        public Vector2 MoveInput;
        public bool FireInput;

        public GameObject projectilePrefab;
        public Transform projectileMount;

        [Header("Game Stats")]
        [SyncVar]
        public int health;
        [SyncVar]
        public int score;
        [SyncVar]
        public string playerName;
        [SyncVar]
        public bool isReady;

        public bool IsDead => health <= 0;

        public TextMesh nameText;

        public bool prevFire = false;

        void Update()
        {
            if (Camera.main)
            {
                nameText.text = playerName;
                nameText.transform.rotation = Camera.main.transform.rotation;
            }

            //Set local players name color to green
            nameText.color = Color.green;

            if (IsDead)
                return;

            // rotate
            float horizontal = MoveInput.x;
            transform.Rotate(0, horizontal * agent.angularSpeed * Time.deltaTime, 0);

            // move
            float vertical = MoveInput.y;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            agent.velocity = agent.speed * Mathf.Max(vertical, 0) * forward;
            animator.SetBool("Moving", agent.velocity != Vector3.zero);

            // shoot
            if (FireInput && !prevFire)
            {
                Debug.Log("Firing");
                Fire();                
            }
            prevFire = FireInput;
        }

        public void InputMove(CallbackContext context)
        {
            if (IsLocalPlayer)
                MoveInput = context.ReadValue<Vector2>();
        }

        public void InputFire(CallbackContext context)
        {
            if (IsLocalPlayer)
                FireInput = context.ReadValueAsButton();
        }

        // this is called on the server
        void Fire()
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
            projectile.GetComponent<Projectile>().source = gameObject;
            animator.SetTrigger("Shoot");
        }

        public void SendReadyToServer(string playername)
        {
            if (!IsLocalPlayer)
                return;

            CmdReady(playername);
        }

        [ServerRpc]
        void CmdReady(string playername)
        {
            if (string.IsNullOrEmpty(playername))
            {
                playerName = "PLAYER" + Random.Range(1, 99);
            }
            else
            {
                playerName = playername;
            }

            isReady = true;
        }
    }
}
