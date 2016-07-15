using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented, represents a 3D-transformation that can be represented by a transformation-matrix.
    /// </summary>
    public interface ITransformation3D
    {
        /// <summary>
        /// The matrix representing the transformation.
        /// </summary>
        Matrix TransformationMatrix
        {
            get;
        }
    }
}
