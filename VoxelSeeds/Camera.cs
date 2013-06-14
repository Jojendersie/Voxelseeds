using SharpDX;
using SharpDX.Toolkit;
using System.Windows.Input;

namespace VoxelSeeds
{
    class Camera
    {
        // matrices
        private Matrix _projectionMatrix;
        private Matrix _viewMatrix = Matrix.Identity;
        // vectors
        private Vector3 _viewDirection = new Vector3(0, 0, 1);
        private Vector3 _position = new Vector3(0, 100, 0);

        // projection stuff
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; RebuildProjectionMatrix(); }
        }
        private float _aspectRatio;
        private float _fov;
        private float _nearPlane;
        private float _farPlane;

        // movement factors variables
        private float _rotationSpeed = 0.01f;

        // some intern controlling variables
        private double _phi = 0.0f;
        private double _theta = -MathUtil.Pi / 2;
        private double _lastMouseX = 0; // last x position of the mouse
        private double _lastMouseY = 0; // last y position of the mouse


        private const float MIN_ZOOM = 10.0f;
        private const float MAX_ZOOM = 100.0f;
        private const float ZOOM_PER_WHEEL_STEP = 1.0f;

        private float _zoom;

        /// <summary>
        /// creates a new camera and sets a projection matrix up
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height. 
        ///                          To match aspect ratio of the viewport, the property AspectRatio.</param>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="nearPlane">Distance to the near view plane.</param>
        /// <param name="farPlane">Distance to the far view plane.</param>
        public Camera(float aspectRatio, float fov, float nearPlane, float farPlane)
        {
            this._aspectRatio = aspectRatio;
            this._fov = fov;
            this._nearPlane = nearPlane;
            this._farPlane = farPlane;
            RebuildProjectionMatrix();

 //           System.Windows.Forms.Application.
  //          ((System.Windows.Controls.Control)nativeWindow).MouseWheel += new MouseWheelEventHandler(MouseWheelHandler);
        }

        /// <summary>
        /// The projection matrix for this camera
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return _projectionMatrix; }
        }

        /// <summary>
        /// The view matrix for this camera
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return _viewMatrix; }
        }

        /// <summary>
        /// Current position of the camera
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Current view-direction of the camera
        /// </summary>
        public Vector3 Direction
        {
            get { return _viewDirection; }
        }

        /// <summary>
        /// Intern function for recreating the projection matrix.
        /// Capsuling the Matrix.Create... makes it easy to exchange the type of projection
        /// </summary>
        private void RebuildProjectionMatrix()
        {
            _projectionMatrix = Matrix.PerspectiveFovLH(_fov, _aspectRatio, _nearPlane, _farPlane);
        }

    /*    private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            _zoom += ZOOM_PER_WHEEL_STEP * e.Delta;
        }
        */
        /// <summary>
        /// Updates the Camera 
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // mouse movement
            UpdateThetaPhiFromMouse((float)gameTime.TotalGameTime.TotalSeconds);

            // resulting view direction
            _viewDirection = new Vector3((float)(System.Math.Cos(_phi) * System.Math.Sin(_theta)),
                                        (float)(System.Math.Cos(_theta)),
                                        (float)(System.Math.Sin(_phi) * System.Math.Sin(_theta)));
            // up vector - by rotation 90°
         /*   float theta2 = (float)_theta + (float)System.Math.PI / 2.0f;
            Vector3 upVec = new Vector3((float)(System.Math.Cos(_phi) * System.Math.Sin(theta2)),
                                        (float)(System.Math.Cos(theta2)),
                                        (float)(System.Math.Sin(_phi) * System.Math.Sin(theta2)));*/
            Vector3 upVec = Vector3.UnitY;

            // compute side
            Vector3 sideVec = Vector3.Cross(upVec, _viewDirection);

            Position = -_viewDirection * _zoom;

            // compute view matrix
            _viewMatrix = Matrix.LookAtLH(Position, Position + _viewDirection, upVec);
        }

        /// <summary>
        /// intern helper to update view angles by mouse
        /// </summary>
        protected void UpdateThetaPhiFromMouse(float passedTimeSinceLastFrame)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                // mouse movement
                double deltaX = Mouse.GetPosition(null).X - _lastMouseX;
                double deltaY = Mouse.GetPosition(null).Y - _lastMouseY;
                _phi -= deltaX * _rotationSpeed;
                _theta -= deltaY * _rotationSpeed;
            }
            else
            {
                _theta += (Keyboard.IsKeyDown(Key.Up) ? _rotationSpeed * passedTimeSinceLastFrame * 0.3f : 0.0f);
                _theta -= (Keyboard.IsKeyDown(Key.Down) ? _rotationSpeed * passedTimeSinceLastFrame * 0.3f : 0.0f);
                _phi -= (Keyboard.IsKeyDown(Key.Right) ? _rotationSpeed * passedTimeSinceLastFrame * 0.3f : 0.0f);
                _phi += (Keyboard.IsKeyDown(Key.Left) ? _rotationSpeed * passedTimeSinceLastFrame * 0.3f : 0.0f);
            }

            _lastMouseX = Mouse.GetPosition(null).X;
            _lastMouseY = Mouse.GetPosition(null).Y;
        }
    }
}
