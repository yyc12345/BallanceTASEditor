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
        /// <param name="seq">所要操作的TAS存储容器。</param>
        void Execute(ITasSequence seq);
        /// <summary>
        /// 检查该操作是否已经被执行过。
        /// </summary>
        /// <remarks>
        /// 所有Tas操作类创建后只能执行一次或者不执行，
        /// 因此有此函数用于获取是否已经执行过。
        /// </remarks>
        /// <returns>如果已经执行过，返回true，否则返回false。</returns>
        bool IsExecuted();
    }

    /// <summary>
    /// 可撤销的TAS操作接口，所有可撤销的TAS操作均需支持此接口。
    /// </summary>
    public interface ITasRevocableOperation : ITasOperation {
        /// <summary>
        /// 撤销对应TAS操作。
        /// </summary>
        /// <param name="seq">所要撤销操作的TAS存储容器。</param>
        void Revoke(ITasSequence seq);
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

    internal static class OperationExceptions {
        internal static readonly InvalidOperationException ExecutionEnvironment = new InvalidOperationException("Can not execute one TAS operation multiple times.");
        internal static readonly InvalidOperationException RevokeEnvironment = new InvalidOperationException("Can not revoke an not executed TAS operation.");
    }

    public enum CellKeysOperationKind {
        Set, Unset, Flip
    }

    public class CellKeysOperation : ITasRevocableOperation {
        public static CellKeysOperation FromSingleCell(CellKeysOperationKind kind, int index, TasKey key) {
            return new CellKeysOperation(kind, index, index, key, key);
        }

        public static CellKeysOperation FromCellRange(CellKeysOperationKind kind, int startIndex, int endIndex, TasKey startKey, TasKey endKey) {
            return new CellKeysOperation(kind, startIndex, endIndex, startKey, endKey);
        }

        private CellKeysOperation(CellKeysOperationKind kind, int startIndex, int endIndex, TasKey startKey, TasKey endKey) {
            // Check arguments.
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, endIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startKey.ToIndex(), endKey.ToIndex());
            // Setup members.
            m_Kind = kind;
            m_StartIndex = startIndex;
            m_EndIndex = endIndex;
            m_StartKey = startKey;
            m_EndKey = endKey;
            m_FramesBackup = null;
        }

        private CellKeysOperationKind m_Kind;
        private int m_StartIndex, m_EndIndex;
        private TasKey m_StartKey, m_EndKey;
        private RawTasFrame[]? m_FramesBackup;

        public bool IsExecuted() {
            return m_FramesBackup is not null;
        }

        public void Execute(ITasSequence seq) {
            if (m_FramesBackup is not null) {
                throw OperationExceptions.ExecutionEnvironment;
            }

            // Check index range.
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(m_EndIndex, seq.GetCount());
            ArgumentOutOfRangeException.ThrowIfLessThan(m_StartIndex, 0);

            // Do backup and set values at the same time
            var backups = new RawTasFrame[m_EndIndex - m_StartIndex];
            // Pre-build key list for fast fetching.
            var keys = Enumerable.Range(m_StartKey.ToIndex(), m_EndKey.ToIndex() - m_StartKey.ToIndex()).Select((i) => TasKey.FromIndex(i)).ToArray();
            for (int index = m_StartIndex; index <= m_EndIndex; index++) {
                // Fetch frame
                var frame = seq.Visit(index);
                // Do backup
                frame.ToRawImplace(ref backups[index - m_StartIndex]);
                // Modify keys
                foreach (var key in keys) {
                    switch (m_Kind) {
                        case CellKeysOperationKind.Set:
                            frame.SetKeyPressed(key, true);
                            break;
                        case CellKeysOperationKind.Unset:
                            frame.SetKeyPressed(key, false);
                            break;
                        case CellKeysOperationKind.Flip:
                            frame.FlipKeyPressed(key);
                            break;
                    }
                }
            }

            // Assign backups
            m_FramesBackup = backups;
        }

        public void Revoke(ITasSequence seq) {
            if (m_FramesBackup is null) {
                throw OperationExceptions.RevokeEnvironment;
            }

            // Index range is checked,
            // so we directly restore backup.
            for (int index = m_StartIndex; index <= m_EndIndex; index++) {
                seq.Visit(index).FromRawImplace(m_FramesBackup[index - m_StartIndex]);
            }

            // Clear backups
            m_FramesBackup = null;
        }

        public int Occupation() {
            return (m_EndIndex - m_StartIndex) * (m_EndKey.ToIndex() - m_StartKey.ToIndex());
        }

    }

    public class CellFpsOperation : ITasRevocableOperation {
        public bool IsExecuted() {
            throw new NotImplementedException();
        }

        public void Execute(ITasSequence seq) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence seq) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }

    }

    public class RemoveFrameOperation : ITasRevocableOperation {
        public bool IsExecuted() {
            throw new NotImplementedException();
        }

        public void Execute(ITasSequence seq) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence seq) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class AddFrameOperation : ITasRevocableOperation {
        public AddFrameOperation(int index, uint fps, int count) {
            // Check argument
            if (!FpsConverter.IsValidFps(fps)) {
                throw new ArgumentOutOfRangeException(nameof(fps));
            }
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            // Assign argument
            m_Index = index;
            m_Fps = fps;
            m_Count = count;
            m_IsExecuted = false;
        }

        private int m_Index;
        private uint m_Fps;
        private int m_Count;
        private bool m_IsExecuted;

        public bool IsExecuted() {
            return m_IsExecuted;
        }

        public void Execute(ITasSequence seq) {
            if (m_IsExecuted) {
                throw OperationExceptions.ExecutionEnvironment;
            }

            // Check argument.
            ArgumentOutOfRangeException.ThrowIfGreaterThan(m_Index, seq.GetCount());

            // Skip if count is zero.
            if (m_Count != 0) {
                // Prepare data builder.
                var iter = Enumerable.Range(0, m_Count).Select((_) => TasFrame.FromFps(m_Fps));
                var exactSizedIter = new ExactSizeEnumerableAdapter<TasFrame>(iter, m_Count);
                // Execute inserting.
                seq.Insert(m_Index, exactSizedIter);
            }

            // Set status
            m_IsExecuted = true;
        }

        public void Revoke(ITasSequence seq) {
            if (!m_IsExecuted) {
                throw OperationExceptions.RevokeEnvironment;
            }

            // Arguments were checked so we directly resotre them.
            // If we inserted count is not zero, remove inserted frames, otherwise do nothing.
            if (m_Count != 0) {
                seq.Remove(m_Index, m_Index + m_Count - 1);
            }
            // Modify execution status
            m_IsExecuted = false;
        }

        public int Occupation() {
            return 1;
        }
    }

    public class InsertFrameOperation : ITasRevocableOperation {
        public bool IsExecuted() {
            throw new NotImplementedException();
        }

        public void Execute(ITasSequence seq) {
            throw new NotImplementedException();
        }

        public void Revoke(ITasSequence seq) {
            throw new NotImplementedException();
        }

        public int Occupation() {
            throw new NotImplementedException();
        }
    }

    public class ClearKeysOperation : ITasOperation {
        public ClearKeysOperation() {
            m_IsExecuted = false;
        }

        private bool m_IsExecuted;

        public void Execute(ITasSequence seq) {
            // Check execution status first.
            if (m_IsExecuted) {
                throw OperationExceptions.ExecutionEnvironment;
            }
            // Execute operation
            foreach (var frame in seq) {
                frame.ClearKeyPressed();
            }
            m_IsExecuted = true;
        }

        public bool IsExecuted() {
            return m_IsExecuted;
        }
    }

    public class UniformFpsOperation : ITasOperation {
        public UniformFpsOperation(float deltaTime) {
            m_DeltaTime = deltaTime;
            m_IsExecuted = false;
        }

        private float m_DeltaTime;
        private bool m_IsExecuted;

        public void Execute(ITasSequence seq) {
            // Check execution status first.
            if (m_IsExecuted) {
                throw OperationExceptions.ExecutionEnvironment;
            }
            // Execute operation
            foreach (var frame in seq) {
                frame.SetTimeDelta(m_DeltaTime);
            }
            m_IsExecuted = true;
        }

        public bool IsExecuted() {
            return m_IsExecuted;
        }
    }

}
