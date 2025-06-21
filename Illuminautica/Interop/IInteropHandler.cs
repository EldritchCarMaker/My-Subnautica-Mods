using System.Collections;
using UnityEngine;

namespace Illuminautica.Interop;

public interface IInteropHandler
{
    public IEnumerator SetColor(Color color);
}
