using UnityEngine;

internal class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private PlayerView _view;
    [SerializeField] private float _speed = 1;

    private float _prevXMovement = 0;
    private float _prevYMovement = 0;

    private void FixedUpdate()
    {
        if (HasMoveInput())
        {
            float xMovement = Input.GetAxisRaw("Horizontal");
            float yMovement = Input.GetAxisRaw("Vertical");

            Vector2 direction = new Vector2(xMovement, yMovement).normalized;
            Vector2 startPos = _rb.position;
            Vector2 offset = direction * _speed * Time.fixedDeltaTime;
            
            _rb.MovePosition(startPos + offset);
            _view.PlayMove(direction.x, direction.y, direction.magnitude);

            _prevXMovement = xMovement;
            _prevYMovement = yMovement;
        }
        else
        {
            _view.PlayMove(_prevXMovement, _prevYMovement, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            _view.PlayAttack();
    }

    private bool HasMoveInput()
    {
        return
            Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.UpArrow)
            ;
    }
}
