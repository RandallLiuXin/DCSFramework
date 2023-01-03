using Unity.Entities;

namespace Anvil.Common
{
    public struct StatefulProperty<T> : IComponentData
    {
        private bool Dirty;
        public T Value;

        public bool IsTouch() => Dirty;
        public void Touch()
        {
            Dirty = true;
        }
        public void UnTouch()
        {
            Dirty = false;
        }
        public T GetValue() => Value;
        public void SetValue(T value)
        {
            Value = value;
            Touch();
        }
    }
}
