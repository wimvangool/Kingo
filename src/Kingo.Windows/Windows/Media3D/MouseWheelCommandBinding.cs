using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between certain <see cref="MouseWheelEvents">MouseWheel events</see> and specific camera-commands.
    /// </summary>
    public sealed class MouseWheelCommandBinding : ControlModeCommandBinding<MouseWheelEvents>
    {
        /// <inheritdoc />
        protected override void OnActivated()
        {
            ControlMode.InputSource.MouseWheel += HandleMouseWheelEvent;                       
        }       

        /// <inheritdoc />
        protected override void OnDeactivating()
        {
            ControlMode.InputSource.MouseWheel -= HandleMouseWheelEvent;
        }        

        private void HandleMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            HandleMouseWheelEvent(e.Delta < 0 ? MouseWheelEvents.ScrollDown : MouseWheelEvents.ScrollUp);
            
            e.Handled = true;
        }        

        private void HandleMouseWheelEvent(MouseWheelEvents mouseWheelEvent)
        {
            Vector3D moveDirection;

            if (ControlMode.Controller.CanMove && TryGetNextMove(mouseWheelEvent, out moveDirection))
            {
                ControlMode.Controller.Move(moveDirection);
            }            
        }

        private bool TryGetNextMove(MouseWheelEvents mouseWheelEvent, out Vector3D moveDirection)
        {
            var controller = ControlMode.Controller;
            var moveBuilder = new MoveBuilder(MoveSpeed);

            if (IsMatch(Left, mouseWheelEvent))
            {
                moveBuilder.AddMove(controller.Left);
            }
            if (IsMatch(Right, mouseWheelEvent))
            {
                moveBuilder.AddMove(controller.Right);
            }

            if (IsMatch(Up, mouseWheelEvent))
            {
                moveBuilder.AddMove(controller.Up);
            }
            if (IsMatch(Down, mouseWheelEvent))
            {
                moveBuilder.AddMove(controller.Down);
            }

            if (IsMatch(Forward, mouseWheelEvent))
            {
                moveBuilder.AddMove(controller.Forward);
            }
            if (IsMatch(Backward, mouseWheelEvent))
            {
                moveBuilder.AddMove(controller.Backward);
            }

            if (moveBuilder.MoveCount > 0)
            {
                moveDirection = moveBuilder.BuildMove();
                return true;
            }
            moveDirection = MoveBuilder.NoMove;
            return false;
        }

        private static bool IsMatch(MouseWheelEvents expectedEvent, MouseWheelEvents actualEvent)
        {
            return EnumOperators<MouseWheelEvents>.IsDefined(actualEvent, expectedEvent);
        }        
    }
}
