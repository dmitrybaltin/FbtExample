using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Baltin.UFBT.Example2a
{
    public interface INpcVision
    {
        Transform FindTarget(Transform origin);

        UniTask<Transform> FindTargetAsync(Transform origin);

        void DrawDebug(Transform origin);
    }
}