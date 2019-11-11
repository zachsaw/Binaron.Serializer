using System;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Accessors
{
    internal class MemberSetterHandler<T, TResult> : IMemberSetterHandler<T>
    {
        private readonly MemberSetter<TResult> setter;
        private readonly Func<T, TResult> func;

        public MemberSetterHandler(MemberSetter<TResult> setter, Func<T, TResult> func)
        {
            this.setter = setter;
            this.func = func;
        }

        public string MemberName => setter.MemberName;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref readonly MemberSetter<TResult> GetSetter() => ref setter;

        public void Handle(T state, object target)
        {
            ref readonly var pSetter = ref GetSetter();
            pSetter.Set(target, func(state));
        }
    }
}