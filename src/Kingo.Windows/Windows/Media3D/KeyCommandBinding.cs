using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between certain <see cref="Key">keys</see> and specific camera-commands.
    /// </summary>
    public sealed class KeyCommandBinding : ControlModeCommandBinding<Key>
    {                
        /// <inheritdoc />
        protected override void OnActivated()
        {
            CompositionTarget.Rendering += HandleNextFrame;
        }

        /// <inheritdoc />
        protected override void OnDeactivating()
        {
            CompositionTarget.Rendering -= HandleNextFrame;
        }

        private void HandleNextFrame(object sender, EventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.None)
            {
                return;
            }
            Vector3D moveDirection;

            if (TryGetNextMove(out moveDirection))
            {
                ControlMode.Controller.Move(moveDirection);                
            }
            double zoomFactor;

            if (TryGetZoomFactor(out zoomFactor))
            {
                ControlMode.Controller.Zoom(zoomFactor);
            }
        }

        private bool TryGetNextMove(out Vector3D moveDirection)
        {                       
            var controller = ControlMode.Controller;
            var moveBuilder = new MoveBuilder(MoveSpeed);

            // Left & Right.
            if (IsKeyDown(Left))
            {
                moveBuilder.AddMove(controller.Left);
            }
            if (IsKeyDown(Right))
            {
                moveBuilder.AddMove(controller.Right);
            }

            // Up & Down.
            if (IsKeyDown(Up))
            {
                moveBuilder.AddMove(controller.Up);
            }
            if (IsKeyDown(Down))
            {
                moveBuilder.AddMove(controller.Down);
            }

            // Forward & Backward.
            if (IsKeyDown(Forward))
            {
                moveBuilder.AddMove(controller.Forward);
            }
            if (IsKeyDown(Backward))
            {
                moveBuilder.AddMove(controller.Backward);
            }

            if (moveBuilder.HasMoves)
            {
                moveDirection = moveBuilder.BuildMove();
                return true;
            }            
            moveDirection = MoveBuilder.NoMove;
            return false;
        }    
        
        private bool TryGetZoomFactor(out double zoomFactor)
        {
            if (IsKeyDown(ZoomIn))
            {
                if (!IsKeyDown(ZoomOut))
                {
                    zoomFactor = ZoomSpeed;
                    return true;
                }                
            }
            else if (IsKeyDown(ZoomOut))
            {
                zoomFactor = -ZoomSpeed;
                return true;
            }
            zoomFactor = 0;
            return false;
        }                   

        private static bool IsKeyDown(Key key)
        {
            return key != Key.None && Keyboard.IsKeyDown(key);
        }                                 
    }
}
