using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {

    /// <summary>
    /// TAS操作接口。所有TAS操作均需要支持此接口。
    /// </summary>
    public interface ITasOperation {
        /// <summary>
        /// 执行对应的TAS操作。
        /// </summary>
        /// <param name="storage">所要操作的TAS存储容器。</param>
        void Execute(ITasSequence storage);
    }

    /// <summary>
    /// 可撤销的TAS操作接口，所有可撤销的TAS操作均需支持此接口。
    /// </summary>
    public interface ITasRevocableOperation : ITasOperation {
        /// <summary>
        /// 撤销对应TAS操作。
        /// </summary>
        /// <param name="storage">所要撤销操作的TAS存储容器。</param>
        void Revoke(ITasSequence storage);
        /// <summary>
        /// 返回该TAS操作占用的内存大小。
        /// </summary>
        /// <remarks>
        /// 可撤销的TAS操作会在内存中存储一定数据，用于撤销对应操作。
        /// 该函数返回的占用用于衡量该操作的开销。
        /// 我们应当基于大小，而非写死的个数决定撤销栈中的最大操作次数，
        /// 例如对于小型操作我们可以存储100个，对于大型操作则只能存储5个等。
        /// 用于解决编辑者目前认为撤销栈大小不足的情况。
        /// <para/>
        /// 该函数返回的大小可以不是特别精确，但要准确反映空间复杂度。
        /// </remarks>
        /// <returns>占用的内存大小（以byte为单位）。</returns>
        int Occupation();
    }

    public enum CellKeysOperationKind {
        Set, Unset, Flip
    }

    public class CellKeysOperation : ITasRevocableOperation {

        private CellKeysOperationKind m_Kind;

        public void Execute(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class CellFpsOperation : ITasRevocableOperation {
        public void Execute(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class RemoveFrameOperation : ITasRevocableOperation {
        public void Execute(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class AddFrameOperation : ITasRevocableOperation {
        public void Execute(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class InsertFrameOperation : ITasRevocableOperation {
        public void Execute(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence storage) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class ClearKeysOperation : ITasOperation {
        public ClearKeysOperation() { }

        public void Execute(ITasSequence storage) {
            foreach (var frame in storage) {
                frame.ClearKeyPressed();
            }
        }
    }

    public class UniformFpsOperation : ITasOperation {
        public UniformFpsOperation(float deltaTime) {
            m_DeltaTime = deltaTime;
        }

        private float m_DeltaTime;

        public void Execute(ITasSequence storage) {
            foreach (var frame in storage) {
                frame.SetTimeDelta(m_DeltaTime);
            }
        }
    }

}
