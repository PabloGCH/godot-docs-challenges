using Godot;
using System;
using PlayerMovementDirection;

public partial class player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();
    

    [Export]
    public int speed = 400; // How fast the player will move (pixels/sec).
    public Vector2 ScreenSize; // Size of the game window.
    private AnimatedSprite2D AnimatedSprite;


    private void OnBodyEntered(PhysicsBody2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hide();
        ScreenSize = GetViewportRect().Size;
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    private Vector2 getMovementInputDirection()
    {
        //SE OBTIENE LA DIRECCION DEL JUGADOR
        //LA CUAL DEPENDE DE LAS TECLAS QUE SE ESTEN PRESIONANDO
        var direction = Vector2.Zero;
        if(Input.IsActionPressed("move_right")) {
            direction.X += 1;
        }
        if(Input.IsActionPressed("move_left")) {
            direction.X -= 1;
        }
        if(Input.IsActionPressed("move_down")) {
            direction.Y += 1;
        }
        if(Input.IsActionPressed("move_up")) {
            direction.Y -= 1;
        }
        return direction.Normalized();
    } 

    private void updatePositionDuringMovement(Vector2 direction, double delta)
    {
        //NORMALIZA LA VELOCIDAD, PARA QUE EL JUGADOR NO SE MUEVA MAS RAPIDO EN DIAGONAL
        //Y LUEGO LA MULTIPLICA POR LA VELOCIDAD
        var velocity = direction * speed;
        //SE MUEVE EL JUGADOR, MULTIPLICANDO LA VELOCIDAD POR EL TIEMPO TRANSCURRIDO
        //DELTA ES EL TIEMPO TRANSCURRIDO DESDE EL ULTIMO FRAME
        Position += velocity * (float)delta;
        //SE ASEGURA QUE NO SE SALGA DE LA PANTALLA
        Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
                y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
                );
    }

    public void updateAnimation(Boolean isMoving, Vector2 direction = default(Vector2))
    {
        if(!isMoving) {
            AnimatedSprite.Stop();
            return;
        }
        AnimatedSprite.Play();
        //CAMBIA LA ANIMACION DEPENDIENDO DE LA DIRECCION
        if(direction.X != (float)PLAYER_MOVEMENT_DIRECTION.STOP) {
            AnimatedSprite.Animation = "walk";
            AnimatedSprite.FlipV = false;
            AnimatedSprite.FlipH = direction.X < 0;
        } else if(direction.Y != (float)PLAYER_MOVEMENT_DIRECTION.STOP) {
            AnimatedSprite.Animation = "up";
            AnimatedSprite.FlipV = direction.Y > 0;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //SE OBTIENE LA DIRECCION DEL JUGADOR
        var direction = getMovementInputDirection();
        if(direction.Length() > 0) {
            updatePositionDuringMovement(direction, delta);
            //SE CAMBIA LA ANIMACION
            updateAnimation(true, direction);
        } else {
            //SI NO SE ESTA MOVIENDO, SE PARA LA ANIMACION
            updateAnimation(false);
        }
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
}






