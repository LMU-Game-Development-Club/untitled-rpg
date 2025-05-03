// Name: Michail (Xan) Mavromatis
// Date of Creation: 12/2/2025
// Description: Script responsible for overworld player movement

using UnityEngine;
using UnityEngine.InputSystem;

public class MI_OverworldPlayerController : MonoBehaviour {
    private enum Direction { Up, Down, Left, Right }

    [Header("Player Settings")]
    public float interactDistance = 1.0f;
    public float moveSpeed = 4.2f;
    public LayerMask interactionLayerMask;

    [Header("ReadOnly Debug Data")]
    [SerializeField] private Direction _direction;
    [SerializeField] private bool _isInteracting;
    [SerializeField] private bool _inDialogue;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _animator;

    private DialogueController _dialogueController;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

#if UNITY_2023_1_OR_NEWER
        _dialogueController = FindFirstObjectByType<DialogueController>();
#else
        _dialogueController = FindObjectOfType<DialogueController>();
#endif

        if (_dialogueController == null) {
            Debug.LogError("DialogueController not found in scene!");
        } else {
            _inDialogue = false;
            _dialogueController.OnDialogueStart.AddListener(() => _inDialogue = true);
            _dialogueController.OnDialogueEnd.AddListener(() => _inDialogue = false);
        }
    }

    public void OnMove(InputValue value) {
        _movement = value.Get<Vector2>();
        if (_movement.y > 0) _direction = Direction.Up;
        else if (_movement.y < 0) _direction = Direction.Down;
        else if (_movement.x > 0) _direction = Direction.Right;
        else if (_movement.x < 0) _direction = Direction.Left;

        _animator.SetInteger("Direction", (int)_direction);
    }

    public void OnInteract(InputValue input) {
        _isInteracting = true;
    }

    private void Interaction() {
        Vector2 directionVector = GetDirectionVector();
        Vector3 startPosition = transform.position + (Vector3)directionVector * 0.75f;

        Debug.DrawRay(startPosition, directionVector * interactDistance, Color.red, 1f);

        RaycastHit2D hit = Physics2D.Raycast(startPosition, directionVector, interactDistance, interactionLayerMask);

        if (hit && hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable)) {
            interactable.Interact();
        }
    }

    private Vector2 GetDirectionVector() {
        return _direction switch {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            _ => Vector2.zero,
        };
    }

    private void FixedUpdate() {
        if (!_inDialogue) {
            _rb.MovePosition(_rb.position + _movement * moveSpeed * Time.fixedDeltaTime);
            _animator.SetFloat("Speed", _movement.sqrMagnitude);

            if (_isInteracting) {
                Interaction();
                _isInteracting = false;
            }
        } else {
            _animator.SetFloat("Speed", 0);
        }
    }
}
