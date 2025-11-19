using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Utils {

    /// <summary>
    /// TAS操作接口。所有TAS操作均需要支持此接口。
    /// </summary>
    public interface ITasOperation {
        /// <summary>
        /// 执行对应的TAS操作。
        /// </summary>
        /// <param name="storage">所要操作的TAS存储容器。</param>
        void Execute(ITasStorage<TasFrame> storage);
    }

    /// <summary>
    /// 可撤销的TAS操作接口，所有可撤销的TAS操作均需支持此接口。
    /// </summary>
    public interface ITasRevocableOperation : ITasOperation {
        /// <summary>
        /// 撤销对应TAS操作。
        /// </summary>
        /// <param name="storage">所要撤销操作的TAS存储容器。</param>
        void Revoke(ITasStorage<TasFrame> storage);
    }

    public enum CellKeysOperationKind {
        Set, Unset, Flip
    }

    public class CellKeysOperation : ITasRevocableOperation {

        private CellKeysOperationKind m_Kind;

        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

    public class CellFpsOperation : ITasRevocableOperation {
        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

    public class RemoveFrameOperation : ITasRevocableOperation {
        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

    public class AddFrameOperation : ITasRevocableOperation {
        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

    public class InsertFrameOperation : ITasRevocableOperation {
        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

    public class ClearKeysOperation : ITasOperation {
        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

    public class UniformFpsOperation : ITasOperation {
        public void Execute(ITasStorage<TasFrame> storage) {
            throw new NotImplementedException();
        }
    }

}
