using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Baltin.UFBT.Example2
{

    [Serializable]
    public class NpcConfig
    {
        /// <summary>
        /// Coefficient to a gravity force between the NPC and the player when them are close to each other  
        /// </summary>
        [SerializeField] public float attackForce = 10f;

        /// <summary>
        /// Coefficientto a gravity force between the NPC and the player when them are not close to each other  
        /// </summary>
        [SerializeField] public float movingForce = 1f;

        [SerializeField] public float rangePreparingDuration = 1f;

        [SerializeField] public float rangeAttackDuration = 2f;

        [SerializeField] public float patrolForce = 0.2f;

        [SerializeField] public float patrolTorque = 1f;
        
        public float relaxDuration = 3f;
        
        [Header("Настройки обзора")]
        
        [SerializeField] [Range(0f, 360f)] public float viewAngle = 90f;
        
        [SerializeField] [Min(1)] public int rayCount = 5;
        
        [SerializeField] [Min(0.1f)] public float viewDistance = 10f;
        
        [SerializeField] public LayerMask targetMask;
    }

    /// <summary>
    /// NpcMonoBehaviour object that describe the NPC in scene and serve as a entry point for its behaviour 
    /// </summary>
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(MeshRenderer))]
    public class UnitaskNpcMonoBehaviour2 : MonoBehaviour
    {
        [SerializeField] private NpcConfig config;

        private bool _treeIsExecuting = false;

        private static readonly int ColorPropertyID = Shader.PropertyToID("_Color");
        
        private int _instanceId; 
        
        private Rigidbody _body;
        private MeshRenderer _meshRenderer;
        
        private Vector3 _initialLocalScale;

        private NpcVision _vision;
        
        private Transform _target; 
        
        private Vector3 _targetDirection;

        private float _targetDistance;

        private float _shakeValue = 0;
        
        void Start()
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _body = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();
           
            _initialLocalScale = _body.transform.localScale;
            
            _instanceId = _body.GetInstanceID();
            
            name = "Npc " + _instanceId;
            
            _vision = new NpcVision(config.viewAngle, config.rayCount, config.viewDistance, config.targetMask);
        }

        public void Update()
        {
            if (!_treeIsExecuting)
                UpdateFbtAsync(CancellationToken.None).Forget();
            
            _vision.DrawDebug(transform);
        }

        /// <summary>
        /// Shortest tree definition with minimal boilerplate
        /// It works but is not zero memory because the delegates containing closures allocate memory 
        /// </summary>
        /// <param name="c"></param>
        private async UniTask UpdateFbtAsync(CancellationToken c)
        {
            if (_treeIsExecuting)
                return;

            _treeIsExecuting = true;

            try
            {
                await this.Selector( //Sequencer node
                    static b => b.Sequencer( //Sequencer node
                        static b => b.FindTarget(), //Action node realized as a simple delegate 
                        static b => b.Selector(     //Selector node
                            static b => b.If(       //Conditional node 
                                static b => b._targetDistance < 1f,  //Condition
                                static b => b.MeleeAttack()),       //Action
                            static b => b.If(
                                static b => b._targetDistance < 3f,
                                static b => b.RangeAttack()),       //Continuous function that can be "running"
                            static b => b.If(
                                static b => b._targetDistance < 8f,
                                static b => b.Move()))),
                    static b => b.PatrolMove());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _treeIsExecuting = false;
            }
        }

        /// <summary>
        /// Some action at the beginning of execution
        /// Calculation the distance between NPC and the player
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<bool> FindTarget()
        {
            //todo: Надо сделать функцию продолжительной. Пока не нашил цель - ищем ее

            _target = _vision.FindTarget(transform);

            if (_target is null)
            {
                _targetDistance = 1000;
                return UniTask.FromResult(false);
            }

            _targetDistance = Vector3.Distance(_target.position, _body.worldCenterOfMass);
            if (_targetDistance != 0)
                _targetDirection = (_target.position - _body.worldCenterOfMass) / _targetDistance;
            else
                _targetDirection = Vector3.zero;

            return UniTask.FromResult(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<bool> Idle()
        {
            SetColor(Color.yellow);
            SetScale(1f, 0.1f);

            return UniTask.FromResult(true);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UniTask<bool> PatrolMove()
        {
            // Визуализация
            SetColor(Color.yellow);
            SetScale(1f, 0.1f);

            // Обновляем случайное направление через интервал
            var angle = Random.Range(-45f, 45f);
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);
            var desiredDirection = rot * transform.up;

            // Плавный поворот через момент
            float angleDiff = Vector3.SignedAngle(transform.up, desiredDirection, Vector3.forward);
            AddTorque(Vector3.forward * angleDiff * config.patrolTorque);

            // Движение вперед
            AddForce(config.patrolForce);

            return UniTask.FromResult(true);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UniTask<bool> Move()
        {
            RotateToTarget(1f);
            
            SetColor(Color.magenta, 0.25f);
            SetScale(1f, 0.25f);
            AddForce(config.movingForce);

            //Добавить поворот в сторону цели
            
            return UniTask.FromResult(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UniTask<bool> MeleeAttack()
        {
            RotateToTarget(2f);
            
            SetColor(Color.red, 0.25f   );
            OscillateScale(1, 1.5f, 0.25f);
            AddForce(config.attackForce);

            return UniTask.FromResult(true);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask<bool> RangeAttack()
        {
            //Preaparing to attack
            var startTime = Time.timeSinceLevelLoad;
            for (var t = 0f; t <= 1; t = (Time.timeSinceLevelLoad - startTime) / config.rangePreparingDuration)
            {
                RotateToTarget(2f);
            
                SetScale(1.5f, config.rangePreparingDuration / 2);
                SetColor(Color.red, config.rangePreparingDuration / 2);
                //Shake(50f, config.rangePreparingDuration / 2);

                await UniTask.Yield();
                Shake(0, 0);
            }

            if(!await FindTarget())
                return false;

            //Attack
            startTime = Time.timeSinceLevelLoad;
            for (var t = 0f; t <= 1; t = (Time.timeSinceLevelLoad - startTime) / config.rangeAttackDuration)
            {
                RotateToTarget(0.5f);
            
                SetScale(1f, config.rangeAttackDuration / 2);
                SetColor(Color.red, config.rangeAttackDuration / 2);
                AddForce(config.attackForce);

                await UniTask.Yield();
            }

            //Relax
            startTime = Time.timeSinceLevelLoad;
            for (var t = 0f; t <= 1; t = (Time.timeSinceLevelLoad - startTime) / config.relaxDuration)
            {
                SetScale(1f, 0.25f);
                SetColor(Color.gray, 0.25f);

                await UniTask.Yield();
            }

            return true;
        }

        private void OscillateScale(float fromScale, float toScale, float period)
        {
            if (period <= 0)
                throw new ArgumentException("Period must be greater than zero.", nameof(period));
            
            if (fromScale < 0 || toScale < 0)
                throw new ArgumentException("Scale values must be non-negative.");

            var triangleWave = Mathf.PingPong(Time.realtimeSinceStartup / period, 1f);

            _body.transform.localScale = _initialLocalScale * Mathf.Lerp(fromScale, toScale, triangleWave);
        }

        private void SetScale (float scale, float smoothTime = 0)
        {
            if (scale < 0)
                throw new ArgumentException("Scale values must be non-negative.");

            _body.transform.localScale = smoothTime < Time.deltaTime
                ? _initialLocalScale * scale
                : Vector3.Lerp(
                    _body.transform.localScale,
                    _initialLocalScale * scale,
                    Time.deltaTime/smoothTime);
        }

        private void Shake(float value, float smoothTime = 0)
        {
            _shakeValue = smoothTime < Time.deltaTime
                ? value
                : Mathf.Lerp(
                    _shakeValue,
                    value,
                    Time.deltaTime / smoothTime);

            var shakeForce = RandomDirection() * (_shakeValue * Time.deltaTime);
            _body.position = _body.position + shakeForce;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetColor(Color color, float smoothTime = 0)
        {
            if (smoothTime >= Time.deltaTime)
                color = Color.Lerp(
                    _meshRenderer.material.GetColor(ColorPropertyID),
                    color,
                    Time.deltaTime / smoothTime);

            _meshRenderer.material.SetColor(ColorPropertyID, color);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddForce(float forceToTarget)
        {
            //var force = _targetDirection * (forceToTarget * Time.deltaTime);
            var force = _body.transform.up * (forceToTarget * Time.deltaTime);
            _body.AddForce(force, ForceMode.VelocityChange);
        }

        private void AddTorque(Vector3 torqueToTarget)
        {
            var torque = torqueToTarget * Time.deltaTime;
            _body.AddTorque(torque, ForceMode.VelocityChange);
        }

        private static Vector3 RandomDirection()
        {
            var dir = UnityEngine.Random.onUnitSphere;

            return new Vector3(dir.x, dir.y, 0);
        }
        
        private void RotateToTarget(float rotationSpeed)
        {
            Vector3 direction = _target.position - transform.position;
            direction.z = 0; // фиксируем плоскость XY
            if (direction.sqrMagnitude < 0.0001f) return;

            // создаём поворот, где "вверх" объекта (ось Y) смотрит в direction
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction);

            // плавный поворот вокруг Z
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed
            );
        }
    }
}