using LSystem;
using OpenGL;
using System;

namespace LSystem
{
    public class FPSCamera : Camera
    {
        private const float MAX_PITCH = 89;
        private const float MAX_VARIANCE = 10; // 처음시작할 때 급격한 변동을 막기 위한 변수
        private float _pitch = 0.0f;
        private float _yaw = 0.0f;

        /// <summary>
        /// 각도 degree
        /// </summary>
        public float CameraPitch
        {
            get => _pitch;
            set => _pitch = value;
        }

        /// <summary>
        /// 각도 degree
        /// </summary>
        public float CameraYaw
        {
            get => _yaw;
            set => _yaw = value;
        }

        public FPSCamera(string name, float x, float y, float z, float yaw, float pitch) : base(name, x, y, z)
        {
            _pitch = pitch;
            _yaw = yaw;
            UpdateCameraVectors();
        }

        public override Matrix4x4f ViewMatrix
        {
            get
            {
                return Extension.CreateViewMatrix(_position, _cameraRight, _cameraUp, _cameraForward);
                //return Matrix4x4f.LookAtDirection(_position, _cameraForward, _cameraUp);
            }
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
        }

        public override void Update(int deltaTime)
        {
            UpdateCameraVectors();
        }

        // calculates the front vector from the Camera's (updated) Euler Angles
        /// <summary>
        /// 
        /// </summary>
        public void UpdateCameraVectors()
        {
            Vertex3f direction = Vertex3f.Zero;
            float yawRad = _yaw.ToRadian();
            float pitchRad = _pitch.ToRadian();
            direction.x = Cos(yawRad) * Cos(pitchRad);
            direction.y = Sin(yawRad) * Cos(pitchRad);
            direction.z = Sin(pitchRad);

            _cameraForward = direction.Normalized;
            _cameraRight = _cameraForward.Cross(Vertex3f.UnitZ).Normalized;
            _cameraUp = _cameraRight.Cross(_cameraForward).Normalized;

            float Cos(float radian)=> (float)Math.Cos(radian);
            float Sin(float radian)=> (float)Math.Sin(radian);
        }

        public void UpdateDirection(Vertex3f direction)
        {
            if (direction == Vertex3f.UnitZ)
            {
                _cameraForward = direction;
                _cameraRight = Vertex3f.UnitX;
                _cameraUp = Vertex3f.UnitY;
                _pitch = 90;
                _yaw = 0;
            }
            else
            {
                _cameraForward = direction.Normalized;
                _cameraRight = _cameraForward.Cross(Vertex3f.UnitZ).Normalized;
                _cameraUp = _cameraRight.Cross(_cameraForward).Normalized;

                Vertex3f d0 = new Vertex3f(_cameraForward.x, _cameraForward.y, 0);
                float pitch = ((float)Math.Acos(_cameraForward.Dot(d0.Normalized))).ToDegree();
                _pitch = (_cameraForward.z > 0) ? pitch : -pitch;

                float yaw = ((float)Math.Acos(d0.Dot(Vertex3f.UnitX))).ToDegree();
                _yaw = (d0.y > 0) ? yaw : 360 - yaw;
            }
        }

        public override void GoForward(float deltaDistance)
        {
            _position += _cameraForward * deltaDistance;
        }

        public override void GoRight(float deltaDistance)
        {
            _position += _cameraRight * deltaDistance;
        }

        public override void GoUp(float deltaDistance)
        {
            _position += _cameraUp * deltaDistance;
        }

        public override void Pitch(float deltaDegree)
        {
            if (Math.Abs(deltaDegree) > MAX_VARIANCE) return;
            _pitch += SENSITIVITY * deltaDegree;
            _pitch = _pitch.Clamp(-MAX_PITCH, MAX_PITCH);
        }

        /// <summary>
        /// delta 각도만큼 변화시킨다.
        /// </summary>
        /// <param name="deltaDegree"></param>
        public override void Yaw(float deltaDegree)
        {
            if (Math.Abs(deltaDegree) > MAX_VARIANCE) return;
            _yaw += SENSITIVITY * deltaDegree;
            if (_yaw < -180) _yaw += 360;
            if (_yaw > 180) _yaw -= 360;
        }

        /// <summary>
        /// delta 각도만큼 변화시킨다.
        /// </summary>
        /// <param name="position"></param>
        public override void GoTo(Vertex3f position)
        {
            _position = position;
        }


    }
}