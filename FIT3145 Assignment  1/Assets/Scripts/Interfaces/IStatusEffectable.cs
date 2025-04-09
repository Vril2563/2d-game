using System.Collections;

namespace Interfaces
{
    public interface IStatusEffectable
    {
        void ApplyStatusEffect(IEnumerator statusEffect);
    }
}
