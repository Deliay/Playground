using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPlayground.Library
{
    /// <summary>
    /// Flag the undoable object to constraint pass the literal value must box.
    /// </summary>
    interface IUndoable { }

    /// <summary>
    /// Base undoable Action class
    /// <para>Implement two base method for <see cref="ActionBin"/> TypeEarse call</para>
    /// </summary>
    internal abstract class UndoableAction
    {
        /// <summary>
        /// Call on Undo request
        /// </summary>
        public abstract void Revert();
        public abstract void Apply();
    }

    /// <summary>
    /// A GenericBase layer to fetch sub-generic-class infomation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class UndoableAction<T> : UndoableAction
    {
        static Type[] types;
        static UndoableAction()
        {
            types = typeof(T).GetGenericArguments();
        }

        public Type GetTargetObjecType()
        {
            return types[0];
        }

        public Type GetArgumentType()
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
