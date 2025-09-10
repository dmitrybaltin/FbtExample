using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Baltin.UFBT;
using Random = UnityEngine.Random;

namespace Baltin.UFBT.Example1
{
    /// <summary>
    /// NpcMonoBehaviour object that describe the NPC in scene and serve as a entry point for its behaviour 
    /// </summary>
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(MeshRenderer))]
    public class UnitaskNpcMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private NpcConfig config;

        /// <summary>
        /// Blackboard object that contains methods and data of controlled object
        /// </summary>
        private NpcBoard npcBoard;

        private bool _treeIsExecuting = false;

        void Start()
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            
            npcBoard = new NpcBoard(
                config,
                GetComponent<Rigidbody>(), 
                GetComponent<MeshRenderer>(),
                player);
            name = "Npc " + npcBoard.InstanceId;
        }

        public void Update()
        {
            if (!_treeIsExecuting)
                UpdateFbtAsync(CancellationToken.None).Forget();
        }

        private async UniTask UpdateFbtAsync(CancellationToken c)
        {
            if (_treeIsExecuting)
                return;

            _treeIsExecuting = true;

            try
            {
                await npcBoard.Sequencer( //Sequencer node
                    static b => b.FindTarget(), //Action node realized as a delegate Func<NpcBoard, UniTask<bool>> 
                    static b => b.Selector(   //Selector node
                        static b => b.If(     //Conditional node 
                            static b => b.TargetDistance < 1f, //Condition
                            static b => b.MeleeAttack()), //Action
                        static b => b.If(
                            static b => b.TargetDistance < 3f,
                            static b =>
                                b.RangeAttack()), //The only continuous function here that can be "running"
                        static b => b.If(
                            static b => b.TargetDistance < 8f,
                            static b => b.Move()),
                        static b => b.Idle()));
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

    }

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

        [SerializeField] public float relaxDuration = 3f;
    }

    /// <summary>
    /// Blackboard object that contains methods and data of controlled object
    /// </summary>
    public class NpcBoard
    {
        private static readonly int ColorPropertyID = Shader.PropertyToID("_Color");
        
        private NpcConfig Config;
        
        public readonly int InstanceId; 
        
        public float TargetDistance;

        private readonly Rigidbody _body;
        private readonly MeshRenderer _meshRenderer;
        private readonly Transform _player; 
        
        private readonly Vector3 _initialLocalScale;

        private Vector3 _targetDirection;

        private float _shakeValue = 0;

        public NpcBoard(NpcConfig config, Rigidbody body, MeshRenderer meshRenderer, Transform player)
        {
            Config = config;
            _player = player;
            _body = body;
            _meshRenderer = meshRenderer;
           
            _initialLocalScale = body.transform.localScale;
            
            InstanceId = body.GetInstanceID();
        }
        
        /// <summary>
        /// Some action at the beginning of execution
        /// Calculation the distance between NPC and the player
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<bool> FindTarget()
        {
            TargetDistance = Vector3.Distance(_player.position, _body.worldCenterOfMass);
            if (TargetDistance != 0)
                _targetDirection = (_player.position - _body.worldCenterOfMass) / TargetDistance;
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
        public UniTask<bool> Move()
        {
            SetColor(Color.magenta, 0.25f);
            SetScale(1f, 0.25f);
            AddForce(Config.movingForce);

            return UniTask.FromResult(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<bool> MeleeAttack()
        {
            SetColor(Color.red, 0.25f   );
            OscillateScale(1, 1.5f, 0.25f);
            AddForce(Config.attackForce);

            return UniTask.FromResult(true);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<bool> RangeAttack()
        {
            //Preaparing to attack
            var startTime = Time.timeSinceLevelLoad;
            for (var t = 0f; t <= 1; t = (Time.timeSinceLevelLoad - startTime) / Config.rangePreparingDuration)
            {
                SetScale(1.5f, Config.rangePreparingDuration / 2);
                SetColor(Color.red, Config.rangePreparingDuration / 2);
                Shake(50f, Config.rangePreparingDuration / 2);

                await UniTask.Yield();
                Shake(0, 0);
            }

            await FindTarget();

            //Attack
            startTime = Time.timeSinceLevelLoad;
            for (var t = 0f; t <= 1; t = (Time.timeSinceLevelLoad - startTime) / Config.rangeAttackDuration)
            {
                SetScale(1f, Config.rangeAttackDuration / 2);
                SetColor(Color.red, Config.rangeAttackDuration / 2);
                AddForce(Config.attackForce);

                await UniTask.Yield();
            }

            //Relax
            startTime = Time.timeSinceLevelLoad;
            for (var t = 0f; t <= 1; t = (Time.timeSinceLevelLoad - startTime) / Config.relaxDuration)
            {
                SetScale(1f, 0.25f);
                SetColor(Color.gray, 0.25f);

                await UniTask.Yield();
            }

            return true;
        }

        public void OscillateScale(float fromScale, float toScale, float period)
        {
            if (period <= 0)
                throw new ArgumentException("Period must be greater than zero.", nameof(period));
            
            if (fromScale < 0 || toScale < 0)
                throw new ArgumentException("Scale values must be non-negative.");

            var triangleWave = Mathf.PingPong(Time.realtimeSinceStartup / period, 1f);

            _body.transform.localScale = _initialLocalScale * Mathf.Lerp(fromScale, toScale, triangleWave);
        }

        public void SetScale (float scale, float smoothTime = 0)
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

        public void Shake(float value, float smoothTime = 0)
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
        public void SetColor(Color color, float smoothTime = 0)
        {
            if (smoothTime >= Time.deltaTime)
                color = Color.Lerp(
                    _meshRenderer.material.GetColor(ColorPropertyID),
                    color,
                    Time.deltaTime / smoothTime);

            _meshRenderer.material.SetColor(ColorPropertyID, color);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddForce(float forceToTarget)
        {
            var force = _targetDirection * (forceToTarget * Time.deltaTime);
            _body.AddForce(force, ForceMode.VelocityChange);
        }


        private static Vector3 RandomDirection()
        {
            var dir = UnityEngine.Random.onUnitSphere;

            return new Vector3(dir.x, dir.y, 0);
        }
    }
}