using System;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Accessors
{
    internal class MemberGetterHandler<T, TResult> : IMemberGetterHandler<T>
    {
        private readonly MemberGetter<TResult> getter;
        private readonly Action<T, TResult> action;

        public MemberGetterHandler(MemberGetter<TResult> getter, Action<T, TResult> action)
        {
            this.getter = getter;
            this.action = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref readonly MemberGetter<TResult> GetGetter() => ref getter;

        public void Handle(T state, object target)
        {
            ref readonly var pGetter = ref GetGetter();
            action(state, pGetter.Get(target));
        }
    }
}