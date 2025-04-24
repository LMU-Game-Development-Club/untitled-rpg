// Name: Michail (Xan) Mavromatis
// Date of Creation: 12/2/2025
// Description: Script responsible for overworld player movement

using UnityEngine;
using UnityEngine.InputSystem;

public class MI_OverworldPlayerController : MonoBehaviour
{
    private enum Direction { Up, Down, Left, Right }

    [Header("Player Settings")]
    public float interactDistance = 1.0f; // Can be changed in editor. 1.0 recommended for now.
    public float moveSpeed = 4.2f; // Can be changed in editor. 4.2 is the default speed for now.
    public LayerMask interactionLayerMask; // The layer mask for the interaction raycast. Set in editor to layer of interactable objects only.

    [Header("ReadOnly Debug Data")]
    [SerializeField]
    private Direction _direction;
    [SerializeField]
    private bool _isInteracting;

    [SerializeField]
    private bool _inDialogue;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;

    private UI_DialogueController _dialogueController;

    private float _footstepTimer;
    public string FootStepEvent = "";


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _dialogueController = UI_DialogueManager.Instance.transform.Find("DialogueController").GetComponent<UI_DialogueController>();
        _inDialogue = false;
        _dialogueController.OnDialogueStart.AddListener(() => _inDialogue = true);
        _dialogueController.OnDialogueEnd.AddListener(() => _inDialogue = false);

        _footstepTimer = 0.0f;
    }

    public void OnMove(InputValue value)
    {
        _movement = value.Get<Vector2>();
        if (_movement.y > 0)
        {
            _direction = Direction.Up;
        }
        else if (_movement.y < 0)
        {
            _direction = Direction.Down;
        }
        else if (_movement.x > 0)
        {
            _direction = Direction.Right;
        }
        else if (_movement.x < 0)
        {
            _direction = Direction.Left;
        }
        _animator.SetInteger("Direction", (int)_direction);
        if (_footstepTimer <= 0.0f)
        {
            _footstepTimer = 1.0f;
        }
    }

    public void OnInteract(InputValue input)
    {
        // We will have to do more if we want hold interactions
        // For now, we will just do a single interaction on button press
        _isInteracting = true; // This updates in FixedUpdate so we can do a raycast
    }

    private void Interaction() // Actual interaction function
    {
        Vector2 directionVector = GetDirectionVector();
        Vector3 startPosition = transform.position + (Vector3)directionVector * 0.75f; // Offset the start position to avoid self-interaction

        // TODO: Delete this debug line
        Debug.DrawRay(startPosition, directionVector * interactDistance, Color.red, 1f);

        // Cast a ray in the direction the player is facing
        RaycastHit2D hit = Physics2D.Raycast(startPosition, directionVector, interactDistance, interactionLayerMask);

        if (hit)
        {
            Debug.Log(hit.collider.name); // TODO: Delete this debug line
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }

    private Vector2 GetDirectionVector() // Helper function to convert the current direction enum to vector
    {
        switch (_direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!_inDialogue) {
            _rb.MovePosition(_rb.position + _movement * moveSpeed * Time.fixedDeltaTime);
            _animator.SetFloat("Speed", _movement.sqrMagnitude);
            if (_isInteracting)
            {
                Interaction(); // Raycasts must be done in FixedUpdate
                _isInteracting = false; // Reset interacting state
            }
        }
        if (_footstepTimer > 0.0f)
            if (_footstepTimer == 1f)
            {

            }
        {
            _footstepTimer = _footstepTimer - Time.deltaTime;
        }
    }
}
