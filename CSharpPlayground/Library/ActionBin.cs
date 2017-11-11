using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPlayground.Library
{
    interface IUndoable { }

    internal abstract class UndoableAction
    {
        public abstract void Revert();
        public abstract void Apply();
    }

    internal abstract class UndoableAction<T> : UndoableAction
    {
        static Type[] types;
        static UndoableAction()
        {
            types = typeof(T).GetGenericArguments();
        }

        public static Type GetTargetObjecType()
        {
            return types[0];
        }

        public static Type GetArgumentType()
        {
            return types[1];
        }
    }

    abstract class UndoableAction<TObject, TArgument> : UndoableAction<UndoableAction<TObject, TArgument>> where TObject : class, IUndoable
    {
        TArgument arg;
        TObject src;
        protected abstract void Restore(TObject src, TArgument arg);
        protected abstract void Apply(TObject src, TArgument arg);

        public UndoableAction(TObject src, TArgument arg)
        {
            this.arg = arg;
            this.src = src;
            
        }

        public override void Apply()
        {
            Apply(src, arg);
        }

        public override void Revert()
        {
            Restore(src, arg);
        }

    }

    class ActionBin
    {
        private Stack<UndoableAction> queuedUndoAction = new Stack<UndoableAction>();
        private Queue<UndoableAction> queuedRedoAction = new Queue<UndoableAction>();
        public void @Do<TObject, TArgument>(UndoableAction action) where TObject : class, IUndoable
        {
            queuedUndoAction.Push(action);
            action.Apply();
        }

        public void Undo()
        {
            var p = queuedUndoAction.Pop();
            p.Revert();
            queuedRedoAction.Enqueue(p);
        }

        public void Redo()
        {
            var p = queuedRedoAction.Dequeue();
            p.Apply();
            queuedUndoAction.Push(p);
        }
    }
}
