using SharpDX;
using SharpDX.Toolkit;
using System.Windows;
using System.Windows.Input;

namespace VoxelSeeds
{
    class Camera
    {
        private float _deltaX;
        private float _deltaY;
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
        private float _moveSpeed = 0.2f;

        // some intern controlling variables
        private double _phi = MathUtil.Pi * 0.25f;
        private double _theta = MathUtil.Pi * 1.3f;

        private const double MIN_THETA = MathUtil.Pi * 1.1f;
        private const double MAX_THETA = MathUtil.Pi * 0.4f + MathUtil.Pi;


        /// <summary>
        /// last processed mouse position
        /// </summary>
        private System.Drawing.Point _lastMousePosition;

        private System.Drawing.Point _currentMousePosition;


        private const float MIN_ZOOM = 5.0f;
        private const float MAX_ZOOM = 100.0f;
        private const float ZOOM_PER_WHEEL_STEP = 0.01f;

        private float _zoom = 80.0f;

        private bool _cameraMouseRotateOn = false;
        private bool _cameraMouseMoveOn = false;


        /// <summary>
        /// creates a new camera and sets a projection matrix up
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height. 
        ///                          To match aspect ratio of the viewport, the property AspectRatio.</param>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="nearPlane">Distance to the near view plane.</param>
        /// <param name="farPlane">Distance to the far view plane.</param>
        /// <param name="inputControlElement">control element that will be hooked for input</param>
        public Camera(float aspectRatio, float fov, float nearPlane, float farPlane, System.Windows.Forms.Control inputControlElement)
        {
            this._aspectRatio = aspectRatio;
            this._fov = fov;
            this._nearPlane = nearPlane;
            this._farPlane = farPlane;
            RebuildProjectionMatrix();


            // input handling...
            inputControlElement.MouseWheel += (object sender, System.Windows.Forms.MouseEventArgs e) =>
                {
                    _zoom += ZOOM_PER_WHEEL_STEP * e.Delta;
                    _zoom = System.Math.Min(MAX_ZOOM, _zoom);
                    _zoom = System.Math.Max(MIN_ZOOM, _zoom);
                };
            inputControlElement.MouseDown += (object sender, System.Windows.Forms.MouseEventArgs e) => 
                _cameraMouseRotateOn = e.Button == System.Windows.Forms.MouseButtons.Middle;
            inputControlElement.MouseUp += (object sender, System.Windows.Forms.MouseEventArgs e) =>
                {
                    if (_cameraMouseRotateOn && e.Button == System.Windows.Forms.MouseButtons.Middle)
                        _cameraMouseRotateOn = false;
                };
            inputControlElement.MouseDown += (object sender, System.Windows.Forms.MouseEventArgs e) =>
                _cameraMouseMoveOn = e.Button == System.Windows.Forms.MouseButtons.Right;
            inputControlElement.MouseUp += (object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (_cameraMouseMoveOn && e.Button == System.Windows.Forms.MouseButtons.Right)
                    _cameraMouseMoveOn = false;
            };
            inputControlElement.MouseMove += (object sender, System.Windows.Forms.MouseEventArgs e) =>
                _currentMousePosition = e.Location;
        }

        public Ray GetPickingRay(int screenResolutionX, int screenResolutionY)
        {
            Vector2 deviceCor = new Vector2((float)_currentMousePosition.X / screenResolutionX - 0.5f, -(float)_currentMousePosition.Y / screenResolutionY + 0.5f) * 2;
            Matrix viewProjection = ViewMatrix * ProjectionMatrix;
            Matrix viewProjectionInverse = viewProjection; viewProjectionInverse.Invert();

            var rayOrigin = Vector3.TransformCoordinate(new Vector3(deviceCor, 0), viewProjectionInverse);
            var rayTarget = Vector3.TransformCoordinate(new Vector3(deviceCor, 1), viewProjectionInverse);
            var dir = rayTarget - rayOrigin;
            dir.Normalize();
            return new Ray(rayOrigin, dir);
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
        
        /// <summary>
        /// Updates the Camera 
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // mouse movement
            UpdateThetaPhiFromMouse((float)gameTime.ElapsedGameTime.TotalSeconds);

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
            Vector3 coVec = Vector3.Cross(upVec, sideVec);

            Vector3 center = -sideVec * _deltaX - coVec * _deltaY + Vector3.UnitY * 5;
            Position = -_viewDirection * _zoom + center;

            // compute view matrix
            _viewMatrix = Matrix.LookAtLH(Position, Position + _viewDirection, upVec);
            /*_viewMatrix.Column1 = new Vector4(coVec, 0);
            _viewMatrix.Column2 = new Vector4(sideVec, 0);
            _viewMatrix.Column3 = new Vector4(_viewDirection, 0);
            _viewMatrix.Column4 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            _viewMatrix *= Matrix.Translation(Position);*/
        }

        /// <summary>
        /// intern helper to update view angles by mouse
        /// </summary>
        protected void UpdateThetaPhiFromMouse(float passedTimeSinceLastFrame)
        {
            if (_cameraMouseMoveOn)
            {
                _deltaX += (_currentMousePosition.X - _lastMousePosition.X) * _moveSpeed;
                _deltaY += (_currentMousePosition.Y - _lastMousePosition.Y) * _moveSpeed;
            }

            if (_cameraMouseRotateOn)
            {
                // mouse movement
                double deltaX = _currentMousePosition.X - _lastMousePosition.X;
                double deltaY = _currentMousePosition.Y - _lastMousePosition.Y;
                _phi -= deltaX * _rotationSpeed;
                _theta -= deltaY * _rotationSpeed;
            }
            else
            {
                _theta += (Keyboard.IsKeyDown(Key.Up) ? _rotationSpeed * passedTimeSinceLastFrame : 0.0f) * 100;
                _theta -= (Keyboard.IsKeyDown(Key.Down) ? _rotationSpeed * passedTimeSinceLastFrame : 0.0f) * 100;
                _phi -= (Keyboard.IsKeyDown(Key.Right) ? _rotationSpeed * passedTimeSinceLastFrame : 0.0f) * 100;
                _phi += (Keyboard.IsKeyDown(Key.Left) ? _rotationSpeed * passedTimeSinceLastFrame : 0.0f) * 100;
            }

            _theta = System.Math.Min(_theta, MAX_THETA);
            _theta = System.Math.Max(_theta, MIN_THETA);

            _lastMousePosition = _currentMousePosition;
        }
    }
}
