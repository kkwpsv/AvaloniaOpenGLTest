using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL.Helpers
{
    internal class Camera
    {
        private readonly GL _gl;
        private readonly Control _control;

        public Camera(GL gl, Control control)
        {
            _gl = gl;
            _control = control;
            control.KeyDown += Control_KeyDown;
            control.PointerPressed += Control_PointerPressed;
            control.PointerMoved += Control_PointerMoved;
            control.PointerReleased += Control_PointerReleased;
            control.PointerWheelChanged += Control_PointerWheelChanged;
        }

        public Vector3 CameraPos { get; set; } = new Vector3(0, 0, 3);
        private Vector3 _cameraFront = new Vector3(0, 0, -1);
        private Vector3 _cameraUp = new Vector3(0, 1, 0);

        private void Control_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            var key = e.Key;
            var speed = 0.2f;
            switch (key)
            {
                case Key.W:
                    CameraPos += speed * _cameraFront;
                    break;
                case Key.S:
                    CameraPos -= speed * _cameraFront;
                    break;
                case Key.A:
                    CameraPos -= speed * Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp));
                    break;
                case Key.D:
                    CameraPos += speed * Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp));
                    break;
            }
        }

        private bool _isDown = false;
        private Point _lastPosition;
        private float yaw = -90;
        private float pitch = 0;
        private void Control_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            _isDown = true;
            _lastPosition = e.GetCurrentPoint(_control).Position;
        }

        private void Control_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (_isDown)
            {
                var position = e.GetCurrentPoint(_control).Position;
                var sensitivity = 0.05f;
                var xOffset = sensitivity * (position.X - _lastPosition.X);
                var yOffset = sensitivity * (_lastPosition.Y - position.Y);

                yaw += (float)xOffset;
                pitch += (float)yOffset;
                if (pitch > 89)
                {
                    pitch = 89;
                }
                else if (pitch < -89)
                {
                    pitch = -89;
                }

                var x = (float)(Math.Cos(yaw / 180 * Math.PI) * Math.Cos(pitch / 180 * Math.PI));
                var y = (float)(Math.Sin(pitch / 180 * Math.PI));
                var z = (float)(Math.Sin(yaw / 180 * Math.PI) * Math.Cos(pitch / 180 * Math.PI));
                _cameraFront = Vector3.Normalize(new Vector3(x, y, z));
                Console.WriteLine(_cameraFront);

                _lastPosition = position;
            }
        }

        private void Control_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            _isDown = false;
        }

        private float _fieldOfView = 45;

        private void Control_PointerWheelChanged(object? sender, Avalonia.Input.PointerWheelEventArgs e)
        {
            _fieldOfView -= (float)e.Delta.Y;
            if (_fieldOfView <= 1)
            {
                _fieldOfView = 1;
            }
            else if (_fieldOfView >= 45)
            {
                _fieldOfView = 45;
            }
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(CameraPos, CameraPos + _cameraFront, _cameraUp);
        }

        public Matrix4x4 GetProjection(float aspectRadio)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView((float)(_fieldOfView / 180 * Math.PI), aspectRadio, 0.1f, 100f);
        }
    }
}
