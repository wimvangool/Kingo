using System;
using System.Diagnostics;
using System.Windows;
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
            Vector3D moveDirection;

            if (ControlMode.Controller.CanMove && TryGetNextMove(out moveDirection))
            {
                ControlMode.Controller.Move(moveDirection);
                
            }            
        }

        private bool TryGetNextMove(out Vector3D moveDirection)
        {           
            if (Keyboard.Modifiers == ModifierKeys.None)
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

                if (moveBuilder.MoveCount > 0)
                {
                    moveDirection = moveBuilder.BuildMove();
                    return true;
                }
            }
            moveDirection = MoveBuilder.NoMove;
            return false;
        }                

        private static bool IsKeyDown(Key key)
        {
            return key != Key.None && Keyboard.IsKeyDown(key);
        }                                 
    }
}
