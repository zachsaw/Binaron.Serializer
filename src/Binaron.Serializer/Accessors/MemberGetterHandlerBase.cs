using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Binaron.Serializer.Accessors
{
    internal abstract class MemberGetterHandlerBase<T, TResult> : IMemberGetterHandler<T>
    {
        private readonly MemberGetter<TResult> getter;

        protected MemberGetterHandlerBase(MemberGetter<TResult> getter)
        {
            this.getter = getter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref readonly MemberGetter<TResult> GetGetter() => ref getter;

        public MemberInfo MemberInfo
        {
            get
            {
                ref readonly var pGetter = ref GetGetter();
                return pGetter.MemberInfo;
            }
        }
        
        public string MemberName
        {
            get
            {
                ref readonly var pGetter = ref GetGetter();
                return pGetter.MemberName;
            }
        }

        public void Handle(T state, object target)
        {
            ref readonly var pGetter = ref GetGetter();
            HandleInternal(state, pGetter.Get(target));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract Task HandleInternal(T state, TResult result);
    }
}