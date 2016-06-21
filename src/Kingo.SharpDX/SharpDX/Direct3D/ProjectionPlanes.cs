using System;
using Kingo.Resources;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a set of projection planes used by a camera to determine its clip space.
    /// </summary>
    public sealed class ProjectionPlanes
    {        
        /// <summary>
        /// A default set of projection planes ([0.5, 100]).
        /// </summary>
        public static readonly ProjectionPlanes Default = new ProjectionPlanes(0.5f, 100f);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionPlanes" /> class.
        /// </summary>
        /// <param name="nearPlane">Distance of the near plane.</param>
        /// <param name="farPlane">Distance of the far plane.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="nearPlane"/> or <paramref name="farPlane"/> is <c>0</c> or les than <c>0</c>,
        /// or <paramref name="nearPlane"/> is greater than or equal to <paramref name="farPlane"/>.
        /// </exception>
        public ProjectionPlanes(float nearPlane, float farPlane)
            : this(new Length(nearPlane), new Length(farPlane)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionPlanes" /> class.
        /// </summary>
        /// <param name="nearPlane">Distance of the near plane.</param>
        /// <param name="farPlane">Distance of the far plane.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="nearPlane"/> or <paramref name="farPlane"/> is <c>0</c>,
        /// or <paramref name="nearPlane"/> is greater than or equal to <paramref name="farPlane"/>.
        /// </exception>
        public ProjectionPlanes(Length nearPlane, Length farPlane)
        {
            if (nearPlane == Length.Zero)
            {
                throw NewNearPlaneZeroException(nameof(nearPlane));
            }
            if (nearPlane >= farPlane)
            {
                throw NewNearPlaneTooLargeException(nearPlane, farPlane);
            }
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        private static Exception NewNearPlaneZeroException(string paramName)
        {            
            return new ArgumentOutOfRangeException(paramName, ExceptionMessages.ProjectionPlanes_NearPlaneZero);
        }

        private static Exception NewNearPlaneTooLargeException(Length nearPlane, Length farPlane)
        {
            var messageFormat = ExceptionMessages.ProjectionPlanes_NearPlaneTooLarge;
            var message = string.Format(messageFormat, nearPlane, farPlane);
            return new ArgumentOutOfRangeException(nameof(nearPlane), message);
        }

        /// <summary>
        /// Distance of the near-plane.
        /// </summary>
        public Length NearPlane
        {
            get;
        }

        /// <summary>
        /// Distance of the far-plane.
        /// </summary>
        public Length FarPlane
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{NearPlane} - {FarPlane}]";
        }

        /// <summary>
        /// Moves the position of the near-plane.
        /// </summary>
        /// <param name="distance">Direction and distance of the plane to move.</param>
        /// <returns>A new set of projection-planes where the move has been applied.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="distance"/> causes the near-plane to move behind the camera or further than the far-plane.
        /// </exception>
        public ProjectionPlanes MoveNearPlane(float distance)
        {
            return new ProjectionPlanes(Move(NearPlane, distance), FarPlane);
        }

        /// <summary>
        /// Moves the position of the far-plane.
        /// </summary>
        /// <param name="distance">Direction and distance of the plane to move.</param>
        /// <returns>A new set of projection-planes where the move has been applied.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="distance"/> causes the far-plane to move behind the near-plane.
        /// </exception>
        public ProjectionPlanes MoveFarPlane(float distance)
        {
            return new ProjectionPlanes(NearPlane, Move(FarPlane, distance));
        }

        /// <summary>
        /// Moves the position of both the near-plane and the far-plane.
        /// </summary>
        /// <param name="distance">Direction and distance of the planes to move.</param>
        /// <returns>A new set of projection-planes where the move has been applied.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="distance"/> causes the near-plane to move behind the camera.
        /// </exception>
        public ProjectionPlanes Move(float distance)
        {
            return new ProjectionPlanes(Move(NearPlane, distance), Move(FarPlane, distance));
        }

        private static float Move(Length plane, float distance)
        {
            return plane.ToSingle() + distance;
        }
    }
}
