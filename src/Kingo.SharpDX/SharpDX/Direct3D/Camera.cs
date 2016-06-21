using System;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a camera within a 3D coordinate system that is used to observe the virtual world.
    /// </summary>
    public abstract class Camera : ICamera
    {
        private readonly MoveableObject _moveableObject;
        private readonly RotatableObject _rotatableObject;
        private ProjectionPlanes _planes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera" /> class.
        /// </summary>
        protected Camera()
        {
            _moveableObject = new MoveableObject(this);  
            _rotatableObject = new RotatableObject(this);                                          
        }
        
        /// <summary>
        /// Gets or sets the projection planes of this camera, which make up the boundaries of the ClipSpace.
        /// </summary>
        public ProjectionPlanes Planes
        {
            get { return _planes; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _planes = value;
            }
        }        

        #region [====== Position ======]

        /// <inheritdoc />
        public Position3D Position
        {
            get { return _moveableObject.Position; }
        }


        /// <inheritdoc />
        public event EventHandler<Position3DChangedEventArgs> PositionChanged
        {
            add { _moveableObject.PositionChanged += value; }
            remove { _moveableObject.PositionChanged -= value; }
        }

        /// <inheritdoc />
        public void MoveX(float x)
        {
            _moveableObject.MoveX(x);
        }

        /// <inheritdoc />
        public void MoveY(float y)
        {
            _moveableObject.MoveY(y);
        }

        /// <inheritdoc />
        public void MoveZ(float z)
        {
            _moveableObject.MoveZ(z);
        }

        /// <inheritdoc />
        public void Move(float x, float y, float z)
        {
            _moveableObject.Move(x, y, z);
        }

        /// <inheritdoc />
        public void Move(Vector3 direction)
        {
            _moveableObject.Move(direction);
        }

        /// <inheritdoc />
        public void MoveToX(float x)
        {
            _moveableObject.MoveToX(x);
        }

        /// <inheritdoc />
        public void MoveToY(float y)
        {
            _moveableObject.MoveToY(y);
        }

        /// <inheritdoc />
        public void MoveToZ(float z)
        {
            _moveableObject.MoveToZ(z);
        }

        /// <inheritdoc />
        public void MoveTo(float x, float y, float z)
        {
            _moveableObject.MoveTo(x, y, z);
        }

        /// <inheritdoc />
        public void MoveTo(Vector3 position)
        {
            _moveableObject.MoveTo(position);
        }

        /// <inheritdoc />
        public void MoveTo(Position3D position)
        {
            _moveableObject.MoveTo(position);
        }

        #endregion

        #region [====== Rotation ======]

        /// <inheritdoc />
        public Rotation3D Rotation
        {
            get { return _rotatableObject.Rotation; }
        }

        /// <inheritdoc />
        public event EventHandler<Rotation3DChangedEventArgs> RotationChanged
        {
            add { _rotatableObject.RotationChanged += value; }
            remove { _rotatableObject.RotationChanged -= value; }
        }

        /// <inheritdoc />
        public void RotateX(Angle angle)
        {
            _rotatableObject.RotateX(angle);
        }

        /// <inheritdoc />
        public void RotateY(Angle angle)
        {
            _rotatableObject.RotateY(angle);
        }

        /// <inheritdoc />
        public void RotateZ(Angle angle)
        {
            _rotatableObject.RotateZ(angle);
        }

        /// <inheritdoc />
        public void Rotate(Angle x, Angle y, Angle z)
        {
            _rotatableObject.Rotate(x, y, z);
        }

        /// <inheritdoc />
        public void RotateToX(Angle angle)
        {
            _rotatableObject.RotateToX(angle);
        }

        /// <inheritdoc />
        public void RotateToY(Angle angle)
        {
            _rotatableObject.RotateToY(angle);
        }

        /// <inheritdoc />
        public void RotateToZ(Angle angle)
        {
            _rotatableObject.RotateToZ(angle);
        }

        /// <inheritdoc />
        public void RotateTo(Angle x, Angle y, Angle z)
        {
            _rotatableObject.RotateTo(x, y, z);
        }

        /// <inheritdoc />
        public void RotateTo(Rotation3D rotation)
        {
            _rotatableObject.RotateTo(rotation);
        }

        /// <inheritdoc />
        public void Pitch(Angle angle)
        {
            _rotatableObject.Pitch(angle);
        }

        /// <inheritdoc />
        public void Yaw(Angle angle)
        {
            _rotatableObject.Yaw(angle);
        }

        /// <inheritdoc />
        public void Roll(Angle angle)
        {
            _rotatableObject.Roll(angle);
        }

        #endregion

        #region [====== LookAt ======]

        /// <inheritdoc />
        public void LookAt(Vector3 target)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Panning ======]        

        /// <inheritdoc />
        public void PanHorizontal(float distance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void PanVertical(float distance)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Matrices ======] 

        /// <summary>
        /// Returns the ViewMatrix of this camera that is determined by its position and rotation.
        /// </summary>
        protected Matrix ViewMatrix
        {
            get
            {
                var worldMatrix = RotationMatrix * TranslationMatrix;
                worldMatrix.Invert();
                return worldMatrix;
            }
        }            

        private Matrix RotationMatrix
        {
            get { return _rotatableObject.Rotation.RotationMatrix; }
        }

        private Matrix TranslationMatrix
        {
            get { return _moveableObject.Position.TranslationMatrix; }
        }

        /// <summary>
        /// Returns the ProjectionMatrix of this camera, which defines the ClipSpace.
        /// </summary>
        protected abstract Matrix ProjectionMatrix
        {
            get;
        }

        /// <summary>
        /// Creates and returns a Matrix that can be used to transform an object from its local space to the camera's ClipSpace.
        /// </summary>
        /// <param name="worldMatrix">The WorldMatrix of the object.</param>
        /// <returns>A new Matrix.</returns>
        public Matrix ToClipSpaceMatrix(Matrix worldMatrix)
        {
            return worldMatrix * (ViewMatrix * ProjectionMatrix);
        }        

        #endregion
    }
}
