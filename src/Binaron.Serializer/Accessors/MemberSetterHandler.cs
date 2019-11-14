using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Accessors
{
    internal abstract class MemberSetterHandlerBase<T, TResult> : IMemberSetterHandler<T>
    {
        private readonly MemberSetter<TResult> setter;

        protected MemberSetterHandlerBase(MemberSetter<TResult> setter)
        {
            this.setter = setter;
        }

        public string MemberName => setter.MemberName;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref readonly MemberSetter<TResult> GetSetter() => ref setter;

        public void Handle(T state, object target)
        {
            ref readonly var pSetter = ref GetSetter();
            pSetter.Set(target, HandleInternal(state));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TResult HandleInternal(T state);
    }
}