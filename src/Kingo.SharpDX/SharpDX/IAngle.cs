using System;

namespace Kingo.SharpDX
{
    /// <summary>
    /// When implemented by a class, represents an angle that can be expressed in radians and degrees.
    /// </summary>
    public interface IAngle : IFormattable
    {
        /// <summary>
        /// Returns this angle in term of radians.
        /// </summary>        
        float ToRadians();

        /// <summary>
        /// Returns this angle in term of degrees.
        /// </summary>        
        float ToDegrees();
    }
}
