using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.Core {
    public class LimitedStack<T> {
        private static readonly int STACK_LENGTH = 20;

        public LimitedStack() {
            _stack = new LinkedList<T>();
        }

        private LinkedList<T> _stack;

        public void Push(T data) {
            _stack.AddLast(data);
            if (_stack.Count > STACK_LENGTH) {
                _stack.RemoveFirst();
            }
        }

        public T Pop() {
            if (_stack.Last == null) return default(T);
            var data = _stack.Last.Value;
            _stack.RemoveLast();
            return data;
        }

        public void Clear() {
            _stack.Clear();
        }

        public bool IsEmpty() {
            return _stack.Count == 0;
        }
        
    }
}
